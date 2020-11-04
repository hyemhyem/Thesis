using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
public class ExperimentManager : MonoBehaviour
{
    [Header("Subject")]
    public string subjectName = "";


    [Header("File")]
    public string _path;

    [SerializeField] private string subDirectory;

    [Header("Exp Mode")]
    public bool isTransition = false;
    public bool isVirtualSpace = false;


    [Header("Player")]
    public Camera head;
    public GameObject augmentedEyes;
    
    [Header("Vive SR WORKS")]
    public LimboCanvas limboCanvas;
    public GameObject SRWorks;
  

    [Header("Pointer Collider")]
    public GameObject viveController;
    public Transform pointer;

    [Header("Pointer Sound")]
    public AudioSource clickFeedback;
    public AudioSource roomtone;

    [Header("Targets")]
    public GameObject targetList;
    [SerializeField] private Transform[] target; 

    [Header("Debug Options")]
    public KeyCode controlKey;
    public GameObject shinySphere;

    private string file;
    private StreamWriter writer;
    private bool isTriggerPressed = false;
    private Transform estimatedPoint = null;


    IEnumerator Start()
    {

        if(subjectName.Length == 0)
        {
            Debug.LogError("이름 입력하시오");
            yield break;
            
        }

        if (!Initialize(_path, subjectName))
        {
            Debug.LogError("Directory 생성 실패");
            yield break;
        }

        try
        {
            file = getFilePath(subDirectory);
            writer = new StreamWriter(file);

            writer.WriteLine("Time,Target Name,Origin Pos X, Origin Pos Y, Origin Pos Z,Estimated Pos X,Estimated Pos Y, Estimated Pos Z, Real Pos X, Real Pos Y, Real Pos Z, Estimated Distance, Real Distance, Accuracy, Signed Error");
        }
        catch (IOException e)
        {
            Debug.LogError(e);
            yield break;
        }

        if(!isTransition)
        {
            augmentedEyes.SetActive(false);
            limboCanvas.enabled = false;
            SRWorks.SetActive(false);
        }


        target = targetList.GetComponentsInChildren<Transform>().Where(go => go.gameObject != targetList.gameObject).ToArray<Transform>();
        
        ShuffleArray(target);
        
        System.Text.StringBuilder str = new System.Text.StringBuilder();


        Debug.Log("테스트 준비 완료되면 엔터 눌러 실행");

        yield return new WaitUntil(() => Input.GetKeyDown(controlKey));

        if(isTransition)
            StartCoroutine(limboCanvas.Limbo(roomtone));

        for (int i = 0; i < target.Length; i++)
        {
            
            target[i].gameObject.SetActive(true);

            Debug.Log(target[i].gameObject.name+": 팔을 제자리에 둔 후, 엔터 눌러서 확인");

            yield return new WaitUntil(() => Input.GetKeyDown(controlKey));
            Debug.Log(target[i].gameObject.name+": 시작");
            Vector3 orginPos = pointer.position;

            yield return new WaitUntil(() => isTriggerPressed == true);

            float estDist = Vector3.Distance(orginPos, estimatedPoint.transform.position);
            float realDist = Vector3.Distance(orginPos, target[i].transform.position);
            float accuracy = 1.0f - Mathf.Abs((estDist - realDist)/realDist);
            float signed = estDist/realDist - 1.0f;


            str.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
            Time.time,
            target[i].gameObject.name,
            orginPos.x, orginPos.y, orginPos.z,
            estimatedPoint.position.x, estimatedPoint.position.y, estimatedPoint.position.z, 
            target[i].position.x, target[i].position.y, target[i].position.z,
            estDist,
            realDist,
            accuracy,
            signed
            );


            writer.WriteLine(str);

            str.Clear();

            Debug.Log(target[i].name + " accuracy:" + accuracy.ToString("F3") + " signed error :" + signed.ToString("F3"));


            isTriggerPressed = false;
            target[i].gameObject.SetActive(false);
        }

        writer.Close();

    }

    bool Initialize(string path, string name)
    {

        string folder;

        try
        {
            folder = Path.Combine(path, name);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Debug.Log("New Directory : " + folder);
            }
            else
            {
                Debug.LogError("Directory already exist" + folder);
                return false;
            }


        }
        catch (IOException e)
        {
            Debug.LogError(e);
            return false;
        }

        subDirectory = folder;
        return true;
    }

    void ShuffleArray(Transform[] array){
        Transform tmp;
        for (int i = 0; i < array.Length; i++) {
             
            int rnd = Random.Range(0, array.Length);
            tmp = array[rnd];
            array[rnd] = array[i];
            array[i] = tmp;
        }
        
        for (int i = 0; i < array.Length; i++) {
            array[i].gameObject.SetActive(false);
        }
    }

    string getFilePath(string path)
    {

        string fileName = "";

        if (!isTransition)
            fileName += "NT";
        else
            fileName += "LB";

        if (!isVirtualSpace)
            fileName += "_ROOM";
        else
            fileName += "_VSPACE";

        fileName += ".csv";

        return Path.Combine(path, fileName);
    }


    public Vector3 GetPointerPosition(Transform pointer)
    {
        return pointer.position;
    }

    public void LogPointer()
    {
        clickFeedback.Play();
        isTriggerPressed = true;
        Debug.Log("Trigger Pressed : " + pointer.position.ToString("F3"));
        estimatedPoint = Instantiate(shinySphere, pointer.position, pointer.rotation).transform; 
    }

    

}
