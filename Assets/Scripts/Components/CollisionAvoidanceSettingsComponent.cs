
using Unity.Entities;
using Unity.Mathematics;

public partial struct CollisionAvoidanceSettingsComponent: IComponentData
{
    public float cohesionBias;
    public float separationBias;
    public float alignmentBias;
    public float targetBias;
    public float perceptionRadius;
    public int cellSize;
}

