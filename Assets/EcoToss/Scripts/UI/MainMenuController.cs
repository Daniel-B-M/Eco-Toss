using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private Text statusText;
    [SerializeField] private string gameplayScene = "Gameplay";

    private void Awake()
    {
        if (EventSystem.current == null)
        {
            var events = new GameObject("EventSystem", typeof(EventSystem));
            events.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playButton.onClick.AddListener(Play);
        helpButton.onClick.AddListener(() => ShowHelp(true));
        backButton.onClick.AddListener(() => ShowHelp(false));
        quitButton.onClick.AddListener(Quit);
        ShowHelp(false);
    }

    private void Update()
    {
        if (helpPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ShowHelp(false);
    }

    private void ShowHelp(bool visible)
    {
        menuPanel.SetActive(!visible);
        helpPanel.SetActive(visible);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(visible ? backButton.gameObject : playButton.gameObject);
    }

    private void Play()
    {
        if (!Application.CanStreamedLevelBeLoaded(gameplayScene))
        {
            statusText.text = "La escena Gameplay no está disponible en la compilación.";
            return;
        }
        playButton.interactable = false;
        SceneManager.LoadSceneAsync(gameplayScene);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        statusText.text = "Puedes cerrar la pestaña para salir del juego.";
#else
        Application.Quit();
#endif
    }
}
