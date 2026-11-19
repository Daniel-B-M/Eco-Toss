using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoToss.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menú principal")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text statusText;

        [Header("Panel de ayuda")]
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private Button helpBackButton;

        [Header("Panel de ajustes")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button settingsBackButton;

        [Header("Escenas")]
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private void Awake()
        {
            if (EventSystem.current == null)
            {
                var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
                eventSystemObject.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playButton.onClick.AddListener(StartGame);
            helpButton.onClick.AddListener(ShowHelpPanel);
            settingsButton.onClick.AddListener(ShowSettingsPanel);
            quitButton.onClick.AddListener(QuitGame);
            helpBackButton.onClick.AddListener(ShowMainMenu);
            settingsBackButton.onClick.AddListener(ShowMainMenu);

            ShowMainMenu();
        }

        private void Update()
        {
            bool isSubPanelOpen = !menuPanel.activeSelf;
            bool escapePressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

            if (isSubPanelOpen && escapePressed)
                ShowMainMenu();
        }

        private void ShowMainMenu() => ShowPanel(menuPanel, playButton);
        private void ShowHelpPanel() => ShowPanel(helpPanel, helpBackButton);
        private void ShowSettingsPanel() => ShowPanel(settingsPanel, settingsBackButton);

        // Enciende solo el panel indicado, apaga los demás y selecciona un botón
        // para que la navegación con teclado o mando siga funcionando.
        private void ShowPanel(GameObject panelToShow, Button buttonToSelect)
        {
            menuPanel.SetActive(panelToShow == menuPanel);
            helpPanel.SetActive(panelToShow == helpPanel);
            settingsPanel.SetActive(panelToShow == settingsPanel);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }

        private void StartGame()
        {
            if (!Application.CanStreamedLevelBeLoaded(gameplaySceneName))
            {
                statusText.text = "La escena Gameplay no está disponible en la compilación.";
                return;
            }
            playButton.interactable = false;
            SceneManager.LoadSceneAsync(gameplaySceneName);
        }

        private void QuitGame()
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
}
