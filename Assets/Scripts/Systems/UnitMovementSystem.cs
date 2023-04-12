

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(NavAgentSystem))]
public partial struct UnitMovementSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        float deltaTime = SystemAPI.Time.DeltaTime;
        new UnitMovementJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMovementJob: IJobEntity
{
    public float deltaTime;
    private void Execute(UnitAgentAspect unitAspect, [ChunkIndexInQuery]int sortKey)
    {

        if(unitAspect.NavAgentRouted && unitAspect.buffer.Length > 0)
        {
            unitAspect.WayPointDirection = 
                math.normalize((unitAspect.buffer[unitAspect.CurrentBufferIndex].wayPoint) - unitAspect.transform.ValueRO.Position);

            unitAspect.transform.ValueRW.Position += 
                math.normalize(unitAspect.WayPointDirection + unitAspect.Offset) * unitAspect.Speed * deltaTime;

            if(!unitAspect.Reached && 
                math.distance(unitAspect.transform.ValueRO.Position, unitAspect.buffer[unitAspect.CurrentBufferIndex].wayPoint) <= unitAspect.MinDistane 
                && unitAspect.CurrentBufferIndex < unitAspect.buffer.Length - 1)
            {
                unitAspect.CurrentBufferIndex++;
                if(unitAspect.CurrentBufferIndex == unitAspect.buffer.Length - 1) 
                    unitAspect.Reached = true;
            }
            else if(unitAspect.Reached &&
                math.distance(unitAspect.transform.ValueRO.Position, unitAspect.buffer[unitAspect.CurrentBufferIndex].wayPoint) <= unitAspect.MinDistane 
                && unitAspect.CurrentBufferIndex > 0)
            {
                unitAspect.CurrentBufferIndex--;
                if(unitAspect.CurrentBufferIndex == 0) 
                    unitAspect.Reached = false;
            }
        }

    }
}

