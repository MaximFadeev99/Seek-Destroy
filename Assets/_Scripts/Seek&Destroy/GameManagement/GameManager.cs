using Cysharp.Threading.Tasks;
using SeekAndDestroy.UI;
using SeekAndDestroy.UI.LevelMarking;
using TMPro;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeekAndDestroy.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TMP_Text _gameOverLabel;
        [SerializeField] private ReloadButton _reloadButton;
        [SerializeField] private SubScene _currentSubScene;
        [SerializeField] private LevelMarkPlacer _levelMarkPlacer;

        private GameManagementSystem _gameManagementSystem;

        private void Awake()
        {
            _gameManagementSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<GameManagementSystem>();
        }

        private void OnEnable()
        {
            _gameManagementSystem.GameEnded += OnGameOver;
            _gameManagementSystem.SubSceneReloaded += OnSubSceneReloaded;
        }

        private void OnDisable()
        {
            _gameManagementSystem.GameEnded -= OnGameOver;
            _gameManagementSystem.SubSceneReloaded -= OnSubSceneReloaded;
        }

        private void OnGameOver(bool isPlayerDead) 
        {
            _gameOverLabel.text = isPlayerDead ? "You are dead!" : "You are victoriuos!";
            _gameOverPanel.SetActive(true);
            _reloadButton.SceneReloadOrdered += _gameManagementSystem.OrderSceneReload;
        }

        private void OnSubSceneReloaded() 
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }      
    }
}