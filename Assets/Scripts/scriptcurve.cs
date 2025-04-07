using UnityEngine;

public class CurvedUI : MonoBehaviour
{
    public float curveStrength = 1.0f;

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            float curveAmount = Mathf.Pow(x, 2) * curveStrength;
            vertices[i].z += curveAmount;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
