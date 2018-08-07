using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
    public bool generateNewMesh = false;
    public float height;
    public float width;
    [Range(3, 360)]
    public int resolution;

	// Use this for initialization
	void Start ()
    {
        generateMesh();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(generateNewMesh)
        {
            generateMesh();
            generateNewMesh = false;
        }
	}

    public void generateMesh ()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[1 + resolution];
        Vector3[] normals = new Vector3[1 + resolution];
        Vector2[] uvs = new Vector2[1 + resolution];
        int[] triangles = new int[resolution * 3];

        vertices[0] = new Vector3(0, 0, 0);
        normals[0] = Vector3.up;
        uvs[0] = 0.5f * Vector2.one;

        for (int i = 0; i < resolution; i++)
        {
            Vector3 pointOnCircle = Quaternion.AngleAxis(i * (360.0f / resolution), Vector3.forward) * (Vector3.up * width + Vector3.forward * height);
            vertices[1 + i] = pointOnCircle;
            normals[1 + i] = pointOnCircle.normalized;
            uvs[1 + i] = 0.5f * Vector2.one + new Vector2(0.5f * Mathf.Cos((2 * Mathf.PI) / (i+1)), 0.5f * Mathf.Sin((2 * Mathf.PI) / (i+1)));
        }

        // Outside
        for (int i = 0; i < resolution; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = 1 + i;
            if (i != resolution - 1)
                triangles[3 * i + 2] = 1 + i + 1;
            else
                triangles[3 * i + 2] = 1;
        }

        mesh.SetVertices(new List<Vector3>(vertices));
        mesh.SetNormals(new List<Vector3>(normals));
        mesh.SetTriangles(triangles, 0, false);
        mesh.SetUVs(0, new List<Vector2>(uvs));
        //mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
