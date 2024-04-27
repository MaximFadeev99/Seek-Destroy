using SeekAndDestroy.Input;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace SeekAndDestroy.UI.LevelMarking
{
    public class LevelMarkPlacer : MonoBehaviour
    {
        [SerializeField] private LevelMark _levelMarkPrefab;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private UnityEngine.Camera _camera;

        private readonly Dictionary<Entity, LevelMark> _markedCapsules = new();
        private readonly List<LevelMark> _markPool = new();

        private LevelMarkingSystem _levelMarkingSystem;
        private EntityManager _entityManager;

        private void Awake()
        {
            _levelMarkingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelMarkingSystem>();
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _camera = UnityEngine.Camera.main;
        }

        private void OnEnable()
        {
            _levelMarkingSystem.CapsuleCreated += OnCapsuleCreated;
            _levelMarkingSystem.CapsuleDestroyed += OnCapsuleDestroyed;
            _levelMarkingSystem.CapsuleLevelChanged += OnCapsuleLevelChanged;
            _levelMarkingSystem.EnemyCapsuleSurpassedPlayer += OnEnemyCapsuleSurpassedPlayer;
            _levelMarkingSystem.EnemyCapsuleGotBehindPlayer += OnEnemyCapsuleGotBehindPlayer;
        }

        private void OnDisable()
        {
            _levelMarkingSystem.CapsuleCreated -= OnCapsuleCreated;
            _levelMarkingSystem.CapsuleDestroyed -= OnCapsuleDestroyed;
            _levelMarkingSystem.CapsuleLevelChanged -= OnCapsuleLevelChanged;
            _levelMarkingSystem.EnemyCapsuleSurpassedPlayer -= OnEnemyCapsuleSurpassedPlayer;
            _levelMarkingSystem.EnemyCapsuleGotBehindPlayer -= OnEnemyCapsuleGotBehindPlayer;
        }

        private void Update()
        {
            foreach (KeyValuePair<Entity, LevelMark> markedCapsule in _markedCapsules)
            {             
                SetPositionOnCanvas(markedCapsule.Key);
            }
        }

        private void OnCapsuleCreated(Entity entity, int intialLevel) 
        {
            LevelMark newMark = GetIdleMark();
            _markedCapsules.Add(entity, newMark);
            newMark.SetLevel(intialLevel);
            newMark.gameObject.SetActive(true);
            SetPositionOnCanvas(entity);
        }

        private void OnCapsuleDestroyed(Entity entity) 
        {           
            _markedCapsules[entity].gameObject.SetActive(false);
            _markedCapsules.Remove(entity);
        }

        private void OnCapsuleLevelChanged(Entity entity, int newLevel) 
        {
            _markedCapsules[entity].SetLevel(newLevel);
        }

        private void OnEnemyCapsuleSurpassedPlayer(Entity entity) 
        {
            _markedCapsules[entity].SetColor(Color.red);
        }

        private void OnEnemyCapsuleGotBehindPlayer(Entity entity) 
        {
            _markedCapsules[entity].SetColor(Color.black);
        }

        private void SetPositionOnCanvas(Entity entity) 
        {
            if (_entityManager.Exists(entity) == false ||
                _entityManager.HasComponent(entity, typeof(LocalToWorld)) == false)
            {
                return;
            }

            LocalToWorld LocalToWorld = _entityManager.GetComponentData<LocalToWorld>(entity);

            bool isEntityInViewport = true;

            float distanceToFarFrustrum = GeometryUtility
                .CalculateFrustumPlanes(_camera)[5]
                .GetDistanceToPoint(LocalToWorld.Position);

            if (distanceToFarFrustrum > _camera.farClipPlane) 
            {
                isEntityInViewport = false;
            }

            if (isEntityInViewport)
            {
                _markedCapsules[entity].gameObject.SetActive(true);
                _markedCapsules[entity].SetPosition(LocalToWorld.Position, _canvasTransform);
                _markedCapsules[entity].SetScale(_camera.farClipPlane, distanceToFarFrustrum);
            }
            else 
            {
                _markedCapsules[entity].gameObject.SetActive(false);
            }
        }

        private LevelMark GetIdleMark() 
        {
            LevelMark idleMark = _markPool.FirstOrDefault(levelMark => levelMark.gameObject.activeSelf == false);

            if (idleMark != null)
                return idleMark;

            idleMark = Instantiate(_levelMarkPrefab);
            idleMark.Initialize();
            _markPool.Add(idleMark);
            return idleMark;
        }
    }
}
