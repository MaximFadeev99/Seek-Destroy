using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

namespace SeekAndDestroy.Input
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    //custom entities are updated in SimulationSystemGroup,
    //so the update of this system will happen before that
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerActionMap _playerActionMap;
        private float2 _currentMovementInput;
        private float2 _currentRotationInput;

        protected override void OnCreate()
        {
            RequireForUpdate<MainPlayer.Player>();
            _playerActionMap = new ();
            EntityManager.CreateSingleton(new PlayerInput());
        }

        protected override void OnStartRunning()
        {
            _playerActionMap.Enable();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            _currentMovementInput = _playerActionMap.Base.Movement.ReadValue<Vector2>();
            _currentRotationInput = _playerActionMap.Base.Rotation.ReadValue<Vector2>();

            SystemAPI.SetSingleton(new PlayerInput
            {
                MovementInput = _currentMovementInput,
                RotationInput = _currentRotationInput,
            });
        }

        protected override void OnStopRunning()
        {
            _playerActionMap.Disable();
        }
    }
}