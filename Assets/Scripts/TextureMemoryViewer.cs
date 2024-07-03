using UnityEngine;

using System.Collections;
using TMPro;

public class TextureMemoryDisplay : MonoBehaviour
{
    public TextMeshProUGUI memoryText; // ȭ�鿡 ǥ���� �ؽ�Ʈ UI
    public float updateInterval = 0.2f; // ������Ʈ ����

    private void Start()
    {
        StartCoroutine(UpdateTextureMemory());
    }
    IEnumerator UpdateTextureMemory()
    {
        while (true)
        {
            // ��ü �ؽ�ó �޸� ��뷮�� ���
            long totalTextureMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            long usedTextureMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();

            // ����Ʈ�� �ް�����Ʈ�� ��ȯ
            float totalMemoryMB = totalTextureMemory / (1024.0f * 1024.0f);
            float usedMemoryMB = usedTextureMemory / (1024.0f * 1024.0f);

            // �ؽ�Ʈ UI�� �޸� ���� ǥ��
            memoryText.text = $"Total Texture Memory: {totalMemoryMB:F2} MB\nUsed Texture Memory: {usedMemoryMB:F2} MB";
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
