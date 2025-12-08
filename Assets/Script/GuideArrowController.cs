using UnityEngine;

public class GuideArrowController : MonoBehaviour
{
    // [싱글톤] 체크포인트들이 나를 쉽게 찾을 수 있게 함
    public static GuideArrowController instance;
    private void Awake() { if (instance == null) instance = this; }

    [Header("연결할 것들")]
    public Transform player;          // 플레이어
    public Transform[] targets;       // [수정됨] 목적지 목록 (배열)

    [Header("설정")]
    public float radius = 1.5f;       // 반지름
    public bool hideOnClose = true;   // 가까워지면 숨기기
    public float hideDistance = 3.0f; // 숨기는 거리

    // 현재 몇 번째 체크포인트를 가리키고 있는지
    private int currentIndex = 0;

    void Update()
    {
        // 타겟이 없거나, 모든 타겟을 다 돌았으면 화살표 끄기
        if (player == null || targets == null || targets.Length == 0 || currentIndex >= targets.Length)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        // 현재 가리켜야 할 타겟 가져오기
        Transform currentTarget = targets[currentIndex];

        // 1. 방향 및 거리 계산
        Vector3 direction = (currentTarget.position - player.position).normalized;
        float distance = Vector3.Distance(player.position, currentTarget.position);

        // 2. 숨기기 로직
        if (hideOnClose && distance < hideDistance)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;

            // 3. 위치 잡기 & 회전
            transform.position = player.position + (direction * radius);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // [핵심] 다음 타겟으로 넘기는 함수 (체크포인트가 호출함)
    public void NextTarget()
    {
        currentIndex++; // 번호 증가 (0 -> 1 -> 2...)
        
        // 마지막 체크포인트까지 다 왔으면?
        if (currentIndex >= targets.Length)
        {
            Debug.Log("모든 체크포인트 통과! 화살표 끄기");
            gameObject.SetActive(false); // 화살표 아주 끄기
        }
        else
        {
            Debug.Log("다음 체크포인트로 화살표 변경! -> " + targets[currentIndex].name);
        }
    }
}