using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualCameraRender : MonoBehaviour
{
    public Camera dualCamera;
    public UnityEngine.UI.RawImage raw;

    IEnumerator Start()
    {
        raw = this.GetComponent<UnityEngine.UI.RawImage>();
        yield return new WaitUntil(()=>(raw.texture = dualCamera.targetTexture) != null);   
    }

}
