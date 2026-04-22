using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [Header("我是卖什么的？")]
    public ItemData item; // 在面板里拖入白菜、萝卜等数据

    [Header("我的图标是谁？")]
    public Image iconImage; // 拖入这个格子自己的 Image 组件

    private void Start()
    {
        if (item != null)
        {
            // 游戏开始时，自动把格子的图片换成商品的图标
            if (iconImage != null) iconImage.sprite = item.itemIcon;

            // 获取格子上的 Button 组件，并绑定点击事件
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnClickSlot);
            }
        }
    }

    // 当玩家点击这个格子时触发
    private void OnClickSlot()
    {
        // 呼叫店长（ShopManager），把自己的商品数据传过去
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ShowItemDetails(item);
        }
    }
}