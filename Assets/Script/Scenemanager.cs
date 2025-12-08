using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.

public class MenuManager : MonoBehaviour
{
    // === 인스펙터 설정 ===
    // 메인 게임 씬의 이름을 여기에 정확히 입력해야 합니다.
    public string gameSceneName = "forest scene"; 

    void Start()
    {
        // 시작 화면에서는 마우스 커서를 보이게 합니다.
        Cursor.visible = true;
    }

    // 1. "게임 시작" 버튼에 연결할 함수
    public void StartGame()
    {
        // 지정된 이름의 씬을 로드합니다.
        SceneManager.LoadScene(gameSceneName);
    }

    // 2. "게임 종료" 버튼에 연결할 함수
    public void QuitGame()
    {
        // 런타임 환경에 따라 게임을 종료하거나 에디터 플레이 모드를 중지합니다.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}