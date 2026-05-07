using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("速度设置")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;
    public float crouchSpeed = 2.5f;

    [Header("物理与跳跃")]
    public float gravity = -25f;
    public float jumpHeight = 1.8f;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("蹲下配置")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchTransitionSpeed = 10f;
    private bool isCrouching = false;

    [Header("运动平滑")]
    public float acceleration = 10f; // 速度切换的灵敏度

    private CharacterController controller;
    private float currentSpeed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // 初始化高度
        controller.height = standHeight;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // 1. 重力与地面环境刷新
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. 状态锁：处理非游玩状态（如睡觉）
        if (GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
            ApplyGravity();
            return;
        }

        // 3. 执行核心逻辑
        HandleCrouch();   // 蹲下
        HandleMovement(); // 移动
        HandleJump();     // 跳跃
        ApplyGravity();   // 应用物理
    }

    private void HandleMovement()
    {
        // 获取原始输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 确定目标速度：优先级为 蹲下 > 跑步 > 走路
        float targetSpeed = walkSpeed;
        if (isCrouching) targetSpeed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = runSpeed;

        // 核心修改：第一人称移动逻辑
        // 直接使用 transform.right 和 transform.forward，不再进行模型旋转计算
        Vector3 moveDir = transform.right * h + transform.forward * v;

        if (moveDir.magnitude >= 0.1f)
        {
            // 速度平滑过渡
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            // 执行移动
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            // 停止时平滑减速
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        // 只有在地面上且没在蹲着时才能跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleCrouch()
    {
        // 检测左 Ctrl 键
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        float lastHeight = controller.height;

        // 平滑切换 CharacterController 的高度
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // 关键：位置补偿。计算高度差并应用到 transform，保证脚底始终贴地，坐标值看起来正常
        float heightDifference = controller.height - lastHeight;
        transform.position += new Vector3(0, heightDifference / 2f, 0);

        // 更新中心点
        controller.center = new Vector3(0, controller.height / 2f, 0);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}