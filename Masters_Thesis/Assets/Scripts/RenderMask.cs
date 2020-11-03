using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMask : MonoBehaviour
{
    public Material material;

    public Color color;

    [Range(0.0f, 1.0f)]
    public float alpha = 1.0f;


    private IEnumerator Start(){
        yield return new WaitForSeconds(3.0f);

        material = GetComponent<MeshRenderer>().material;
        material.shader = Shader.Find("Particles/Standard Unlit");
        //while(true)
        //{    
        //    material.SetColor("", color);
        //    yield return null;
        //}
    }

}
