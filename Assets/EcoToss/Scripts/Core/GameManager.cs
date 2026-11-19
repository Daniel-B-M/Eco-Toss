using UnityEngine;

namespace EcoToss.Core
{
    /// <summary>
    /// Controls the flow of the match (Singleton pattern).
    /// It is the only script allowed to change the GameState, and it announces
    /// every change through GameEvents.GameStateChanged.
    /// Lives only in the Gameplay scene: restarting the match means reloading the scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"[GameManager] Duplicate found on '{name}'. Destroying it.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        #endregion

        #region State
        public GameState CurrentState { get; private set; }

        private void Start()
        {
            // Start (not Awake) so every listener has already subscribed in its OnEnable.
            ApplyState(GameState.Playing);
        }

        /// <summary>Changes the match state. Ignores the call if the state is the same.</summary>
        public void ChangeState(GameState newState)
        {
            if (newState == CurrentState)
                return;

            ApplyState(newState);
        }

        private void ApplyState(GameState newState)
        {
            CurrentState = newState;
            Time.timeScale = newState == GameState.Paused ? 0f : 1f;
            GameEvents.RaiseGameStateChanged(newState);
        }
        #endregion
    }
}
