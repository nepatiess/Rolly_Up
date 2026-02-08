using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) // Engele çarparsa
        {
            // Ball'a bu küpün kaybedildiðini bildir
            Ball ball = FindObjectOfType<Ball>();
            if (ball != null)
            {
                ball.RemoveCube(gameObject);
            }

            // Küpü toptan ayýr
            gameObject.transform.SetParent(null);

            // Fizik ekle düþsün
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Çarpma sesi çal
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayAudio(4);
            }
        }
    }
}