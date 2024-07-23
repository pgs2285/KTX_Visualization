using UnityEngine;

public class TextureToSphere : MonoBehaviour
{
    public Texture2D mapTexture; // 위경도 맵 텍스처
    public float radius = 1.0f; // 구체의 반지름

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

        // 위도와 경도에 따른 vertex의 개수 정의
        int[] latitudeSegments = new int[] { 0, 22, 45, 67, 90 }; 
        int[] longitudeSegments = new int[] { 45, 90, 180, 360, 360 };

        // 버텍스와 삼각형 개수 계산
        int vertCount = CalculateVertexCount(latitudeSegments, longitudeSegments);
        int triCount = CalculateTriangleCount(latitudeSegments, longitudeSegments);

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[triCount * 3];

        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 1; i < latitudeSegments.Length; i++)
        {
            int latSegmentsPrev = latitudeSegments[i - 1];
            int latSegments = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = latSegmentsPrev; lat <= latSegments; lat++)
            {
                float latFraction = (float)lat / 90f;
                float latitude = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, latFraction);

                for (int lon = 0; lon <= lonSegments; lon++)
                {
                    float lonFraction = (float)lon / lonSegments;
                    float longitude = Mathf.Lerp(-Mathf.PI, Mathf.PI, lonFraction);

                    // ECEF 좌표 계산
                    float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                    float y = radius * Mathf.Sin(latitude);
                    float z = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
                    vertices[vertIndex] = new Vector3(x, y, z);

                    // UV 좌표 계산
                    uv[vertIndex] = new Vector2(lonFraction, latFraction);

                    if (lat < latSegments && lon < lonSegments)
                    {
                        int current = vertIndex;
                        int next = vertIndex + lonSegments + 1;

                        triangles[triIndex++] = current;
                        triangles[triIndex++] = next;
                        triangles[triIndex++] = current + 1;

                        triangles[triIndex++] = current + 1;
                        triangles[triIndex++] = next;
                        triangles[triIndex++] = next + 1;
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
            int latSegmentsPrev = latitudeSegments[i - 1];
            int latSegments = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = latSegmentsPrev; lat <= latSegments; lat++)
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
            int latSegmentsPrev = latitudeSegments[i - 1];
            int latSegments = latitudeSegments[i];
            int lonSegments = longitudeSegments[i];

            for (int lat = latSegmentsPrev; lat < latSegments; lat++)
            {
                count += lonSegments * 2;
            }
        }
        return count;
    }
}
