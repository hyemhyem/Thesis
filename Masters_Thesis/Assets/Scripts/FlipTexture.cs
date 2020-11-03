using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlipTexture : MonoBehaviour
{
    private RawImage raw;
    public Material material;
    public Image background;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        raw = this.GetComponent<RawImage>();
        
        yield return new WaitUntil(()=>(raw.texture = material.GetTexture("_MainTex"))!=null);
        
        raw.material = null;

        StartCoroutine(FollowAlpha());
    }

    IEnumerator FollowAlpha(){
        while(background.isActiveAndEnabled)
        {
            background.color = new Color(0f,0f,0f,raw.color.a);
            yield return null;
        }
    }

}
