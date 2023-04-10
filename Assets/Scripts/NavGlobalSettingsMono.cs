
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NavGlobalSettingsMono: MonoBehaviour
{
    public int maxPathSize;
    public int maxEntitiesRoutedPerFrame;
    public int maxPathNodePoolSize;
    public int maxIterations;
    public float3 extents;
}

public partial class NavGlobalSettingsBaker : Baker<NavGlobalSettingsMono>
{
    public override void Bake(NavGlobalSettingsMono authoring)
    {
        var NavGlobalEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(NavGlobalEntity, new NavAgentGlobalSettings
        {
            maxPathSize = authoring.maxPathSize,
            maxEntitiesRoutedPerFrame = authoring.maxEntitiesRoutedPerFrame,
            maxPathNodePoolSize = authoring.maxPathNodePoolSize,
            maxIterations = authoring.maxIterations,
            extents = authoring.extents
        });
    }
}

