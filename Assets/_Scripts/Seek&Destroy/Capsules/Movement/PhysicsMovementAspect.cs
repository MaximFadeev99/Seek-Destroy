using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace SeekAndDestroy.Capsules
{
    public readonly partial struct PhysicsMovementAspect : IAspect
    {
        public readonly RefRO<MovementParameters> MovementParameters;
        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRW<PhysicsVelocity> PhysicsVelocity;

        public void Rotate(float rotationXInput)
        {
            float3 newAngular = new(0f, -rotationXInput * MovementParameters.ValueRO.RotationSpeedOnPhysics,
                0f);

            PhysicsVelocity.ValueRW.Angular = newAngular;
        }

        public void Move(float2 movementInput, float deltaTime)
        {
            float3 movementDirection = ConstructMovementDirection(movementInput);

            PhysicsVelocity.ValueRW.Linear = 
                (movementDirection * MovementParameters.ValueRO.MovementSpeed  * deltaTime);            
        }

        private float3 ConstructMovementDirection(float2 movementInput)
        {
            float3 movementDirection = float3.zero;

            if (movementInput.y != 0f)
            {
                movementDirection = new float3()
                {
                    x = Transform.ValueRO.Forward().x * movementInput.y,
                    y = 0f,
                    z = Transform.ValueRO.Forward().z * movementInput.y
                };
            }

            if (movementInput.x != 0f)
            {
                if (movementInput.y == 0f)
                {
                    movementDirection = new float3()
                    {
                        x = Transform.ValueRO.Right().x * movementInput.x,
                        y = 0f,
                        z = Transform.ValueRO.Right().z * movementInput.x
                    };
                }
                else
                {
                    movementDirection = math.lerp
                        (movementDirection,
                        new float3()
                        {
                            x = Transform.ValueRO.Right().x * movementInput.x,
                            y = 0f,
                            z = Transform.ValueRO.Right().z * movementInput.x
                        }, 0.5f);
                }
            }

            return movementDirection;
        }
    } 
}