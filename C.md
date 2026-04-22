
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