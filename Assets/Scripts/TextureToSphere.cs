using UnityEngine;

public class TextureToSphere : MonoBehaviour
{
    public Texture2D mapTexture; // ���浵 �� �ؽ�ó
    public float radius = 1.0f; // ��ü�� ������

    private Mesh mesh;

    public void CreateMapToSphere(Texture2D tex)
    {
        mapTexture = tex;
        if (mapTexture != null)
        {
            CreateSphere();
        }
    }

    void CreateSphere()
    {
        mesh = new Mesh();

        // ������ �浵�� ����
        //int[] latitudeSegments = new int[] { 0, 69, 113, 158, 180 };
        //int[] longitudeSegments = new int[] { 45, 180, 360, 180, 45 };

        int[] latitudeSegments = new int[] { 0, 1, 179,180 };
        int[] longitudeSegments = new int[]{ 0, 45, 360, 45 };

        // ���ؽ��� �ﰢ�� ���� ���
        int vertCount = CalculateVertexCount(latitudeSegments, longitudeSegments);
        int triCount = CalculateTriangleCount(latitudeSegments, longitudeSegments);

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[triCount * 3];

        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 1; i < latitudeSegments.Length; i++)
        {
            int prevLat = latitudeSegments[i - 1];
            int currLat = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = prevLat; lat <= currLat; lat++)
            {
                float latFraction = (float)lat / 180f;
                float latitude = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, latFraction);

                for (int lon = 0; lon <= lonSegments; lon++)
                {
                    float lonFraction = (float)lon / lonSegments;
                    float longitude = Mathf.Lerp(-Mathf.PI, Mathf.PI, lonFraction);

                    // ECEF ��ǥ ���
                    float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                    float y = radius * Mathf.Sin(latitude);
                    float z = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
                    vertices[vertIndex] = new Vector3(x, y, z);

                    // UV ��ǥ ���
                    uv[vertIndex] = new Vector2(lonFraction, latFraction);

                    if (lat < currLat && lon < lonSegments)
                    {
                        int current = vertIndex;
                        int next = vertIndex + lonSegments + 1;

                        if (next < vertices.Length && current + 1 < vertices.Length && next + 1 < vertices.Length)
                        {
                            triangles[triIndex++] = current;
                            triangles[triIndex++] = next;
                            triangles[triIndex++] = current + 1;
                            
                            triangles[triIndex++] = current + 1;
                            triangles[triIndex++] = next;
                            triangles[triIndex++] = next + 1;
                        }
                    }

                    vertIndex++;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<Renderer>().material.mainTexture = mapTexture;
    }

    int CalculateVertexCount(int[] latitudeSegments, int[] longitudeSegments)
    {
        int count = 0;
        for (int i = 1; i < latitudeSegments.Length; i++)
        {
            int prevLat = latitudeSegments[i - 1];
            int currLat = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = prevLat; lat <= currLat; lat++)
            {
                count += (lonSegments + 1);
            }
        }
        return count;
    }

    int CalculateTriangleCount(int[] latitudeSegments, int[] longitudeSegments)
    {
        int count = 0;
        for (int i = 1; i < latitudeSegments.Length; i++)
        {
            int prevLat = latitudeSegments[i - 1];
            int currLat = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = prevLat; lat < currLat; lat++)
            {
                count += lonSegments * 2;
            }
        }
        return count;
    }
}
