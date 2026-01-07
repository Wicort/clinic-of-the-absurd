using System;
using UnityEngine;
using UnityEngine.UI;

public class WardRoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _patientVisual;
    [SerializeField] private Text _patientNameText;
    [SerializeField] private Transform _gagButtonsParent;
    [SerializeField] private GameObject _gagButtonPrefab;
    [SerializeField] private Transform _playerSpawnPoint;

    private PatientProfile _currentPatient;
    private string _currentDoorId;
    private bool _isPatientCured;

    public void Initialize(PatientProfile patient, string doorId, bool isCured)
    {
        _currentPatient = patient;
        _currentDoorId = doorId;
        _isPatientCured = isCured;

        if (_patientNameText != null)
            _patientNameText.text = patient.patientName;

        // ВАЖНО: сначала ВСЕГДА показываем пациента
        if (_patientVisual != null)
            _patientVisual.SetActive(true);

        // И только потом — скрываем, если вылечен
        if (_isPatientCured)
        {
            _patientVisual.SetActive(false);
        }

        CreateGagButtons();

        // Инициализация медицинских карт
        MedicalRecord[] records = GetComponentsInChildren<MedicalRecord>();
        foreach (var record in records)
        {
            record.Initialize(patient);
        }

        // Инициализация дверей выхода
        WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
        foreach (var door in exitDoors)
        {
            door.Initialize(_currentDoorId, _isPatientCured);
        }
    }

    public Transform GetPlayerSpawnPoint() => _playerSpawnPoint;

    private void CreateGagButtons()
    {
        foreach (Transform child in _gagButtonsParent)
            Destroy(child.gameObject);

        HumorType[] allGags = (HumorType[])Enum.GetValues(typeof(HumorType));

        foreach (HumorType gagType in allGags)
        {
            GameObject buttonObj = Instantiate(_gagButtonPrefab, _gagButtonsParent);
            GagButton gagButton = buttonObj.GetComponent<GagButton>();
            if (gagButton != null)
            {
                gagButton.Setup(gagType, OnGagSelected);
            }
        }
    }

    private void OnGagSelected(HumorType gagType)
    {
        if (_currentPatient == null)
        {
            Debug.LogWarning("WardRoom: patient is null, ignoring gag.");
            return;
        }

        if (_isPatientCured)
        {
            Debug.Log("Пациент уже вылечен!");
            return;
        }

        bool isCorrect = (gagType == _currentPatient.actualHumorType);
        bool isForbidden = System.Array.IndexOf(_currentPatient.forbiddenTypes, gagType) >= 0;

        if (isForbidden)
        {
            Debug.Log($"{_currentPatient.patientName} разозлился! Запрещённый гэг.");
            return;
        }

        if (isCorrect)
        {
            Debug.Log($"Ура! {_currentPatient.patientName} выздоровел!");
            _isPatientCured = true;

            if (_patientVisual != null)
                _patientVisual.SetActive(false);

            // Скрываем панель гэгов — лечить больше некого
            if (_gagButtonsParent != null)
                _gagButtonsParent.gameObject.SetActive(false);

            // Обновить двери выхода
            WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
            foreach (var door in exitDoors)
            {
                door.Initialize(_currentDoorId, true);
            }

            // Выдача награды
            HumorType[] rewards = GagDeck.Instance.GenerateRewardOptions(3);
            GagRewardScreen.Instance.ShowRewardScreen(rewards, OnGagRewardSelected);
        }
        else
        {
            Debug.Log($"{_currentPatient.patientName} не смеётся... Попробуй другой гэг.");
        }
    }

    private void OnGagRewardSelected(HumorType gagType)
    {
        GagDeck.Instance.AddGag(gagType);
        // Обновляем кнопки — теперь с новыми уровнями
        CreateGagButtons();
    }
}
