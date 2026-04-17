using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("物理与跳跃")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f; // 新增：跳跃高度
    private Vector3 velocity;
    private bool isGrounded;

    [Header("依赖组件")]
    public Transform cam;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;
        // === 1. 重力与地面检测 ===
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // === 新增：2. 跳跃逻辑 ===
        // 如果踩在地上，并且按下了跳跃键（默认是空格键 Space）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 这是 Unity 标准的跳跃物理公式：v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // === 3. 水平移动控制 ===
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // === 4. 应用重力 ===
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}