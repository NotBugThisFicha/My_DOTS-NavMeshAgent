using Unity.Entities;
using Unity.Mathematics;

public partial struct NavAgentGlobalSettings: IComponentData
{
    public int maxPathSize;
    public int maxEntitiesRoutedPerFrame;
    public int maxPathNodePoolSize;
    public int maxIterations;
    public float3 extents;
}

