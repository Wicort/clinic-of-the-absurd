using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GagDeck : MonoBehaviour
{
    public static GagDeck Instance { get; private set; }

    [SerializeField] private List<GagCard> _cards = new List<GagCard>();

    public IReadOnlyList<GagCard> Cards => _cards.AsReadOnly();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeStarterDeck();
    }

    private void InitializeStarterDeck()
    {
        _cards.Clear();
        foreach (HumorType type in System.Enum.GetValues(typeof(HumorType)))
        {
            _cards.Add(new GagCard(type));
        }
    }

    public GagCard GetCardByType(HumorType type)
    {
        return _cards.FirstOrDefault(c => c.gagType == type);
    }

    public void AddGag(HumorType type)
    {
        GagCard existing = GetCardByType(type);
        if (existing != null)
        {
            existing.LevelUp();
            Debug.Log($"Уровень {existing.displayName} повышен до {existing.level}!");
        }
        else
        {
            _cards.Add(new GagCard(type));
            Debug.Log($"Добавлена новая шутка: {type}");
        }
    }

    // Генерирует уникальные варианты наград
    public HumorType[] GenerateRewardOptions(int count = 2)
    {
        HumorType[] allTypes = (HumorType[])System.Enum.GetValues(typeof(HumorType));
        System.Collections.Generic.List<HumorType> availableTypes = new System.Collections.Generic.List<HumorType>(allTypes);
        HumorType[] options = new HumorType[count];

        for (int i = 0; i < count; i++)
        {
            if (availableTypes.Count == 0)
            {
                Debug.LogWarning("Недостаточно уникальных типов юмора для генерации наград!");
                break;
            }
            
            int randomIndex = Random.Range(0, availableTypes.Count);
            options[i] = availableTypes[randomIndex];
            availableTypes.RemoveAt(randomIndex);
        }

        return options;
    }
}
