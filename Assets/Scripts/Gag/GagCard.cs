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
    public string displayName => GagTypeNames[(int)gagType];                          
    private static readonly string[] GagTypeNames = {                                        
        "Клоунский",
        "Словесный",
        "Абсурдный",
        "Ироничный"
    };

    public GagCard(HumorType type)
    {
        gagType = type;
        ApplyLevelEffects();
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
