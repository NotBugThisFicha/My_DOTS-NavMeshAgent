
using Unity.Entities;
using Unity.Transforms;

public readonly partial struct UnitAgentAspect : IAspect
{
    public readonly Entity entity => unitAgentComponent.ValueRO.entity;
    public readonly RefRW<NavAgentComponent> navAgentComponent;
    public readonly RefRO<UnitAgentComponent> unitAgentComponent;
    public readonly RefRW<LocalTransform> transform;
    public readonly DynamicBuffer<NavBuffer> navBuffer;
}
