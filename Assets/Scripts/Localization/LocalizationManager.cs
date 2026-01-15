using UnityEngine;
using Random = UnityEngine.Random;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    
    [SerializeField] private TextConfiguration _textConfiguration;
    
    public static TextConfiguration Texts 
    { 
        get 
        { 
            if (Instance != null && Instance._textConfiguration != null)
                return Instance._textConfiguration;
                
            // Пробуем загрузить из Resources
            var config = Resources.Load<TextConfiguration>("TextConfiguration");
            if (config != null && Instance != null)
            {
                Instance._textConfiguration = config;
                return config;
            }
            
            return null; 
        } 
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Пытаемся загрузить конфигурацию
            if (_textConfiguration == null)
            {
                _textConfiguration = Resources.Load<TextConfiguration>("TextConfiguration");
                if (_textConfiguration == null)
                {
                    Debug.LogWarning("TextConfiguration не найден в Resources! Создайте его через Assets/Create/Game/Text Configuration");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static string GetGagText(HumorType gagType)
    {
        if (Texts == null) 
        {
            Debug.LogWarning("TextConfiguration не загружен! Проверьте что ассет существует в Resources/TextConfiguration.asset");
            return "Текст не найден";
        }
        
        string[] gagArray;
        switch (gagType)
        {
            case HumorType.Clownish:
                gagArray = Texts.GagTexts.Clownish;
                break;
            case HumorType.Verbal:
                gagArray = Texts.GagTexts.Verbal;
                break;
            case HumorType.Absurdist:
                gagArray = Texts.GagTexts.Absurdist;
                break;
            case HumorType.Ironic:
                gagArray = Texts.GagTexts.Ironic;
                break;
            default:
                gagArray = new[] { Texts.DialogueTexts.DefaultGag };
                break;
        }
        
        return gagArray[Random.Range(0, gagArray.Length)];
    }
    
    public static string GetPatientReaction(PatientReactionType reactionType)
    {
        if (Texts == null) 
        {
            Debug.LogWarning("TextConfiguration не загружен! Проверьте что ассет существует в Resources/TextConfiguration.asset");
            return "Текст не найден";
        }
        
        string[] reactionArray;
        switch (reactionType)
        {
            case PatientReactionType.Angry:
                reactionArray = Texts.PatientReactions.Angry;
                break;
            case PatientReactionType.Happy:
                reactionArray = Texts.PatientReactions.Happy;
                break;
            case PatientReactionType.Neutral:
                reactionArray = Texts.PatientReactions.Neutral;
                break;
            case PatientReactionType.BossContinue:
                reactionArray = Texts.PatientReactions.BossContinue;
                break;
            case PatientReactionType.BossFail:
                reactionArray = Texts.PatientReactions.BossFail;
                break;
            default:
                reactionArray = new[] { "..." };
                break;
        }
        
        return reactionArray[Random.Range(0, reactionArray.Length)];
    }
    
    public static string GetGagAnimationText(GagAnimationType animationType)
    {
        if (Texts == null) return "Текст не найден";
        
        string[] animationArray;
        switch (animationType)
        {
            case GagAnimationType.Verbal:
                animationArray = Texts.GagTexts.VerbalGag;
                break;
            case GagAnimationType.Ironic:
                animationArray = Texts.GagTexts.IronicGag;
                break;
            default:
                animationArray = new[] { "..." };
                break;
        }
        
        return animationArray[Random.Range(0, animationArray.Length)];
    }
    
    public static string GetUIText(UIKeyType keyType)
    {
        if (Texts == null) return "Текст не найден";
        
        switch (keyType)
        {
            case UIKeyType.RewardScreenTitle:
                return Texts.UITexts.RewardScreenTitle;
            case UIKeyType.SelectReward:
                return Texts.UITexts.SelectReward;
            case UIKeyType.LoadingFloor:
                return Texts.UITexts.LoadingFloor;
            case UIKeyType.ReturningToMenu:
                return Texts.UITexts.ReturningToMenu;
            case UIKeyType.VictoryMessage:
                return Texts.UITexts.VictoryMessage;
            default:
                return "Текст не найден";
        }
    }
    
    public static string GetStaircaseText(StaircaseTextType textType, int remainingPatients = 0)
    {
        if (Texts == null) 
        {
            Debug.LogWarning("TextConfiguration не загружен! Проверьте что ассет существует в Resources/TextConfiguration.asset");
            return "Лестница недоступна";
        }
        
        switch (textType)
        {
            case StaircaseTextType.NotCured:
                string[] notCuredTexts = Texts.UITexts.StaircaseNotCured;
                if (notCuredTexts.Length > 0)
                {
                    string template = notCuredTexts[Random.Range(0, notCuredTexts.Length)];
                    return template.Replace("{count}", remainingPatients.ToString());
                }
                break;
                
            case StaircaseTextType.NotVisited:
                string[] notVisitedTexts = Texts.UITexts.StaircaseNotVisited;
                if (notVisitedTexts.Length > 0)
                    return notVisitedTexts[Random.Range(0, notVisitedTexts.Length)];
                break;
        }
        
        return "Лестница недоступна";
    }
}

public enum PatientReactionType
{
    Angry,
    Happy,
    Neutral,
    BossContinue,
    BossFail
}

public enum GagAnimationType
{
    Verbal,
    Ironic
}

public enum UIKeyType
{
    RewardScreenTitle,
    SelectReward,
    LoadingFloor,
    ReturningToMenu,
    VictoryMessage
}

public enum StaircaseTextType
{
    NotCured,
    NotVisited
}
