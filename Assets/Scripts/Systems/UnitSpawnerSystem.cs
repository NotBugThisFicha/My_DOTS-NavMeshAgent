
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct UnitSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
       
        new SpawnUnitJob
        {
            deltaTime = deltaTime,
            ecb = ecb.CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }
}

[BurstCompile]
public partial struct SpawnUnitJob: IJobEntity
{
    public float deltaTime;
    public EntityCommandBuffer ecb;

    private float totalSpawned;

    private void Execute(UnitSpawnerAspect spawnerAspect)
    {
        spawnerAspect.ElapsedTime += deltaTime;

        if (spawnerAspect.ElapsedTime >= spawnerAspect.Interval)
        {
            spawnerAspect.ElapsedTime = 0f;
            for(int i = 0; i< spawnerAspect.GridXZCount.x; i++)
            {
                for (int j = 0; j< spawnerAspect.GridXZCount.y; j++)
                {
                    if (totalSpawned == spawnerAspect.MaxEntitysCount)
                        break;

                    
                    Entity e;
                    e = i == 0 ? 
                        spawnerAspect.EntityUnit : 
                        ecb.Instantiate(spawnerAspect.EntityUnit);

                    spawnerAspect.CountSpawn++;

                    float3 pos = new float3(i * spawnerAspect.PaddingXZ.x, spawnerAspect.BaseOffset, j * spawnerAspect.PaddingXZ.y) + float3.zero;
                    ecb.SetComponent(e, new LocalTransform { Position = pos, Scale = 1f });

                    ecb.AddComponent(e, new NavAgentComponent
                    {
                        entity = e,
                        fromLocation = pos,
                        toLocation = new float3(pos.x, pos.y, pos.z + spawnerAspect.DestinationDistanceZAxis),
                        routed = false
                    });
                    ecb.AddComponent(e, new UnitAgentComponent
                    {
                        entity = e,
                        speed = 5,
                        currentBufferIndex = 0,
                        minDistance = spawnerAspect.MinDistance,
                        offset = spawnerAspect.Offset
                    });
                    ecb.AddComponent(e, new CollisionAvoidanceComponent
                    {
                        perceptionRadius = spawnerAspect.PerceptionRadius,
                        separationBias = spawnerAspect.SeparationBias,
                        alignmentBias= spawnerAspect.AlignmentBias,
                        cohesionBias= spawnerAspect.CohesionBias,
                        targetBias= spawnerAspect.TargetBias,
                        cellSize = spawnerAspect.cellSize,
                        wayPointZOffset = 1.5f,
                        speed = 5
                    });
                    ecb.AddComponent<NavAgent_ToBeRoutedTag>(e);
                    ecb.SetComponentEnabled<NavAgent_ToBeRoutedTag>(e, true);

                    ecb.AddBuffer<NavBuffer>(e);
                    totalSpawned++;
                }
            }
        }
    }
}
