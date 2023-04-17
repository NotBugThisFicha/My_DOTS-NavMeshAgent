
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

unsafe public struct NavMeshQuerePointer
{
    [NativeDisableUnsafePtrRestriction]
    public void* Value;
}

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(NavAgentPreProcessSystem))]
unsafe public partial struct NavAgentSystem : ISystem
{
    private NavMeshWorld _navMeshWorld;
    private NavAgentGlobalSettings _navGlobalSettings;
    private bool _navMeshQuereAssign;

    private NativeArray<NavMeshQuerePointer> PointerArray;
    private NativeList<NavMeshQuery> quereList;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NavAgentGlobalSettings>();

        _navMeshQuereAssign = false;
        _navMeshWorld = NavMeshWorld.GetDefaultWorld();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        if (!_navMeshQuereAssign)
        {
            _navGlobalSettings = SystemAPI.GetSingleton<NavAgentGlobalSettings>();
            ParallelizePointerQuere();
            _navMeshQuereAssign = true;
        }

        float3 extents = _navGlobalSettings.extents;
        int maxiterations = _navGlobalSettings.maxIterations;
        int maxPathSize = _navGlobalSettings.maxPathSize;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new NavAgentJob
        {
            querePointerArray = PointerArray,
            extents = extents,
            maxIterations = maxiterations,
            maxPathSize = maxPathSize,
            ecb_Parallel = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();

        _navMeshWorld.AddDependency(state.Dependency);
        
    }
    [BurstCompile]
    private void ParallelizePointerQuere()
    {
        var pointerArray = new NativeArray<NavMeshQuerePointer>(JobsUtility.MaxJobThreadCount, Allocator.Temp);
        quereList = new NativeList<NavMeshQuery>(Allocator.Persistent);

        for (var i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
        {
            pointerArray[i] = new NavMeshQuerePointer
            {
                Value = UnsafeUtility.Malloc(
                    UnsafeUtility.SizeOf<NavMeshQuery>(),
                    UnsafeUtility.AlignOf<NavMeshQuery>(),
                    Allocator.Persistent)
            };

            var quere = new NavMeshQuery(
                _navMeshWorld,
                Allocator.Persistent,
                _navGlobalSettings.maxPathNodePoolSize);

            quereList.Add(quere);

            UnsafeUtility.CopyStructureToPtr(ref quere, pointerArray[i].Value);
        }

        PointerArray = new NativeArray<NavMeshQuerePointer>(pointerArray, Allocator.Persistent);
        pointerArray.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
        foreach (var queue in quereList) queue.Dispose();
        foreach (var pointer in PointerArray) UnsafeUtility.Free(pointer.Value, Allocator.Persistent);
        quereList.Dispose();
        PointerArray.Dispose();
    }
}

[BurstCompile]
unsafe public partial struct NavAgentJob: IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<NavMeshQuerePointer> querePointerArray;

    public float3 extents;
    public int maxIterations;
    public int maxPathSize;

    public EntityCommandBuffer.ParallelWriter ecb_Parallel;

    [NativeSetThreadIndex]
    private int index;

    private void Execute(NavAgentAspect navAgentAspect, [ChunkIndexInQuery]int sortKey)
    {
        PathQueryStatus status = PathQueryStatus.Failure;

        float3 FromLocation = navAgentAspect.FromLocation;
        float3 ToLocation = navAgentAspect.ToLocation;
        NavMeshLocation Nml_fromLocation = navAgentAspect.Nml_fromLocation;
        NavMeshLocation Nml_toLocation = navAgentAspect.Nml_toLocation;

        var navMeshQuerePoiner = querePointerArray[index];
        UnsafeUtility.CopyPtrToStructure(navMeshQuerePoiner.Value, out NavMeshQuery quere);

        Nml_fromLocation = quere.MapLocation(FromLocation, extents, 0);
        Nml_toLocation = quere.MapLocation(ToLocation, extents, 0);

        if (quere.IsValid(Nml_fromLocation) && 
            quere.IsValid(Nml_toLocation))
        {
            status = quere.BeginFindPath(Nml_fromLocation, Nml_toLocation, -1);
   
        }
           

        if(status == PathQueryStatus.InProgress)
            status = quere.UpdateFindPath(maxIterations, out int iterationPerfomed);

        if(status == PathQueryStatus.Success)
        {
  
            status = quere.EndFindPath(out int polygonSize);

            NativeArray<NavMeshLocation> navMeshLocations = new NativeArray<NavMeshLocation>(polygonSize, Allocator.Temp);
            NativeArray<StraightPathFlags> straightPathFlags = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
            NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
            NativeArray<PolygonId> polys = new NativeArray<PolygonId>(polygonSize, Allocator.Temp);
            int straightPathCount = 0;

            quere.GetPathResult(polys);
            status = PathUtils.FindStraightPath(
                quere,
                FromLocation,
                ToLocation,
                polys,
                polygonSize,
                ref navMeshLocations,
                ref straightPathFlags,
                ref vertexSide,
                ref straightPathCount,
                maxPathSize);

            if(status == PathQueryStatus.Success)
            {
                for(int i =0; i< straightPathCount; i++)
                    navAgentAspect.navBuffer.Add(new NavBuffer { wayPoint = navMeshLocations[i].position });

                navAgentAspect.Routed = true;
                ecb_Parallel.SetComponentEnabled<NavAgent_ToBeRoutedTag>(sortKey,navAgentAspect.Entity, false);
            }
            navMeshLocations.Dispose();
            straightPathFlags.Dispose();
            polys.Dispose();
            vertexSide.Dispose();
           
        }
    
    }
}
