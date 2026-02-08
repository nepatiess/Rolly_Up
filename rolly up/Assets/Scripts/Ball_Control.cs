using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Control : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveBorderX = 2.5f;
    [SerializeField] float forwardSpeed = 5f;

    [Header("Touch Control")]
    [SerializeField] float touchSensitivity = 0.015f; // Çok daha hassas
    [SerializeField] float dragSpeed = 8f; // Sürükleme hýzý

    private Vector3 targetPosition;
    private bool isDragging = false;
    private float lastTouchX;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // 1. KONTROL: Oyun baþlamadýysa dur
        if (!GameManager.Instance.isGameStart)
        {
            return;
        }

        // 2. ÝLERÝ GÝTME
        transform.Translate(forwardSpeed * Time.fixedDeltaTime * Vector3.left);

        // 3. SAÐ-SOL HAREKETÝ
        HandleTouchControl();

        // Hedef pozisyona yumuþak geçiþ
        float smoothX = Mathf.Lerp(
            transform.position.x,
            targetPosition.x,
            dragSpeed * Time.fixedDeltaTime
        );

        transform.position = new Vector3(
            smoothX,
            transform.position.y,
            transform.position.z
        );
    }

    private void HandleTouchControl()
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
                // Parmaðýn hareket ettiði mesafe (pixel cinsinden)
                float deltaX = touch.position.x - lastTouchX;

                // Hassasiyet ile çarp ve hedef pozisyona ekle
                float movement = deltaX * touchSensitivity;

                targetPosition.x += movement;
                targetPosition.x = Mathf.Clamp(targetPosition.x, -moveBorderX, moveBorderX);

                lastTouchX = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        // Mouse desteði (test için)
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
                float movement = deltaX * touchSensitivity;

                targetPosition.x += movement;
                targetPosition.x = Mathf.Clamp(targetPosition.x, -moveBorderX, moveBorderX);

                lastTouchX = Input.mousePosition.x;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}