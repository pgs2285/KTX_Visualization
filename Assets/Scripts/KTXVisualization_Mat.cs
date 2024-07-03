using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KtxUnity;

public class KTXVisualization : MonoBehaviour
{
    #region Variables
    [SerializeField] private string _fileName; // 파일네임 .KTX 확장자 받는듯?
    [SerializeField] public Material _targetMaterial; // material 대입
    public TextureResult result;
    #endregion Variables

    #region Unity Methods
    async void Awake()
    {
        KtxTexture texture = new KtxTexture();

        result = await texture.LoadFromStreamingAssets(_fileName);

        if(result != null)
        { // 
            _targetMaterial.mainTexture = result.texture;
        }

        //var scale = _targetMaterial.mainTextureScale;
        //scale.x = result.orientation.IsXFlipped() ? -1 : 1;
        //scale.y = result.orientation.IsXFlipped() ? -1 : 1;


    }
    #endregion Unity Methods


}
