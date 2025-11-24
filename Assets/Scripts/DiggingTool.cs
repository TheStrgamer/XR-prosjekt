using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiggingTool : MonoBehaviour
{

    [Header("Dig Settings")]
    [SerializeField] private float digRadius = 3f;
    [SerializeField] private float digStrength = 3f;

    [SerializeField] private float coolDown = 0.2f;
    [SerializeField] private float digHitDist = 0.25f;
    private float currentCoolDown = 0.5f;

    [SerializeField] private Transform[] holdPoints = null;
    [SerializeField] private Transform hitPoint = null;

    void Start()
    {
        
    }

    void Update()
    {
        HandleDig(Time.deltaTime);
    }

    private void HandleDig(float dt)
    {
        currentCoolDown -= dt;
        if ( currentCoolDown < 0) { currentCoolDown = 0; }

        //Debug.DrawRay(hitPoint.position, hitPoint.forward * digHitDist, Color.red);
        Ray ray = new Ray(hitPoint.position, hitPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, digHitDist))
        {
            MarchingCubes mc = hit.collider.GetComponent<MarchingCubes>();
            if (mc != null)
            {
                float mod = 1 - currentCoolDown / coolDown; //weaker if dug in rapid succession.
                mc.Dig(hit.point, digRadius*mod, digStrength);
                currentCoolDown = coolDown;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (hitPoint == null) return;
        Vector3 origin = hitPoint.position;
        Vector3 direction = hitPoint.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + direction * digHitDist);
    }

}
