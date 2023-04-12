
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct UnitAgentAspect : IAspect
{
    public readonly Entity entity => unitAgentComponent.ValueRO.entity;
    public readonly RefRO<NavAgentComponent> navAgentComponent;
    private readonly RefRW<UnitAgentComponent> unitAgentComponent;
    public readonly RefRW<LocalTransform> transform;
    public readonly DynamicBuffer<NavBuffer> buffer;

    public readonly float Speed => unitAgentComponent.ValueRO.speed;
    public readonly float3 Offset => unitAgentComponent.ValueRO.offset;
    public readonly float MinDistane => unitAgentComponent.ValueRO.minDistance;
    public readonly bool Reached { 
        get => unitAgentComponent.ValueRO.reached; 
        set => unitAgentComponent.ValueRW.reached = value; 
    }
    public readonly int CurrentBufferIndex { 
        get => unitAgentComponent.ValueRO.currentBufferIndex;
        set => unitAgentComponent.ValueRW.currentBufferIndex = value;
    }
    public readonly float3 WayPointDirection{
        get => unitAgentComponent.ValueRO.waypointDirection;
        set => unitAgentComponent.ValueRW.waypointDirection = value;
    }

    public readonly bool NavAgentRouted => navAgentComponent.ValueRO.routed;

}
