using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 닿으면 매니저에게 "부활시켜줘!"라고 요청
            if (VocalManager.instance != null)
            {
                VocalManager.instance.RespawnPlayer();
            }
        }
    }
}