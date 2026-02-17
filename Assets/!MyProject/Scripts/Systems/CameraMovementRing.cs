using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementRing : MonoBehaviour
{
    [System.Serializable]
    public class CircleSettings
    {
        public float radius = 20f;
        public Color gizmoColor = Color.red;
        public int segments = 100;
        public bool showGizmo = true;
    }

    [Header("Настройки цели")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;

    [Header("Настройки камеры")]
    [SerializeField] private float cameraHeight = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Настройки круга")]
    [SerializeField] private CircleSettings circleSettings = new();

    [Header("Input System")]
    [SerializeField] private InputAction rotateAction;

    private float currentAngle;
    private float angleVelocity;
    private Vector3 centerPosition;
    private const float LerpMultiplier = 10f;

    private void Awake()
    {
        if (rotateAction == null)
        {
            rotateAction = new InputAction("RotateCamera", InputActionType.Value);
            rotateAction.AddBinding("<Keyboard>/leftArrow");
            rotateAction.AddBinding("<Keyboard>/a");
            rotateAction.AddBinding("<Keyboard>/rightArrow");
            rotateAction.AddBinding("<Keyboard>/d");
        }
    }

    private void Start()
    {
        UpdateCenterPosition();
        SetInitialPosition();
    }

    private void OnEnable()
    {
        rotateAction?.Enable();
    }

    private void OnDisable()
    {
        rotateAction?.Disable();
    }

    private void Update()
    {
        UpdateCenterPosition();

        float input = GetRotationInput();

        if (Mathf.Abs(input) > 0.01f)
        {
            currentAngle += input * rotationSpeed * Time.deltaTime;
        }

        float smoothedAngle = Mathf.SmoothDampAngle(
            currentAngle, currentAngle, ref angleVelocity, smoothTime);

        UpdateCameraPosition(smoothedAngle);
    }

    private float GetRotationInput()
    {
        float input = 0f;
        var keyboard = Keyboard.current;

        if (keyboard == null) return 0f;

        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            input += 1f;

        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            input -= 1f;

        return input;
    }

    private void UpdateCenterPosition()
    {
        if (centerPoint != null)
            centerPosition = centerPoint.position + centerOffset;
    }

    private void SetInitialPosition()
    {
        currentAngle = 0f;
        transform.position = CalculatePosition(0f);
        LookAtCenter();
    }

    private void UpdateCameraPosition(float angle)
    {
        Vector3 newPosition = CalculatePosition(angle);
        transform.position = Vector3.Lerp(
            transform.position, newPosition, Time.deltaTime * LerpMultiplier);
        LookAtCenter();
    }

    private Vector3 CalculatePosition(float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(angleRad);
        float cos = Mathf.Cos(angleRad);

        return new Vector3(
            centerPosition.x + sin * circleSettings.radius,
            centerPosition.y + cameraHeight,
            centerPosition.z + cos * circleSettings.radius);
    }

    private void LookAtCenter()
    {
        if (centerPoint != null)
            transform.LookAt(centerPosition);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void OnDrawGizmos()
    {
        if (!circleSettings.showGizmo) return;
        DrawGizmos(false);
    }

    private void DrawGizmos(bool selected)
    {
        if (!circleSettings.showGizmo || centerPoint == null) return;

        Vector3 drawCenter = GetDrawCenter();
        float alpha = selected ? 1f : 0.3f;

        Color circleColor = circleSettings.gizmoColor;
        circleColor.a = alpha;
        Gizmos.color = circleColor;

        DrawCircle(drawCenter + Vector3.up * cameraHeight,
                  circleSettings.radius, circleSettings.segments);

        if (selected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(drawCenter, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(drawCenter + Vector3.up * cameraHeight, transform.position);
        }
    }

    private Vector3 GetDrawCenter()
    {
        return Application.isPlaying ?
            centerPosition :
            (centerPoint != null ? centerPoint.position + centerOffset : Vector3.zero);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        if (segments < 3) return;

        Vector3 previousPoint = GetCirclePoint(center, radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            Vector3 currentPoint = GetCirclePoint(center, radius, (float)i / segments);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    private Vector3 GetCirclePoint(Vector3 center, float radius, float t)
    {
        float angle = t * Mathf.PI * 2f;
        return new Vector3(
            center.x + Mathf.Sin(angle) * radius,
            center.y,
            center.z + Mathf.Cos(angle) * radius);
    }
}