using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using SeekAndDestroy.Points;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace SeekAndDestroy.Points
{
    public partial struct PointSpawningSystem : ISystem, ISystemStartStop
    {
        private PointSpawner _pointSpawner;
        private EntityCommandBuffer _entityCommandBuffer;
        private NativeArray<Entity> _pointEntities;
        private EntityQuery _pointEntitesQuery;
        private int _upheldAmount;

        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<PointSpawner>();
            _pointEntitesQuery = SystemAPI.QueryBuilder().WithAll<Point>().Build();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state) 
        {
            _pointSpawner = SystemAPI.GetSingleton<PointSpawner>();
            _pointEntities = new NativeArray<Entity>(_pointSpawner.AmountToSpawn, Allocator.Temp);
            _upheldAmount = _pointSpawner.AmountToSpawn;
            state.EntityManager.Instantiate(_pointSpawner.Point, _pointEntities);

            foreach (Entity entity in _pointEntities)
            {
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                localTransform.ValueRW.Position = new float3
                {
                    x = UnityEngine.Random.Range(2f, 98f),
                    y = 1.05f,
                    z = UnityEngine.Random.Range(2f, 98f)
                };
            }
        }

        public void OnUpdate (ref SystemState state) 
        {
            _pointEntities = _pointEntitesQuery.ToEntityArray(Allocator.Temp);

            if (_pointEntities.Length == _upheldAmount) 
                return;

            int additionalAmountToSpawn = _upheldAmount - _pointEntities.Length;
            _entityCommandBuffer = new(Allocator.Temp);

            for (int i = 0; i < additionalAmountToSpawn; i++) 
            {
                Entity newEntity = _entityCommandBuffer.Instantiate(_pointSpawner.Point);
                float3 newPosition = new()
                {
                    x = UnityEngine.Random.Range(2f, 98f),
                    y = 1.05f,
                    z = UnityEngine.Random.Range(2f, 98f)
                };
                _entityCommandBuffer.SetComponent(newEntity, new LocalTransform 
                {
                    Position = newPosition,
                    Scale = 1f               
                });
            }

            _entityCommandBuffer.Playback(state.EntityManager);
            _entityCommandBuffer.Dispose();
        }

        public void OnStopRunning(ref SystemState state)
        {

        }
    }
}
