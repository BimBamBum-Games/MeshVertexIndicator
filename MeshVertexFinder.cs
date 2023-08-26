using UnityEngine;

[ExecuteInEditMode]
public class MeshVertexFinder : MonoBehaviour
{
    public Mesh mesh;
    public int[] meshTriangles;
    public Vector3[] meshVertices;
    public bool isVertexShow;

    private Vector3[] cVertices;
    private int[] cTriangles;

    [Range(0.05f, 1f)] public float vertexScale = 0.05f;
    void Start()
    {
        Init();
        GetClone();
    }
    
    public void Init()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = mesh.triangles;
        meshVertices = mesh.vertices;
        Debug.Log("Vertex Miktari >>>>> " + meshVertices.Length);
    }

    public void GetClone()
    {
        cVertices = meshVertices;
        cTriangles = meshTriangles;
    }

    [ContextMenu("Recovery")]
    public void Recovery()
    {
        mesh.vertices = cVertices;
        mesh.triangles = cTriangles;
    }
}
