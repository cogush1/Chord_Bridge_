using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // 일반 이동 속도
    public float jumpForce = 10f;       // 점프 힘
    public float rhythmStepForce = 5f;  // 리듬 이동 힘

    [Header("Ground Detection")]
    public Transform groundCheck;       // 발밑 감지 위치
    public Vector2 boxSize = new Vector2(0.8f, 0.2f); // 감지 박스 크기
    public LayerMask groundLayer;       // 땅 레이어

    private Rigidbody2D rb;
    private Animator anim;
    
    public bool isGrounded;
    private bool isJumpingInput = false;
    private float lastMoveTime; // 리듬 이동 쿨타임용

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. 바닥 감지
        CheckGround();

        // 2. 이동 방식 결정 [핵심 로직]
        // 매니저가 있고 && (보컬 존이거나 OR 코드 존이라면) -> 부드러운 이동 (소리 X)
        if (VocalManager.instance != null && 
           (VocalManager.instance.canUseVocal || VocalManager.instance.isChordZone))
        {
             HandleNormalMovement();
        }
        else
        {
             // 그 외 구역(Base) -> 리듬 이동 (소리 O)
             HandleRhythmMovement();
        }

        // 3. 점프 및 애니메이션
        HandleJumpInput();
        UpdateAnimationParameters();
    }

    // [일반 이동] 소리 재생 없음, 부드러운 이동
    void HandleNormalMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        FlipSprite(horizontalInput);
    }

    // [리듬 이동] 성공/실패 소리 재생, 박자 이동
    void HandleRhythmMovement()
    {
        // 방향키 입력 (꾹 누르기 X, 순간 입력 O)
        float inputX = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) inputX = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) inputX = -1;

        if (inputX != 0)
        {
            // 쿨타임 체크 (0.2초)
            if (Time.time < lastMoveTime + 0.2f) return;

            if (VocalManager.instance != null && VocalManager.instance.IsOnBeat())
            {
                // [성공]
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // 미끄러짐 방지
                rb.AddForce(Vector2.right * inputX * rhythmStepForce, ForceMode2D.Impulse);
                FlipSprite(inputX);
                lastMoveTime = Time.time;
                
                // 성공 효과음 재생! (여기서만 남)
                VocalManager.instance.PlayHitSound(); 
                
                Debug.Log("리듬 스텝 성공! 👟");
            }
            else
            {
                // [실패]
                // 실패 효과음 재생!
                VocalManager.instance.PlayFailSound();
                
                Debug.Log("박자가 안 맞아요! 😵");
            }
        }
        
        // 키 입력 없으면 멈춤 (미끄러짐 방지)
        if (!Input.anyKey && isGrounded)
        {
             rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // 바닥 감지 (네모 박스)
    void CheckGround()
    {
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);
    }

    // 점프 입력
    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumpingInput = true;
        }
    }

    // 물리 업데이트 (점프 실행)
    void FixedUpdate()
    {
        if (isJumpingInput)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpingInput = false;
        }
    }

    // 애니메이션 설정
    void UpdateAnimationParameters()
    {
        // 입력이 있거나 속도가 있을 때만 걷기 모션
        bool hasInput = Input.GetAxisRaw("Horizontal") != 0 || Input.anyKey;
        
        if (isGrounded)
        {
            if (hasInput && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                anim.SetBool("IsRunning", true);
            }
            else
            {
                anim.SetBool("IsRunning", false);
            }
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        anim.SetBool("IsJumping", !isGrounded);
    }

    // 캐릭터 좌우 반전
    void FlipSprite(float direction)
    {
        if (direction > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }
    
    // 에디터에서 바닥 감지 박스 그리기
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }
    }
}