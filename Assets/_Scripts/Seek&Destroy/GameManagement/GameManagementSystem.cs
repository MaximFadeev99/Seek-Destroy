using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Scenes;
using static UnityEngine.Time;
using static Unity.Scenes.SceneSystem;
using SeekAndDestroy.Enemies;

namespace SeekAndDestroy.GameManagement
{
    [UpdateInGroup(typeof(PhysicsInitializeGroup))]
    public partial class GameManagementSystem : SystemBase
    {
        private MainPlayer.Player _player;
        private Entity _sceneEntity;
        private bool _isReloadingOrdered;
        private Hash128 _sceneGUID;
        private int _remainingEnemyCount;

        public Action<bool> GameEnded;
        public Action SubSceneReloaded;

        protected override void OnCreate()
        {
            _sceneGUID = SceneSystem.GetSceneGUID(ref CheckedStateRef,
                        "Assets/Scenes/Seek&Destroy/Seek&DestroyEntitySubscene.unity");
        }

        protected override void OnUpdate()
        {
            if (SystemAPI.TryGetSingleton(out _player) == false)
                return;

            _remainingEnemyCount = GetRemainingEnemyCount();

            if ((_player.IsAlive == false && _isReloadingOrdered == false) ||
                (_player.IsAlive && _remainingEnemyCount == 0 && _isReloadingOrdered == false))
            {
                GameEnded?.Invoke(!_player.IsAlive);
                timeScale = 0f;
            }

            if (_isReloadingOrdered)
            {
                TryReloadCurrentSubScene();
            }           
        }

        public void OrderSceneReload() 
        {
            _isReloadingOrdered = true;
            timeScale = 1.0f;
        }

        //Moments to take into account when loading / reloading SubScenes

        //1) SceneSystem can be accessed both from ISystem and SystemBase as well as from Monobehaviours. 
        //However, we should not try to load or reload subscenes from Monobehaviours as it results in unexpected behavior.
        //For example, IsSceneLoaded method didn't show correct results when called from a Monobehavior.

        //2) On reloading a SubScene, we should unload it first. Otherwise, no changes will happen.

        //3) Setting Time.timeScale to 0f seems to disable all updates in all Systems. If we meddle with timeScale, 
        //we should make sure to reset it to 1f, before we expect any System to run. UnityEngine.Time is accessible from 
        //Systems.

        //4)Unitechs say that getting a SceneGUID is resource consuming, so we should use SceneEntityReference as 
        //much as possible. SceneEntityReference can be created if we have a reference to the target SubScene. We need
        //to provide SubScene.SubSceneAsset when creating a SceneEntityReference.
        private void TryReloadCurrentSubScene() 
        {
            if (_sceneEntity == Entity.Null)
            {
                SceneSystem.UnloadScene(World.Unmanaged, _sceneGUID, UnloadParameters.DestroyMetaEntities);
                _sceneEntity = SceneSystem.LoadSceneAsync(World.Unmanaged, _sceneGUID);
            }

            if (IsSceneLoaded(World.Unmanaged, _sceneEntity))
            {
                _isReloadingOrdered = false;
                _sceneEntity = Entity.Null;
                SubSceneReloaded?.Invoke();
            }
        }

        private int GetRemainingEnemyCount() 
        {
            int remainingEnemyCount = 0;

            foreach (RefRO<Enemy> enemy in SystemAPI.Query<RefRO<Enemy>>())
            {
                remainingEnemyCount++;
            }

            return remainingEnemyCount;
        }
    }
}