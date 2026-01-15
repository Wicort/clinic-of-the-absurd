using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WardRoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _patientVisual;
    [SerializeField] private Text _patientNameText;
    [SerializeField] private Transform _gagButtonsParent;
    [SerializeField] private GameObject _gagButtonPrefab;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _absurdObjectPrefab;

    private PatientProfile _currentPatient;
    private string _currentDoorId;
    private bool _isPatientCured;

    private int _bossCurrentStep = 0;
    private bool _isBossMode => _currentPatient != null && _currentPatient.isBoss;

    public void Initialize(PatientProfile patient, string doorId, bool isCured)
    {
        _currentPatient = patient;
        _currentDoorId = doorId;
        _isPatientCured = isCured;

        if (_patientNameText != null)
            _patientNameText.text = patient.patientName;

        if (_patientVisual != null)
            _patientVisual.SetActive(!_isPatientCured);

        if (_gagButtonsParent != null)
            _gagButtonsParent.gameObject.SetActive(!_isPatientCured);

        CreateGagButtons();

        MedicalRecord[] records = GetComponentsInChildren<MedicalRecord>();
        foreach (var record in records)
        {
            record.Initialize(patient);
        }

        WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
        foreach (var door in exitDoors)
        {
            door.Initialize(_currentDoorId, _isPatientCured);
        }
    }

    public Transform GetPlayerSpawnPoint() => _playerSpawnPoint;

    private void OnDestroy()
    {
        if (DialogueBoxUI.Instance != null)
            DialogueBoxUI.Instance.onDialogueClosed = null;
    }

    private void CreateGagButtons()
    {
        if (_gagButtonsParent == null || _gagButtonPrefab == null) return;

        foreach (Transform child in _gagButtonsParent)
            Destroy(child.gameObject);

        HumorType[] allGags = (HumorType[])System.Enum.GetValues(typeof(HumorType));

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
        if (_currentPatient == null || _isPatientCured) return;

        if (_isBossMode)
        {
            HandleBossGag(gagType);
            return;
        }

        bool isCorrect = (gagType == _currentPatient.actualHumorType);
        bool isForbidden = System.Array.IndexOf(_currentPatient.forbiddenTypes, gagType) >= 0;

        if (isForbidden)
        {
            HospitalManager.Instance?.RegisterMistake();
            
            DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { GetGagLine(gagType) });
            DialogueBoxUI.Instance.onDialogueClosed = () =>
            {
                DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { GetAngryLine() });
                DialogueBoxUI.Instance.onDialogueClosed = () => { PlayPatientReaction("злость"); };
            };
            return;
        }

        if (isCorrect)
        {
            HandleGagSequence(gagType, true, false);
            return;
        }

        GagCard card = GagDeck.Instance.GetCardByType(gagType);
        int level = card?.level ?? 1;

        if (level >= 2)
        {
            float successChance = 0.3f * (level - 1); 
            if (Random.value < successChance)
            {
                HandleGagSequence(gagType, true, false);
                return;
            }
        }

        // Обычный провал
        HandleGagSequence(gagType, false, false);
    }

    private void HandleBossGag(HumorType gagType)
    {
        string gagLine = GetGagLine(gagType);
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { gagLine });

        DialogueBoxUI.Instance.onDialogueClosed = () =>
        {
            if (_bossCurrentStep < _currentPatient.bossSequence.Length &&
                gagType == _currentPatient.bossSequence[_bossCurrentStep])
            {
                _bossCurrentStep++;
                Debug.Log($"Boss step: {_bossCurrentStep} / {_currentPatient.bossSequence.Length}");

                if (_bossCurrentStep == _currentPatient.bossSequence.Length)
                {
                    // ✅ ПОБЕДА — сразу!
                    OnBossDefeated();
                }
                else
                {
                    // Продолжаем
                    string reaction = LocalizationManager.GetPatientReaction(PatientReactionType.BossContinue);
                    DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { reaction });
                    // Нет подписки — следующий гэг запустится через кнопки
                }
            }
            else
            {
                // Ошибка
                _bossCurrentStep = 0;
                string failLine = LocalizationManager.GetPatientReaction(PatientReactionType.BossFail);
                DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { failLine });
            }
        };
    }

    private void OnBossDefeated()
    {
        Debug.Log("BOSS DEFEATED! Game completed.");

        PlayPatientReaction("смех");
        _isPatientCured = true;

        if (_patientVisual != null)
            _patientVisual.SetActive(false);

        if (_gagButtonsParent != null)
            _gagButtonsParent.gameObject.SetActive(false);

        WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
        foreach (var door in exitDoors)
        {
            door.Initialize(_currentDoorId, true);
        }

        ShowVictoryScreen();
        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { "Через 5 секунд вы будете возвращены в меню" });

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("Menu");
    }

    private void ShowVictoryScreen()
    {
        string victoryMessage = LocalizationManager.GetUIText(UIKeyType.VictoryMessage);
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { victoryMessage });

        StartCoroutine(ExitToMenu());
    }

    IEnumerator ExitToMenu()
    {
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { "Через 5 секунд произойдет автоматический возврат в главное меню." });
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    private void HandleGagSequence(HumorType gagType, bool isCorrect, bool isForbidden)
    {
        string fullMessage = GetGagLine(gagType);

        if (isForbidden)
        {
            DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { fullMessage, GetAngryLine() });
            DialogueBoxUI.Instance.onDialogueClosed = () =>
            {
                PlayPatientReaction("злость");
            };
            return;
        }

        if (isCorrect)
        {
            DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { fullMessage, GetHappyLine() });
            DialogueBoxUI.Instance.onDialogueClosed = () =>
            {
                // УСПЕХ
                PlayPatientReaction("смех");
                _isPatientCured = true;
                if (_patientVisual != null)
                    _patientVisual.SetActive(false);
                if (_gagButtonsParent != null)
                    _gagButtonsParent.gameObject.SetActive(false);

                WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
                foreach (var door in exitDoors)
                {
                    door.Initialize(_currentDoorId, true);
                }

                // Показываем карты награды с задержкой 1 секунда
                StartCoroutine(ShowRewardCardsWithDelay());

                if (HospitalManager.Instance != null)
                {
                    HospitalManager.Instance.MarkWardAsCured(_currentDoorId);
                }
            };
            return;
        }

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { fullMessage, GetNeutralLine() });
        DialogueBoxUI.Instance.onDialogueClosed = () =>
        {
            PlayPatientReaction("нейтрально");
        };
    }

    private IEnumerator ShowRewardCardsWithDelay()
    {
        // Ждем 1 секунду перед появлением карт
        yield return new WaitForSeconds(1f);
        
        // Показываем карты награды
        WardExitDoor[] exitDoors = GetComponentsInChildren<WardExitDoor>();
        foreach (var door in exitDoors)
        {
            door.Initialize(_currentDoorId, true);
        }

        HumorType[] rewards = GagDeck.Instance.GenerateRewardOptions();
        GagRewardScreen.Instance.ShowRewardScreen(rewards, OnGagRewardSelected);
    }

    private string GetGagLine(HumorType gagType)
    {
        return LocalizationManager.GetGagText(gagType);
    }

    private string GetAngryLine()
    {
        return LocalizationManager.GetPatientReaction(PatientReactionType.Angry);
    }

    private string GetHappyLine()
    {
        return LocalizationManager.GetPatientReaction(PatientReactionType.Happy);
    }

    private string GetNeutralLine()
    {
        return LocalizationManager.GetPatientReaction(PatientReactionType.Neutral);
    }

    private IEnumerator PerformGag(HumorType gagType)
    {
        switch (gagType)
        {
            case HumorType.Clownish:
                yield return ClownishGag();
                break;
            case HumorType.Verbal:
                yield return VerbalGag();
                break;
            case HumorType.Absurdist:
                yield return AbsurdistGag();
                break;
            case HumorType.Ironic:
                yield return IronicGag();
                break;
        }
    }

    private IEnumerator VerbalGag()
    {
        string joke = LocalizationManager.GetGagAnimationText(GagAnimationType.Verbal);
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { joke });
        yield return new WaitForSeconds(2f); // даём время прочитать
    }

    private IEnumerator ClownishGag()
    {
        PlayerMovement player = PlayerMovement.Instance;
        if (player == null) yield break;

        Transform visuals = player.GetVisuals();
        if (visuals == null) yield break;

        // Анимация падения
        visuals.DOScaleY(0.8f, 0.1f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.1f);

        visuals.DORotate(new Vector3(0, 0, -20f), 0.1f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.2f);

        visuals.DOScaleY(1f, 0.2f).SetEase(Ease.OutSine);
        visuals.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator AbsurdistGag()
    {
        if (_absurdObjectPrefab == null) yield break;

        Vector3 spawnPos = _patientVisual ? _patientVisual.transform.position + Vector3.up * 2f : transform.position + Vector3.up;
        GameObject obj = Instantiate(_absurdObjectPrefab, spawnPos, Quaternion.identity);

        float startTime = Time.time;
        while (Time.time - startTime < 2f)
        {
            obj.transform.position += Vector3.right * Mathf.Sin(Time.time * 5f) * 0.5f * Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator IronicGag()
    {
        string phrase = LocalizationManager.GetGagAnimationText(GagAnimationType.Ironic);
        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { phrase });
        yield return new WaitForSeconds(2f);
    }

    private void PlayPatientReaction(string emotion)
    {
        Debug.Log($"{emotion}");
    }

    private void OnGagRewardSelected(HumorType gagType)
    {
        GagDeck.Instance.AddGag(gagType);
        CreateGagButtons();

        HospitalManager.Instance?.ExitWard(_currentDoorId, patientCured: true);
    }
}
