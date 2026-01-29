using System;
using UnityEngine;

[Serializable]
public class GagCard
{
    public HumorType gagType;
    public int level = 1;

    [Header("Бонусы")]
    public float successChanceBonus = 0f; 
    public float angerChanceReduction = 0f;
                                           
    // Гэг
    public Sprite icon;
    public string displayName => LocalizationManager.GetGagTypeName(gagType);

    public GagCard(HumorType type)
    {
        gagType = type;
        ApplyLevelEffects();
    }
    
    // Статический метод для обновления всех UI элементов при смене языка
    public static void RefreshAllGagCardUI()
    {
        // Находим все объекты с GagButton и RewardButton и обновляем их текст
        var gagButtons = UnityEngine.Object.FindObjectsByType<GagButton>(FindObjectsSortMode.None);
        foreach (var button in gagButtons)
        {
            button.RefreshText();
        }
        
        var rewardButtons = UnityEngine.Object.FindObjectsByType<RewardButton>(FindObjectsSortMode.None);
        foreach (var button in rewardButtons)
        {
            button.RefreshText();
        }
    }

    public void LevelUp()
    {
        level++;
        ApplyLevelEffects();
    }

    private void ApplyLevelEffects()
    {
        successChanceBonus = (level - 1) * 0.1f;       // +10% к шансу успеха                    
        angerChanceReduction = (level - 1) * 0.05f;   // -5% к шансу гнева                  
    }
}
