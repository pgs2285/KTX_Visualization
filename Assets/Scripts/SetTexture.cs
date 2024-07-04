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
        if (tex == null) Debug.LogWarning("Texture�� �������� �ʾҽ��ϴ�");
        meshed.GetComponent<MeshRenderer>().material.mainTexture = tex;
        
    }
}
