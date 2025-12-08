using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections; 

public class GameClearTrigger1 : MonoBehaviour
{
    [Header("설정")]
    public string sceneName = "ClearScene"; 
    public AudioClip clearSound;            
    public GameObject fadePanel; 

    private AudioSource audioSource;
    private bool isCleared = false;         

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCleared)
        {
            isCleared = true; 
            StartCoroutine(ClearSequence(other.gameObject));
        }
    }

    IEnumerator ClearSequence(GameObject player)
    {
        Debug.Log("🎉 게임 클리어!");

        // [핵심 수정] 매니저에게 "모든 음악 정지" 명령 내리기
        if (VocalManager.instance != null)
        {
            // 1. 매니저가 관리하는 모든 BGM 끄기
            VocalManager.instance.StopAllMusic(); 
            
            // 2. 혹시 몰라서 기본 BGM 한번 더 확인 사살 (가장 안 꺼지는 녀석)
            if (VocalManager.instance.baseBgmSource != null) 
                VocalManager.instance.baseBgmSource.Stop();
        }

        // 3. 플레이어 멈추기 (물리 & 애니메이션)
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero; 
            rb.simulated = false; 
        }
        
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsJumping", false);
            anim.Play("idle"); 
        }

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        // 4. 클리어 효과음 재생 (BGM 꺼진 뒤 단독 재생)
        if (clearSound != null)
        {
            audioSource.PlayOneShot(clearSound);
            yield return new WaitForSeconds(clearSound.length); // 소리 끝날 때까지 대기
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        // 5. 화면 페이드 아웃
        if (fadePanel != null)
        {
            fadePanel.SetActive(true); 
            Image panelImg = fadePanel.GetComponent<Image>();
            
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / 1.5f; 
                panelImg.color = new Color(0, 0, 0, t);
                yield return null;
            }
        }

        // 6. 씬 이동
        SceneManager.LoadScene(sceneName);
    }
}