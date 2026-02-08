using UnityEngine;

public class Ball_Control : MonoBehaviour
{
    [SerializeField] float moveBorderX = 2.5f;
    [SerializeField] float forwardSpeed = 5f;
    [SerializeField] float touchSensitivity = 0.015f;
    [SerializeField] float dragSpeed = 8f;

    Vector3 targetPosition;
    bool isDragging;
    float lastTouchX;

    void Start()
    {
        targetPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isGameStart) return;

        transform.Translate(forwardSpeed * Time.fixedDeltaTime * Vector3.left);
        HandleTouchControl();

        float smoothX = Mathf.Lerp(transform.position.x, targetPosition.x, dragSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);
    }

    void HandleTouchControl()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchX = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaX = touch.position.x - lastTouchX;
                targetPosition.x += deltaX * touchSensitivity;
                targetPosition.x = Mathf.Clamp(targetPosition.x, -moveBorderX, moveBorderX);
                lastTouchX = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Ended)
                isDragging = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastTouchX = Input.mousePosition.x;
            }
            else if (isDragging)
            {
                float deltaX = Input.mousePosition.x - lastTouchX;
                targetPosition.x += deltaX * touchSensitivity;
                targetPosition.x = Mathf.Clamp(targetPosition.x, -moveBorderX, moveBorderX);
                lastTouchX = Input.mousePosition.x;
            }
        }
        else if (Input.GetMouseButtonUp(0))
            isDragging = false;
    }
}
