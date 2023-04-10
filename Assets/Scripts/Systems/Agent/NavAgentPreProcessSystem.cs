
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(UnitSpawnerSystem))]
public partial struct NavAgentPreProcessSystem : ISystem
{
    private NavAgentGlobalSettings _settings;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NavAgentGlobalSettings>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _settings = SystemAPI.GetSingleton<NavAgentGlobalSettings>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        int maxEntitiesroutedPerFrame = _settings.maxEntitiesRoutedPerFrame;
        int entitiesRoutedPerFrame = 0;
        foreach(var unitAgentAspect in SystemAPI.Query<UnitAgentAspect>().WithNone<NavAgent_ToBeRoutedTag>())
        {

            if(entitiesRoutedPerFrame< maxEntitiesroutedPerFrame && !unitAgentAspect.navAgentComponent.ValueRO.routed) {
                ecb.AddComponent(unitAgentAspect.entity, new NavAgent_ToBeRoutedTag());
                entitiesRoutedPerFrame++;
            }
        }
        ecb.Playback(state.EntityManager);
    }
}
