using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelLoader : MonoBehaviour
{
    // 다음 스테이지 씬의 이름을 "game scene"으로 설정
    public string nextSceneName = "game scene";
    
    // 플레이어가 클리어 지점(트리거)에 진입했을 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 오브젝트가 'Player' 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("스테이지 클리어! 다음 씬 로드 중: " + nextSceneName);
            
            // 씬 로드를 위해 짧은 딜레이를 줄 수 있습니다. (선택 사항)
            // Invoke("LoadNextLevel", 1.5f); 

            // 다음 씬을 즉시 로드합니다.
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // SceneManager.LoadScene(씬 이름)을 사용하여 씬을 로드합니다.
        SceneManager.LoadScene(nextSceneName);
    }
}