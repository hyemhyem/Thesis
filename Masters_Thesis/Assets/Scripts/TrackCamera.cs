using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform hmd;
    
    void FixedUpdate()
    {
        this.transform.rotation = (hmd.rotation);
        //this.transform.rotation = Quaternion.Inverse(hmd.rotation);    
    }
}
