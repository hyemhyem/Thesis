using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualRenderPlane : MonoBehaviour
{
    public MeshRenderer renderPlane;
    private UnityEngine.UI.RawImage raw;
    IEnumerator Start()
    {
        raw = this.GetComponent<UnityEngine.UI.RawImage>();
        Material mat = renderPlane.material;
        yield return new WaitUntil(()=> mat.name.Contains("Instance"));

        //raw.material = renderPlane.material;
        raw.texture = mat.mainTexture;
        raw.material = null;
    }

}
