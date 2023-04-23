
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct UnitAgentAspect : IAspect
{
    public readonly Entity entity => unitAgentComponent.ValueRO.entity;

    public readonly RefRO<NavAgentComponent> navAgentComponent;
    public readonly RefRW<LocalTransform> transform;
    public readonly DynamicBuffer<NavBuffer> buffer;

    private readonly RefRW<UnitAgentComponent> unitAgentComponent;
    public readonly RefRW<CollisionAvoidanceComponent> _collisionAvoidance;

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

    public readonly float3 CorrectedWayPoint{
        get => unitAgentComponent.ValueRO.correctedWaypoint;
        set => unitAgentComponent.ValueRW.correctedWaypoint = value;
    }


    public readonly bool NavAgentRouted => navAgentComponent.ValueRO.routed;


    //---------------Avoidance-----------------
    public readonly int CellSize => _collisionAvoidance.ValueRO.cellSize;
    public readonly float PerceptionRadius => _collisionAvoidance.ValueRO.perceptionRadius;
    public readonly float CohesionBias => _collisionAvoidance.ValueRO.cohesionBias;
    public readonly float AllignmentBias => _collisionAvoidance.ValueRO.alignmentBias;
    public readonly float SeparationBias => _collisionAvoidance.ValueRO.separationBias;
    public readonly float TargetBias => _collisionAvoidance.ValueRO.targetBias;
    public readonly float WayPZ => _collisionAvoidance.ValueRO.wayPointZOffset;
    public readonly float3 CurrentPositionAvoidance
    {
        get => _collisionAvoidance.ValueRO.currentPosition;
        set => _collisionAvoidance.ValueRW.currentPosition = value;
    }
    public readonly float3 VelocityAvoidance
    {
        get => _collisionAvoidance.ValueRO.velocity;
        set => _collisionAvoidance.ValueRW.velocity = value;
    }
    public readonly float3 AccelerationAvoidance
    {
        get => _collisionAvoidance.ValueRO.acceleration;
        set => _collisionAvoidance.ValueRW.acceleration = value;
    }
    public readonly float3 TargetAvoidance
    {
        get => _collisionAvoidance.ValueRO.target;
        set => _collisionAvoidance.ValueRW.target = value;
    }
}
