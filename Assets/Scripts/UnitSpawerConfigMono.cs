using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class UnitSpawerConfigMono: MonoBehaviour
{
    public GameObject unitPrefab;

    public int xGridCount;
    public int zGridCount;
    public float baseOffset;
    public float xPadding;
    public float zPadding;
    public int maxEntitiesToSpawn;
    public float interval;
    public float3 offset;

    public int destinationDistanceZAxis;
    public int minSpeed;
    public int maxSpeed;
    public float minDistance;

    public CollisionAvoidance avoidance;
}

public partial class UnitSpawnerBaker : Baker<UnitSpawerConfigMono>
{
    public override void Bake(UnitSpawerConfigMono authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new UnitSpawnerPropertys
        {
            entity = GetEntity(authoring.unitPrefab, TransformUsageFlags.Dynamic),
            xGridCount = authoring.xGridCount,
            zGridCount= authoring.zGridCount,
            baseOffset = authoring.baseOffset,
            xPadding = authoring.xPadding,
            zPadding = authoring.zPadding,
            maxEntitiesToSpawn= authoring.maxEntitiesToSpawn,
            interval = authoring.interval,
            offset = authoring.offset,
            destinationDistanceZAxis= authoring.destinationDistanceZAxis,
            minSpeed = authoring.minSpeed,
            maxSpeed = authoring.maxSpeed,
            minDistance = authoring.minDistance,
            elapsedTime = 0,
            
        });
        AddComponent(entity, new CollisionAvoidanceSettingsComponent
        {
            cohesionBias = authoring.avoidance.cohesionBias,
            separationBias = authoring.avoidance.separationBias,
            alignmentBias = authoring.avoidance.alignmentBias,
            targetBias = authoring.avoidance.targetBias,
            perceptionRadius = authoring.avoidance.perceptionRadius,
            cellSize = authoring.avoidance.cellSize,
        });
    }
}

