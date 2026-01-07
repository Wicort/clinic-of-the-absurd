using System.Collections.Generic;
using UnityEngine;

public class HospitalManager : MonoBehaviour
{
    public static HospitalManager Instance { get; private set; }

    [Header("Floors")]
    [SerializeField] private GameObject _floor1Prefab;
    [SerializeField] private GameObject _floor2Prefab;
    [SerializeField] private GameObject _floor3Prefab;
    [SerializeField] private Transform _floorContainer;

    [Header("Ward System")]
    [SerializeField] private PatientDatabase _patientDatabase;
    [SerializeField] private GameObject _wardRoomPrefab;
    [SerializeField] private Transform _wardZone;

    // Состояния палат (сохраняются между этажами)
    private Dictionary<string, WardState> _wardStates = new Dictionary<string, WardState>();

    // Текущие объекты
    private GameObject _currentFloorInstance;
    private GameObject _currentWardInstance;
    private Transform _currentSpawnPoint;
    private GameObject _currentHallVisuals; // ← вот оно!
    private int _currentFloorIndex = 1;
    private bool _isWardActive = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadFloor(1);
    }

    public void LoadFloor(int floorIndex)
    {
        if (_currentFloorInstance != null)
            Destroy(_currentFloorInstance);

        GameObject floorPrefab = floorIndex switch
        {
            1 => _floor1Prefab,
            2 => _floor2Prefab,
            3 => _floor3Prefab,
            _ => _floor1Prefab
        };

        _currentFloorInstance = Instantiate(floorPrefab, _floorContainer);
        _currentFloorIndex = floorIndex;

        // 🔥 Ищем HallVisuals внутри нового этажа
        _currentHallVisuals = _currentFloorInstance.transform.Find("HallVisuals")?.gameObject;
        if (_currentHallVisuals == null)
        {
            Debug.LogWarning($"HallVisuals not found in floor {floorIndex} prefab!");
        }
    }

    public void EnterWard(string doorId, int floorLevel, Transform spawnPoint)
    {
        if (_isWardActive) return;

        _currentSpawnPoint = spawnPoint;

        if (!_wardStates.TryGetValue(doorId, out WardState state))
        {
            state = new WardState(doorId);
            _wardStates[doorId] = state;
        }

        PatientProfile patient;

        if (!state.hasBeenEntered)
        {
            DifficultyLevel level = (DifficultyLevel)floorLevel;
            patient = _patientDatabase.GetRandomPatient(level);
            if (patient == null)
            {
                Debug.LogError($"No patient for floor {floorLevel}, door {doorId}");
                return;
            }
            state.assignedPatientAssetName = patient.name;
            state.hasBeenEntered = true;
        }
        else
        {
            patient = _patientDatabase.FindPatientByName(state.assignedPatientAssetName);
            if (patient == null)
            {
                Debug.LogError($"Patient not found: {state.assignedPatientAssetName}");
                return;
            }
        }

        // 🔥 Скрываем визуал ТЕКУЩЕГО этажа
        if (_currentHallVisuals != null)
            _currentHallVisuals.SetActive(false);

        if (_currentWardInstance != null)
            Destroy(_currentWardInstance);

        _currentWardInstance = Instantiate(_wardRoomPrefab, Vector3.zero, Quaternion.identity, _wardZone);
        _isWardActive = true;

        WardRoom wardRoom = _currentWardInstance.GetComponent<WardRoom>();
        if (wardRoom != null)
        {
            wardRoom.Initialize(patient, doorId, state.isCured);
        }
        else
        {
            Debug.LogError("WardRoom component missing!");
            return;
        }

        // Переместить игрока внутрь палаты
        if (PlayerMovement.Instance != null)
        {
            Transform playerSpawn = wardRoom.GetPlayerSpawnPoint();
            if (playerSpawn != null)
            {
                PlayerMovement.Instance.TeleportTo(playerSpawn);
            }
        }
    }

    public void ExitWard(string doorId, bool patientCured = false)
    {
        if (!_isWardActive) return;

        if (_wardStates.TryGetValue(doorId, out WardState state))
        {
            state.isCured = patientCured;
        }

        // Вернуть игрока к двери
        if (PlayerMovement.Instance != null && _currentSpawnPoint != null)
        {
            PlayerMovement.Instance.TeleportTo(_currentSpawnPoint);
        }

        if (_currentWardInstance != null)
        {
            Destroy(_currentWardInstance);
            _currentWardInstance = null;
        }

        // 🔥 Показываем визуал ТЕКУЩЕГО этажа
        if (_currentHallVisuals != null)
            _currentHallVisuals.SetActive(true);

        _currentSpawnPoint = null;
        _isWardActive = false;
    }

}
