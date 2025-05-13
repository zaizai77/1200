using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("旋转中心（Boss Transform）")]
    public Transform bossTransform;

    [Header("相机距离")]
    public float distance = 20f;

    [Header("旋转速度（度/秒）")]
    public float rotationSpeed = 50f;

    void LateUpdate()
    {
        if (bossTransform == null) return;

        // 持续对准 Boss
        transform.LookAt(bossTransform);  // 将摄像机前方向指向 bossTransform:contentReference[oaicite:0]{index=0}

        // 计算围绕 Y 轴的旋转角度
        float angle = rotationSpeed * Time.deltaTime;

        // 按住 Q 键时顺时针旋转
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(
                bossTransform.position,       // 旋转中心:contentReference[oaicite:1]{index=1}
                Vector3.up,                   // Y 轴正方向
                angle                         // 旋转增量（正值表示顺时针）
            );
        }
        // 按住 E 键时逆时针旋转
        else if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(
                bossTransform.position,       // 旋转中心:contentReference[oaicite:2]{index=2}
                Vector3.up,                   // Y 轴正方向
                -angle                        // 负值表示逆时针
            );
        }

        // 保持与 Boss 的距离不变
        Vector3 dir = (transform.position - bossTransform.position).normalized;
        transform.position = bossTransform.position + dir * distance;
    }
}
