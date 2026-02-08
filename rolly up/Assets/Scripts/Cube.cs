using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) // Engele çarparsa
        {
            Ball ball = FindObjectOfType<Ball>();
            if (ball != null)
            {
                //  KALKAN AKTÝFSE KÜP DÜÞMESÝN
                if (ball.shieldActive)
                {
                    // sadece çarpma sesi çal (istersen)
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.PlayAudio(4);
                    }
                    return; // burda çýk  aþaðýsý çalýþmaz
                }

                //  Kalkan yoksa küp düþsün
                ball.RemoveCube(gameObject);
            }

            // Küpü toptan ayýr
            transform.SetParent(null);

            // Fizik ekle düþsün
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Çarpma sesi
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayAudio(4);
            }
        }
    }
}
