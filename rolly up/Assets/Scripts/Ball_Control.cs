using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ball_Control : MonoBehaviour
{
    [SerializeField] float moveBorderX;
    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameStart)
        {
            transform.Translate(5 * Time.fixedDeltaTime * Vector3.left);
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float NewDeltaPos = Mathf.Clamp(Input.touches[0].deltaPosition.x, -1, 1);
                float NewPos = transform.position.x + NewDeltaPos * Time.fixedDeltaTime * 6;
                NewPos = Mathf.Clamp(NewPos, -moveBorderX, moveBorderX);
                transform.position = Vector3.Lerp(transform.position, new Vector3(NewPos, transform.position.y, transform.position.z), 100f * Time.fixedDeltaTime);
            }
        }
    }
}