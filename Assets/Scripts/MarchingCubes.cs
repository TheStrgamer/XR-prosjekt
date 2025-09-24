using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private int heightUnderSurface = 10;



    [SerializeField] private float tresshold = 0.5f;

    [SerializeField] private float noiseResolution = 1;
    [SerializeField] private bool visualizeNoise;
    [SerializeField] private bool loopStart = false;

    private float [,,] heights;

    private List<Vector3> verts = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter meshFilter;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (loopStart) StartCoroutine(StartAll());
        else {
            setHeights();
            marchCubes();
            SetMesh();
        }

    }

    private IEnumerator StartAll()
    {
        while (true)
        {
            setHeights();
            marchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    private void marchCubes() {
        verts.Clear();
        triangles.Clear();

        
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++) 
                for (int z = 0; z < width; z++) {
                
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i<8; i++) {
                        Vector3Int corner = new Vector3Int(x,y,z) + MarchingCubesTables.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }
                    MarchCube(new Vector3(x,y,z), GetConfigIndex(cubeCorners));
                
                }
    }

    private void MarchCube(Vector3 pos, int index) {
        if (index == 0 || index == 255) return;

        int edgeIndex = 0;
        for (int t = 0; t<5; t++) 
            for (int v = 0; v<3; v++) {
                int triTableVal = MarchingCubesTables.triTable[index][edgeIndex];
                if (triTableVal == -1) {
                    return;
                }
                Vector3 edgeStart = pos + MarchingCubesTables.Edges[triTableVal, 0];
                Vector3 edgeEnd = pos + MarchingCubesTables.Edges[triTableVal, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                verts.Add(vertex);
                triangles.Add(verts.Count - 1);

                edgeIndex++;
            }
    }

    private void SetMesh() {
        Mesh mesh = new Mesh();

        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private int GetConfigIndex(float[] cubeCorners) {
        int configIndex = 0;

        for (int i = 0; i<8; i++) {
            if (cubeCorners[i] > tresshold) {
                configIndex |= 1 << i;
            }

        }
        return configIndex;

    }



    private void setHeights() {
        heights = new float[width + 1, height +1, width +1];

        for (int x = 0; x < width + 1; x++)
            for (int y = 0; y < height + 1; y++) 
                for (int z = 0; z < width + 1; z++) {
                    float currentHeight = Mathf.PerlinNoise(x*noiseResolution, z*noiseResolution) * height;
                    float newHeight;

                    if (y > currentHeight) {
                        newHeight = y-currentHeight;
                    } else {
                        newHeight = currentHeight - y;
                    }
                    heights[x,y,z] = newHeight;
                }
            

        
    }

    private void OnDrawGizmosSelected() {
        if (!visualizeNoise || !Application.isPlaying) return;

        for (int x = 0; x < width + 1; x++)
            for (int y = 0; y < height + 1; y++) 
                for (int z = 0; z < width + 1; z++) {
                    Gizmos.color = new Color(heights[x,y,z], heights[x,y,z], heights[x,y,z], 1);
                    Gizmos.DrawSphere(new Vector3(x,y,z), 0.2f);
                } 
    }
}
