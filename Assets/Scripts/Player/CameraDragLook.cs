using UnityEngine;

public class CameraDragLook : MonoBehaviour
{
    [Header("视角设置")]
    public float sensitivity = 3f;
    public float minYAngle = -60f;
    public float maxYAngle = 60f;

    [Tooltip("必须拖入主角的身体(Player1)，用来控制左右转身")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // 自动往上找主角身体
        if (playerBody == null) playerBody = transform.parent;
    }

    void Update()
    {
        // 0 代表按住鼠标左键。如果你想右键，改回 1。
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // 1. 上下看：只让摄像机自己抬头/低头（绕X轴）
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minYAngle, maxYAngle);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // 2. 左右看：直接转动整个主角身体（绕Y轴）
            if (playerBody != null)
            {
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
    }
}