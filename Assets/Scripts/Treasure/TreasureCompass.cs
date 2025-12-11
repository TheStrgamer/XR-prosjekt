using UnityEngine;

public class TreasureCompass : MonoBehaviour
{
    private Transform[] treasures = null;
    [SerializeField] private GameObject compassHead;
    [SerializeField] private float updateNearestRate = 1.0f;
    [SerializeField] private Transform orientation;
    [SerializeField] private float rotationSpeed = 5f; // how fast the compass lerps

    private float currentUpdateTime = 0f;
    private Transform nearestTreasure;

    void Start()
    {
        FindTreasures();
        currentUpdateTime = updateNearestRate;
    }

    void FindTreasures()
    {
        GameObject[] treasureObjects = GameObject.FindGameObjectsWithTag("Treasure");
        treasures = new Transform[treasureObjects.Length];
        for (int i = 0; i < treasureObjects.Length; i++)
        {
            treasures[i] = treasureObjects[i].transform;
        }
    }

    void Update()
    {
        currentUpdateTime -= Time.deltaTime;
        if (currentUpdateTime <= 0f)
        {
            currentUpdateTime = updateNearestRate;
            FindNearestTreasure();
        }

        RotateCompass();
    }

    void FindNearestTreasure()
    {
        if (treasures == null || treasures.Length == 0)
        {
            FindTreasures();
            return;
        }

        float closestDistSq = Mathf.Infinity;
        nearestTreasure = null;

        foreach (Transform t in treasures)
        {
            float distSq = (t.position - transform.position).sqrMagnitude;
            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                nearestTreasure = t;
            }
        }
    }

    void RotateCompass()
    {
        if (nearestTreasure == null) return;

        Vector3 direction = nearestTreasure.position - transform.position;
        float signedAngle = Vector3.SignedAngle(orientation.forward, direction, orientation.up);

        Quaternion targetRotation = Quaternion.Euler(0, signedAngle - 90f, 0);
        compassHead.transform.localRotation = Quaternion.Lerp(compassHead.transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
