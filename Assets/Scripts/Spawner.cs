using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Prefab & Materials")]
    public GameObject collectiblePrefab;
    public Material redMat, greenMat, blueMat;

    [Header("Spawn Settings")]
    public Vector3 spawnAreaMin = new Vector3(-15f, 1f, -15f);
    public Vector3 spawnAreaMax = new Vector3(15f, 1f, 15f);
    public float spawnInterval = 0.9f;
    public int totalToSpawn = 30;

    private int spawnedCount = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (spawnedCount < totalToSpawn)
        {
            SpawnOne();
            spawnedCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Finished spawning " + totalToSpawn + " collectibles!");
    }

    void SpawnOne()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("Spawner: No collectible prefab assigned!");
            return;
        }

        // Random position within defined area
        Vector3 pos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        // Optional: lift slightly above ground
        pos.y += 0.5f;

        // Optional: skip if already something there
        if (Physics.CheckSphere(pos, 0.8f))
        {
            // Avoid overlapping
            return;
        }

        GameObject go = Instantiate(collectiblePrefab, pos, Quaternion.identity);
        GameManager.Instance.RegisterCollectible();
        Collectible c = go.GetComponent<Collectible>();

        if (c == null)
        {
            Debug.LogError("Spawner: Prefab missing Collectible script!");
            return;
        }

        // Randomly choose a color
        int r = Random.Range(0, 3);
        if (r == 0 && redMat != null)
        {
            c.colorType = Collectible.ColorType.Red;
            c.ApplyMaterial(redMat);
        }
        else if (r == 1 && greenMat != null)
        {
            c.colorType = Collectible.ColorType.Green;
            c.ApplyMaterial(greenMat);
        }
        else if (r == 2 && blueMat != null)
        {
            c.colorType = Collectible.ColorType.Blue;
            c.ApplyMaterial(blueMat);
        }
        else
        {
            Debug.LogWarning("Spawner: One or more materials not assigned!");
        }


    }

    //  Visualize spawn area in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube((spawnAreaMin + spawnAreaMax) / 2f, spawnAreaMax - spawnAreaMin);
    }
}
