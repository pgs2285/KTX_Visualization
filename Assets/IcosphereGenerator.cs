using UnityEngine;
using System.Collections.Generic;

public class IcosphereGenerator : MonoBehaviour
{
    public int subdivisions = 3; // 기본 분할 수준
    public float radius = 1f;    // 구의 반지름

    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshFilter.mesh = GenerateIcosphere(subdivisions, radius);
    }

    Mesh GenerateIcosphere(int subdivisions, float radius)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // 정십이면체 초기 정점
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertices.AddRange(new Vector3[]
        {
            new Vector3(-1,  t,  0).normalized * radius,
            new Vector3( 1,  t,  0).normalized * radius,
            new Vector3(-1, -t,  0).normalized * radius,
            new Vector3( 1, -t,  0).normalized * radius,

            new Vector3( 0, -1,  t).normalized * radius,
            new Vector3( 0,  1,  t).normalized * radius,
            new Vector3( 0, -1, -t).normalized * radius,
            new Vector3( 0,  1, -t).normalized * radius,

            new Vector3( t,  0, -1).normalized * radius,
            new Vector3( t,  0,  1).normalized * radius,
            new Vector3(-t,  0, -1).normalized * radius,
            new Vector3(-t,  0,  1).normalized * radius
        });

        triangles.AddRange(new int[]
        {
            0, 11, 5,    0, 5, 1,     0, 1, 7,     0, 7, 10,    0, 10, 11,
            1, 5, 9,     5, 11, 4,    11, 10, 2,   10, 7, 6,    7, 1, 8,
            3, 9, 4,     3, 4, 2,     3, 2, 6,     3, 6, 8,     3, 8, 9,
            4, 9, 5,     2, 4, 11,    6, 2, 10,    8, 6, 7,     9, 8, 1
        });

        // 중심부 가까운 삼각형을 더 많이 분할
        for (int i = 0; i < subdivisions; i++)
        {
            SubdivideCentral(vertices, triangles, radius);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    void SubdivideCentral(List<Vector3> vertices, List<int> triangles, float radius)
    {
        Dictionary<long, int> middlePointCache = new Dictionary<long, int>();
        List<int> newTriangles = new List<int>();

        Vector3 center = Vector3.zero;
        float maxDistance = radius * Mathf.Sqrt(3f); // 정점 최대 거리

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            Vector3 p1 = vertices[v1];
            Vector3 p2 = vertices[v2];
            Vector3 p3 = vertices[v3];

            Vector3 triangleCenter = (p1 + p2 + p3) / 3f;
            float distanceToCenter = triangleCenter.magnitude;

            if (distanceToCenter < maxDistance / 2f) // 중앙부 분할
            {
                int a = GetMiddlePoint(v1, v2, vertices, middlePointCache, radius);
                int b = GetMiddlePoint(v2, v3, vertices, middlePointCache, radius);
                int c = GetMiddlePoint(v3, v1, vertices, middlePointCache, radius);

                newTriangles.AddRange(new int[] { v1, a, c });
                newTriangles.AddRange(new int[] { v2, b, a });
                newTriangles.AddRange(new int[] { v3, c, b });
                newTriangles.AddRange(new int[] { a, b, c });
            }
            else // 외곽부 유지
            {
                newTriangles.AddRange(new int[] { v1, v2, v3 });
            }
        }

        triangles.Clear();
        triangles.AddRange(newTriangles);
    }

    int GetMiddlePoint(int p1, int p2, List<Vector3> vertices, Dictionary<long, int> cache, float radius)
    {
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        if (cache.TryGetValue(key, out int ret))
        {
            return ret;
        }

        Vector3 middle = (vertices[p1] + vertices[p2]).normalized * radius;
        int i = vertices.Count;
        vertices.Add(middle);

        cache.Add(key, i);
        return i;
    }
}
