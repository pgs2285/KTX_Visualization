using UnityEngine;

public class TextureToSphere : MonoBehaviour
{
    public Texture2D mapTexture; // ���浵 �� �ؽ�ó
    public int longitudeSegments = 360; // �浵 �����̽� ��
    public int latitudeSegments = 180; // ���� �����̽� ��
    public float radius = 1.0f; // ��ü�� ������
    public GameObject markerPrefab; // ��Ŀ ������

    private Mesh mesh;

    public void createMapToSphere(Texture2D tex)
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
        Vector3[] vertices = new Vector3[(longitudeSegments + 1) * (latitudeSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[longitudeSegments * latitudeSegments * 6];

        int vertIndex = 0;
        int triIndex = 0;

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                // ������ �浵 ���
                float latFraction = (float)lat / latitudeSegments;
                float lonFraction = (float)lon / longitudeSegments;
                float latitude = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, latFraction);
                float longitude = Mathf.Lerp(-Mathf.PI, Mathf.PI, lonFraction);

                // ECEF ��ǥ ���
                float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                float y = radius * Mathf.Sin(latitude);
                float z = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
                vertices[vertIndex] = new Vector3(x, y, z);

                // UV ��ǥ ���
                uv[vertIndex] = new Vector2(lonFraction, latFraction);

                // �ﰢ�� �ε��� ���
                if (lat < latitudeSegments && lon < longitudeSegments)
                {
                    int current = vertIndex;
                    int next = vertIndex + longitudeSegments + 1;

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

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<Renderer>().material.mainTexture = mapTexture;
    }


}
