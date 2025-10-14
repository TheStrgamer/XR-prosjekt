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
    private GameObject[,] chunks;


    void Start()
    {
        chunks = new GameObject[chunksX, chunksZ];
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                Vector3 pos = transform.position + new Vector3(x * chunkWidth * scale, -chunkHeightUnder, z * chunkWidth * scale);
                GameObject go = Instantiate(chunkPrefab, pos, Quaternion.Euler(0f, 0f, 0f), transform);
                go.GetComponent<MarchingCubes>().SetDimensions(chunkWidth+1, chunkHeight, chunkHeightUnder, scale, new Vector2Int(x,z));
                chunks[x,z] = go;
                
            }
        }
    }

    public void DigNeighbour(Vector2Int ind, int dx, int dz, Vector3 digOrigin, float radius, float strength) {
        int x = ind.x + dx;
        int z = ind.y + dz;
        if (x < 0 || x >= chunksX || z < 0 || z >= chunksZ)
        {
            return;
        }
        GameObject go = chunks[x,z];
        MarchingCubes mc = go.GetComponent<MarchingCubes>();
        mc.Dig(digOrigin, radius, strength, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
