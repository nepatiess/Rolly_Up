using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) // Engele çarparsa
        {
            gameObject.transform.SetParent(null); // Küpü toptan ayýr

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false; // Fizik ekle düþsün

            // Buradaki yorumu kaldýrdýk:
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayAudio(4); // 4 numaralý (Çarpma) sesini çal
            }
        }
    }
}