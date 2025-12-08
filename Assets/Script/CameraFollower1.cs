using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // === 인스펙터에서 설정할 변수 ===
    public Transform target;            // 플레이어
    public float smoothSpeed = 0.125f;  // 따라가는 속도 (0.1 ~ 0.2 추천)
    
    [Header("위치 조정")]
    public float yOffset = 1.5f;        // ⭐ 캐릭터보다 얼마나 더 위를 비출지 (높이 조절)
    // (기존 yPosition 변수는 삭제했습니다. 이제 필요 없으니까요!)

    // === 내부 변수 ===
    private Vector3 velocity = Vector3.zero;
    private float initialZ; 

    void Start()
    {
        initialZ = transform.position.z; // 원래 Z값(-10) 기억하기
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        // 1. 목표 위치 설정
        // [수정됨] 이제 X뿐만 아니라 Y도 target을 따라갑니다!
        Vector3 desiredPosition = new Vector3(
            target.position.x,           // X는 캐릭터 따라가기
            target.position.y + yOffset, // ⭐ Y도 캐릭터 따라가기 (+ 높이 조절)
            initialZ                     // Z는 고정
        );

        // 2. 부드러운 이동
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            smoothSpeed
        );
    }
}