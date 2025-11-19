using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private int heightUnderSurface = 10;
    [SerializeField] private float scale = 1;
    [SerializeField] private float threshold = 0.5f;

    [SerializeField] private float noiseResolution = 1;
    [SerializeField] private bool visualizeNoise;
    [SerializeField] private bool loopStart = false;

    private float[,,] heights;

    private List<Vector3> verts = new List<Vector3>();
    Dictionary<(int x, int y, int z, int edge), int> edgeVertexCache;

    private List<int> triangles = new List<int>();

    private MeshCollider meshCollider;

    private MeshFilter meshFilter;
    private CubeChunks chunk;
    private Vector2Int ind;

    private List<Color> colors = new List<Color>();


    void Start(){
        meshFilter = GetComponent<MeshFilter>();
        if (loopStart) StartCoroutine(StartAll());
        else{
            setHeights();
            MarchCubes();
            SetMesh();
        }

    }
    void init()
    {
        chunk = GetComponentInParent<CubeChunks>();
        meshFilter = GetComponent<MeshFilter>();
        setHeights();
        MarchCubes();
        SetMesh();
    }

    public void SetDimensions(int width, int height, int heightUnder, float scale, Vector2Int ind)
    {
        this.width = width;
        this.height = height;
        this.heightUnderSurface = heightUnder;
        this.scale = scale;
        this.ind = ind;
        init();
    }

    private IEnumerator StartAll()
    {
        while (true){
            //setHeights();
            // MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    private void MarchCubes()
    {
        verts.Clear();
        colors.Clear();
        triangles.Clear();
        edgeVertexCache = new Dictionary<(int, int, int, int), int>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height + heightUnderSurface; y++)
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingCubesTables.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), GetConfigIndex(cubeCorners), cubeCorners);
                }
    }

    private void MarchCube(Vector3 pos, int configIndex, float[] cubeCorners)
    {
        if (configIndex == 0 || configIndex == 255) return;

        int edgeIndex = 0;

        while (MarchingCubesTables.triTable[configIndex][edgeIndex] != -1)
        {
            int edge = MarchingCubesTables.triTable[configIndex][edgeIndex];

            int c1 = MarchingCubesTables.EdgeConnection[edge, 0];
            int c2 = MarchingCubesTables.EdgeConnection[edge, 1];

            Vector3 edgeStart = pos + MarchingCubesTables.Corners[c1];
            Vector3 edgeEnd = pos + MarchingCubesTables.Corners[c2];
            edgeEnd *= scale;
            edgeStart *= scale;

            float v1 = cubeCorners[c1];
            float v2 = cubeCorners[c2];

            float t = Mathf.Clamp01(Mathf.InverseLerp(v1, v2, threshold));
            Vector3 vertex = Vector3.Lerp(edgeStart, edgeEnd, t);

            (int x, int y, int z, int e) key = (Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z), edge);
            if (!edgeVertexCache.TryGetValue(key, out int vertIndex))
            {
                verts.Add(vertex);
                float depthFactor = Mathf.InverseLerp(height + heightUnderSurface, 0, pos.y);
                depthFactor = Mathf.Pow(depthFactor, 0.4f);
                Color col = Color.Lerp(Color.white, Color.black, depthFactor);
                colors.Add(col);

                vertIndex = verts.Count - 1;
                edgeVertexCache[key] = vertIndex;
            }

            triangles.Add(vertIndex);
            edgeIndex++;
        }

    }


    private void SetMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = verts.ToArray();
        mesh.SetColors(colors);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null){
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    private int GetConfigIndex(float[] cubeCorners){
        int configIndex = 0;
        for (int i = 0; i < 8; i++){
            if (cubeCorners[i] > threshold){
                configIndex |= 1 << i;
            }
        }
        return configIndex;

    }

private void setHeights()
{
    heights = new float[width + 1, height + heightUnderSurface + 1, width + 1];

    for (int x = 0; x < width + 1; x++)
    {
        for (int y = 0; y < height + heightUnderSurface + 1; y++)
        {
            for (int z = 0; z < width + 1; z++)
            {
                float worldY = y * scale + transform.position.y;
                float wx = x * scale + transform.position.x;
                float wz = z * scale + transform.position.z;

                float terrainHeight = Mathf.PerlinNoise(wx * noiseResolution, wz * noiseResolution) * height + heightUnderSurface;
                float density = worldY - terrainHeight + heightUnderSurface;
                density += (Mathf.PerlinNoise(wx * noiseResolution * 0.5f, wz * noiseResolution * 0.5f) - 0.5f) * 5f;

                heights[x, y, z] = density;
            }
        }
    }
}


    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying) return;

        for (int x = 0; x < width + 1; x++)
            for (int y = 0; y < height + 1; y++)
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f);
                }
    }

    public void SetDensity(int x, int y, int z, float value)
    {
        if (x < 0 || x >= heights.GetLength(0)) return;
        if (y < 0 || y >= heights.GetLength(1)) return;
        if (z < 0 || z >= heights.GetLength(2)) return;
        heights[x, y, z] = value;
    }

    private void DigNeighbour(int dx, int dz, Vector3 worldPos, float radius, float strength)
    {
        chunk.DigNeighbour(ind, dx, dz, worldPos, radius, strength);
        chunk.SetNeightbourValue(ind, dx, dz);
    }

    private void HandleNeighbour(Vector3 worldPosition, float radius, float strength, int minX, int maxX, int minZ, int maxZ)
    {
        if (minX <= 0)
        {
            DigNeighbour(-1, 0, worldPosition, radius, strength);
            if (minZ <= 0)
            {
                DigNeighbour(-1, -1, worldPosition, radius, strength);
            }
            if (maxZ >= width)
            {
                DigNeighbour(-1, 1, worldPosition, radius, strength);
            }
        }
        if (maxX >= width)
        {
            DigNeighbour(1, 0, worldPosition, radius, strength);
            if (minZ <= 0)
            {
                DigNeighbour(1, -1, worldPosition, radius, strength);
            }
            if (maxZ >= width)
            {
                DigNeighbour(1, 1, worldPosition, radius, strength);
            }
        }
        if (minZ <= 0)
        {
            DigNeighbour(0, -1, worldPosition, radius, strength);
            if (minX <= 0)
            {
                DigNeighbour(-1, -1, worldPosition, radius, strength);
            }
            if (maxX >= width)
            {
                DigNeighbour(1, -1, worldPosition, radius, strength);
            }
        }
        if (maxZ >= width)
        {
            DigNeighbour(0, 1, worldPosition, radius, strength);
            if (minX <= 0)
            {
                DigNeighbour(-1, 1, worldPosition, radius, strength);
            }
            if (maxX >= width)
            {
                DigNeighbour(1, 1, worldPosition, radius, strength);
            }
        }
    }

    public void Dig(Vector3 worldPosition, float radius, float strength, bool fromNeighbour = false)
    {
        if (heights == null) return;
        if (radius <= 0f) return;

        Vector3 localPos = transform.InverseTransformPoint(worldPosition) / scale;
        float radiusInCells = Mathf.Max(0.0001f, radius / scale);
        float radiusSq = radiusInCells * radiusInCells;

        int minX = Mathf.Max(0, Mathf.FloorToInt(localPos.x - radiusInCells));
        int maxX = Mathf.Min(heights.GetLength(0) - 1, Mathf.CeilToInt(localPos.x + radiusInCells));
        int minY = Mathf.Max(0, Mathf.FloorToInt(localPos.y - radiusInCells));
        int maxY = Mathf.Min(heights.GetLength(1) - 1, Mathf.CeilToInt(localPos.y + radiusInCells));
        int minZ = Mathf.Max(0, Mathf.FloorToInt(localPos.z - radiusInCells));
        int maxZ = Mathf.Min(heights.GetLength(2) - 1, Mathf.CeilToInt(localPos.z + radiusInCells));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (y == 0 || y == height + heightUnderSurface)
                    {
                        continue;
                    }
                    float dx = x - localPos.x;
                    float dy = y - localPos.y;
                    float dz = z - localPos.z;

                    float distSq = dx * dx + dy * dy + dz * dz;
                    if (distSq > radiusSq) continue;

                    float dist = Mathf.Sqrt(distSq);
                    float falloff = Mathf.Clamp01(1f - (dist / radiusInCells));

                    heights[x, y, z] = Mathf.Lerp(heights[x, y, z], -strength, falloff);
                }
            }
        }

        if (!fromNeighbour && chunk != null)
        {
            //this ensures syncing across chunks
            //both neighbour and diagonal
            HandleNeighbour(worldPosition, radius, strength, minX, maxX, minZ, maxZ);
            
        }

        MarchCubes();
        SetMesh();
    }
    public float[,] GetSide(int dx, int dz)
    {
        int ySize = height + heightUnderSurface + 1;
        int w = width + 1;
        float[,] side = new float[ySize, w];

        if (dx == 1)
        {
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < w; z++)
                    side[y, z] = heights[width, y, z];
        }
        else if (dx == -1)
        {
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < w; z++)
                    side[y, z] = heights[0, y, z];
        }
        else if (dz == 1)
        {
            for (int y = 0; y < ySize; y++)
                for (int x = 0; x < w; x++)
                    side[y, x] = heights[x, y, width];
        }
        else if (dz == -1)
        {
            for (int y = 0; y < ySize; y++)
                for (int x = 0; x < w; x++)
                    side[y, x] = heights[x, y, 0];
        }

        return side;
    }


    public void SetHeightsSide(float[,] val, int dx, int dz)
    {
        int ySize = height + heightUnderSurface + 1;
        int w = width + 1;

        if (dx == 1)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < w; z++)
                {
                    heights[width, y, z] = val[y, z];
                }
            }
        }
        else if (dx == -1)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < w; z++)
                {
                    heights[0, y, z] = val[y, z];
                }
            }
        }
        else if (dz == 1)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    heights[x, y, width] = val[y, x];
                }
            }
        }
        else if (dz == -1)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    heights[x, y, 0] = val[y, x];
                }
            }
        }
        MarchCubes();
        SetMesh();
    }

}
