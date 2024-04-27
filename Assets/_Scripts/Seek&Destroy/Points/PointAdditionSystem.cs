using SeekAndDestroy.Capsules;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine;

namespace SeekAndDestroy.Points
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PointAdditionSystem : ISystem
    {
        //private TriggerEvents _triggerEvents;
        //private ComponentLookup<Point> _entityPointDictionary;
        //private ComponentLookup<Capsule> _entityCapsuleDictionary;

        private SimulationSingleton _simulationSingleton;
        private JobHandle _jobHandle;

        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<PointSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) 
        {
            //Trigger for foreach loop         
            
            //_triggerEvents = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation().TriggerEvents;
            //state.Dependency.Complete();
            //_entityPointDictionary = SystemAPI.GetComponentLookup<Point>(true);
            //_entityCapsuleDictionary = SystemAPI.GetComponentLookup<Capsule>(true);

            //foreach (TriggerEvent triggerEvent in _triggerEvents)
            //{               
            //    if (_entityPointDictionary.TryGetComponent(triggerEvent.EntityB, out Point point) &&
            //        _entityCapsuleDictionary.TryGetComponent(triggerEvent.EntityA, out Capsule capsule)) 
            //    {
            //        SystemAPI.SetComponent(triggerEvent.EntityA, new Capsule
            //        {
            //            Level = capsule.Level + point.AddedAmount
            //        });

            //        state.EntityManager.DestroyEntity(triggerEvent.EntityB);
            //    }                
            //}


            //Trigger for ITriggerEventsJob

            EntityCommandBuffer entityCommandBuffer = new(Allocator.TempJob);
            _simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            _jobHandle = new();
            state.Dependency = new AddOnTriggerJob 
            {
                EntityCommandBuffer = entityCommandBuffer,
            }.Schedule(_simulationSingleton, _jobHandle);

            state.Dependency.Complete();
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
    
    public struct AddOnTriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (entityManager.HasComponent(triggerEvent.EntityB, typeof(Point)) && 
                entityManager.HasComponent(triggerEvent.EntityA, typeof(Capsule)))
            {
                Point point = entityManager.GetComponentData<Point>(triggerEvent.EntityB);
                int currentLevel = entityManager.GetComponentData<Capsule>(triggerEvent.EntityA).Level;
                EntityCommandBuffer.SetComponent(triggerEvent.EntityA, new Capsule
                {
                    Level = currentLevel + point.AddedAmount
                });

                EntityCommandBuffer.DestroyEntity(triggerEvent.EntityB);
            }
        }
    }
}
