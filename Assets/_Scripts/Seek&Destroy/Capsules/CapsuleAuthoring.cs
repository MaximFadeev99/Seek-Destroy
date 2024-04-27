using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SeekAndDestroy.Capsules
{
    public class CapsuleAuthoring : MonoBehaviour
    {
        [SerializeField] private int _startLevel = 1;
        
        private class Baker : Baker<CapsuleAuthoring>
        {
            public override void Bake(CapsuleAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Capsule
                {
                    Level = authoring._startLevel
                });
            }
        }
    }

    public struct Capsule : IComponentData 
    {
        public int Level;
    }
}
