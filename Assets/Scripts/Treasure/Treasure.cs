using UnityEngine;
using System.Collections.Generic;


public class Treasure : MonoBehaviour
{
    [SerializeField] private List<Transform> revealPoints = new List<Transform>();
    private Rigidbody rb;
    private Transform playerCam;
    private bool revealed = false;

    [SerializeField] private int score = 100;
    [SerializeField] private float displayScale = 0.75f;
    void Start()
    {
        playerCam = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (!revealed)
        {
            if (Vector3.Distance(transform.position, playerCam.transform.position) < 15)
            {
                for (int i = 0; i < revealPoints.Count; i++)
                {
                    RaycastHit hit;
                    Transform point = revealPoints[i];
                    Vector3 dir = point.position - playerCam.position;
                    dir = dir.normalized;
                    float dist = Vector3.Distance(playerCam.position, point.position);

                    if (Physics.Raycast(playerCam.position, dir, out hit, dist))
                    {
                        if (hit.collider.tag != "Ground")
                        {
                            revealPoints.Remove(point);
                            Debug.Log("removed point " + i + " by hitting " + hit.collider.name);
                        }
                    }
                }
                if (revealPoints.Count <= 0)
                {
                    Debug.Log("releasing");
                    rb.isKinematic = false;
                    revealed = true;
                }
            }
        }
        
    }

    public int getScore()
    {
        return score;
    }
    public float getDisplayScale()
    {
        return displayScale;
    }
}
