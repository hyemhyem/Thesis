using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Transform trackedPoint;
    // Start is called before the first frame update
    void Update()
    {
        this.transform.position = trackedPoint.position;   
    }
}
