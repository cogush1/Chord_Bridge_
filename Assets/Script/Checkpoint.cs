using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    public Color activeColor = Color.green;

    [Header("효과 설정")]
    public AudioClip checkSound;    // 효과음
    public ParticleSystem hitEffect;// [추가됨] 파티클 효과
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; 
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // 1. 부활 위치 저장
            if (VocalManager.instance != null)
            {
                VocalManager.instance.respawnPoint = transform.position;
            }

            // 2. 화살표 갱신
            if (GuideArrowController.instance != null)
            {
                GuideArrowController.instance.NextTarget();
            }

            // 3. 색깔 바꾸기
            GetComponent<SpriteRenderer>().color = activeColor;

            // 4. 효과음 재생
            if (checkSound != null)
            {
                audioSource.PlayOneShot(checkSound);
            }

            // 5. [추가됨] 파티클 재생!
            if (hitEffect != null)
            {
                hitEffect.Play();
            }
            
            Debug.Log("🚩 체크포인트 저장완료!");
        }
    }
}