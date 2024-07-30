using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KtxUnity;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using Unity.Collections;
using System.Collections.Generic;
using GLTFast;

public class makeMapWithKTX : MonoBehaviour
{
    public GameObject parentObj;
    public int imageWidth = 100; // 이미지의 너비
    public int imageHeight = 100; // 이미지의 높이
    public int spacing = 10; // 이미지 간의 간격
    public Vector2 anchorPivot = new Vector2(0, 0);
    public TextMeshProUGUI _timeRequired;
    private Texture2D _mergedTexture;
    public Texture2D MergedTexture => _mergedTexture;
    public TMP_Dropdown DD_resolution;
    public Toggle isCombined;

    private Dictionary<int, CachedTexture> _textureCaches = new Dictionary<int, CachedTexture>();
    private Dictionary<int, CachedTexture> _textureCaches2 = new Dictionary<int, CachedTexture>();

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

    #region TestWith gltfast(Managed Native Array)
    public async void LoadImagesKtxGL(string Encode)
    {
        if (isCombined.isOn)
        {
            string resolutionText = DD_resolution.options[DD_resolution.value].text;
            string[] dimensions = resolutionText.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);
            if (Encode == "KTX")
                _mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            else if (Encode == "ETC1S" || Encode == "UASTC")
                _mergedTexture = new Texture2D(width, height, TextureFormat.DXT1, false);
        }
        else
        {
            if (Encode == "KTX")
                _mergedTexture = new Texture2D(256 * 10, 256 * 5, TextureFormat.RGBA32, false);
            else if (Encode == "ETC1S" || Encode == "UASTC")
                _mergedTexture = new Texture2D(256 * 10, 256 * 5, TextureFormat.DXT1, false);
        }

        DestroyCanvasChild();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        if (isCombined.isOn)
        {
            string fileName = $"{DD_resolution.options[DD_resolution.value].text}.ktx2";
            string imagePath = $"SaturateImages/{Encode}/combine/";
            string filePath = Path.Combine(Application.dataPath, imagePath, fileName);
            int fileHash = filePath.GetHashCode();

            if (_textureCaches2.ContainsKey(fileHash))
            {
                CachedTexture cachedTexture = _textureCaches2[fileHash];
                CreateImageObject(cachedTexture.Texture, 0, 0, cachedTexture.IsXFlipped, cachedTexture.IsYFlipped, true);
            }
            else
            {
                var texture = new KtxTexture();
                byte[] fileData = File.ReadAllBytes(filePath);

                
                using (var array = new ManagedNativeArray(fileData))
                {
                    
                    var errorCode = texture.Open(array.nativeArray);
                    if (errorCode != ErrorCode.Success)
                    {
                        Debug.LogError("Failed to open KTX texture: " + errorCode);
                        return;
                    }

                    var result = await texture.LoadTexture2D(false);
                    texture.Dispose();

                    if (result != null)
                    {
                        Texture2D _texture = result.texture;
                        if (_texture != null)
                        {
                            bool isXFlip = result.orientation.IsXFlipped();
                            bool isYFlip = result.orientation.IsYFlipped();
                            if (Encode == "KTX") isYFlip = false;
                            _textureCaches2[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip);
                            CreateImageObject(_texture, 0, 0, isXFlip, isYFlip, true);
                        }
                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    string fileName = $"{y:D4}_{x:D4}.ktx2";
                    string imagePath = $"SaturateImages/{Encode}/{y:D4}/";
                    string filePath = Path.Combine(Application.dataPath, imagePath, fileName);
                    int fileHash = filePath.GetHashCode();

                    if (_textureCaches2.ContainsKey(fileHash))
                    {
                        CachedTexture cachedTexture = _textureCaches2[fileHash];
                        CreateImageObject(cachedTexture.Texture, x, 4 - y, cachedTexture.IsXFlipped, cachedTexture.IsYFlipped);
                        continue;
                    }

                    var texture = new KtxTexture();
                    byte[] fileData = File.ReadAllBytes(filePath);

            
                    using (var array = new ManagedNativeArray(fileData))
                    {
                        var errorCode = texture.Open(array.nativeArray);
                        if (errorCode != ErrorCode.Success)
                        {
                            Debug.LogError("Failed to open KTX texture: " + errorCode);
                            continue;
                        }

                        var result = await texture.LoadTexture2D(false);
                        texture.Dispose();

                        if (result != null)
                        {
                            Texture2D _texture = result.texture;
                            if (_texture != null)
                            {
                                bool isXFlip = result.orientation.IsXFlipped();
                                bool isYFlip = result.orientation.IsYFlipped();
                                if (Encode == "KTX") isYFlip = false;
                                _textureCaches2[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip);
                                CreateImageObject(_texture, x, 4 - y, isXFlip, isYFlip);
                            }
                        }
                    }
                }
            }
        }

        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }
    #endregion

    #region Test LoadFromBytes(NativeArray)

