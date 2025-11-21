using UnityEngine;
using TMPro;

public class TreasureStand : MonoBehaviour
{
    [SerializeField] private GameObject displayGlass;
    private GameObject displayItem;
    [SerializeField] private TextMeshProUGUI displayText;


    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 45, 0);
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private Transform treasurePos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (displayItem != null)
        {
            displayItem.transform.Rotate(rotationSpeed * Time.deltaTime);

            float newY = treasurePos.position.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            displayItem.transform.position = new Vector3(
                displayItem.transform.position.x,
                newY,
                displayItem.transform.position.z
            );
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (displayItem == null && other.gameObject.GetComponent<Treasure>())
        {
            SetDisplayItem(other.gameObject);
        }
    }

    void SetDisplayItem(GameObject go)
    {
        displayItem = go;
        Treasure treasure = displayItem.GetComponent<Treasure>();
        displayItem.transform.position = treasurePos.position;
        displayItem.transform.rotation = treasurePos.rotation;
        displayGlass.SetActive(true);
        string text = displayItem.name;
        displayText.text = text += "\npoints: " + treasure.getScore();
        if (displayItem.GetComponent<Rigidbody>() != null)
        {
            displayItem.GetComponent<Rigidbody>().isKinematic = true;
        }
        float scale = treasure.getDisplayScale();
        displayItem.transform.localScale = new Vector3(scale,scale,scale);
        displayItem.GetComponent<Collider>().enabled = false;
    }
}
