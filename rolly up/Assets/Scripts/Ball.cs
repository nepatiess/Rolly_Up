using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float gravitionalForce = 1.0f;
    [SerializeField] float range = 0.3f;
    [SerializeField] SphereCollider collisionCollider;
    [SerializeField] Ball_Control ballKontrol;
    [SerializeField] Rigidbody rb;

    Vector3 gravitionalDirecton;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void FixedUpdate()
    {
        if(GameManager.Instance.isGameStart)
        {
            transform.Rotate(Vector3.forward, 400 * Time.fixedDeltaTime);
        }
    }
}
