using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Analysis : MonoBehaviour
{
    const int accuracy = 13;
    const int signedError = 14;
    private int count = 0;
    public string path;

    public List<string> NT_ROOM, LB_ROOM, NT_VSPACE, LB_VSPACE;
    public List<string> NT_ROOM_POSE, LB_ROOM_POSE, NT_VSPACE_POSE, LB_VSPACE_POSE;


    private List<Vector3> estPos, headPos, realPos;

    IEnumerator Start()
    {
        NT_ROOM = new List<string>();
        LB_ROOM = new List<string>();
        NT_VSPACE = new List<string>();
        LB_VSPACE = new List<string>();

        NT_ROOM_POSE = new List<string>();
        LB_ROOM_POSE = new List<string>();
        NT_VSPACE_POSE = new List<string>();
        LB_VSPACE_POSE = new List<string>();


        DirFileSearch(path, "csv");


        MergeEverything(
        MergeData(NT_ROOM, NT_ROOM_POSE, "Room.csv"),
        MergeData(LB_ROOM, LB_ROOM_POSE,"Trans_Room.csv"),
        MergeData(NT_VSPACE, NT_VSPACE_POSE,"VSpace.csv"),
        MergeData(LB_VSPACE, LB_VSPACE_POSE,"Trans_VSpace.csv"));


        float[] acc_room = GetData(NT_ROOM, accuracy);
        float[] acc_trans_room = GetData(LB_ROOM, accuracy);
        float[] acc_vspace = GetData(NT_VSPACE, accuracy);
        float[] acc_trans_vspace = GetData(LB_VSPACE, accuracy);

        yield return new WaitForSeconds(2.0f);

        float[] se_room = GetData(NT_ROOM, signedError);
        float[] se_trans_room = GetData(LB_ROOM, signedError);
        float[] se_vspace = GetData(NT_VSPACE, signedError);
        float[] se_trans_vspace = GetData(LB_VSPACE, signedError);

        try
        {
            StreamWriter writer = new StreamWriter(Path.Combine(path, "Stat.csv"));

            writer.WriteLine("Item, Room, Trans-Room, VSpace, Trans-VSpace");

            System.Text.StringBuilder str = new System.Text.StringBuilder();

            float[] avgACC = {
            getAverage(acc_room),
            getAverage(acc_trans_room),
            getAverage(acc_vspace),
            getAverage(acc_trans_vspace)};

            float[] sdACC = {
            getStandardDeviation(avgACC[0],acc_room),
            getStandardDeviation(avgACC[1],acc_trans_room),
            getStandardDeviation(avgACC[2],acc_vspace),
            getStandardDeviation(avgACC[3],acc_trans_vspace),
            };

            str.AppendFormat("{0},{1},{2},{3},{4}",
            "Accuracy (AVG)",
            avgACC[0],
            avgACC[1],
            avgACC[2],
            avgACC[3]);

            writer.WriteLine(str.ToString());
            str.Clear();


            str.AppendFormat("{0},{1},{2},{3},{4}",
            "Accuracy (SD)",
            sdACC[0],
            sdACC[1],
            sdACC[2],
            sdACC[3]);

            writer.WriteLine(str.ToString());
            str.Clear();


            float[] avgSE = {
            getAverage(se_room),
            getAverage(se_trans_room),
            getAverage(se_vspace),
            getAverage(se_trans_vspace)
            };

            float[] sdSE = {
            getStandardDeviation(avgSE[0],se_room),
            getStandardDeviation(avgSE[1],se_trans_room),
            getStandardDeviation(avgSE[2],se_vspace),
            getStandardDeviation(avgSE[3],se_trans_vspace)
            };

            str.AppendFormat("{0},{1},{2},{3},{4}",
            "Signed Error (AVG)",
            avgSE[0], avgSE[1], avgSE[2], avgSE[3]);

            writer.WriteLine(str.ToString());
            str.Clear();

            str.AppendFormat("{0},{1},{2},{3},{4}",
            "Signed Error (SD)",
            sdSE[0], sdSE[1], sdSE[2], sdSE[3]);

            writer.WriteLine(str.ToString());
            str.Clear();


            writer.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    void MergeEverything(string room, string tRoom, string vspace, string tVspace){
        try
        {
            StreamWriter writer = new StreamWriter(Path.Combine(path, "Data.csv"));
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            StreamReader reader;

            reader = new StreamReader(room);
            str.Append("Condition,");
            str.AppendLine(reader.ReadLine());
            while (!reader.EndOfStream)
            {
                str.AppendFormat("{0},{1}", 1, reader.ReadLine());
                writer.WriteLine(str.ToString());
                str.Clear();
            }
            reader.Close();
            
            reader = new StreamReader(tRoom);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                str.AppendFormat("{0},{1}", 2, reader.ReadLine());
                writer.WriteLine(str.ToString());
                str.Clear();
            }
            reader.Close();

            reader = new StreamReader(vspace);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                str.AppendFormat("{0},{1}", 3, reader.ReadLine());
                writer.WriteLine(str.ToString());
                str.Clear();
            }
            reader.Close();
                
            reader = new StreamReader(tVspace);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                str.AppendFormat("{0},{1}", 4, reader.ReadLine());
                writer.WriteLine(str.ToString());
                str.Clear();
            }
            reader.Close();

            writer.Close();

        } catch (IOException e){
            Debug.Log(e.Message);
        }
 
    }

    string MergeData(List<string> list, List<string> poselist, string filename)
    {
        try
        {
            StreamWriter writer = new StreamWriter(Path.Combine(path, filename));

            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("ID,");
            str.Append("Time,Target Name,Origin Pos X, Origin Pos Y, Origin Pos Z,Estimated Pos X,Estimated Pos Y, Estimated Pos Z, Real Pos X, Real Pos Y, Real Pos Z, Estimated Distance, Real Distance, Accuracy, Signed Error");
            str.Append(",Error Distance,Adjusted Accuracy, ,Head X,Head Y,Head Z,Head Est Dist,Head Real Dist,Head Accuracy,Head Signed Error");

            writer.WriteLine(str.ToString());

            for (int idx = 0; idx < list.Count; idx++)
            {
                str.Clear();

                string file = list[idx];
                string posefile = poselist[idx];

                //get ID
                var cd = Directory.GetParent(file);
                string dirName = cd.Name;
                int ID = int.Parse( dirName.Substring(0,dirName.IndexOf('_')));
                //int ID = int.Parse(dirName[0].ToString());

                //Open File and Read Header
                StreamReader reader = new StreamReader(file);
                reader.ReadLine();
                StreamReader poseReader = new StreamReader(posefile);
                poseReader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] data = line.Split(',');
                    string[] poseData = poseReader.ReadLine().Split(',');

                    // Head x/y/z
                    Vector3 org = new Vector3(float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]));
                    Vector3 est = new Vector3(float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                    Vector3 real = new Vector3(float.Parse(data[8]), float.Parse(data[9]), float.Parse(data[10]));
                    Vector3 head = new Vector3(float.Parse(poseData[2]), float.Parse(poseData[3]), float.Parse(poseData[4]));

                    float distErr = Vector3.Distance(real, est);
                    float distHeadEst = Vector3.Distance(head, est);
                    float distHeadReal = Vector3.Distance(head, real);

                    float adjAcc = 1 - (distErr / Vector3.Distance(org, real));
                    float headAcc = 1 - Mathf.Abs(distHeadEst - distHeadReal) / distHeadReal;
                    float headSE = distHeadEst / distHeadReal - 1;

                    str.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    ID,
                    line,
                    distErr,
                    adjAcc,
                    " ",
                    head.x, head.y, head.z,
                    distHeadEst,
                    distHeadReal,
                    headAcc,
                    headSE
                    );
                    str.AppendLine();
                }
                
                writer.Write(str.ToString());
                reader.Close();
            }

            writer.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

    
        StartCoroutine(WaitForDone());
        Debug.Log(filename + " Merge Finished");

        return Path.Combine(path, filename);
    }

    IEnumerator WaitForDone()
    {
        yield return new WaitForSeconds(2.0f);
    }

    float[] GetData(List<string> list, int column)
    {
        List<float> accuracy = new List<float>();

        foreach (var file in list)
        {
            try
            {
                StreamReader sr = new StreamReader(file);

                sr.ReadLine(); // header

                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    string[] data = str.Split(',');

                    //Accuracy: 13 Signed Error: 14
                    accuracy.Add(float.Parse(data[column]));
                }

                sr.Close();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        return accuracy.ToArray();
    }

    float getAverage(float[] data)
    {

        float sum = 0;
        foreach (float item in data)
        {
            sum += item;
        }
        return sum / data.Length;

    }

    float getStandardDeviation(float average, float[] list)
    {
        float sumOfDerivation = 0;
        foreach (float value in list)
        {
            sumOfDerivation += (value) * (value);
        }
        float sumOfDerivationAverage = sumOfDerivation / list.Length;
        return Mathf.Sqrt(sumOfDerivationAverage - (average * average));
    }


    void DirFileSearch(string path, string file)
    {
        try
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path, $"*.{file}");

            foreach (string f in files)
            {
                if (f.Contains("Stat"))
                    continue;

                if (f.Contains("POSETURE"))
                {
                    if (f.Contains("NT_ROOM"))
                        NT_ROOM_POSE.Add(f);
                    else if (f.Contains("LB_ROOM"))
                        LB_ROOM_POSE.Add(f);
                    else if (f.Contains("NT_VSPACE"))
                        NT_VSPACE_POSE.Add(f);
                    else
                        LB_VSPACE_POSE.Add(f);

                }
                else
                {
                    if (f.Contains("NT_ROOM"))
                        NT_ROOM.Add(f);
                    else if (f.Contains("LB_ROOM"))
                        LB_ROOM.Add(f);
                    else if (f.Contains("NT_VSPACE"))
                        NT_VSPACE.Add(f);
                    else
                        LB_VSPACE.Add(f);
                }
                Debug.Log(f);
                //Debug.Log($"[{count++}] path - {f}");
            }
            if (dirs.Length > 0)
            {
                foreach (string dir in dirs)
                {
                    DirFileSearch(dir, file);
                }
            }
        }
        catch (IOException ex) { Debug.LogError(ex); }
    }

}
