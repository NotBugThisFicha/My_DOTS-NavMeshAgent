
using Unity.Entities;
using UnityEngine.UI;


public struct UITextCountComponent: IComponentData
{
    public int count;
}

public readonly partial struct UITextCountAspect: IAspect
{
    public readonly RefRW<UITextCountComponent> countComponent;
    public readonly int Count {
        get => countComponent.ValueRO.count;
        set => countComponent.ValueRW.count = value;
    }
}


