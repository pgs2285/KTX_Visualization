using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTexture : MonoBehaviour
{
    [SerializeField] GameObject meshed;
    [SerializeField] makeMapWithKTX makeMap;
    TextureToSphere glb;
    private void Start()
    {
        glb = GameObject.FindWithTag("Sphere").GetComponent<TextureToSphere>();
    }
    public void setTexture()
    {
        Texture2D tex = makeMap.MergedTexture;
        //if (tex == null) Debug.LogWarning("Texture가 생성되지 않았습니다");
        //meshed.GetComponent<MeshRenderer>().material.mainTexture = tex;
        if(glb != null )
            glb.CreateMapToSphere(tex);
        else
        {
            GameObject.FindWithTag("Sphere").GetComponent<MeshRenderer>().material.mainTexture = makeMap.MergedTexture;
        }

    }
}
