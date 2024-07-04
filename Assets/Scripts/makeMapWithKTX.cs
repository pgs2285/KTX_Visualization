using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KtxUnity;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using Unity.Collections;
using System.Collections.Generic;

public class makeMapWithKTX : MonoBehaviour
{
    public GameObject parentObj;
    public int imageWidth = 100; // 이미지의 너비
    public int imageHeight = 100; // 이미지의 높이
    public int spacing = 10; // 이미지 간의 간격
    public Vector2 anchorPivot = new Vector2(0, 0);
    public TextMeshProUGUI _timeRequired;

    private Dictionary<int, CachedTexture> _textureCaches = new Dictionary<int, CachedTexture>();

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
                string fileName = $"{y:D4}_{x:D4}.ktx";
                string imagePath = $"SaturateImages/{Encode}/{y:D4}/";
                string filePath = Path.Combine(Application.dataPath, imagePath, fileName);
                int fileHash = filePath.GetHashCode();

                if (_textureCaches.ContainsKey(fileHash))
                {
                    CachedTexture cachedTexture = _textureCaches[fileHash];
                    CreateImageObject(cachedTexture.Texture, x, 4 - y, cachedTexture.IsXFlipped, cachedTexture.IsYFlipped);
                    continue;
                }

                var texture = new KtxTexture();
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
                        _textureCaches[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip); 
                        CreateImageObject(_texture, x, 4 - y, isXFlip, isYFlip);
                    }
                }
            }
        }
        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }

    public async void LoadImagesKtx2(string Encode)
    {
        DestroyCanvasChild();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                string fileName = $"{y:D4}_{x:D4}.ktx2";
                string imagePath = $"SaturateImages/{Encode}/{y:D4}/";
                string filePath = Path.Combine(Application.dataPath, imagePath, fileName);
                int fileHash = filePath.GetHashCode();

                if (_textureCaches.ContainsKey(fileHash))
                {
                    CachedTexture cachedTexture = _textureCaches[fileHash];
                    CreateImageObject(cachedTexture.Texture, x, 4 - y, cachedTexture.IsXFlipped, cachedTexture.IsYFlipped);
                    continue;
                }

                var texture = new KtxTexture();
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
                        if (Encode == ("KTX")) isYFlip = false; // ktx는 --lower_left_maps_to_s0t0옵션을 주어도 yflip이 true로 나와 예외처리
                        _textureCaches[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip);
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
                int fileHash = finalPath.GetHashCode();

                if (_textureCaches.ContainsKey(fileHash))
                {
                    CreateImageObject(_textureCaches[fileHash].Texture, x, 4 - y); // 해시로 변환해서 속도 좀 더 이득
                    continue;
                }

                byte[] binaryImageData = File.ReadAllBytes(finalPath);
                Texture2D tmpTexture = new Texture2D(1, 1);

                tmpTexture.LoadImage(binaryImageData);
                if (tmpTexture != null)
                {
                    CreateImageObject(tmpTexture, x, 4 - y);
                    _textureCaches[fileHash] = new CachedTexture(tmpTexture, false, false); // Cache the texture
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

    private class CachedTexture
    {
        public Texture2D Texture { get; }
        public bool IsXFlipped { get; }
        public bool IsYFlipped { get; }

        public CachedTexture(Texture2D texture, bool isXFlipped, bool isYFlipped)
        {
            Texture = texture;
            IsXFlipped = isXFlipped;
            IsYFlipped = isYFlipped;
        } // 이상하게 한번 저장된값 꺼내오면 flip상태가 자기 멋대로 되어있으므로, 여기서 완성
    }
}
