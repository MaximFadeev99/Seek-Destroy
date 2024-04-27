using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Systems;

namespace SeekAndDestroy.Capsules
{
    [UpdateInGroup(typeof(PhysicsInitializeGroup))]
    public partial struct CapsuleCollisionSystem : ISystem
    {
        private SimulationSingleton _simulationSingleton;
        private JobHandle _jobHandle;

        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<Capsule>();
        }

        public void OnUpdate(ref SystemState state) 
        {
            EntityCommandBuffer entityCommandBuffer = new(Allocator.TempJob);
            _simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            _jobHandle = new();

            state.Dependency = new CapsuleCollisionJob 
            {
                EntityCommandBuffer = entityCommandBuffer,
            }.Schedule(_simulationSingleton, _jobHandle);
            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();       
        }
    }

    public struct CapsuleCollisionJob : ITriggerEventsJob
    {
        public EntityCommandBuffer EntityCommandBuffer;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (entityManager.HasComponent(triggerEvent.EntityA, typeof(Capsule)) &&
                entityManager.HasComponent(triggerEvent.EntityB, typeof(Capsule)))         
            {              
                KeyValuePair<Entity, Capsule> pairA = new
                    (triggerEvent.EntityA, entityManager.GetComponentData<Capsule>(triggerEvent.EntityA));
                KeyValuePair<Entity, Capsule> pairB = new
                    (triggerEvent.EntityB, entityManager.GetComponentData<Capsule>(triggerEvent.EntityB));

                if (math.abs(pairA.Value.Level - pairB.Value.Level) < 1)
                    return;

                KeyValuePair<Entity, Capsule> winner = pairA.Value.Level > pairB.Value.Level ? pairA : pairB;
                KeyValuePair<Entity, Capsule> loser = winner.Equals(pairA) ? pairB : pairA;
                int addedLevel = loser.Value.Level / 10 < 1 ? 1 : loser.Value.Level / 10;

                EntityCommandBuffer.SetComponent(winner.Key, new Capsule 
                {
                    Level = winner.Value.Level + addedLevel,
                });

                if (entityManager.HasComponent<MainPlayer.Player>(loser.Key) == false)
                {
                    EntityCommandBuffer.DestroyEntity(loser.Key);
                }
                else
                {
                    EntityCommandBuffer.SetComponent(loser.Key, new MainPlayer.Player
                    {
                        IsAlive = false
                    });
                }
            }
        }
    }
}
