using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

namespace SeekAndDestroy.Camera
{
    public class FollowMark : MonoBehaviour
    {
        private EntityQuery _playerQuery;
        private float3 _previousPosition = float3.zero;
        private quaternion _previousRotation;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _playerQuery = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(MainPlayer.Player), typeof(LocalToWorld));
        }

        //When moving on Physics and using Cinemachine, we must use LocalToWorld component instead of 
        //LocalTransform since the first is used for rendering. Using LocalTransform will cause server 
        //jitters. We also will see jitters if updating rotation and position in anything, but Update().
        //Cinemachine update should be set to LateUpdate. 
        private void Update()
        {
            if (_playerQuery.TryGetSingleton(out LocalToWorld localToWorld) == false)
                return;

            if (localToWorld.Position.Equals(_previousPosition) == false)
            {
                _transform.position = Vector3.Lerp(_previousPosition, localToWorld.Position, 0.05f);
                _previousPosition = _transform.position;
            }

            if (localToWorld.Rotation.Equals(_previousRotation) == false)
            {
                _transform.rotation = localToWorld.Rotation;
                _previousRotation = localToWorld.Rotation;
            }
        }
    }
}
