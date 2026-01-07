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
            Debug.Log($"Новый гэг получен: {type}");
        }
    }

    // Генерация 3 случайных вариантов для награды
    public HumorType[] GenerateRewardOptions(int count = 3)
    {
        HumorType[] allTypes = (HumorType[])System.Enum.GetValues(typeof(HumorType));
        HumorType[] options = new HumorType[count];

        for (int i = 0; i < count; i++)
        {
            options[i] = allTypes[Random.Range(0, allTypes.Length)];
        }

        return options;
    }
}
