using UnityEngine;

public class TextureToSphere : MonoBehaviour
{
    public Texture2D mapTexture; // 위경도 맵 텍스처
    public int longitudeSegments = 360; // 경도 슬라이스 수
    public int latitudeSegments = 180; // 위도 슬라이스 수
    public float radius = 1.0f; // 구체의 반지름
    public GameObject markerPrefab; // 마커 프리팹

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
                // 위도와 경도 계산
                float latFraction = (float)lat / latitudeSegments;
                float lonFraction = (float)lon / longitudeSegments;
                float latitude = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, latFraction);
                float longitude = Mathf.Lerp(-Mathf.PI, Mathf.PI, lonFraction);

                // ECEF 좌표 계산
                float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
                float y = radius * Mathf.Sin(latitude);
                float z = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
                vertices[vertIndex] = new Vector3(x, y, z);

                // UV 좌표 계산
                uv[vertIndex] = new Vector2(lonFraction, latFraction);

                // 삼각형 인덱스 계산
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
