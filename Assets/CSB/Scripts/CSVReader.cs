using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class Datatable
{
    public int id;
    public string symptom;
    public string herb;
    public string tea;
    public string recipe;
    public string description;
    public string link;
    public string imagePath;
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
        string path = Application.streamingAssetsPath + "/CSB/data.csv";

        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            for (int i = 2; i < data.Length; i++) // 첫 번째 줄은 헤더
            {
                string[] row = data[i].Split(',');
                print("1111111111111");
                print(row[1]);
                Datatable table = new Datatable
                {
                    id = int.Parse(row[0]),
                    symptom = row[1],
                    herb = row[2],
                    tea = row[3],
                    recipe = row[4].Trim('\"'),
                    description = row[5].Trim('\"'),
                    link = row[6],
                    imagePath = row[7]
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
