using SeekAndDestroy.Capsules;
using SeekAndDestroy.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace SeekAndDestroy.UI
{
    public partial class LevelMarkingSystem : SystemBase
    {
        public Action<Entity, int> CapsuleCreated;
        public Action<Entity> CapsuleDestroyed;
        public Action<Entity, int> CapsuleLevelChanged;
        public Action<Entity> EnemyCapsuleSurpassedPlayer;
        public Action<Entity> EnemyCapsuleGotBehindPlayer;

        public NativeArray<Entity> _allEntities;

        public NativeHashMap<Entity, int> _capsuleLevelDictionary; 

        protected override void OnCreate()
        {
            RequireForUpdate<Capsule>();
        }

        protected override void OnStartRunning()
        {
            _capsuleLevelDictionary = new(1, Allocator.Persistent);
            RegisterNewCapsules(EntityManager.GetAllEntities().ToArray());
        }

        protected override void OnUpdate()
        {
            _allEntities = EntityManager.GetAllEntities();
            NativeArray<Entity> registeredEntities = _capsuleLevelDictionary.GetKeyArray(Allocator.Temp);
            Entity[] unregisteredEntities = registeredEntities.Except(_allEntities).ToArray();

            foreach (Entity entity in unregisteredEntities) 
            {
                _capsuleLevelDictionary.Remove(entity);
                CapsuleDestroyed?.Invoke(entity);
            }

            unregisteredEntities = _allEntities.Except(registeredEntities).ToArray();
            RegisterNewCapsules(unregisteredEntities);
            CheckIfLevelChanged();
        }

        private void RegisterNewCapsules(params Entity[] allEntities) 
        {
            foreach (Entity entity in allEntities)
            {
                if (EntityManager.HasComponent(entity, typeof(Capsule)) && 
                    _capsuleLevelDictionary.ContainsKey(entity) == false)
                {                 
                    Capsule capsule = EntityManager.GetComponentData<Capsule>(entity);
                    CapsuleCreated?.Invoke(entity, capsule.Level);
                    _capsuleLevelDictionary.Add(entity, capsule.Level);
                }
            }
        }

        private void CheckIfLevelChanged()
        {
            Entity playerEntity = SystemAPI.GetSingletonEntity<MainPlayer.Player>();
            Capsule playerCapsule = EntityManager.GetComponentData<Capsule>(playerEntity);
            int loopCount = _capsuleLevelDictionary.Count; //walkaround for an internal bug. 
            //after the last element of the foreach loop in _capsuleLevelDictionary, the programme as usual 
            //returns to foreach one last time to make sure that there are no elements left. 
            //During this final iteration an error pops up saying that the whole _capsuleLevelDictionary has been deallocated.
            //The error seems to be the result of the absence of another element in the Dictionary.

            foreach (KVPair<Entity, int> capsule in _capsuleLevelDictionary)
            {
                Capsule newCapsule = EntityManager.GetComponentData<Capsule>(capsule.Key);

                if (newCapsule.Level != capsule.Value)
                {
                    _capsuleLevelDictionary[capsule.Key] = newCapsule.Level;
                    CapsuleLevelChanged?.Invoke(capsule.Key, _capsuleLevelDictionary[capsule.Key]);
                }

                if (capsule.Value > playerCapsule.Level && capsule.Key != playerEntity)
                {
                    EnemyCapsuleSurpassedPlayer?.Invoke(capsule.Key);
                }
                else
                {
                    EnemyCapsuleGotBehindPlayer?.Invoke(capsule.Key);
                }

                loopCount--;

                if (loopCount == 0)
                    break;
            }
        }

        protected override void OnDestroy()
        {
            _capsuleLevelDictionary.Dispose();
        }
    }
}
