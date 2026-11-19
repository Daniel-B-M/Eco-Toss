using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Controls the charging mechanism, countdown timer, and physics launch of the waste object.
    /// </summary>
    public class WasteThrower : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Rigidbody wasteBody;
        [SerializeField] private Transform aimDirectionPivot;
        [SerializeField] private Font uiFont;

        [Header("Carga del lanzamiento")]
        [SerializeField] private float minLaunchSpeed = 3f;
        [SerializeField] private float maxLaunchSpeed = 12f;
        [SerializeField] private float timeToFullCharge = 2f;

        [Header("Tiempo de apuntado")]
        [SerializeField, Min(0.1f)] private float aimDuration = 5f;

        // Runtime state variables
        private float currentLaunchSpeed;
        private float remainingAimTime;
        private bool hasLaunched;

        // Public accessors for trajectory calculation and game state
        public Vector3 LaunchVelocity => aimDirectionPivot.forward * currentLaunchSpeed;
        public bool HasLaunched => hasLaunched;
        public float RemainingAimTime => remainingAimTime;
        public Vector3 LaunchStartPosition => wasteBody.transform.position;

        private void Awake()
        {
            // Initialize launch parameters and keep the waste stationary before launch
            currentLaunchSpeed = minLaunchSpeed;
            remainingAimTime = aimDuration;
            wasteBody.isKinematic = true;
        }

        private void Update()
        {
            if (hasLaunched)
                return;

            // Decrement the remaining aiming countdown timer
            remainingAimTime = Mathf.Max(0f, remainingAimTime - Time.deltaTime);

            // Handle charge accumulation while holding the left mouse button, and throw on release
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    float chargeRate = (maxLaunchSpeed - minLaunchSpeed) / timeToFullCharge;

                    currentLaunchSpeed = Mathf.MoveTowards(
                        currentLaunchSpeed,
                        maxLaunchSpeed,
                        chargeRate * Time.deltaTime);
                }

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    Launch();
                    return;
                }
            }

            // Trigger automatic launch if time expires
            if (remainingAimTime <= 0f)
            {
                Launch();
            }
        }

        /// <summary>
        /// Enables physics simulation and applies initial launch velocity.
        /// </summary>
        private void Launch()
        {
            hasLaunched = true;

            wasteBody.isKinematic = false;
            wasteBody.linearVelocity = LaunchVelocity;
        }

        private void OnGUI()
        {
            // Display debug HUD showing aiming countdown timer or launch status
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.font = uiFont;
            style.fontSize = 28;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            string message = hasLaunched
                ? "Residuo lanzado"
                : $"Tiempo: {remainingAimTime:0.0} s";

            GUI.Box(new Rect(20, 20, 320, 60), message, style);
        }
    }
}