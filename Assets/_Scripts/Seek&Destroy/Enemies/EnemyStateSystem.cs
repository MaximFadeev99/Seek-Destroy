using SeekAndDestroy.Capsules;
using SeekAndDestroy.Points;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SeekAndDestroy.Enemies
{
    [CreateAfter(typeof(PointSpawningSystem))]
    public partial struct EnemyStateSystem : ISystem
    {
        private Entity _playerEntity;
        private Capsule _playerCapsule;
        private LocalTransform _playerTransform;

        private float3 _globalUp;
        private float3 _movementVector;
        private float2 _movementInput;

        //required for transform movements!!!
        //private quaternion _lookRotation; 

        private float _signedAngle;
        private float _rotationInput;

        public void OnCreate(ref SystemState state)
        {
            _movementInput = new(0f, 1f);
            _globalUp = new(0f, 1f, 0f);

        }

        //movements on transform
        //[BurstCompile]
        //public void OnUpdate(ref SystemState state) 
        //{
        //    if (SystemAPI.TryGetSingletonEntity<MainPlayer.Player>(out _playerEntity) == false)
        //        return;

        //    _playerCapsule = SystemAPI.GetComponent<Capsule>(_playerEntity);
        //    _playerTransform = SystemAPI.GetComponent<LocalTransform>(_playerEntity);

        //    foreach (EnemyAspect enemyAspect in SystemAPI.Query<EnemyAspect>())
        //    {
        //        if (_playerCapsule.Level < enemyAspect.Capsule.ValueRO.Level)
        //        {
        //            _movementVector = math.normalize
        //                (_playerTransform.Position - enemyAspect.MovementAspect.Transform.ValueRO.Position);
        //        }
        //        else
        //        {
        //            _movementVector = GetVectorToPoint
        //                (ref state, enemyAspect.MovementAspect.Transform.ValueRO.Position);
        //        }

        //        _lookRotation = quaternion.LookRotation
        //                (_movementVector, _globalUp);
        //        enemyAspect.MovementAspect.Transform.ValueRW.Rotation = _lookRotation;
        //        enemyAspect.MovementAspect.Move(_movementInput, SystemAPI.Time.DeltaTime);
        //    }
        //}

        //movements on physics
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.TryGetSingletonEntity<MainPlayer.Player>(out _playerEntity) == false)
                return;

            _playerCapsule = SystemAPI.GetComponent<Capsule>(_playerEntity);
            _playerTransform = SystemAPI.GetComponent<LocalTransform>(_playerEntity);

            foreach (EnemyAspect enemyAspect in SystemAPI.Query<EnemyAspect>())
            {
                if (_playerCapsule.Level < enemyAspect.Capsule.ValueRO.Level)
                {
                    _movementVector = math.normalize
                        (_playerTransform.Position - enemyAspect.MovementAspect.Transform.ValueRO.Position);
                }
                else
                {
                    _movementVector = GetVectorToPoint
                        (ref state, enemyAspect.MovementAspect.Transform.ValueRO.Position);
                }

                _signedAngle = Vector3.SignedAngle
                    (_movementVector, enemyAspect.MovementAspect.Transform.ValueRO.Forward(), _globalUp);
                _rotationInput = _signedAngle <= 0 ? 10f : -10f;

                enemyAspect.MovementAspect.Rotate(_rotationInput);
                enemyAspect.MovementAspect.Move(_movementInput, SystemAPI.Time.DeltaTime);
            }
        }

        private float3 GetVectorToPoint(ref SystemState state, float3 enemyWorldPosition)
        {
            float3 endPosition = float3.zero;
            float maxDistance = float.MaxValue;
            float currentDistance;

            foreach (RefRO<LocalTransform> localTransform in SystemAPI.Query<RefRO<LocalTransform>>()
                .WithAll<Point>()) 
            {
                currentDistance = math.distance(localTransform.ValueRO.Position, enemyWorldPosition);

                if (currentDistance < maxDistance) 
                {
                    endPosition = localTransform.ValueRO.Position;
                    maxDistance = currentDistance;
                }
            }

            return endPosition - enemyWorldPosition;
        }
    }
}