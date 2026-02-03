using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            gameObject.transform.SetParent(null);
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
