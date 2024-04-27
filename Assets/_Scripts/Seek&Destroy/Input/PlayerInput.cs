using Unity.Entities;
using Unity.Mathematics;

namespace SeekAndDestroy.Input
{   
    public struct PlayerInput : IComponentData
    {
        public float2 MovementInput;
        public float2 RotationInput;
        public bool IsDashing;
    }
}
