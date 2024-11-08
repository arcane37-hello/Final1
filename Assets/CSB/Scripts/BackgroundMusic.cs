using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{

    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 추가
        audioSource.clip = backgroundMusic; // 배경음악 지정
        audioSource.loop = true; // 반복재생 설정
        audioSource.playOnAwake = true; // 자동 재생 설정
        audioSource.volume = 0.2f; // 볼륨 조절(0.0~1.0 사이값)
        audioSource.Play(); // 배경음악 재생


    }
}
