using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class PointerManager : MonoBehaviour
{
    private bool isPointerPressed =  false;

    [Header("Player")]
    public Camera head;

    [Header("Pointer Collider")]
    public Transform pointer;

    [Header("Pointer Sound")]
    public AudioClip clickFeedback;

    [Header("Car")]
    public GameObject[] car;

    [Header("Debug Options")]
    public KeyCode controlKey;
    public GameObject shinySphere;
    
    private bool tempPressed = false;
    private GameObject estimatedPoint = null;

    IEnumerator Start(){
        
        yield return new WaitUntil(()=> Input.GetKeyDown(controlKey));

        for(int i = 0;i<car.Length; i++){
            car[i].SetActive(true);
            yield return new WaitUntil(()=>tempPressed == true);
            //Debug.Log(car[i].name+" "+ car[i].transform.position.ToString("F3")
            //+"from head"+head.transform.position.ToString("F3")+" : "+ Vector3.Distance(head.transform.position, car[i].transform.position)+ "\n");
            
            float estDist = Vector3.Distance(head.transform.position,estimatedPoint.transform.position);
            float realDist = Vector3.Distance(head.transform.position,car[i].transform.position);

            Debug.Log(car[i].name +"\n"
            +"Head-Estimated:"+estDist.ToString("F3")+"\n"
            +"Ratio: "+ (estDist/realDist).ToString("F3") +"\n"
            +"Head-Real "+realDist.ToString("F3"));
            
            tempPressed = false;
            car[i].SetActive(false);
            /*
            TargetManager targetManager = car[i].GetComponentInChildren<TargetManager>();
            
            foreach(var point in targetManager.targetPoints){
            
                point.gameObject.SetActive(true);
                yield return new WaitUntil(()=> isPointerPressed);  
                point.gameObject.SetActive(false);
            
            }
            */
        }


    }
    public Vector3 GetPointerPosition(Transform pointer){
        return pointer.position;
    }

    public void LogPointer(){
        tempPressed = true;
        Debug.Log("Trigger Pressed : "+ pointer.position.ToString("F3"));
        estimatedPoint = Instantiate(shinySphere, pointer.position, pointer.rotation);
    }

    public void setTriggerStateOn(){
        isPointerPressed = true;
    }
    public void setTriggerStateOff(){
        isPointerPressed = false;
    }
}
