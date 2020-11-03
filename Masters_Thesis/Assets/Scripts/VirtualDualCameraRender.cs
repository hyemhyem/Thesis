using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualDualCameraRender : MonoBehaviour
{
    
    public Camera dualCamera;
    public GameObject virtualDualCamera;
    public bool isLeftCamera = false;
    IEnumerator Start(){

        yield return new WaitUntil(()=>(dualCamera.targetTexture) != null);   

        var vrCameraRig = virtualDualCamera.transform.parent.gameObject.GetComponentInChildren<Vive.Plugin.SR.ViveSR_VirtualCameraRig>().gameObject;
        var vrCameras = vrCameraRig.GetComponentsInChildren<Camera>();
        
        foreach(var camera in vrCameras)
        {
            
            if(isLeftCamera && camera.gameObject.name.Contains("(Left)")){
                this.transform.SetParent(camera.transform);
                this.transform.localPosition = Vector3.zero;
            }
            else if (!isLeftCamera && camera.gameObject.name.Contains("(Right)"))
            {
                this.transform.SetParent(camera.transform);
                this.transform.localPosition = Vector3.zero;       
            }
            
        }
        /*
        if(isLeftCamera && cameras[0].gameObject.name.Contains("Left"))
            raw.texture = cameras[0].targetTexture;
        else 
            raw.texture = cameras[1].targetTexture;
        */
    }
}
