
## 一 调用示例
### 1.物品相关
```c#
// 1. 给玩家发货 (场景：收获作物、获得奖励)
InventoryManager.Instance.AddItem(itemData, 1);

// 2. 扣除玩家物品 (场景：把种子种进地里、消耗材料)
// 返回 true 代表扣除成功，返回 false 代表玩家包里数量不够
bool isSuccess = InventoryManager.Instance.RemoveItem(itemData, 1);
```

### 2.金币相关
```c#
// 3. 给玩家发钱 (场景：跨天出货箱结算、NPC给的红包)
InventoryManager.Instance.AddGold(100);

// 4. 扣除玩家金币 
// 返回 true 代表扣款成功，返回 false 代表玩家是个穷鬼
bool isSuccess = InventoryManager.Instance.SpendGold(50);

// 5. 查看玩家当前有多少钱 (场景：UI显示余额、判断能不能买得起某些东西)
int myMoney = InventoryManager.Instance.currentGold;
```

### 3.商店相关
```c#
// 6. 尝试购买商店里的某件商品 (场景：玩家点击了商店面板上的"购买"按钮)
// 这个方法内部已经自动包含了“扣钱”和“发货”两步，不需要额外再写！
ShopManager.Instance.BuyItem(itemData);
```

## 二 对接AB
可通过`create FameGame`的方式在Assets\Scripts\Data下创建`Item Data`。

由我来创建这些`Item Data`的话

#### A需要给我提供：
畜牧的动物的：
* 1.名字
* 2.小的时候买的价格
* 3.长大后卖的价格
* 4.一张2D图片

#### B需要给我提供：
作物的：
* 1.名字
* 2.种子的价格
* 3.成熟作物卖出的价格
* 4.种子的2D图片
* 5.成熟作物的2D图片

图片放在项目的Textures文件夹就可以，命名的话用英文命名，价格可以直接告诉我，图片最好是正方形方便做背包。

## 三 关于背包和一些设置

除了需要建立一个空物体挂载上game和time manager以外，InventoryManager也是需要挂载上去的。

#### 如何初始化背包中物品数值？
所有的物品，在创建了ItemData以后才会在背包存在，默认值皆为0。
如果希望初始化为一个值。可在挂载了InventoryManager的物体处设置。在 Item Data 槽位，把你的ItemData源文件拖进去。

## 四 商店打开对接D

了让玩家能顺利打开商店，我提供了一个全局公开的接口：`ShopManager.Instance.OpenShop();`。

为了不影响你现有的对话和时间架构，需要你在`NPCEntity.cs` 脚本里加一点判定逻辑，把这个接口接上去。

只有当与商店的商人交互的时候，才会弹出这个商店的UI，
所以需要你在`NPCEntity.cs`脚本里加一个 `isMerchant` (是否为商人) 的布尔值开关。只有当这个开关为Ture的时候，才会打开商店。下面是AI提供的修改参考：
```csharp
using UnityEngine;

public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    // --- 新增 1：用于在面板里区分普通 NPC 和商人 ---
    [Header("商人设定")]
    public bool isMerchant; 

    public string GetInteractPrompt()
    {
        if (npcData == null) return "未知 NPC (请检查数据配置)"; 
        
        // --- 新增 2 (可选)：如果是商人，UI 提示语可以换成交易 ---
        if (isMerchant) return $"与商贩 {npcData.npcName} 交易";
        
        return $"与 {npcData.npcName} 对话";
    }

    public void Interact()
    {
        if (npcData == null) return;

        // 1. 触发社交系统：调用对话管理器弹出 UI 并显示文本 (保留你的原逻辑)
        DialogueManager.Instance.StartDialogue(npcData);

        // 2. --- 新增 3 (核心对接)：如果是商人，额外呼出我的商店 UI ---
        if (isMerchant && ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop();
        }

        // 3. 触发时间系统 (保留你的原逻辑)
        TimeManager.Instance.AddMinutes(8);
    }
}
```

## 五 游戏开始界面and对接A

目前点击开始游戏按钮后，会将场景切换至我自己创建的UI场景。如果后序需要切入到主场景，步骤如下：

#### 第一步：在面板里替换场景名字
1. 打开 `Scenes/MainMenu` 场景。
2. 在 Hierarchy 层级面板中，选中挂载了 `MainMenuUI` 脚本的物体（Canvas ）。
3. 看向右侧的 Inspector 面板，找到 **`First Scene Name`** 这个输入框。
4. 把里面的 `"UI"` 删掉，**换成你做好的主场景的名字**（注意大小写必须完全一致，比如 `"farm"`）。

#### 第二步：添加进 Build Settings (防报错)
1. 点击 Unity 顶部菜单栏 `File -> Build Settings...`。
2. 确保 `MainMenu` 场景在列表里（序号为 0）。
3. **把你做好的主场景拖进这个列表里**，并确保右边的小方框打上了勾 `[√]`。
*(注意：如果不拖进来，点击开始按钮时会报错找不到场景！)*