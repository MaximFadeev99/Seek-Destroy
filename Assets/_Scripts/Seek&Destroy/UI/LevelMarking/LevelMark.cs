using TMPro;
using UnityEngine;
using SeekAndDestroy.Utilities;

namespace SeekAndDestroy.UI.LevelMarking
{
    [RequireComponent(typeof(TMP_Text), typeof(RectTransform))]
    public class LevelMark : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textField;
        [SerializeField] private Vector2 _correctionOffset;

        private RectTransform _rectTransform;
        private UnityEngine.Camera _mainCamera;

        public void Initialize()
        {
            _textField = GetComponent<TMP_Text>();
            _rectTransform = GetComponent<RectTransform>();
            _mainCamera = UnityEngine.Camera.main;
        }

        public void SetLevel(int newLevel) 
        {
            _textField.text = newLevel.ToString();
        }

        public void SetColor(Color newColor) 
        {
            _textField.color = newColor;
        }

        public void SetPosition(Vector3 worldPosition, RectTransform canvasTransform) 
        {
            worldPosition += (Vector3) _correctionOffset;
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

            _rectTransform.SetParent(canvasTransform);
            _rectTransform.localScale = Vector3.one;
            _rectTransform.localEulerAngles = Vector3.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (canvasTransform, screenPosition, _mainCamera, out Vector2 localPoint);
            _rectTransform.localPosition = localPoint;
        }

        public void SetScale(float farClipPlane, float distanceToLevelMark) 
        {
            float minScale = _textField.text.Length * 0.1f;
            float maxScale = _textField.text.Length * 1f;
            float scale = distanceToLevelMark.Remap(farClipPlane * 0.5f, farClipPlane, minScale, maxScale);

            scale = Mathf.Clamp(scale, minScale, maxScale);
            _rectTransform.localScale = new(scale, scale, scale);
        }
    }
}