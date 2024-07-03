using UnityEngine;

using System.Collections;
using TMPro;

public class TextureMemoryDisplay : MonoBehaviour
{
    public TextMeshProUGUI memoryText; // 화면에 표시할 텍스트 UI
    public float updateInterval = 0.2f; // 업데이트 간격

    private void Start()
    {
        StartCoroutine(UpdateTextureMemory());
    }
    IEnumerator UpdateTextureMemory()
    {
        while (true)
        {
            // 전체 텍스처 메모리 사용량을 계산
            long totalTextureMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            long usedTextureMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();

            // 바이트를 메가바이트로 변환
            float totalMemoryMB = totalTextureMemory / (1024.0f * 1024.0f);
            float usedMemoryMB = usedTextureMemory / (1024.0f * 1024.0f);

            // 텍스트 UI에 메모리 정보 표시
            memoryText.text = $"Total Texture Memory: {totalMemoryMB:F2} MB\nUsed Texture Memory: {usedMemoryMB:F2} MB";
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
