using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KtxUnity;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using Unity.Collections;

public class makeMapWithKTX : MonoBehaviour
{
    public GameObject parentObj;
    public int imageWidth = 100; // 이미지의 너비
    public int imageHeight = 100; // 이미지의 높이
    public int spacing = 10; // 이미지 간의 간격
    public Vector2 anchorPivot = new Vector2(0, 0);
    public TextMeshProUGUI _timeRequired;

    private void DestroyCanvasChild()
    {
        foreach (Transform child in parentObj.transform)
        {
            if (child.name.Contains("Image"))
                Destroy(child.gameObject);
        }
    }

    public void LoadJPGImage()
    {
        DestroyCanvasChild();
        C_LoadJPGImage();
    }

    public async void LoadImages(string Encode)
    {
        DestroyCanvasChild();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var texture = new KtxTexture();

                string fileName = $"{y:D4}_{x:D4}.ktx";
                string imagePath = $"SaturateImages/{Encode}/{y:D4}/";
                string filePath = Path.Combine(Application.dataPath, imagePath, fileName);


                byte[] fileData = File.ReadAllBytes(filePath);
                NativeArray<byte> nativeArray = new NativeArray<byte>(fileData, Allocator.Persistent);
                NativeSlice<byte> nativeSlice = new NativeSlice<byte>(nativeArray);

                var result = await texture.LoadFromBytes(nativeSlice);
                nativeArray.Dispose(); // 로드하고 Dispose
                if (result != null)
                {
                    Texture2D _texture = result.texture;
                    if (_texture != null)
                    {
                        bool isXFlip = result.orientation.IsXFlipped();
                        bool isYFlip = result.orientation.IsYFlipped();
                        CreateImageObject(_texture, x, 4 - y, isXFlip, isYFlip);
                    }
                }

            }
        }
        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }

    void C_LoadJPGImage()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                string fileName = $"{y:D4}_{x:D4}.jpg";
                string imagePath = $"/SaturateImages/JPG/{y:D4}/";
                string finalPath = Application.dataPath + imagePath + fileName;
                byte[] binaryImageData = File.ReadAllBytes(finalPath);
                Texture2D tmpTexture = new Texture2D(1,1);

                tmpTexture.LoadImage(binaryImageData);
                if (tmpTexture != null)
                {
                    CreateImageObject(tmpTexture, x, 4 - y);
                    //Destroy(tmpTexture);
                }
            }
        }
        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }

    void CreateImageObject(Texture2D texture, int x, int y, bool isXFlip = false, bool isYFlip = false)
    {
        GameObject imageObject = new GameObject($"Image_{y}_{x}");
        imageObject.transform.SetParent(parentObj.transform);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        rectTransform.pivot = anchorPivot;
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(x * (imageWidth + spacing), -y * (imageHeight + spacing));

        Image image = imageObject.AddComponent<Image>();

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        if (isXFlip)
        {
            rect.x = texture.width;
            rect.width = -texture.width;
        }

        if (isYFlip)
        {
            rect.y = texture.height;
            rect.height = -texture.height;
        }

        image.sprite = Sprite.Create(texture, rect, pivot);
    }
}
  