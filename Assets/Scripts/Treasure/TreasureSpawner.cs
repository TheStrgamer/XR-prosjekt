using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{

    [Header("Treasure Settings")]
    [SerializeField] private GameObject[] treasurePrefabs;
    [SerializeField] private int treasuresCount = 3;
    [SerializeField] private bool allowDupes = true;
    [SerializeField] private float minDistanceBetweenTreasures = 2f;

    [Header("Spawn Area")]
    [SerializeField] private Transform spawnAreaCornerOne = null;
    [SerializeField] private Transform spawnAreaCornerTwo = null;

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private GameObject[] tmpTreasurePrefabs;


    void Start()
    {
        if (!allowDupes)
        {
            var tempList = new List<GameObject>(treasurePrefabs);
            tmpTreasurePrefabs = tempList.ToArray();
        }
        SpawnTreasures();
    }
    private void SpawnTreasures()
    {
        if (treasurePrefabs.Length == 0 || spawnAreaCornerOne == null || spawnAreaCornerTwo == null)
            return;

        spawnedPositions.Clear();

        Vector3 min = Vector3.Min(spawnAreaCornerOne.position, spawnAreaCornerTwo.position);
        Vector3 max = Vector3.Max(spawnAreaCornerOne.position, spawnAreaCornerTwo.position);

        int attempts = 0;
        int maxAttempts = treasuresCount * 10;

        if (!allowDupes && treasuresCount > treasurePrefabs.Length)
        {
            treasuresCount = treasurePrefabs.Length;
        } 

        int spawned = 0;
        while (spawned < treasuresCount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPos = new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );

            bool tooClose = false;
            foreach (var pos in spawnedPositions)
            {
                if (Vector3.Distance(pos, randomPos) < minDistanceBetweenTreasures)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            GameObject prefab;
            if (allowDupes)
                prefab = treasurePrefabs[Random.Range(0, treasurePrefabs.Length)];
            else
            {
                if (tmpTreasurePrefabs.Length == 0) break;
                int index = Random.Range(0, tmpTreasurePrefabs.Length);
                prefab = tmpTreasurePrefabs[index];
                var tempList = new List<GameObject>(tmpTreasurePrefabs);
                tempList.RemoveAt(index);
                tmpTreasurePrefabs = tempList.ToArray();
            }

            Instantiate(prefab, randomPos, Quaternion.identity, transform);
            spawnedPositions.Add(randomPos);
            spawned++;
        }

        if (spawned < treasuresCount)
        {
            Debug.LogWarning($"Could only spawn {spawned}/{treasuresCount} treasures due to space constraints.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreaCornerOne == null || spawnAreaCornerTwo == null) return;

        Vector3 min = Vector3.Min(spawnAreaCornerOne.position, spawnAreaCornerTwo.position);
        Vector3 max = Vector3.Max(spawnAreaCornerOne.position, spawnAreaCornerTwo.position);

        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawCube((min + max) / 2, max - min);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((min + max) / 2, max - min);
    }
}