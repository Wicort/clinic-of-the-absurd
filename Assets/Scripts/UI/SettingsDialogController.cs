using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SettingsDialogController : MonoBehaviour
{
    private static SettingsDialogController _instance;

    public static SettingsDialogController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SettingsDialogController>();
                if (_instance == null)
                {
                    var go = new GameObject("SettingsDialogController");
                    _instance = go.AddComponent<SettingsDialogController>();
                }
            }
            return _instance;
        }
    }

    private SettingsDialog _dialog;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Toggle();
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
#endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolveDialog();
    }

    private void ResolveDialog()
    {
        if (_dialog != null) return;

        var dialogs = Resources.FindObjectsOfTypeAll<SettingsDialog>();
        foreach (var d in dialogs)
        {
            if (d == null) continue;
            if (!d.gameObject.scene.isLoaded) continue;
            _dialog = d;
            break;
        }
    }

    public static void Open()
    {
        Instance.ResolveDialog();
        if (Instance._dialog != null)
            Instance._dialog.Open();
    }

    public static void Close()
    {
        Instance.ResolveDialog();
        if (Instance._dialog != null)
            Instance._dialog.Close();
    }

    public static void Toggle()
    {
        Instance.ResolveDialog();
        if (Instance._dialog == null) return;

        if (Instance._dialog.IsOpen()) Instance._dialog.Close();
        else Instance._dialog.Open();
    }
}
