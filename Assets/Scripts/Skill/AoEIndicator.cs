using UnityEngine;

public class AoEIndicator : MonoBehaviour
{
    public Projector projector;  // 拖入 Projector 组件
    public float radius = 5f;    // 技能半径

    void Start()
    {
        UpdateRadius(radius);
        Hide();  // 默认隐藏
    }

    /// <summary> 显示并设置半径 </summary>
    public void Show(float r)
    {
        radius = r;
        UpdateRadius(r);
        projector.enabled = true;
    }

    /// <summary> 隐藏指示器 </summary>
    public void Hide()
    {
        projector.enabled = false;
    }

    private void UpdateRadius(float r)
    {
        // 在正交模式下，size 即直径
        projector.orthographicSize = r;
    }
}
