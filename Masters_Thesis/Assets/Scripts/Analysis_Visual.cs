using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analysis_Visual : MonoBehaviour
{
    public string filePath;

    public bool isRoom = false;
    public GameObject[] targets;
    public GameObject[] spheres;
    // Start is called before the first frame update
   
    IEnumerator Start()
    {
        yield return null;

        try
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(filePath);

            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                
                string[] data = reader.ReadLine().Split(',');
                int condition = int.Parse(data[0]) - 1;
                if ((condition < 2) != isRoom)
                    continue;

                Transform parent = null;
                foreach (var t in targets)
                {
                    if (t.name.Equals(data[3]))
                    {
                        parent = t.transform;
                        break;
                    }
                }
                
                GameObject go = Instantiate(spheres[condition], new Vector3(float.Parse(data[7]), float.Parse(data[8]), float.Parse(data[9])),Quaternion.identity);
                go.transform.parent = parent;

            }

            reader.Close();
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
    
}
