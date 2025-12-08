using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    // 구역 종류를 고를 수 있게 목록을 만듭니다
    public enum ZoneType
    {
        Vocal, // 보컬 존 (R키 사용, Dancing Dots)
        Chord  // 코드 존 (R키 불가, Chord BGM)
    }

    [Header("설정")]
    public ZoneType zoneType; // Inspector에서 여기서 'Vocal'인지 'Chord'인지 고르세요!
    
    [Header("매니저 연결")]
    public VocalManager vocalManager;

    // 플레이어가 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (vocalManager != null)
            {
                // 선택된 존 타입에 따라 다른 음악을 틉니다
                if (zoneType == ZoneType.Vocal)
                {
                    vocalManager.PlayVocalMusic();
                    Debug.Log("✅ 보컬 존 진입!");
                }
                else if (zoneType == ZoneType.Chord)
                {
                    vocalManager.PlayChordMusic();
                    Debug.Log("🎹 코드 존 진입!");
                }
            }
        }
    }

    // 플레이어가 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (vocalManager != null)
            {
                // 어디서 나갔든 기본 음악으로 복귀
                vocalManager.PlayBaseMusic();
                Debug.Log("🏃 구역 이탈 (기본 음악 재생)");
            }
        }
    }
}