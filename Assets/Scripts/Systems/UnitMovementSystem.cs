

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
    private void Execute(UnitAgentAspect unitAspect)
    {

        if(unitAspect.NavAgentRouted && unitAspect.buffer.Length > 0)
        {

            unitAspect.CorrectedWayPoint = unitAspect.buffer[unitAspect.CurrentBufferIndex].wayPoint;

            /* -------Collision Avoidance----------
            //unitAspect.TargetAvoidance = unitAspect.CorrectedWayPoint;

            //unitAspect.transform.ValueRW.Position = math.lerp(
            //        unitAspect.transform.ValueRO.Position,
            //        (unitAspect.transform.ValueRO.Position + new float3(unitAspect.VelocityAvoidance.x, 0, unitAspect.VelocityAvoidance.z)),
            //        deltaTime);
            */

            //------Defolt NavMeshMovement----Comment this for CollisionAvoidanceMovement
            unitAspect.WayPointDirection =
                math.normalize((unitAspect.CorrectedWayPoint) - unitAspect.transform.ValueRO.Position);

            unitAspect.transform.ValueRW.Position +=
                math.normalize(unitAspect.WayPointDirection + unitAspect.Offset) * unitAspect.Speed * deltaTime;
            //------------------------------

            if (!unitAspect.Reached && 
                math.distance(unitAspect.transform.ValueRO.Position, unitAspect.CorrectedWayPoint) <= unitAspect.MinDistane 
                && unitAspect.CurrentBufferIndex < unitAspect.buffer.Length - 1)
            {
                unitAspect.CurrentBufferIndex++;
                if(unitAspect.CurrentBufferIndex == unitAspect.buffer.Length - 1) 
                    unitAspect.Reached = true;
            }
            else if(unitAspect.Reached &&
                math.distance(unitAspect.transform.ValueRO.Position, unitAspect.CorrectedWayPoint) <= unitAspect.MinDistane 
                && unitAspect.CurrentBufferIndex > 0)
            {
                unitAspect.CurrentBufferIndex = unitAspect.CurrentBufferIndex - 1;
                if(unitAspect.CurrentBufferIndex == 0) 
                    unitAspect.Reached = false;
            }
        }

    }
}

