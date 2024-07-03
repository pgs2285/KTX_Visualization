using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KtxUnity;
using UnityEngine.UI;
using Unity.VisualScripting;
public class KTXVisualization_Sprite : MonoBehaviour
{
    [SerializeField] private string _fileName;
    // Start is called before the first frame update
    async void Start()
    {

        var texture = new KtxTexture();
        
        var result = await texture.LoadFromStreamingAssets(_fileName);

        Debug.Log(result);
        if (result != null)
        {
            var pos = new Vector2(0, 0);
            var size = new Vector2(result.texture.width, result.texture.height);

            // Flip Sprite, if required
            if (result.orientation.IsXFlipped())
            {
                pos.x = size.x;
                size.x *= -1;
            }

            if (result.orientation.IsYFlipped())
            {
                pos.y = size.y;
                size.y *= -1;
            }

            // Create a Sprite and assign it to the Image
            GetComponent<Image>().sprite = Sprite.Create(result.texture, new Rect(pos, size), Vector2.zero);

            const float scale = 0.01f; // Set this to whatever size you need it - best make it a serialized class field
            var rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(result.texture.width * scale, result.texture.height * scale);
        }
        ScreenCapture.CaptureScreenshot("screenshot.png");
    }

}
