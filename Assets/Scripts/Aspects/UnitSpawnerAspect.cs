

using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct UnitSpawnerAspect : IAspect
{
    private readonly RefRW<UnitSpawnerPropertys> _unitSpawnerPropetys;
    private readonly RefRO<CollisionAvoidanceSettingsComponent> _collisionAvoidance;
    public readonly Entity EntityUnit => _unitSpawnerPropetys.ValueRO.entity;
    public readonly Entity Entity;
    public readonly float ElapsedTime {
        get => _unitSpawnerPropetys.ValueRO.elapsedTime;
        set => _unitSpawnerPropetys.ValueRW.elapsedTime = value;
    }
    public readonly int2 GridXZCount => new int2 { x = _unitSpawnerPropetys.ValueRO.xGridCount, y = _unitSpawnerPropetys.ValueRO.zGridCount };
    public readonly float BaseOffset => _unitSpawnerPropetys.ValueRO.baseOffset;
    public readonly float2 PaddingXZ => new float2 { x = _unitSpawnerPropetys.ValueRO.xPadding, y = _unitSpawnerPropetys.ValueRO.zPadding };
    public readonly int MaxEntitysCount => _unitSpawnerPropetys.ValueRO.maxEntitiesToSpawn;
    public readonly float Interval => _unitSpawnerPropetys.ValueRO.interval;

    public readonly float3 Offset => _unitSpawnerPropetys.ValueRO.offset;

    public readonly int DestinationDistanceZAxis => _unitSpawnerPropetys.ValueRO.destinationDistanceZAxis;
    public readonly int MinSpeed => _unitSpawnerPropetys.ValueRO.minSpeed;
    public readonly int MaxSpeed => _unitSpawnerPropetys.ValueRO.maxSpeed;

    public readonly float MinDistance => _unitSpawnerPropetys.ValueRO.minDistance;

    public readonly int CountSpawn { 
        get => _unitSpawnerPropetys.ValueRO.countSpawn;
        set => _unitSpawnerPropetys.ValueRW.countSpawn = value; 
    }

    //--------CollisionAvoidance-------------
    public readonly float CohesionBias => _collisionAvoidance.ValueRO.cohesionBias;
    public readonly float SeparationBias => _collisionAvoidance.ValueRO.separationBias;

    public readonly float AlignmentBias => _collisionAvoidance.ValueRO.alignmentBias;
    public readonly float TargetBias => _collisionAvoidance.ValueRO.targetBias;

    public readonly float PerceptionRadius => _collisionAvoidance.ValueRO.perceptionRadius;
    public readonly int cellSize => _collisionAvoidance.ValueRO.cellSize;
}
