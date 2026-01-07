using System.Collections.Generic;
using UnityEngine;

public class WardManager : MonoBehaviour
{
    public static WardManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PatientDatabase _patientDatabase;
    [SerializeField] private GameObject _wardRoomPrefab;
    [SerializeField] private Transform _wardZone;
    [SerializeField] private GameObject _hallVisuals;

    private Dictionary<string, WardState> _wardStates = new Dictionary<string, WardState>();
    private GameObject _currentWardInstance;
    private Transform _currentSpawnPoint;
    private bool _isWardActive = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (_patientDatabase == null)
            Debug.LogError("PatientDatabase not assigned!");
        if (_wardRoomPrefab == null)
            Debug.LogError("WardRoom prefab not assigned!");
        if (_wardZone == null)
            Debug.LogError("WardZone not assigned!");
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

        if (_hallVisuals != null)
            _hallVisuals.SetActive(false);

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

        if (PlayerMovement.Instance != null && _currentSpawnPoint != null)
        {
            PlayerMovement.Instance.TeleportTo(_currentSpawnPoint);
        }

        if (_currentWardInstance != null)
        {
            Destroy(_currentWardInstance);
            _currentWardInstance = null;
        }

        if (_hallVisuals != null)
            _hallVisuals.SetActive(true);

        _currentSpawnPoint = null;
        _isWardActive = false;
    }
}
