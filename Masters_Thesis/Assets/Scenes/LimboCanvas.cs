using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimboCanvas : MonoBehaviour
{

    public GameObject playerCamera;

    [Header("Augemented Canvas Camera")]
    public GameObject augmentedEyes;

    [Header("Blended Canvas - Raw Images")]
    public UnityEngine.UI.RawImage[] realWorldRaw;
    public UnityEngine.UI.RawImage[] virtualWorldRaw;

    [Header("Rendering Cameras")]
    public GameObject SRWorks;
    //107.5321
    //110.0398
    [Header("Options")]
    public float fadeTime = 5f;

    IEnumerator Start()
    {
        yield return new WaitUntil(()=> (FindObjectOfType<Vive.Plugin.SR.ViveSR_VirtualCameraRig>())!=null);
        
        playerCamera = FindObjectOfType<Vive.Plugin.SR.ViveSR_VirtualCameraRig>().gameObject;
        Camera[] cameras = playerCamera.GetComponentsInChildren<Camera>();
        
        foreach (var camera in cameras) {
            camera.fieldOfView = 110.0398f;
            camera.clearFlags = CameraClearFlags.Depth;
        }

        yield return StartCoroutine(Crossfade(fadeTime));

        SRWorks.SetActive(false);
        augmentedEyes.SetActive(false);

        
        foreach (var camera in cameras)
        {
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.targetTexture = null;
            if (camera.name.Contains("Left"))
                camera.stereoTargetEye = StereoTargetEyeMask.Both;
            else
            {
                camera.gameObject.SetActive(false);
            }
        }

    }

    IEnumerator Crossfade(float totalTime)
    {
        float alpha = 1f;
        float step = Time.fixedDeltaTime / totalTime;

        float fadeAlpha = step;
        
        yield return new WaitForSeconds(3.0f);

        Debug.Log("STEP:" + step);
        while (alpha > -0.1f)
        {
            realWorldRaw[0].color = realWorldRaw[1].color = new Color(1f, 1f, 1f, alpha);
            virtualWorldRaw[0].color = virtualWorldRaw[1].color = new Color(1f, 1f, 1f, fadeAlpha);

            alpha -= step;
            fadeAlpha = 1.0f - alpha;

            yield return new WaitForFixedUpdate();
        }

        //yield return StartCoroutine(Calibrate(1.0f - (alpha)*0.5f));
        
    }
    void EnableCamera(){
         Camera[] eyes = augmentedEyes.GetComponentsInChildren<Camera>();
        eyes[0].cullingMask |= 1 << 8;
        eyes[1].cullingMask |= 1 << 8;
    }
    IEnumerator Calibrate(float alpha){

        float step = Time.fixedDeltaTime;
        while (alpha > -0.1f)
        {
            virtualWorldRaw[0].color = virtualWorldRaw[1].color = new Color(1f, 1f, 1f, alpha);

            alpha -= step;
            yield return new WaitForFixedUpdate();
        }
    }
}
