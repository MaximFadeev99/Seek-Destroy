using Unity.Entities;
using Unity.Transforms;
using SeekAndDestroy.Input;
using Unity.Burst;
using SeekAndDestroy.Capsules;

namespace SeekAndDestroy.MainPlayer
{
    [CreateAfter(typeof(PlayerInputSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {     
        public void OnCreate(ref SystemState state) 
        {
            //Correct way to make queries through EntityQueryBuilder
            //EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp);
            //EntityQuery enemyQuery = queryBuilder.WithAll<Enemy, LocalTransform>().Build(ref state);
            //EntityQuery playerQuery = queryBuilder.WithAll<MainPlayer.Player, LocalTransform>().Build(ref state);

            //Correct way to make queries through SystemAPI.QueryBuilder()
            //you CAN NOT create separately SystemAPIQueryBuilder queryBuilder = SystemAPI.QueryBuilder();
            //_playerInput = SystemAPI.GetSingleton<PlayerInput>();
            EntityQuery playerQuery = SystemAPI.QueryBuilder().WithAll<MainPlayer.Player, LocalTransform>().Build();

            state.RequireForUpdate(playerQuery);
        }

        //Movements on Transform
        //[BurstCompile]
        //public void OnUpdate(ref SystemState state) 
        //{
        //    foreach (TransformMovementAspect transformMovementAspect in SystemAPI.Query<TransformMovementAspect>().WithAll<MainPlayer.Player>()) 
        //    {
        //        PlayerInput playerInput = SystemAPI.GetSingleton<PlayerInput>();

        //        if (playerInput.MovementInput.Equals(float2.zero) == false)
        //            transformMovementAspect.Move(playerInput.MovementInput, SystemAPI.Time.DeltaTime);

        //        if (playerInput.RotationInput.x != 0f)
        //            transformMovementAspect.Rotate(playerInput.RotationInput.x, SystemAPI.Time.DeltaTime);
        //    }
        //}

        //Movements on Physics
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (PhysicsMovementAspect physicsMovementAspect in SystemAPI
                .Query<PhysicsMovementAspect>().WithAll<MainPlayer.Player>())
            {
                PlayerInput playerInput = SystemAPI.GetSingleton<PlayerInput>();

                physicsMovementAspect.Move(playerInput.MovementInput, SystemAPI.Time.DeltaTime);
                physicsMovementAspect.Rotate(playerInput.RotationInput.x);
            }
        }
    }
}
