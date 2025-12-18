using UnityEngine;

public class TreasureCompass : MonoBehaviour
{
    private Transform[] treasures = null;
    [SerializeField] private GameObject compassHead;
    [SerializeField] private float updateNearestRate = 1.0f;
    [SerializeField] private Transform orientation;
    [SerializeField] private float rotationSpeed = 5f;

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
        Vector3 worldDir = nearestTreasure.position - transform.position;
        worldDir.y = 0f;

        if (worldDir.sqrMagnitude < 0.001f) return;

        Vector3 localDir = transform.InverseTransformDirection(worldDir);

        float targetY = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        float currentY = compassHead.transform.localEulerAngles.y;

        float newY = Mathf.MoveTowardsAngle(
            currentY,
            targetY - 90f,
            rotationSpeed * 360f * Time.deltaTime
        );

        compassHead.transform.localEulerAngles = new Vector3(0f, newY, 0f);
    }


}
