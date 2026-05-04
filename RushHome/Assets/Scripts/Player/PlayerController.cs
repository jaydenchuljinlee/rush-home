using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] float laneWidth = 2.5f;
    [SerializeField] float laneSwitchSpeed = 12f;

    [Header("Movement - Acceleration")]
    [SerializeField] float acceleration = 20f;       // 가속 (키 누를 때)
    [SerializeField] float deceleration = 15f;       // 감속 (키 놓을 때, 지면)
    [SerializeField] float airDeceleration = 3f;     // 감속 (키 놓을 때, 공중 — 관성 유지)
    [SerializeField] float airControlFactor = 0.4f;  // 공중에서 가속력 비율

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 4.5f;         // 발이 약 1유닛(허리)까지 — 자연스러운 점프
    [SerializeField] float fallMultiplier = 3f;      // 하강 시 추가 중력
    [SerializeField] float lowJumpMultiplier = 2f;   // 상승 중에도 약간 추가 중력 (체공 시간 단축) (빠르게 착지)

    [Header("Slide Settings")]
    [SerializeField] float slideDuration = 0.8f;

    [Header("Model Rotation")]
    [SerializeField] float rotationSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer;

    Rigidbody rb;
    CapsuleCollider col;
    Animator animator;
    Transform modelTransform;

    int currentLane;
    float targetX;
    bool isGrounded;
    bool isSliding;
    float slideTimer;
    float originalHeight;
    float originalCenterY;

    float currentSpeedZ; // 현재 전후 속도 (가속도로 변화)
    float moveInputZ;    // 입력 (-1 ~ 1)
    float currentLateralSpeed; // 실제 X축 이동 속도
    bool isDying;

    // 터치 입력
    Vector2 touchStartPos;
    bool isSwiping;
    const float SwipeThreshold = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        animator = GetComponentInChildren<Animator>();
        if (animator != null) modelTransform = animator.transform;

        originalHeight = col.height;
        originalCenterY = col.center.y;
        currentLane = 0;
        targetX = 0f;
        currentSpeedZ = 0f;
    }

    void Update()
    {
        var state = GameManager.Instance.CurrentState;

        if (isDying)
        {
            CheckGround();
            if (isGrounded)
            {
                isDying = false;
                currentSpeedZ = 0f;
                if (animator != null) animator.SetFloat("Speed", 0f);
                GameManager.Instance.PlayerLanded();
            }
            return;
        }

        if (state != GameManager.GameState.Playing) return;

        CheckGround();
        HandleInput();
        UpdateSlide();
        MoveToLane();
        UpdateAnimator();
    }

    void LateUpdate()
    {
        var state = GameManager.Instance.CurrentState;
        if (state != GameManager.GameState.Playing && !isDying) return;
        UpdateTilt();
    }

    void FixedUpdate()
    {
        if (isDying)
        {
            // 죽는 중에도 중력만 적용 (낙하)
            if (!isGrounded)
            {
                if (rb.linearVelocity.y < 0)
                    rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
                else
                    rb.AddForce(Physics.gravity * (lowJumpMultiplier - 1f), ForceMode.Acceleration);
            }

            // 속도를 서서히 줄임
            Vector3 dyingVel = rb.linearVelocity;
            dyingVel.x = 0f;
            dyingVel.z = Mathf.MoveTowards(dyingVel.z, 0f, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = dyingVel;
            return;
        }

        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // === 상승/하강 모두 추가 중력 → 체공 단축, 묵직한 점프 ===
        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0)
                rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
            else
                rb.AddForce(Physics.gravity * (lowJumpMultiplier - 1f), ForceMode.Acceleration);
        }

        // === 가속도 기반 전후 이동 ===
        float maxSpeed = DifficultyManager.Instance.CurrentSpeed;
        float dt = Time.fixedDeltaTime;

        if (Mathf.Abs(moveInputZ) > 0.01f)
        {
            // 입력 있음 → 가속
            float accel = isGrounded ? acceleration : acceleration * airControlFactor;
            float targetSpeed = moveInputZ * maxSpeed;
            currentSpeedZ = Mathf.MoveTowards(currentSpeedZ, targetSpeed, accel * dt);
        }
        else
        {
            // 입력 없음 → 감속 (공중에서는 관성 유지, 천천히 감속)
            float decel = isGrounded ? deceleration : airDeceleration;
            currentSpeedZ = Mathf.MoveTowards(currentSpeedZ, 0f, decel * dt);
        }

        Vector3 vel = rb.linearVelocity;
        vel.z = currentSpeedZ;
        rb.linearVelocity = vel;
    }

    void CheckGround()
    {
        float capsuleBottom = col.bounds.min.y;
        Vector3 origin = new Vector3(transform.position.x, capsuleBottom + 0.2f, transform.position.z);
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.3f, groundLayer);
    }

    void HandleInput()
    {
        HandleKeyboardInput();
        HandleTouchInput();
    }

    void HandleKeyboardInput()
    {
        // 전/후 이동 입력
        moveInputZ = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            moveInputZ = 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            moveInputZ = -0.5f;

        // 좌/우 레인 전환
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            SwitchLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            SwitchLane(1);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
            Jump();

        // 슬라이드
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSliding)
            StartSlide();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Ended:
                if (!isSwiping) break;
                isSwiping = false;

                Vector2 delta = touch.position - touchStartPos;

                if (delta.magnitude < SwipeThreshold)
                {
                    if (isGrounded && !isSliding) Jump();
                    break;
                }

                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    SwitchLane(delta.x > 0 ? 1 : -1);
                else if (delta.y > 0)
                {
                    if (isGrounded && !isSliding) Jump();
                }
                else
                {
                    if (isGrounded && !isSliding) StartSlide();
                }
                break;
        }
    }

    void SwitchLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, -1, 1);
        if (newLane == currentLane) return;
        currentLane = newLane;
        targetX = currentLane * laneWidth;
    }

    void MoveToLane()
    {
        Vector3 pos = transform.position;
        float prevX = pos.x;
        pos.x = Mathf.MoveTowards(pos.x, targetX, laneSwitchSpeed * Time.deltaTime);
        transform.position = pos;
        currentLateralSpeed = (pos.x - prevX) / Time.deltaTime;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        if (animator != null) animator.SetTrigger("Jump");
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        col.height = originalHeight * 0.4f;
        col.center = new Vector3(col.center.x, originalCenterY - originalHeight * 0.3f, col.center.z);
    }

    void EndSlide()
    {
        isSliding = false;
        col.height = originalHeight;
        col.center = new Vector3(col.center.x, originalCenterY, col.center.z);
    }

    void UpdateSlide()
    {
        if (!isSliding) return;
        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0f)
            EndSlide();
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(currentSpeedZ));
        animator.SetBool("IsGrounded", isGrounded);
    }

    void UpdateTilt()
    {
        if (modelTransform == null) return;

        Vector3 moveDir = new Vector3(currentLateralSpeed, 0f, currentSpeedZ);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }

    // === 외부 효과 API (장애물 IObstacleEffect에서 호출) ===

    public void ApplyKnockback(float force)
    {
        currentSpeedZ = -force;
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;
        moveInputZ = 0f;

        if (isGrounded)
        {
            // 이미 지면 → 즉시 GameOver
            isDying = false;
            currentSpeedZ = 0f;
            if (animator != null) animator.SetFloat("Speed", 0f);
            GameManager.Instance.PlayerLanded();
        }
    }
}
