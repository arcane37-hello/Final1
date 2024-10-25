using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public string imagePath = ""; // 이미지 파일 경로
    public Image uiImage; // UI에서 표시할 Image 컴포넌트

    void Start()
    {

    }

    public void LoadImage()
    {
        // 파일 경로 설정
        string path = Application.streamingAssetsPath + imagePath;

        if (File.Exists(path))
        {
            // 이미지 파일 읽기
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // 이미지 로드

            // Texture2D를 Sprite로 변환
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // UI Image에 Sprite 설정
            uiImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image file not found at " + path);
        }
    }
}
