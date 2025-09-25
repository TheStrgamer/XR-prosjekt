using UnityEngine;

public class CubeChunks : MonoBehaviour
{
    public GameObject chunkPrefab;
    public int chunkWidth = 16;
    public int chunkHeight = 16;
    public int chunkHeightUnder = 64;
    public float scale = 1;

    public int chunksX = 3;
    public int chunksZ = 3;


    void Start()
    {
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                Vector3 pos = transform.position + new Vector3(x * chunkWidth, 0, z * chunkWidth);
                GameObject go = Instantiate(chunkPrefab, pos, Quaternion.Euler(180f, 0f, 0f), transform);
                go.GetComponent<MarchingCubes>().SetDimensions(chunkWidth, chunkHeight, chunkHeightUnder, scale);
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
