using SeekAndDestroy.Capsules;
using Unity.Entities;

namespace SeekAndDestroy.Enemies
{
    public readonly partial struct EnemyAspect : IAspect
    {
        public readonly RefRO<Capsule> Capsule;
        public readonly RefRO<Enemy> Enemy;
        public readonly PhysicsMovementAspect MovementAspect;
    }
}