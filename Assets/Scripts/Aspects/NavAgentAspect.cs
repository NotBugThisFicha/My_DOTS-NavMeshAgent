

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

public readonly partial struct NavAgentAspect: IAspect
{
    public readonly Entity Entity => navAgentComponent.ValueRO.entity;

    private readonly RefRW<NavAgentComponent> navAgentComponent;
    public readonly DynamicBuffer<NavBuffer> navBuffer;
    private readonly EnabledRefRO<NavAgent_ToBeRoutedTag> roted;

    public readonly float3 FromLocation => navAgentComponent.ValueRO.fromLocation;
    public readonly float3 ToLocation => navAgentComponent.ValueRO.toLocation;
    public readonly NavMeshLocation Nml_fromLocation => navAgentComponent.ValueRO.nml_FromLocation; 
    public readonly NavMeshLocation Nml_toLocation => navAgentComponent.ValueRO.nml_ToLocation;
    public readonly bool Routed {
        get => navAgentComponent.ValueRO.routed; 
        set => navAgentComponent.ValueRW.routed = value; }
}

