using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 6f;

    [Header("物理与跳跃")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private Vector3 velocity;
    private bool isGrounded;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 完美保留你的 GameManager 状态检测，这句非常重要
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
            return;

        // === 1. 重力与地面检测 ===
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // === 2. 跳跃逻辑 ===
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // === 3. 纯粹的第一人称移动控制（彻底移除了导致乱转的代码） ===
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 【核心修改】直接拿主角身体的正前方和右方去走！不再依赖相机的旋转去算，也不再强行扭转身体！
        Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;

        if (moveDir.magnitude >= 0.1f)
        {
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // === 4. 应用重力 ===
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}