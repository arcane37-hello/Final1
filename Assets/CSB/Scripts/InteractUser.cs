using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InteractUser : MonoBehaviour
{
    public Text chatBox;
    public GameObject csv;
    string url = "";
    List<string> imageDirs = new List<string>();
    int currentImageIndex = 0;

    public Image herbImage;
    public Button prevButton;
    public Button nextButton;

    public Button prevImageButton;
    public Button nextImageButton;

    AudioSource audioSource;

    string descriptionPath = "";


    private List<string> pages;              // 페이지 단위로 나눠진 텍스트 저장 리스트
    private int currentPage = 0;             // 현재 페이지 인덱스
    private int maxCharsPerPage = 85;       // 한 페이지에 표시할 최대 문자 수

    // Start is called before the first frame update
    void Start()
    {
        prevImageButton.onClick.AddListener(ShowPreviousImage);
        nextImageButton.onClick.AddListener(ShowNextImage);
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Chat(string symptom)
    {
        List<Datatable> datas = csv.GetComponent<CSVReader>().datatable;
        //foreach (Datatable table in csv.GetComponent<CSVReader>().datatable)
        for(int i = 0; i < datas.Count; i++)
        {
            if (symptom.Contains(datas[i].symptom.ToString()))
            {
                string msg = $"음... 그러시군요. {datas[i].herb}이(가) {datas[i].symptom}에 좋은데요,\n{datas[i].recipe}";
                descriptionPath = Application.streamingAssetsPath + datas[i].description;
                url = datas[i].link;

                imageDirs.Clear();

                imageDirs.Add(Application.streamingAssetsPath + datas[i].imagePath);
                imageDirs.Add(Application.streamingAssetsPath + datas[i].imagePath2);
                imageDirs.Add(Application.streamingAssetsPath + datas[i].imagePath3);

                currentImageIndex = 0; // Reset to show the first image
                LoadImage();

                StartCoroutine(PrintChat(msg));
                //PlayAudio(Application.dataPath + datas[i].audioPath);
                StartCoroutine(PlayAudio(Application.streamingAssetsPath + datas[i].audioPath));
            }
        }


        //if (symptom.Contains("비염"))
        //{
        //    string msg = "음... 그러시군요. 수세미차가 비염에 좋은데요, \n" +
        //        "건조 수세미 15g을 1L 물에 넣고 20분간 끓이면 됩니다. 수시로 마시면 좋습니다.";
        //    StartCoroutine(PrintChat(msg));

        //    // Application.OpenURL("https://youtu.be/eNztXV8p4CI?si=9UJZiQGMCic9RwXH");
        //    print("https://youtu.be/rr7IuMMyY54?si=RIpn_6ADrY36IBz6");

        //}
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
        if(url == "")
        {
            string msg = "증상을 먼저 입력해 주세요.";
            StartCoroutine(PrintChat(msg));
            return;
        }
        //Application.OpenURL("https://youtu.be/eNztXV8p4CI?si=9UJZiQGMCic9RwXH");
        Application.OpenURL(url);
    }

    public void ButtonShowDescription()
    {
        LoadTextFromFile(descriptionPath);
        UpdateChatbox();
        prevButton.onClick.AddListener(ShowPreviousPage);
        nextButton.onClick.AddListener(ShowNextPage);
    }

    public void LoadImage()
    {

        if (File.Exists(imageDirs[currentImageIndex]))
        {
            // 이미지 파일 읽기
            byte[] fileData = File.ReadAllBytes(imageDirs[currentImageIndex]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // 이미지 로드

            // Texture2D를 Sprite로 변환
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // UI Image에 Sprite 설정
            herbImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image file not found at " + imageDirs[currentImageIndex]);
        }
    }

    public void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            LoadImage();
        }
    }

    public void ShowNextImage()
    {
        if (currentImageIndex < imageDirs.Count - 1)
        {
            currentImageIndex++;
            LoadImage();
        }
    }

    private void UpdateImageButtonState()
    {
        prevImageButton.interactable = currentImageIndex > 0;
        nextImageButton.interactable = currentImageIndex < imageDirs.Count - 1;
    }



    private void LoadTextFromFile(string path)
    {
        pages = new List<string>();

        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            int contentLength = content.Length;

            for (int i = 0; i < contentLength; i += maxCharsPerPage)
            {
                int length = Mathf.Min(maxCharsPerPage, contentLength - i);
                string pageContent = content.Substring(i, length);
                pages.Add(content.Substring(i, length));
                Debug.Log("Page added: " + pageContent); // 각 페이지의 내용을 확인
            }
        }
        else
        {
            Debug.LogError("File not found at: " + path);
        }
    }


    private void UpdateChatbox()
    {
        Debug.Log("Updating chatbox for page: " + currentPage);
        if (pages.Count > 0)
        {
            chatBox.text = pages[currentPage];
            Debug.Log("Displaying page content: " + chatBox.text); // 표시 중인 페이지 내용 확인
            UpdateButtonState();
        }
    }

    private void ShowPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateChatbox();
        }
    }

    private void ShowNextPage()
    {
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            UpdateChatbox();
        }
    }

    private void UpdateButtonState()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < pages.Count - 1;
    }


    //public void PlayAudio(string audioPath)
    //{
    //    if (File.Exists(audioPath))
    //    {
    //        StartCoroutine(LoadAudio(audioPath));
    //    }
    //    //else
    //    //{
    //    //    Debug.LogError("Audio file not found at: " + audioPath);
    //    //}
    //}

    private IEnumerator PlayAudio(string audioPath)
    {
        if (File.Exists(audioPath))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
        }

        else
        {
            Debug.LogError("Audio file not found at: " + audioPath);
        }
    }

    private IEnumerator LoadAudio(string audioPath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
