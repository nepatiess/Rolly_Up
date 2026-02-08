using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float gravitionalForce = 1.0f;
    [SerializeField] float range = 0.3f;
    [SerializeField] SphereCollider collisionCollider;
    [SerializeField] Rigidbody rb;
    [SerializeField] int totalLevelCubes = 100;
    [SerializeField] Transform startLinePoint;
    [SerializeField] Transform finishLineMaxPoint;
    [SerializeField] float maxRollSpeed = 12f;
    [SerializeField] float brakingDistance = 20f;
    [SerializeField] float minCrawlSpeed = 0.5f;

    Vector3 gravitionalDirecton;
    Collider[] cubes;
    int maxCubeNum = 15;

    private List<GameObject> collectedCubesList = new List<GameObject>();

    bool isRollingToTarget = false;
    Vector3 targetPosition;

    // ===== BOOSTLAR =====
    public bool shieldActive;
    public bool magnetActive;
    public bool slowActive;
    public bool percentActive;

    void Start()
    {
        collectedCubesList.Clear();

        magnetActive = PlayerPrefs.GetInt("Boost_Magnet", 0) == 1;
        shieldActive = PlayerPrefs.GetInt("Boost_Shield", 0) == 1;
        slowActive = PlayerPrefs.GetInt("Boost_Slow", 0) == 1;
        percentActive = PlayerPrefs.GetInt("Boost_Percent", 0) == 1;


        if (slowActive)
        {
            Time.timeScale = 0.6f; // oyunu yavaşlat
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameStart)
        {
            transform.Rotate(Vector3.forward, 400 * Time.fixedDeltaTime);

            // SADECE MIKNATIS VARSA ÇEKSİN
            if (magnetActive)
                MagneticEffect();
        }
        else if (isRollingToTarget)
        {
            float distance = targetPosition.z - transform.position.z;

            if (distance <= 0.05f)
            {
                StopBall();
            }
            else
            {
                float currentSpeed = maxRollSpeed;

                if (distance < brakingDistance)
                {
                    float ratio = distance / brakingDistance;
                    float curveFactor = Mathf.Sqrt(ratio);
                    currentSpeed = Mathf.Lerp(minCrawlSpeed, maxRollSpeed, curveFactor);
                }

                Vector3 targetVel = rb.velocity;
                targetVel.z = currentSpeed;
                targetVel.x = 0;
                rb.velocity = targetVel;

                float rotationSpeed = currentSpeed * 30f;
                transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void StopBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, targetPosition.z);
        isRollingToTarget = false;

        CalculateAndFinish();
    }

    void MagneticEffect()
    {
        cubes = new Collider[maxCubeNum];
        int sumOfCubesNum = Physics.OverlapSphereNonAlloc(transform.position, range, cubes);

        for (int i = 0; i < sumOfCubesNum; i++)
        {
            if (cubes[i] == null) continue;

            Rigidbody rbCube = cubes[i].GetComponent<Rigidbody>();

            if (rbCube != null && !collectedCubesList.Contains(cubes[i].gameObject))
            {
                gravitionalDirecton = new Vector3(transform.position.x, 0, transform.position.z) - cubes[i].transform.position;
                rbCube.AddForceAtPosition(gravitionalForce * gravitionalDirecton.normalized, transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            if (!collectedCubesList.Contains(other.gameObject))
            {
                collectedCubesList.Add(other.gameObject);
                other.attachedRigidbody.isKinematic = true;
                other.gameObject.transform.SetParent(transform);

                AddCube();
                GameManager.Instance.PlayAudio(2);
            }
        }

        if (other.CompareTag("Finish"))
        {
            GameManager.Instance.PlayAudio(5);
            GameManager.Instance.isGameStart = false;

            ScatterCubes();

            int realCount = collectedCubesList.Count;

            float visualRatio = (float)realCount / (float)totalLevelCubes;
            if (visualRatio > 1.0f) visualRatio = 1.0f;
            if (visualRatio < 0.05f) visualRatio = 0.05f;

            targetPosition = Vector3.Lerp(startLinePoint.position, finishLineMaxPoint.position, visualRatio);

            rb.isKinematic = false;
            collisionCollider.enabled = true;
            isRollingToTarget = true;
        }
    }

    void ScatterCubes()
    {
        Transform[] AllChild = GetComponentsInChildren<Transform>();

        foreach (var item in AllChild)
        {
            if (item.gameObject.CompareTag("Cube"))
            {
                item.SetParent(null);

                Rigidbody cubeRb = item.GetComponent<Rigidbody>();
                if (cubeRb != null)
                {
                    cubeRb.isKinematic = false;

                    Vector3 explosionDir = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-0.5f, 0.5f));
                    cubeRb.AddForce(explosionDir * 5f, ForceMode.Impulse);
                }
            }
        }
    }

    void CalculateAndFinish()
    {
        float rawRatio = (float)collectedCubesList.Count / (float)totalLevelCubes;

        //  YÜZDE BOOST VARSA BONUS
        if (percentActive)
            rawRatio += 0.1f; // +10%

        float scoreRatio = Mathf.Ceil(rawRatio * 10) / 10.0f;

        int score = (int)(scoreRatio * 100);
        if (score > 100) score = 100;

        GameManager.Instance.GameOver(score);

        // boostları temizle
        PlayerPrefs.DeleteKey("Boost_Magnet");
        PlayerPrefs.DeleteKey("Boost_Shield");
        PlayerPrefs.DeleteKey("Boost_Slow");
        PlayerPrefs.DeleteKey("Boost_Percent");

        Time.timeScale = 1f;
    }

    void AddCube()
    {
        range += 0.0009f;
        collisionCollider.radius += 0.0012f;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.0018f, transform.position.z);
    }

    public void RemoveCube(GameObject cube)
    {
        if (collectedCubesList.Contains(cube))
        {
            collectedCubesList.Remove(cube);

            range -= 0.0009f;
            collisionCollider.radius -= 0.0012f;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.0018f, transform.position.z);

            if (range < 0.1f) range = 0.1f;
            if (collisionCollider.radius < 0.1f) collisionCollider.radius = 0.1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
