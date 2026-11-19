using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
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

        private float currentLaunchSpeed;
        private float remainingAimTime;
        private bool hasLaunched;

        public Vector3 LaunchVelocity => aimDirectionPivot.forward * currentLaunchSpeed;
        public bool HasLaunched => hasLaunched;
        public float RemainingAimTime => remainingAimTime;
        public Vector3 LaunchStartPosition => wasteBody.transform.position;

        private void Awake()
        {
            currentLaunchSpeed = minLaunchSpeed;
            remainingAimTime = aimDuration;
            wasteBody.isKinematic = true;
        }

        private void Update()
        {
            if (hasLaunched)
                return;

            remainingAimTime = Mathf.Max(0f, remainingAimTime - Time.deltaTime);

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

            if (remainingAimTime <= 0f)
            {
                Launch();
            }
        }

        private void Launch()
        {
            hasLaunched = true;

            wasteBody.isKinematic = false;
            wasteBody.linearVelocity = LaunchVelocity;
        }

        private void OnGUI()
        {
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