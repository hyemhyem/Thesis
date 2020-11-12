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
    
    public float fadeTime = 10f;
    public bool isLerpFade = false;
    public bool isLerpFadeByTime = false;
    public float lerpStep = 0.001f;


    public IEnumerator Limbo(AudioSource roomtone)
    {
        yield return new WaitUntil(()=> (FindObjectOfType<Vive.Plugin.SR.ViveSR_VirtualCameraRig>())!=null);
        
        playerCamera = FindObjectOfType<Vive.Plugin.SR.ViveSR_VirtualCameraRig>().gameObject;
        Camera[] cameras = playerCamera.GetComponentsInChildren<Camera>();
        
        foreach (var camera in cameras) {
            camera.fieldOfView = 110.0398f;
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        roomtone.volume = 0f;
        roomtone.Play();
        
       
        if(isLerpFade)
        {
            if(isLerpFadeByTime)
                yield return StartCoroutine(CrossfadeLerpByTime(fadeTime,roomtone));
            else
                yield return StartCoroutine(CrossfadeLerp(lerpStep,roomtone));
        }
        else 
            yield return StartCoroutine(Crossfade(fadeTime,roomtone));

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

    IEnumerator Crossfade(float totalTime,AudioSource roomtone)
    {
        float alpha = 1f;
        float step = Time.fixedDeltaTime / totalTime;

        float fadeAlpha = step;
        
        Debug.Log("CROSSFADE STEP:" + step);
        while (alpha > 0.0f)
        {
            realWorldRaw[0].color = realWorldRaw[1].color = new Color(1f, 1f, 1f, alpha);
            virtualWorldRaw[0].color = virtualWorldRaw[1].color = new Color(1f, 1f, 1f, fadeAlpha);

            alpha -= step;
            fadeAlpha = 1.0f - alpha;

            roomtone.volume = fadeAlpha;

            yield return new WaitForFixedUpdate();
        }

        roomtone.volume = 1f;
    }

    IEnumerator CrossfadeLerp(float step,AudioSource roomtone)
    {
        float alpha = 1f;

        Debug.Log("CROSSFADE (LERP) STEP:" + step);
        while (alpha > 0.005f)
        {
            alpha  = Mathf.Lerp(alpha, -0.005f, step);

            realWorldRaw[0].color = realWorldRaw[1].color = new Color(1f, 1f, 1f, alpha);
            virtualWorldRaw[0].color = virtualWorldRaw[1].color = new Color(1f, 1f, 1f, 1-alpha);

            roomtone.volume = 1f - alpha;
            yield return new WaitForFixedUpdate();
        }
        
        roomtone.volume = 1f;
    }

    IEnumerator CrossfadeLerpByTime(float fadetime, AudioSource roomtone)
    {
        float startTime = Time.time;
        float alpha = 1f;

        Debug.Log("CROSSFADE (LERP) STEP by Time");
        while (alpha > 0f)
        {
            alpha  = Mathf.Lerp(1f, 0f, (Time.time-startTime)/fadetime);

            realWorldRaw[0].color = realWorldRaw[1].color = new Color(1f, 1f, 1f, alpha);
            virtualWorldRaw[0].color = virtualWorldRaw[1].color = new Color(1f, 1f, 1f, 1-alpha);

            roomtone.volume = 1f - alpha;
            yield return new WaitForFixedUpdate();
        }
        
        roomtone.volume = 1f;
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
