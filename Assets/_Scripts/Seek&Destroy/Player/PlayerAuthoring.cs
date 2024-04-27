using UnityEngine;
using Unity.Entities;

namespace SeekAndDestroy.MainPlayer
{
    public class PlayerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Player 
                {
                    IsAlive = true
                });
            }
        }
    }

    public struct Player : IComponentData
    {
        public bool IsAlive;
    }
}