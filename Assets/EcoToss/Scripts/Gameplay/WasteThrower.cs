using UnityEngine;
using UnityEngine.InputSystem;

public class WasteThrower : MonoBehaviour
{
    [SerializeField] private Rigidbody wasteBody;
    [SerializeField] private Transform throwOrigin;

    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float chargeDuration = 2f;

    private float currentSpeed;
    private bool hasLaunched;

    public Vector3 LaunchVelocity => throwOrigin.forward * currentSpeed;
    public bool HasLaunched => hasLaunched;

    [SerializeField, Min(0.1f)] private float aimDuration = 5f;
    private float remainingTime;
    public float RemainingTime => remainingTime;


    private void Awake()
    {
        currentSpeed = minSpeed;
        remainingTime = aimDuration;
    }

    private void Update()
    {
        if (hasLaunched)
            return;

        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                float chargeRate = (maxSpeed - minSpeed) / chargeDuration;

                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    maxSpeed,
                    chargeRate * Time.deltaTime);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Launch();
                return;
            }
        }

        if (remainingTime <= 0f)
        {
            Launch();
        }
    }

    private void Launch()
    {
        hasLaunched = true;

        wasteBody.position = throwOrigin.position;
        wasteBody.isKinematic = false;
        wasteBody.linearVelocity = LaunchVelocity;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 28;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        string message = hasLaunched
            ? "Residuo lanzado"
            : $"Tiempo: {remainingTime:0.0} s";

        GUI.Box(new Rect(20, 20, 320, 60), message, style);
    }
}