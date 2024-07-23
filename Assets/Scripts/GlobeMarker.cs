using UnityEngine;

public class GlobeMarker : MonoBehaviour
{
    public GameObject markerPrefab; // ��Ŀ ������
    public float globeRadius = 10.0f; // ��ü ������

    public void globeMarker(Texture2D globeTexture)
    {
        // ��ü ���� �� �ؽ�ó ����
        GameObject globe = GameObject.FindWithTag("Sphere");
        globe.transform.localScale = new Vector3(globeRadius, globeRadius, globeRadius);
        ApplyTextureMapping(globe);
        globe.GetComponent<Renderer>().material.mainTexture = globeTexture;
    }

    public void PlaceMarker(float lat, float lon)
    {
        Vector3 ecefPosition = LatLonToECEF(lat, lon, globeRadius);

        // ��Ŀ ������Ʈ ���� �� ��ġ ����
        GameObject marker = Instantiate(markerPrefab, ecefPosition, Quaternion.identity);
        marker.transform.SetParent(GameObject.FindWithTag("Sphere").transform, false); // �������� �ڽ� ������Ʈ�� ����
        marker.transform.localPosition = ecefPosition.normalized * globeRadius;
    }

    Vector3 LatLonToECEF(float lat, float lon, float radius)
    {
        float latRad = lat * Mathf.Deg2Rad;
        float lonRad = lon * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
        float z = radius * Mathf.Sin(latRad);

        return new Vector3(x, y, z);
    }

    Vector3 ECEFtoLLA(Vector3 ecef)
    {
        float x = ecef.x;
        float y = ecef.y;
        float z = ecef.z;

        float radius = globeRadius;

        float lat = Mathf.Asin(z / radius) * Mathf.Rad2Deg;
        float lon = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        float alt = Mathf.Sqrt(x * x + y * y + z * z) - radius;

        return new Vector3(lat, lon, alt);
    }

    Vector2 LatLonToUV(float lat, float lon)
    {
        float u = lon / 360.0f + 0.5f; // �浵�� 0-1�� ��ȯ (�߾��� 0)
        float v = 0.5f - lat / 180.0f; // ������ 0-1�� ��ȯ (�߾��� 0)

        return new Vector2(u, v);
    }

    void ApplyTextureMapping(GameObject globe)
    {
        Mesh mesh = globe.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i].normalized; // ����ȭ�� ���� ��ǥ
            Vector3 lla = ECEFtoLLA(vertex * globeRadius); // ECEF�� LLA�� ��ȯ

            // UV ��ǥ ���
            uv[i] = LatLonToUV(lla.x, lla.y);

            // ���� ó��
            if (Mathf.Abs(lla.x) > 89.9999f)
            {
                uv[i].x = 0.5f;
            }
        }

        mesh.uv = uv;
    }
}
