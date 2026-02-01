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
    Collider[] cubes;
    int maxCubeNum = 15;

    bool ballJumped;
    bool WhereitComes;
    bool HitTarget;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ballJumped)
        {
            if (rb.velocity.magnitude < 0.15f && rb.velocity.magnitude != 0)
            {
                WhereitComes = true;
                return;
            }
        }
    }



    private void FixedUpdate()
    {
        if(GameManager.Instance.isGameStart)
        {
            transform.Rotate(Vector3.forward, 400 * Time.fixedDeltaTime);

            cubes = new Collider[maxCubeNum];
            int sumOfCubesNum = Physics.OverlapSphereNonAlloc(transform.position, range, cubes);

            for(int i = 0; i < sumOfCubesNum; i++)
            {
                Rigidbody rb = cubes[i].GetComponent<Rigidbody>();
                
                if(rb != null)
                {
                    gravitionalDirecton = new Vector3(transform.position.x, 0, transform.position.z) - cubes[i].transform.position;
                    rb.AddForceAtPosition(gravitionalForce * gravitionalDirecton.normalized, transform.position);
                }
            }




            /* YÖNTEM 2
            cubes = Physics.OverlapSphere(transform.position, range);

            for (int i = 0; i < cubes.Length; i++)
            {
                Rigidbody rb = cubes[i].GetComponent<Rigidbody>();

                gravitionalDirecton = new Vector3(transform.position.x, 0, transform.position.z) - cubes[i].transform.position;
                rb.AddForceAtPosition(gravitionalForce * gravitionalDirecton.normalized, transform.position);
            }
            */
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            other.attachedRigidbody.isKinematic = true;
            other.gameObject.transform.SetParent(transform);
            AddCube();
        }

        if (other.CompareTag("Finish"))
        {
            Transform[] AllChild = GetComponentsInChildren<Transform>();

            GameManager.Instance.isGameStart = false;
            transform.SetParent(null);
            rb.isKinematic = false;

            rb.AddForce(AllChild.Length * Time.deltaTime * Vector3.forward, ForceMode.Impulse);
            ballJumped = true;
            collisionCollider.enabled = false;

            foreach (var item in AllChild)
            {
                if (item.gameObject.CompareTag("Cube"))
                {
                    item.gameObject.transform.SetParent(null);
                    item.GetComponent<Rigidbody>().isKinematic = false;
                }
            }



            /* YÖNTEM 2
            // fiziksel iþlemler
            for (int i = 0; i < AllChild.Length; i++)
            {
                if (AllChild[i].gameObject.CompareTag("Cube"))
                {
                    AllChild[i].gameObject.transform.SetParent(null);
                    AllChild[i].GetComponent<Rigidbody>().isKinematic = false;
                }
            }
            */
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (WhereitComes)
        {
            if (!other.gameObject.CompareTag("Plane") && !other.gameObject.CompareTag("Cube") && !HitTarget)
            {
                HitTarget = true;
                rb.velocity = Vector3.zero;
                GameManager.Instance.GameOver(int.Parse(other.name));
            }
        }
        
    }


    void AddCube()
    {
        range += 0.0009f;
        collisionCollider.radius += 0.0012f;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.0018f, transform.position.z);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
