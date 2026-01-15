using Assets.Scripts.Core;
using UnityEngine;

public class WardDoor : MonoBehaviour, IInteractive
{
    [Header("Door Settings")]
    [SerializeField] private string _doorId = "1_01";
    [SerializeField] private int _floorLevel = 1;
    [SerializeField] private Transform _spawnPoint;

    public string GetInteractionPrompt() => LocalizationManager.GetInteractionPrompt(InteractionPromptType.WardDoor);

    public Transform GetSpawnPoint() => _spawnPoint;
    
    public string GetDoorId() => _doorId;

    private void OnValidate()
    {
        if (_spawnPoint == null)
        {
            _spawnPoint = transform.Find("SpawnPoint");
            if (_spawnPoint == null)
            {
#if UNITY_EDITOR
                GameObject sp = new GameObject("SpawnPoint");
                sp.transform.SetParent(transform);
                sp.transform.localPosition = Vector3.zero;
                _spawnPoint = sp.transform;
#endif
            }
        }

        if (!string.IsNullOrEmpty(_doorId) && char.IsDigit(_doorId[0]))
        {
            if (int.TryParse(_doorId[0].ToString(), out int firstDigit))
            {
                _floorLevel = Mathf.Clamp(firstDigit, 1, 3);
            }
        }
    }

    public void Interact()
    {
        if (WardManager.Instance == null)
        {
            Debug.LogError("WardManager not found!");
            return;
        }

        WardManager.Instance.EnterWard(_doorId, _floorLevel, _spawnPoint);
    }
}
