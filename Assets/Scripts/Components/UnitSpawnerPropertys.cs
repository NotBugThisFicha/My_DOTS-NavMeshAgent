
using Unity.Entities;
using Unity.Mathematics;

public partial struct UnitSpawnerPropertys: IComponentData
{
    public Entity entity;

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

    public float elapsedTime;
    public float totalSpawned;
}
