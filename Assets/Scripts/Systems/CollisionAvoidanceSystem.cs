
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(UnitMovementSystem))]
public partial struct CollisionAvoidanceSystem: ISystem
{
    private NativeParallelMultiHashMap<int, CollisionAvoidanceComponent> cellVsEntityPos;

    public static int GetUniqueKeyForPosition(float3 pos, int cellSize) =>
        (int)((15 * math.floor(pos.x / cellSize)) + (17 * math.floor(pos.y / cellSize)) + (19 * math.floor(pos.z / cellSize)));


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        cellVsEntityPos = new NativeParallelMultiHashMap<int, CollisionAvoidanceComponent>(0, Allocator.Persistent);

        //Delete state.enabled if you want activate system
        state.Enabled = false;
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityQuery eq = state.GetEntityQuery(typeof(CollisionAvoidanceComponent));
        cellVsEntityPos.Clear();

        if (eq.CalculateEntityCount() > cellVsEntityPos.Capacity)
            cellVsEntityPos.Capacity = eq.CalculateEntityCount();

        new ParallelizeCellVsEntityPosJob
        {
            cellVsEntityPosParallel = cellVsEntityPos.AsParallelWriter()
        }.ScheduleParallel();

        new BoidsJob
        {
            cellVsEntityPositionsForJob = cellVsEntityPos
        }.ScheduleParallel();
    }

    public void OnDestroy(ref SystemState state)
    {
        cellVsEntityPos.Dispose();
    }

    [BurstCompile]
    public partial struct ParallelizeCellVsEntityPosJob: IJobEntity
    {
        public NativeParallelMultiHashMap<int, CollisionAvoidanceComponent>.ParallelWriter cellVsEntityPosParallel;
        private void Execute(UnitAgentAspect unitAgent)
        {
            unitAgent.CurrentPositionAvoidance = unitAgent.transform.ValueRO.Position;
            cellVsEntityPosParallel
                .Add(
                GetUniqueKeyForPosition(unitAgent.transform.ValueRO.Position, unitAgent.CellSize),
                unitAgent._collisionAvoidance.ValueRO);
        }
    }

    [BurstCompile]
    public partial struct BoidsJob : IJobEntity
    {
        [ReadOnly]
        public NativeParallelMultiHashMap<int, CollisionAvoidanceComponent> cellVsEntityPositionsForJob;
        private void Execute(UnitAgentAspect unitAgent)
        {
            int key = GetUniqueKeyForPosition(unitAgent.transform.ValueRO.Position, unitAgent.CellSize);
            NativeParallelMultiHashMapIterator<int> nmhKeyIterator;
            CollisionAvoidanceComponent neighbor;

            int total = 0;
            float3 separation = float3.zero;
            float3 alignment= float3.zero;
            float3 cohesion= float3.zero;


            unitAgent.VelocityAvoidance = 
                unitAgent.TargetAvoidance - unitAgent.transform.ValueRO.Position;

            if(cellVsEntityPositionsForJob.TryGetFirstValue(key, out neighbor, out nmhKeyIterator))
            {
                do
                {
                    if (!unitAgent.transform.ValueRO.Position.Equals(neighbor.currentPosition) && 
                        math.distance(unitAgent.transform.ValueRO.Position, neighbor.currentPosition) < unitAgent.PerceptionRadius)
                    {
                        float3 distanceFromTo = unitAgent.transform.ValueRO.Position - neighbor.currentPosition;
                        separation += (distanceFromTo / math.distance(unitAgent.transform.ValueRO.Position, neighbor.currentPosition));
                        cohesion += neighbor.currentPosition;
                        alignment += neighbor.velocity;
                        total++;
                    }
                } while (cellVsEntityPositionsForJob.TryGetNextValue(out neighbor, ref nmhKeyIterator));
                if (total > 0)
                {
                    cohesion = cohesion / total;
                    cohesion = cohesion - (unitAgent.transform.ValueRO.Position + unitAgent.VelocityAvoidance);
                    cohesion = math.normalize(cohesion) * unitAgent.CohesionBias;

                    separation = separation / total;
                    separation = separation - unitAgent.VelocityAvoidance;
                    separation = math.normalize(separation) * unitAgent.SeparationBias;

                    alignment = alignment / total;
                    alignment = alignment - unitAgent.VelocityAvoidance;
                    alignment = math.normalize(alignment) * unitAgent.AllignmentBias;

                }

                unitAgent.AccelerationAvoidance += (cohesion + separation + alignment);
                unitAgent.VelocityAvoidance = unitAgent.VelocityAvoidance + unitAgent.AccelerationAvoidance;
                unitAgent.VelocityAvoidance = math.normalize(unitAgent.VelocityAvoidance) * unitAgent.Speed;
                unitAgent.AccelerationAvoidance =
                    math.normalize(unitAgent.TargetAvoidance - unitAgent.transform.ValueRO.Position) * unitAgent.TargetBias;
            }
        }
    }
}

