using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUser : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Chat(string symptom)
    {
        if (symptom.Contains("비염"))
        {
            print("비염에 좋은 약재는 수세미입니다. 건조 수세미 15g을 1L 물에 넣고 20분간 끓이면 됩니다. 수시로 차로 먹으면 좋습니다.");
            Application.OpenURL("https://youtu.be/eNztXV8p4CI?si=9UJZiQGMCic9RwXH");
            print("https://youtu.be/rr7IuMMyY54?si=RIpn_6ADrY36IBz6");
            
        }
    }
}
