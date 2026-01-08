using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalManager : MonoBehaviour
{
    public static HospitalManager Instance { get; private set; }

    public System.Action onWardStateChanged;

    [Header("Floors")]
    [SerializeField] private GameObject _floor1Prefab;
    [SerializeField] private GameObject _floor2Prefab;
    [SerializeField] private GameObject _floor3Prefab;
    [SerializeField] private Transform _floorContainer;

    [Header("Ward System")]
    [SerializeField] private PatientDatabase _patientDatabase;
    [SerializeField] private GameObject _wardRoomPrefab;
    [SerializeField] private Transform _wardZone;

    private Dictionary<string, WardState> _wardStates = new Dictionary<string, WardState>();

    private GameObject _currentFloorInstance;
    private GameObject _currentWardInstance;
    private Transform _currentSpawnPoint;
    private GameObject _currentHallVisuals; 
    private int _currentFloorIndex = 1;
    private bool _isWardActive = false;

    private int _currentFloorMistakes = 0;
    private const int MAX_MISTAKES_PER_FLOOR = 3;

    public int CurrentFloorIndex => _currentFloorIndex;
    public Dictionary<string, WardState> WardStates => _wardStates;

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
        _currentFloorMistakes = 0;

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

        state.hasBeenEntered = true;

        PatientProfile patient;

        if (state.assignedPatientAssetName == null) // ← используй это вместо hasBeenEntered
        {
            DifficultyLevel level = (DifficultyLevel)floorLevel;
            patient = _patientDatabase.GetRandomPatient(level);
            if (patient == null)
            {
                Debug.LogError($"No patient for floor {floorLevel}, door {doorId}");
                return;
            }
            state.assignedPatientAssetName = patient.name;
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

        if (PlayerMovement.Instance != null && _currentSpawnPoint != null)
        {
            PlayerMovement.Instance.TeleportTo(_currentSpawnPoint);
        }

        if (_currentWardInstance != null)
        {
            Destroy(_currentWardInstance);
            _currentWardInstance = null;
        }

        if (_currentHallVisuals != null)
            _currentHallVisuals.SetActive(true);

        _currentSpawnPoint = null;
        _isWardActive = false;
    }

    public void MarkWardAsCured(string doorId)
    {
        if (_wardStates.TryGetValue(doorId, out WardState state))
        {
            state.isCured = true;
            Debug.Log($"[MarkWardAsCured] {doorId} is now cured.");
        }
        else
        {
            // Если состояния нет — создаём
            var newState = new WardState(doorId);
            newState.hasBeenEntered = true;
            newState.isCured = true;
            _wardStates[doorId] = newState;
            Debug.Log($"[MarkWardAsCured] Created new state for {doorId}.");
        }

        // Уведомляем подписчиков (лестницу)
        onWardStateChanged?.Invoke();
    }

    public void RegisterMistake()
    {
        _currentFloorMistakes++;
        Debug.Log($"Mistakes on floor: {_currentFloorMistakes}/{MAX_MISTAKES_PER_FLOOR}");

        if (_currentFloorMistakes > MAX_MISTAKES_PER_FLOOR)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        string gameOverText =
            "❌ ИГРА ОКОНЧЕНА ❌\n\n" +
            "Вы слишком часто злили пациентов.\n" +
            "Главврач Грустин уволил вас за вредительство психическому здоровью!\n\n" +
            "Попробуйте снова — и будьте добрее!";

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { gameOverText });


        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { "Через 5 секунд вы будете возвращены в меню" });

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("Menu");
    }

}
