using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{

    public AudioClip clickSound; // 버튼 클릭 효과음
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // AudioSource 컴포넌트를 가져오거나 새로 추가.
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clickSound;
    }

    public void PlayClickSound()
    {
        audioSource.Play();
    }    
}
