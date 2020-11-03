using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQueueMask : MonoBehaviour
{
    public MeshRenderer mask;
    public MeshRenderer obj;
    void Start()
    {
        mask.sortingOrder = obj.sortingOrder + 1;
        
    }

}
