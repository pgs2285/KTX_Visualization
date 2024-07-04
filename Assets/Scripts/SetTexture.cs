using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTexture : MonoBehaviour
{
    [SerializeField] GameObject meshed;
    [SerializeField] makeMapWithKTX makeMap;

    
    public void setTexture()
    {
        Texture2D tex = makeMap.MergedTexture;
        if (tex == null) Debug.LogWarning("Texture가 생성되지 않았습니다");
        meshed.GetComponent<MeshRenderer>().material.mainTexture = tex;
        
    }
}
