using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [Header("Debug")]
    public bool isVisible = false;
    private string pointName;
    private bool isTriggered = false;
    void Start()
    {
        pointName = this.gameObject.name;
        
        MeshRenderer rend = this.GetComponent<MeshRenderer>();
        
        if(isVisible)
            rend.enabled = true;
        else
            rend.enabled = false;
        
    }
    private void OnTriggerEnter(Collider other) {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other) {
        isTriggered = false;
    }
}
