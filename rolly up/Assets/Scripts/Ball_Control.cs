using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Control : MonoBehaviour
{
    [SerializeField] float moveBorderX;

    private void FixedUpdate()
    {
        // 1. KONTROL: Oyun baþlamadýysa aþaðýdakilerin hiçbirini yapma
        if (!GameManager.Instance.isGameStart)
        {
            return;
        }

        // 2. ÝLERÝ GÝTME: Bu sadece oyun baþladýðýnda çalýþýr
        transform.Translate(5 * Time.fixedDeltaTime * Vector3.left);

        // 3. SAÐ-SOL HAREKETÝ: Hassasiyetin eski (100f) haline getirildi
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float NewDeltaPos = Mathf.Clamp(touch.deltaPosition.x, -1, 1);
                float NewPos = transform.position.x + NewDeltaPos * Time.fixedDeltaTime * 6;
                NewPos = Mathf.Clamp(NewPos, -moveBorderX, moveBorderX);

                // Buradaki 100f deðerini geri getirdim, böylece top parmaðýna anýnda tepki verecek
                transform.position = Vector3.Lerp(
                    transform.position,
                    new Vector3(NewPos, transform.position.y, transform.position.z),
                    100f * Time.fixedDeltaTime
                );
            }
        }
    }
}