
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct CountSpawnSystem: ISystem
{
    public void OnUpdate(ref SystemState state) { 
    
        foreach(var uiCountAspect in SystemAPI.Query<UnitSpawnerAspect>())
        {
            UITextCountMono.Instance.SetCount(uiCountAspect.CountSpawn);
        }
    }
}

