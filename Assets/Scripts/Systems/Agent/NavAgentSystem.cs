
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.AI;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(NavAgentPreProcessSystem))]
public partial struct NavAgentSystem : ISystem
{
    private NavMeshWorld _navMeshWorld;
    private NavMeshQuery _navMeshQuere;
    private NavAgentGlobalSettings _navGlobalSettings;
    private bool _navMeshQuereAssign;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NavAgentGlobalSettings>();

        _navMeshQuereAssign= false;
        _navMeshWorld = NavMeshWorld.GetDefaultWorld();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        if (!_navMeshQuereAssign)
        {
            _navGlobalSettings = SystemAPI.GetSingleton<NavAgentGlobalSettings>();
            _navMeshQuere = new NavMeshQuery(_navMeshWorld, Allocator.Persistent, _navGlobalSettings.maxPathNodePoolSize);
            _navMeshQuereAssign = true;
        }

        float3 extents = _navGlobalSettings.extents;
        int maxiterations = _navGlobalSettings.maxIterations;
        int maxPathSize = _navGlobalSettings.maxPathSize;

        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new NavAgentJob
        {
            quere = _navMeshQuere,
            extents = extents,
            maxIterations = maxiterations,
            maxPathSize = maxPathSize,
            ecb_Parallel = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();

        _navMeshWorld.AddDependency(state.Dependency);
        
    }

    public void OnDestroy(ref SystemState state)
    {
        _navMeshQuere.Dispose();
    }
}

[BurstCompile]
public partial struct NavAgentJob: IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NavMeshQuery quere;

    public float3 extents;
    public int maxIterations;
    public int maxPathSize;

    public EntityCommandBuffer.ParallelWriter ecb_Parallel;

    private void Execute(NavAgentAspect navAgentAspect, [ChunkIndexInQuery]int sortKey)
    {
        PathQueryStatus status = PathQueryStatus.Failure;

        float3 FromLocation = navAgentAspect.FromLocation;
        float3 ToLocation = navAgentAspect.ToLocation;
        NavMeshLocation Nml_fromLocation = navAgentAspect.Nml_fromLocation;
        NavMeshLocation Nml_toLocation = navAgentAspect.Nml_toLocation;

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
