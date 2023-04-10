
using Unity.Entities;
using Unity.Mathematics;

public struct UnitAgentComponent: IComponentData
{
    public Entity entity;
    public float speed;
    public int currentBufferIndex;
    public float3 waypointDirection;
    public float3 offset;
    public float minDistance;
    public bool reached;
}

