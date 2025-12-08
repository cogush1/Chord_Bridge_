using System.Collections;
using UnityEngine;

public class BlinkingSprite : MonoBehaviour
{
    [Header("깜빡임 설정")]
    [Tooltip("숫자가 클수록 빨리 깜빡입니다.")]
    public float blinkSpeed = 2.0f; // 속도 조절용

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            StartCoroutine(BlinkRoutine());
        }
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // [핵심] PingPong: 0에서 1까지 갔다가, 다시 0으로 부드럽게 돌아오는 숫자 만들기
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);

            // 색깔 가져와서 투명도(a)만 바꾸기
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;

            // '다음 프레임'까지 대기 (매 프레임마다 부드럽게 변해야 하니까)
            yield return null; 
        }
    }
}