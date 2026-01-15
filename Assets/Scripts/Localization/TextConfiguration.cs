using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextConfiguration", menuName = "Game/Text Configuration")]
public class TextConfiguration : ScriptableObject
{
    [Header("Языковые конфигурации")]
    public List<LanguageData> Languages;
    public int DefaultLanguageIndex = 0;
    
    // Для обратной совместимости - возвращаем первый язык или язык по умолчанию
    [Header("Устаревшие поля (для совместимости)")]
    public GagTexts GagTexts => Languages != null && Languages.Count > 0 ? Languages[0].GagTexts : new GagTexts();
    public PatientReactionTexts PatientReactions => Languages != null && Languages.Count > 0 ? Languages[0].PatientReactions : new PatientReactionTexts();
    public UITexts UITexts => Languages != null && Languages.Count > 0 ? Languages[0].UITexts : new UITexts();
    public DialogueTexts DialogueTexts => Languages != null && Languages.Count > 0 ? Languages[0].DialogueTexts : new DialogueTexts();
}
