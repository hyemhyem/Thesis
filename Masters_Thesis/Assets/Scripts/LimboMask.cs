using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimboMask : MonoBehaviour
{
    public MeshRenderer wall;

    [Header("Image Planes")]
    public GameObject leftImagePlane;
    public GameObject rightImagePlane;

    [Header("Render Planes")]
    public GameObject leftRenderPlane;
    public GameObject rightRenderPlane;

    private Material leftImgMaterial, rightImgMaterial;
    public Material leftRendMaterial,rightRendMaterial;
    
    [Header("Mask Options")]
    public float step = 0.002f;

    //[Range(0.0f,1.0f)]
    //public float alpha = 1f;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);

        leftImgMaterial = leftImagePlane.GetComponent<MeshRenderer>().material;
        rightImgMaterial = rightImagePlane.GetComponent<MeshRenderer>().material;
        
        leftRendMaterial = leftRenderPlane.GetComponent<MeshRenderer>().material;
        rightRendMaterial = rightRenderPlane.GetComponent<MeshRenderer>().material;
        
        leftImgMaterial.shader = rightImgMaterial.shader = leftRendMaterial.shader = rightRendMaterial.shader = Shader.Find("ViveSR_Experience/UI");

        InitializeMaterial(leftImagePlane, leftImgMaterial);
        InitializeMaterial(rightImagePlane, rightImgMaterial);
        InitializeMaterial(leftRenderPlane, leftRendMaterial);
        InitializeMaterial(rightRenderPlane, rightRendMaterial);
        
    
        StartCoroutine(ImagePlaneFadeOut(leftImgMaterial,rightImgMaterial));
        StartCoroutine(RenderPlaneFade(leftRendMaterial,rightRendMaterial));
        
        //StartCoroutine(Fade());
        //StartCoroutine(CameraFadeOut());
    }
    void InitializeMaterial(GameObject obj, Material mat){
        //mat.renderQueue = 4000;
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        //renderer.sortingLayerName = "RenderPlane";
        //Debug.Log(obj.name+" : "+renderer.sortingOrder+"  Wall: "+wall.sortingOrder +" ");
        renderer.sortingOrder = wall.sortingOrder+100;

        Debug.Log(obj.name+" : "+renderer.sortingOrder+"  Wall: "+wall.sortingOrder +" ");

        mat.SetFloat("_Cutoff",0f);
        mat.SetColor("_Color", new Color(1f,1f,1f,1f));
    }

    IEnumerator ImagePlaneFadeOut(Material left, Material right){
        float alpha = 1.0f;
        while(alpha > 0){
            left.SetColor("_Color", new Color(1f,1f,1f,alpha));
            right.SetColor("_Color", new Color(1f,1f,1f,alpha));

            yield return null;

            alpha -= step;
        }
    }

    IEnumerator RenderPlaneFade(Material left, Material right){
        float alpha = 1.0f;

        while(alpha > 0.5){
            left.SetColor("_Color", new Color(1f,1f,1f,alpha));
            right.SetColor("_Color", new Color(1f,1f,1f,alpha));

            yield return null;

            alpha -= step;
        }

        while(alpha < 1.0f){
            left.SetColor("_Color", new Color(1f,1f,1f,alpha));
            right.SetColor("_Color", new Color(1f,1f,1f,alpha));

            yield return null;

            alpha += step;
        }
    }

    /*
    IEnumerator Fade(){
        while (true){
            
            material.SetColor("_Color", new Color(1f,1f,1f, alpha));
            yield return null;
        }
    }
    IEnumerator CameraFadeOut(){
        
        float alpha = 1f;
        
        while (alpha > 0){
            Color color = new Color(1f,1f,1f, alpha);
            material.SetColor("_Color", color);

            yield return null;

            alpha -= step;
        }
    }*/
}
