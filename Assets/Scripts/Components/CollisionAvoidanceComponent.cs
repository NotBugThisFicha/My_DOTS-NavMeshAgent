
using Unity.Entities;
using Unity.Mathematics;

public partial struct CollisionAvoidanceComponent: IComponentData
{
    public float cohesionBias;
    public float separationBias;
    public float alignmentBias;
    public float targetBias;
    public float perceptionRadius;
    public int cellSize;
    public int speed;

    public float wayPointZOffset;

    public float3 currentPosition;
    public float3 velocity;
    public float3 acceleration;
    public float3 target;
}

