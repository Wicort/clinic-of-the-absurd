using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextConfiguration", menuName = "Game/Text Configuration")]
public class TextConfiguration : ScriptableObject
{
    [Header("Гэги по типам юмора")]
    public GagTexts GagTexts;
    
    [Header("Реакции пациентов")]
    public PatientReactionTexts PatientReactions;
    
    [Header("Интерфейс и системные сообщения")]
    public UITexts UITexts;
    
    [Header("Диалоги и сюжетные тексты")]
    public DialogueTexts DialogueTexts;
}

[Serializable]
public class GagTexts
{
    [Header("Клоунские гэги")]
    public string[] Clownish;
    
    [Header("Словесные гэги")]
    public string[] Verbal;
    
    [Header("Абсурдные гэги")]
    public string[] Absurdist;
    
    [Header("Ироничные гэги")]
    public string[] Ironic;
    
    [Header("Гэги для анимаций")]
    public string[] VerbalGag;
    public string[] IronicGag;
}

[Serializable]
public class PatientReactionTexts
{
    [Header("Злые реакции")]
    public string[] Angry;
    
    [Header("Радостные реакции")]
    public string[] Happy;
    
    [Header("Нейтральные реакции")]
    public string[] Neutral;
    
    [Header("Реакции босса")]
    public string[] BossContinue;
    public string[] BossFail;
}

[Serializable]
public class UITexts
{
    [Header("Экран наград")]
    public string RewardScreenTitle;
    public string SelectReward;
    
    [Header("Системные сообщения")]
    public string LoadingFloor;
    public string ReturningToMenu;
    public string VictoryMessage;
    
    [Header("Лестница")]
    public string[] StaircaseNotCured;
    public string[] StaircaseNotVisited;
}

[Serializable]
public class DialogueTexts
{
    [Header("Сюжетные диалоги")]
    public string[] BatmanIntro;
    public string[] BatmanOutro;
    
    [Header("Подсказки")]
    public string[] Hints;
    
    [Header("Ошибка по умолчанию")]
    public string DefaultGag;
}
