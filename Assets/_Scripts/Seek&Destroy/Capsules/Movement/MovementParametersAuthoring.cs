using UnityEngine;
using Unity.Entities;

namespace SeekAndDestroy.Capsules
{
    public class MovementParametersAuthoring : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _rotationSpeedOnTransform;
        [SerializeField] private float _rotationSpeedOnPhysics = 0.5f;
        
        private class Baker : Baker<MovementParametersAuthoring>
        {
            public override void Bake(MovementParametersAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new MovementParameters()
                {
                    MovementSpeed = authoring._movementSpeed,
                    RotationSpeedOnTransform = authoring._rotationSpeedOnTransform,
                    RotationSpeedOnPhysics = authoring._rotationSpeedOnPhysics,
                });
            }
        }
    }

    public struct MovementParameters : IComponentData 
    {
        public float MovementSpeed;
        public float RotationSpeedOnTransform; //this parameter for transform movements
        public float RotationSpeedOnPhysics; //this parameter for physics movements
    }
}
