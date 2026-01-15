using UnityEngine;

public class LocalizationSetup : MonoBehaviour
{
    [Header("Localization Settings")]
    [SerializeField] private TextConfiguration _textConfiguration;
    
    private void Awake()
    {
        // Создаем LocalizationManager если его нет
        if (LocalizationManager.Instance == null)
        {
            GameObject localizationManager = new GameObject("LocalizationManager");
            LocalizationManager manager = localizationManager.AddComponent<LocalizationManager>();
            
            // Устанавливаем конфигурацию
            if (_textConfiguration != null)
            {
                // Используем рефлексию для установки приватного поля
                var field = typeof(LocalizationManager).GetField("_textConfiguration", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field?.SetValue(manager, _textConfiguration);
            }
            
            DontDestroyOnLoad(localizationManager);
        }
    }
}
