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
                go.name = "Chunk_" + x +"_"+ z;
                go.GetComponent<MarchingCubes>().SetDimensions(chunkWidth, chunkHeight, chunkHeightUnder, scale, new Vector2Int(x,z));
                chunks[x,z] = go;
                
            }
        }
    }

    public void SetDensity(Vector2Int ind, int dx, int dz, int x, int y, int z, float value)
    {
        int cx = ind.x + dx;
        int cz = ind.y + dz;
        if (cx < 0 || cx >= chunksX || cz < 0 || cz >= chunksZ)
        {
            return;
        }
        GameObject go = chunks[cx, cz];
        MarchingCubes mc = go.GetComponent<MarchingCubes>();
        mc.SetDensity(x,y,z,value);
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
    public void SetNeightbourValue(Vector2Int ind, int dx, int dz)
    {
        float[,] heights;
        int x = ind.x + dx;
        int z = ind.y + dz;
        if (x < 0 || x >= chunksX || z < 0 || z >= chunksZ)
        {
            return;
        }
        GameObject go = chunks[x, z];
        MarchingCubes mc = go.GetComponent<MarchingCubes>();
        if (dx * dz == 0) //is not diagonal
        {
            heights = chunks[ind.x, ind.y].GetComponent<MarchingCubes>().GetSide(dx, dz);
            mc.SetHeightsSide(heights, -dx, -dz);
        }
        else
        {
            heights = chunks[ind.x, z].GetComponent<MarchingCubes>().GetSide(dx, 0);
            mc.SetHeightsSide(heights, -dx, 0);
            heights = chunks[x, ind.y].GetComponent<MarchingCubes>().GetSide(0, dz);
            mc.SetHeightsSide(heights, 0, -dz);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
