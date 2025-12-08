using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동 필수!

public class ReturnToTitle : MonoBehaviour
{
    // 버튼에 연결할 함수
    public void GoToMain()
    {
        // "StartMenu" 부분에 유저님의 시작 씬 이름을 정확히 적으세요!
        // (대소문자, 띄어쓰기 틀리면 안 됩니다)
        SceneManager.LoadScene("title scene");
    }
}