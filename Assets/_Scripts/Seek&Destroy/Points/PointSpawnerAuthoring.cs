using Unity.Entities;
using UnityEngine;

namespace SeekAndDestroy.Points
{
    public class PointSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _pointPrefab;
        [SerializeField] private int _amountToSpawn;

        private class Baker : Baker<PointSpawnerAuthoring>
        {
            public override void Bake(PointSpawnerAuthoring authoring)
            {
                Entity point = GetEntity(authoring._pointPrefab, TransformUsageFlags.None);
                AddComponent(GetEntity(TransformUsageFlags.None), new PointSpawner
                {
                    Point = point,
                    AmountToSpawn = authoring._amountToSpawn
                });
            }
        }
    }

    public struct PointSpawner : IComponentData 
    {
        public Entity Point;
        public int AmountToSpawn;
    }
}