    public async void LoadImagesKtx2(string Encode)
    {
        if (isCombined.isOn)
        {
            string resolutionText = DD_resolution.options[DD_resolution.value].text;
            string[] dimensions = resolutionText.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);
            if (Encode == "KTX")
                _mergedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            else if (Encode == "ETC1S" || Encode == "UASTC")
                _mergedTexture = new Texture2D(width, height, TextureFormat.DXT1, false);
        }
        else
        {
            if (Encode == "KTX")
                _mergedTexture = new Texture2D(256 * 10, 256 * 5, TextureFormat.RGBA32, false);
            else if (Encode == "ETC1S" || Encode == "UASTC")
                _mergedTexture = new Texture2D(256 * 10, 256 * 5, TextureFormat.DXT1, false);
        }

        DestroyCanvasChild();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        if (isCombined.isOn)
        {
            string fileName = $"{DD_resolution.options[DD_resolution.value].text}.ktx2";
            string imagePath = $"SaturateImages/{Encode}/combine/";
            string filePath = Path.Combine(Application.dataPath, imagePath, fileName);
            int fileHash = filePath.GetHashCode();

            if (_textureCaches.ContainsKey(fileHash))
            {
                CachedTexture cachedTexture = _textureCaches[fileHash];
                CreateImageObject(cachedTexture.Texture, 0, 0, cachedTexture.IsXFlipped, cachedTexture.IsYFlipped, true);
            }
            else
            {
                var texture = new KtxTexture();
                byte[] fileData = File.ReadAllBytes(filePath);
                NativeArray<byte> nativeArray = new NativeArray<byte>(fileData, Allocator.Persistent);
                NativeSlice<byte> nativeSlice = new NativeSlice<byte>(nativeArray);

                var result = await texture.LoadFromBytes(nativeSlice);
            
                nativeArray.Dispose();
                if (result != null)
                {
                    Texture2D _texture = result.texture;
                    if (_texture != null)
                    {
                        bool isXFlip = result.orientation.IsXFlipped();
                        bool isYFlip = result.orientation.IsYFlipped();
                        if (Encode == "KTX") isYFlip = false;
                        _textureCaches[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip);
                        CreateImageObject(_texture, 0, 0, isXFlip, isYFlip, true);
                    }
                }
            }
        }
        else
        {
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
                    nativeArray.Dispose();
                    if (result != null)
                    {
                        Texture2D _texture = result.texture;
                        if (_texture != null)
                        {
                            bool isXFlip = result.orientation.IsXFlipped();
                            bool isYFlip = result.orientation.IsYFlipped();
                            if (Encode == "KTX") isYFlip = false;
                            _textureCaches[fileHash] = new CachedTexture(_texture, isXFlip, isYFlip);
                            CreateImageObject(_texture, x, 4 - y, isXFlip, isYFlip);
                        }
                    }
                }
            }
        }
  

        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }
    #endregion 
    void C_LoadJPGImage()
    {
        _mergedTexture = new Texture2D(256 * 10, 256 * 5, TextureFormat.RGBA32, false);
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
                    CreateImageObject(_textureCaches[fileHash].Texture, x, 4 - y);
                    continue;
                }

                byte[] binaryImageData = File.ReadAllBytes(finalPath);
                Texture2D tmpTexture = new Texture2D(1, 1);

                tmpTexture.LoadImage(binaryImageData);
                if (tmpTexture != null)
                {
                    CreateImageObject(tmpTexture, x, 4 - y);
                    _textureCaches[fileHash] = new CachedTexture(tmpTexture, false, false);
                }
            }
        }
        sw.Stop();
        _timeRequired.text = sw.ElapsedMilliseconds.ToString() + "ms";
    }

    void CreateImageObject(Texture2D texture, int x, int y, bool isXFlip = false, bool isYFlip = false, bool isCombined = false)
    {
        GameObject imageObject = new GameObject(isCombined ? "CombinedImage" : $"Image_{y}_{x}");
        imageObject.transform.SetParent(parentObj.transform);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        if (isCombined)
        {
            // Get parent RectTransform
            RectTransform parentRectTransform = parentObj.GetComponent<RectTransform>();

            // Set size
            rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

            // Center the image
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Set anchors to center
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Modify anchorMin to center
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f); // Modify anchorMax to center
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
            rectTransform.anchoredPosition = new Vector2(x * (imageWidth + spacing), -y * (imageHeight + spacing));
            rectTransform.pivot = anchorPivot;

            // Set anchors to top-left for individual images
            rectTransform.anchorMin = new Vector2(0, 1); // No change needed
            rectTransform.anchorMax = new Vector2(0, 1); // No change needed
        }

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

        if (isCombined)
        {
            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, MergedTexture, 0, 0, 0, 0);
        }
        else
        {
            Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, MergedTexture, 0, 0, x * texture.width, (4 - y) * texture.height);
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
        }
    }
}
