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
    public string recipe;
    public string description;
    public string link;
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
                Datatable table = new Datatable
                {
                    symptom = row[0],
                    herb = row[1],
                    tea = row[2],
                    recipe = row[3].Trim('\"'),
                    description = row[4].Trim('\"'),
                    link = row[5]
                };
                datatable.Add(table);
            }
        }
        else
        {
            Debug.LogError("CSV file not found at " + path);
        }
    }
}
