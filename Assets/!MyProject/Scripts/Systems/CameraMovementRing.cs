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

    [SerializeField] private Transform centerPoint;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private float cameraHeight = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private CircleSettings circleSettings = new CircleSettings();
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
        currentAngle = 0f;
        transform.position = CalculatePosition(0f);
        LookAtCenter();
    }

    private void OnEnable() => rotateAction?.Enable();
    private void OnDisable() => rotateAction?.Disable();

    private void Update()
    {
        UpdateCenterPosition();

        float input = 0f;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) input += 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) input -= 1f;
        }

        if (Mathf.Abs(input) > 0.01f)
            currentAngle += input * rotationSpeed * Time.deltaTime;

        float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, currentAngle, ref angleVelocity, smoothTime);

        Vector3 targetPosition = CalculatePosition(smoothedAngle);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * LerpMultiplier);
        LookAtCenter();
    }

    private void UpdateCenterPosition()
    {
        if (centerPoint != null)
            centerPosition = centerPoint.position + centerOffset;
    }

    private Vector3 CalculatePosition(float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(
            centerPosition.x + Mathf.Sin(angleRad) * circleSettings.radius,
            centerPosition.y + cameraHeight,
            centerPosition.z + Mathf.Cos(angleRad) * circleSettings.radius);
    }

    private void LookAtCenter()
    {
        if (centerPoint != null)
            transform.LookAt(centerPosition);
    }

    private void OnDrawGizmos()
    {
        if (!circleSettings.showGizmo || centerPoint == null) return;

        Vector3 drawCenter = Application.isPlaying ? centerPosition : centerPoint.position + centerOffset;
        Vector3 circleCenter = drawCenter + Vector3.up * cameraHeight;

        Gizmos.color = circleSettings.gizmoColor;

        Vector3 prevPoint = circleCenter + new Vector3(Mathf.Sin(0) * circleSettings.radius, 0, Mathf.Cos(0) * circleSettings.radius);

        for (int i = 1; i <= circleSettings.segments; i++)
        {
            float t = (float)i / circleSettings.segments;
            float angle = t * Mathf.PI * 2f;
            Vector3 point = circleCenter + new Vector3(Mathf.Sin(angle) * circleSettings.radius, 0, Mathf.Cos(angle) * circleSettings.radius);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}