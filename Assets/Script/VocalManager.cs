using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VocalManager : MonoBehaviour
{
    public static VocalManager instance;
    private void Awake() { if (instance == null) instance = this; else Destroy(gameObject); }

    [Header("1. 연결할 것들")]
    public GameObject platformPrefab;
    public Transform playerTransform;
    public GameObject[] breathIcons;
    
    [Header("2. UI 및 연출")]
    public GameObject noticeTextObject;
    public GameObject fadePanel;
    public GameObject gameOverUI;

    [Header("3. 오디오 소스")]
    public AudioSource bgmAudioSource;  // Vocal Zone
    public AudioSource baseBgmSource;   // Base
    public AudioSource chordBgmSource;  // Chord Zone
    
    // 효과음 스피커
    private AudioSource sfxSource;

    [Header("4. 효과음 설정")]
    public AudioClip gameOverSound;
    public AudioClip hitSound;
    public AudioClip failSound;

    [Header("5. 설정 값")]
    public int maxCharges = 5;
    public float regenCooldown = 3.0f;
    public Vector3 platformOffset = new Vector3(0, -1.5f, 0);

    [Header("6. 리듬 설정")]
    public float vocalBpm = 95f;
    public float baseBpm = 120f;
    public float chordBpm = 100f;
    public float tolerance = 0.15f;

    [Header("7. 상태 확인용")]
    public bool canUseVocal = false;
    public bool isChordZone = false;
    public bool isDead = false;
    public bool isClear = false; // [추가됨] 클리어 상태 체크용
    
    public int currentCharges;
    public Vector3 respawnPoint;
    private float regenTimer = 0f;

    void Start()
    {
        canUseVocal = false;
        isChordZone = false;
        isDead = false;
        isClear = false; // 초기화
        currentCharges = maxCharges;
        UpdateUI();

        // 효과음 스피커 생성
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // 시작 시 모든 음악 끄고 기본 음악만 재생
        StopAllMusic();
        if (baseBgmSource != null) baseBgmSource.Play();

        if (noticeTextObject != null) noticeTextObject.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (playerTransform != null) respawnPoint = playerTransform.position;
    }

    void Update()
    {
        // 죽었거나, 클리어했거나, 보컬 능력을 못 쓰면 R키 무시
        if (isDead || isClear || canUseVocal == false) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentCharges > 0)
            {
                if (IsOnBeat())
                {
                    UseCharge();
                    PlayHitSound();
                    Debug.Log("Perfect! 🎵");
                }
                else 
                {
                    PlayFailSound();
                    Debug.Log("Miss... ❌");
                }
            }
            else Debug.Log("게이지 부족!");
        }
        HandleRegen();
    }

    // [추가] 클리어 시 호출할 함수 (음악 끄기 + 상태 잠금)
    public void OnGameClear()
    {
        isClear = true; // 클리어 상태 ON
        StopAllMusic(); // 모든 음악 정지
    }

    public void PlayHitSound()
    {
        if (hitSound != null && sfxSource != null) sfxSource.PlayOneShot(hitSound);
    }

    public void PlayFailSound()
    {
        if (failSound != null && sfxSource != null) sfxSource.PlayOneShot(failSound);
    }

    public bool IsOnBeat()
    {
        if (isDead || isClear) return false; // 죽거나 깼으면 판정 안 함

        AudioSource currentSource;
        float currentBpm;

        if (canUseVocal) { currentSource = bgmAudioSource; currentBpm = vocalBpm; }
        else if (isChordZone) { currentSource = chordBgmSource; currentBpm = chordBpm; }
        else { currentSource = baseBgmSource; currentBpm = baseBpm; }

        // [중요] 음악이 없으면 무조건 실패 (움직임 막기)
        if (currentSource == null || !currentSource.isPlaying) return false;

        float secPerBeat = 60f / currentBpm;
        float songTime = currentSource.time;
        float positionInBeat = songTime % secPerBeat;

        return positionInBeat < tolerance || positionInBeat > (secPerBeat - tolerance);
    }

    void UseCharge()
    {
        if (currentCharges <= 0) return;
        currentCharges--;
        regenTimer = 0f;
        if (platformPrefab != null) Instantiate(platformPrefab, playerTransform.position + platformOffset, Quaternion.identity);
        UpdateUI();
    }

    void HandleRegen()
    {
        if (currentCharges < maxCharges)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenCooldown)
            {
                currentCharges++;
                regenTimer = 0f;
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < breathIcons.Length; i++)
        {
            if (breathIcons[i] == null) continue;
            if (i < currentCharges) breathIcons[i].SetActive(true);
            else breathIcons[i].SetActive(false);
        }
    }

    // [음악 제어] 클리어 상태 체크 추가
    public void PlayBaseMusic()
    {
        if (isDead || isClear) return; // [중요] 클리어 상태면 무시

        StopAllMusic();
        if (baseBgmSource) baseBgmSource.Play();
        isChordZone = false;
        canUseVocal = false;
        if(noticeTextObject) noticeTextObject.SetActive(false);
    }

    public void PlayVocalMusic()
    {
        if (isDead || isClear) return;

        StopAllMusic();
        if (bgmAudioSource) bgmAudioSource.Play();
        isChordZone = false;
        canUseVocal = true;
        if(noticeTextObject) noticeTextObject.SetActive(true);
    }

    public void PlayChordMusic()
    {
        if (isDead || isClear) return;

        StopAllMusic();
        if (chordBgmSource) chordBgmSource.Play();
        isChordZone = true;
        canUseVocal = false;
        
        if(noticeTextObject != null) 
        {
            noticeTextObject.GetComponent<TextMeshProUGUI>().text = "You entered in Chord Zone! ⚠️\nPass the road with the beat!";
            noticeTextObject.SetActive(true);
        }
    }

    public void StopAllMusic()
    {
        if (baseBgmSource) baseBgmSource.Stop();
        if (bgmAudioSource) bgmAudioSource.Stop();
        if (chordBgmSource) chordBgmSource.Stop();
    }

    public void RespawnPlayer()
    {
        if (isDead || isClear) return; 
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDead = true;
        StopAllMusic();
        if (baseBgmSource != null && gameOverSound != null) baseBgmSource.PlayOneShot(gameOverSound);

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            Image panelImg = fadePanel.GetComponent<Image>();
            float t = 0;
            while (t < 1) { t += Time.deltaTime * 2; panelImg.color = new Color(0, 0, 0, t); yield return null; }
        }

        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    public void OnClickRetry()
    {
        StartCoroutine(ReviveSequence());
    }

    IEnumerator ReviveSequence()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (playerTransform != null)
        {
            playerTransform.position = respawnPoint;
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        currentCharges = maxCharges;
        UpdateUI();
        
        isDead = false; 
        // 마지막 위치에 따라 음악 재생해야 하지만, 일단 기본 음악 재생
        PlayBaseMusic(); 

        if (fadePanel != null)
        {
            Image panelImg = fadePanel.GetComponent<Image>();
            float t = 1;
            while (t > 0) { t -= Time.deltaTime * 2; panelImg.color = new Color(0, 0, 0, t); yield return null; }
            fadePanel.SetActive(false);
        }

        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        if (playerRb != null) playerRb.simulated = true;
    }
}