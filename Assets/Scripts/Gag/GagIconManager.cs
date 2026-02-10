using UnityEngine;

public class GagIconManager : MonoBehaviour
{
    private static GagIconManager _instance;
    public static GagIconManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GagIconManager>();
                if (_instance == null)
                {
                    Debug.LogError("GagIconManager not found in scene!");
                }
            }
            return _instance;
        }
    }
    
    [Header("Иконки для типов гэгов")]
    [SerializeField] private Sprite clownishIcon;
    [SerializeField] private Sprite verbalIcon;
    [SerializeField] private Sprite absurdistIcon;
    [SerializeField] private Sprite ironicIcon;
    
    public static Sprite ClownishIcon => Instance?.clownishIcon;
    public static Sprite VerbalIcon => Instance?.verbalIcon;
    public static Sprite AbsurdistIcon => Instance?.absurdistIcon;
    public static Sprite IronicIcon => Instance?.ironicIcon;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public static Sprite GetGagIcon(HumorType gagType)
    {
        if (Instance == null) return GetFallbackIcon(gagType);
        
        switch (gagType)
        {
            case HumorType.Clownish:
                return Instance.clownishIcon ?? GetFallbackIcon(gagType);
            case HumorType.Verbal:
                return Instance.verbalIcon ?? GetFallbackIcon(gagType);
            case HumorType.Absurdist:
                return Instance.absurdistIcon ?? GetFallbackIcon(gagType);
            case HumorType.Ironic:
                return Instance.ironicIcon ?? GetFallbackIcon(gagType);
            default:
                return GetFallbackIcon(gagType);
        }
    }
    
    public static Sprite GetFallbackIcon(HumorType gagType)
    {
        // Создаем простую цветную текстуру как запасной вариант
        switch (gagType)
        {
            case HumorType.Clownish:
                return CreateColorSprite(Color.red);
            case HumorType.Verbal:
                return CreateColorSprite(Color.blue);
            case HumorType.Absurdist:
                return CreateColorSprite(Color.green);
            case HumorType.Ironic:
                return CreateColorSprite(Color.yellow);
            default:
                return CreateColorSprite(Color.gray);
        }
    }
    
    private static Sprite CreateColorSprite(Color color)
    {
        // Создаем простую текстуру 64x64
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
}
