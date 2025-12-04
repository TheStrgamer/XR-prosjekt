using UnityEngine;

public class TreasureCompass : MonoBehaviour
{
    private Transform[] Treasures = null;
    [SerializeField] private GameObject compassHead;
    [SerializeField] private float updateNearestRate = 1.0f;
    [SerializeField] private Transform orientation;

    private float currentUpdateTime = 0.0f;
    private Transform nearestTreasure;


    void Start()
    {
        findTreasures();
    }

    void findTreasures()
    {
        GameObject[] treasureObjects = GameObject.FindGameObjectsWithTag("Treasure");
        Treasures = new Transform[treasureObjects.Length];
        for (int i = 0; i < treasureObjects.Length; i++)
        {
            Treasures[i] = treasureObjects[i].transform;
        }
    }
    private void Update()
    {
        currentUpdateTime -= Time.deltaTime;

        if (currentUpdateTime <= 0f)
        {
            currentUpdateTime = updateNearestRate;
            FindNearestTreasure();
        }

    }

    void FindNearestTreasure()
    {
        if (Treasures == null || Treasures.Length == 0 ) {
            findTreasures();
            return; 
        }
        float closestDist = Mathf.Infinity;
        nearestTreasure = null;

        foreach (Transform t in Treasures)
        {
            float dist = Vector3.Distance(transform.position, t.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearestTreasure = t;
            }
        }
    }

    void FixedUpdate()
    {
        if (nearestTreasure == null) { return; }
        Vector3 direction = nearestTreasure.position - transform.position;

        float signedAngle = Vector3.SignedAngle(orientation.forward, direction, Vector3.up);

        Quaternion rotation = Quaternion.Euler(0, signedAngle - 90f, 0);

        compassHead.transform.localRotation = rotation;

    }

}
