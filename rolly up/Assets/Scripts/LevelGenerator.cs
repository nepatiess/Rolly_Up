using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : MonoBehaviour
{
    // MOD SEÇÝMÝ
    public enum SpawnMode
    {
        HybridPath,  // Yýlan ve Düz karýþýk sýralý yol
        RandomArea   // Eski koddaki gibi belli alana daðýnýk
    }

    [Header("Mod Seçimi")]
    public SpawnMode spawnMode = SpawnMode.HybridPath;

    [Header("Küpler (Rengarenk)")]
    // ARTIK BURASI LÝSTE OLDU
    public GameObject[] cubePrefabs;

    [Header("Temel Ayarlar")]
    public Transform parentObject;
    public int cubeCount = 100;
    public float startZ = 10f;
    public float yPos = 0.5f;

    [Header("Eski Usül Ayarlar (Random Area Modu)")]
    public float roadWidth = 5.0f;
    public float roadLength = 150f;

    [Header("Sýralý Yol Ayarlarý (Hybrid Path Modu)")]
    public float minStepDistance = 1.2f;
    public float maxStepDistance = 3.5f;

    [Space(5)]
    public float waveWidth = 4.0f;
    public float waveFrequency = 0.25f;
    public int minSegmentLength = 8;
    public int maxSegmentLength = 15;

    [Header("Güvenlik")]
    public float minCheckRadius = 0.6f;

    private float currentZ;
    private float currentX;
    private float sineAngle;

    [ContextMenu("Yolu Oluþtur")]
    public void Generate()
    {
        // Önce listede küp var mý kontrol et
        if (cubePrefabs == null || cubePrefabs.Length == 0)
        {
            Debug.LogError("HATA: Hiç küp prefabý eklememiþsin! Inspector'dan 'Cube Prefabs' listesini doldur.");
            return;
        }

        ClearCubes();

        if (spawnMode == SpawnMode.HybridPath)
        {
            GenerateHybridPath();
        }
        else
        {
            GenerateRandomArea();
        }
    }

    void GenerateHybridPath()
    {
        currentZ = startZ;
        currentX = 0;
        sineAngle = 0;
        int cubesSpawned = 0;
        bool isSnakeMode = true;

        while (cubesSpawned < cubeCount)
        {
            int segmentLength = Random.Range(minSegmentLength, maxSegmentLength);
            if (cubesSpawned + segmentLength > cubeCount)
                segmentLength = cubeCount - cubesSpawned;

            for (int i = 0; i < segmentLength; i++)
            {
                float stepZ = Random.Range(minStepDistance, maxStepDistance);
                currentZ += stepZ;

                float targetX = 0;
                if (isSnakeMode)
                {
                    sineAngle += waveFrequency;
                    targetX = Mathf.Sin(sineAngle) * waveWidth;
                }
                else
                {
                    targetX = 0;
                    sineAngle = 0;
                }

                currentX = Mathf.Lerp(currentX, targetX, 0.4f);

                SpawnCube(new Vector3(currentX, yPos, currentZ));
                cubesSpawned++;
            }
            isSnakeMode = !isSnakeMode;
        }
        Debug.Log($"Hybrid Yol: {cubesSpawned} adet Rengarenk küp oluþturuldu.");
    }

    void GenerateRandomArea()
    {
        int spawnedCount = 0;
        int safetyLoop = 0;

        while (spawnedCount < cubeCount)
        {
            safetyLoop++;
            if (safetyLoop > 10000) break;

            float randomX = Random.Range(-roadWidth, roadWidth);
            float randomZ = Random.Range(startZ, startZ + roadLength);
            Vector3 candidatePos = new Vector3(randomX, yPos, randomZ);

            // --- DEÐÝÞÝKLÝK BURADA ---
            // Kontrolü yaparken spawn noktasýnýn (candidatePos) 1 birim yukarýsýna bakýyoruz.
            // Böylece küre yere deðmiyor ama diðer küplere deðerse onlarý yine algýlar.
            Vector3 checkPos = candidatePos + Vector3.up * 1.0f;

            if (!Physics.CheckSphere(checkPos, minCheckRadius))
            {
                SpawnCube(candidatePos);
                spawnedCount++;
            }
            // -------------------------
        }
        Debug.Log($"Random Alan: {spawnedCount} adet Rengarenk küp oluþturuldu.");
    }

    void SpawnCube(Vector3 pos)
    {
        // RASTGELE RENK SEÇÝMÝ
        // Listenin içinden rastgele bir tane seç (Örn: 0 ile 7 arasý)
        int randomIndex = Random.Range(0, cubePrefabs.Length);
        GameObject selectedPrefab = cubePrefabs[randomIndex];

#if UNITY_EDITOR
        GameObject newCube;
        if (selectedPrefab != null)
        {
            newCube = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            newCube.transform.position = pos;
            newCube.transform.rotation = Quaternion.identity;
        }
        else
        {
            // Eðer boþ bir slot varsa hata vermesin, normal oluþturmaya çalýþsýn
            if (selectedPrefab != null)
                newCube = Instantiate(selectedPrefab, pos, Quaternion.identity);
            else return;
        }

        if (parentObject != null) newCube.transform.SetParent(parentObject);
        else newCube.transform.SetParent(transform);
#endif
    }

    [ContextMenu("Sahneyi Temizle")]
    public void ClearCubes()
    {
        Transform container = parentObject != null ? parentObject : transform;
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(container.GetChild(i).gameObject);
        }
    }
}