using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class Datatable
{
    public string symptom;
    public string herb;
    public string tea;
    public string description;
}

public class CSVReader : MonoBehaviour
{
    //[System.Serializable]
    //public class Datatable
    //{
    //    public string symptom;
    //    public string herb;
    //    public string tea;
    //    public string description;
    //}

    public List<Datatable> datatable = new List<Datatable>();

    void Start()
    {
        ReadCSV();
    }

    void ReadCSV()
    {
        string path = Application.streamingAssetsPath + "/data.csv";

        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            for (int i = 1; i < data.Length; i++) // 첫 번째 줄은 헤더
            {
                string[] row = data[i].Split(',');
                Datatable player = new Datatable
                {
                    symptom = row[0],
                    herb = row[1],
                    tea = row[2],
                    description = row[3]
                };
                datatable.Add(player);
            }
        }
        else
        {
            Debug.LogError("CSV file not found at " + path);
        }
    }
}
