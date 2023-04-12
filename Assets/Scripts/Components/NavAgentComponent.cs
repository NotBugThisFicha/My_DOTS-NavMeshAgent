using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

public partial struct NavAgentComponent: IComponentData
{
    public Entity entity;
    public float3 fromLocation;
    public float3 toLocation;
    public NavMeshLocation nml_FromLocation;
    public NavMeshLocation nml_ToLocation;
    public bool routed;
}

public readonly partial struct NavAgent_ToBeRoutedTag: IComponentData ,IEnableableComponent
{

}