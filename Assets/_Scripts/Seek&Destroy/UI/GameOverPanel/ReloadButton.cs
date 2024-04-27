using UnityEngine;
using UnityEngine.UI;
using System;

namespace SeekAndDestroy.UI
{
    [RequireComponent(typeof(Button))]
    public class ReloadButton : MonoBehaviour
    {              
        private Button _reloadButton;

        public Action SceneReloadOrdered;
        
        private void Awake()
        {
            _reloadButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _reloadButton.onClick.AddListener(ReloadCurrentScene);
        }

        private void OnDisable()
        {
            _reloadButton.onClick.RemoveListener(ReloadCurrentScene);
        }

        private void ReloadCurrentScene() 
        {
            SceneReloadOrdered?.Invoke(); 
        }
    }
}
