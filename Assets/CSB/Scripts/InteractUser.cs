using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractUser : MonoBehaviour
{
    public Text chatBox;
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
            string msg = "음... 그러시군요. 수세미차가 비염에 좋은데요, \n" +
                "건조 수세미 15g을 1L 물에 넣고 20분간 끓이면 됩니다. 수시로 마시면 좋습니다.";
            StartCoroutine(PrintChat(msg));

            // Application.OpenURL("https://youtu.be/eNztXV8p4CI?si=9UJZiQGMCic9RwXH");
            print("https://youtu.be/rr7IuMMyY54?si=RIpn_6ADrY36IBz6");

        }
    }

    IEnumerator PrintChat(string text)
    {
        string showText = "";
        for (int i = 0; i < text.Length; i++)
        {
            showText += text[i];
            chatBox.text = showText;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ButtonGoToOpenURL()
    {
        Application.OpenURL("https://youtu.be/eNztXV8p4CI?si=9UJZiQGMCic9RwXH");
    }
}
