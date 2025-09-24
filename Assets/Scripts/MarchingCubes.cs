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
    [SerializeField] private float tresshold = 0.5f;

    [SerializeField] private float noiseResolution = 1;
    [SerializeField] private bool visualizeNoise;
    [SerializeField] private bool loopStart = false;

    private float[,,] heights;

    private List<Vector3> verts = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshCollider meshCollider;

    private MeshFilter meshFilter;
    void Start(){
        meshFilter = GetComponent<MeshFilter>();
        if (loopStart) StartCoroutine(StartAll());
        else{
            setHeights();
            MarchCubes();
            SetMesh();
        }

    }

    private IEnumerator StartAll()
    {
        while (true){
            setHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    private void MarchCubes(){
        verts.Clear();
        triangles.Clear();


        for (int x = 0; x < width; x++)
            for (int y = 0; y < height + heightUnderSurface; y++)
                for (int z = 0; z < width; z++){

                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++){
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingCubesTables.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }
                    MarchCube(new Vector3(x, y, z), GetConfigIndex(cubeCorners));

                }
    }

    private void MarchCube(Vector3 pos, int index){
        if (index == 0 || index == 255) return;

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
            for (int v = 0; v < 3; v++){
                int triTableVal = MarchingCubesTables.triTable[index][edgeIndex];
                if (triTableVal == -1){
                    return;
                }
                Vector3 edgeStart = pos + MarchingCubesTables.Edges[triTableVal, 0];
                Vector3 edgeEnd = pos + MarchingCubesTables.Edges[triTableVal, 1];
                edgeEnd *= scale;
                edgeStart *= scale;

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                verts.Add(vertex);
                triangles.Add(verts.Count - 1);

                edgeIndex++;
            }
    }

private void SetMesh()
{
    Mesh mesh = new Mesh();
    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

    mesh.vertices = verts.ToArray();
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
            if (cubeCorners[i] > tresshold){
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
                float worldY = y * scale;
                float terrainHeight = Mathf.PerlinNoise(x * noiseResolution, z * noiseResolution) * height;
                float density = terrainHeight - worldY;
                density += (Mathf.PerlinNoise(x * noiseResolution * 0.5f, z * noiseResolution * 0.5f) - 0.5f) * 5f;

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

    public void Dig(Vector3 worldPosition, float radius, float strength)
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

        MarchCubes();
        SetMesh();
    }


}
