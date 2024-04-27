using Unity.Entities;
using UnityEngine;

namespace SeekAndDestroy.Enemies
{
    public class EnemyAuthrong : MonoBehaviour 
    {
        private class Baker : Baker<EnemyAuthrong>
        {
            public override void Bake(EnemyAuthrong authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Enemy());
            }
        }
    }
    
    public struct Enemy: IComponentData{}
}
