using Unity.Entities;
using UnityEngine;

namespace SeekAndDestroy.Points
{
    public class PointAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PointAuthoring>
        {
            public override void Bake(PointAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new Point
                {
                    AddedAmount = 1
                });
            }
        }
    }

    public struct Point : IComponentData , IEnableableComponent
    {
        public int AddedAmount;
    }
}