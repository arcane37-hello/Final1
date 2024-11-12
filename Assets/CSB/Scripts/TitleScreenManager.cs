
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // 버튼 클릭 시 호출되는 함수
    public void OnStartButtonClicked()
    {
        // MainScene으로 전환
        SceneManager.LoadScene(1);
    }
}