# 一个简单的背包系统，来自正在开发中的桌宠项目……
该背包系统分为[数据存取](#data)和[UI显示](#ui)两部分。  

<h2 id="data">数据存储</h2>  

首先创建物品基类，包含几个物体的基本信息:  
```c#
public class Item
{
    //顺序无所谓，和json数据对应的是名字，名字一定要一样
    public int id;
    public string name;
    public string description;
    public ItemType type;
    public int price;
    public string icoPath;
    public Sprite ico;
}
```
可对此基类进行继承拓展：
```c#
public class Fruit : Item
{
    public int full;
    public int enegy;
    public int pressure;
}
```
创建存放数量和物品id的slot：
```c#
//仅存放数据的slot
[Serializable]
public class Slot : System.Object//为了显示在inspector继承一下
{
    public int num;
    public int itemId;

    public Slot(int id)
    {
        if (ItemManager.IsContainKey(id)) {
            itemId = id;
            num = 1;
        } else {
            Debug.LogError("ID错误");
        }
    }
}
```

物品创建完毕之后，开始创建物品的静态管理类：
```c#
public static class ItemManager
{
    private static readonly Dictionary<int, Item> itemDic = new Dictionary<int, Item>();

    public static void Init()
    {
        //所有物品载入（JsonUtility）
        var fruitlist = JsonUtility.FromJson<Response<Fruit>>(Resources.Load<TextAsset>("ItemData/Fruits").text);//注意编码为UTF-8不带BOM
        foreach (var item in fruitlist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }
        ...
    }
    [Serializable] class Response<T> { public List<T> list; }//JsonUtility弊端，读取数组需要多做一个数组类;注意所有json数组名都为list

    public static Item LookupItem(int id)
    {
        try {
            return itemDic[id];
        } catch (Exception) {
            Debug.LogError("ID错误");
            return null;
        }
    }
    public static bool IsContainKey(int id)
    {
        return itemDic.ContainsKey(id);
    }
}
```

物品数据的载入使用JsonUtility。这里的方法是将物品数据制成表，导出为csv格式再转换为json格式  
为方便数据转换，写了一个csv2json的小插件，可批量转化数据：  

<https://github.com/lizzzeeeden/BagSystem/tree/main/CSVtoJSON/Editor>  
![itemlist](/img/img_ItemList.PNG)  

↑使用方法：选中csv文件后菜单Tools->CSVtoJSON  

然后来到背包类：  

```c#
public class BagController : System.Object
{
    public List<Slot> bagContent;
    
    /// 获取物品
    private Slot GetSlot(int id){}

    /// 减少物品-1出错，0物品为0，1还剩物品
    private int ReduceItem(Slot slot, int n = 1){}
    public int ReduceItem(int id, int n = 1){}

    /// 获得物品
    public void AddItem(int id, int n = 1){}

    /// 使用物品
    public int UseItem(int id){}

    /// 卖出物品
    public int SellItem(int id, int n = 1){}
    /// 买入物品
    public bool BuyItem(int id, int n=1){}

    /// 检索是否拥有某物，主要是床、车等非消耗品用
    public bool IsOwnItem(int itemid){}
}
```
至此背包的数据管理类创建完毕。
<h2 id="ui">UI显示</h2>

接下来进入UI阶段，UI阶段重点在于和数据类的链接、背包的展示
先做一个UI的slot：
```c#
public class BagSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //等待BagUI内动态加载
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    public Image img;
    public Text text;
    public int itemId;
    public Button btn;
    
    //鼠标与背包格子的交互
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }
}
```

然后创建背包UI管理类，主要字段：  
```c#
//背包页相关，背包按物品类分为n页，每页固定m个格子
private Transform[] pages;
private BagSlot[][] slots;
```
背包UI初始化时读取数据，填充至每个背包slot：
```c#
for (int i = 0; i < pageParent.childCount; i++) {
   slots[i] = new BagSlot[contents[i].childCount];
   slots[i] = contents[i].GetComponentsInChildren<BagSlot>();
   for (int j = 0; j < slots[i].Length; j++) {
       slots[i][j].btn.onClick.AddListener(() => UseItem());
   }
}
```
UI类包含和背包slot交互的具体操作：
```c#
public void ChangePage(bool isOn){}
public void UseItem(){}
```
每次交互时更新UI显示（重新从数据管理类中拉取数据，十分麻烦，尚待优化）：
```c#
private void UpdateBagDisplay()
    {
        //清空格子内容，格子按键设置为不可交互
        for (int i = 0; i < slots.Length; i++) {
            for (int j = 0; j < slots[i].Length; j++) {
                slots[i][j].btn.interactable = false;
                slots[i][j].img.sprite = null;
                slots[i][j].text.text = "";
                slots[i][j].onPointerEnter.RemoveAllListeners();
                slots[i][j].onPointerExit.RemoveAllListeners();
            }
        }
        cnt0 = cnt1 = cnt2 = cnt3 = 0;//计数归零
        if (EmoDataManager.BagContent.Count <= 0) return;
        foreach (var item in EmoDataManager.BagContent) {
            int temp = 0;
            switch (ItemManager.LookupItem(item.itemId).type) {
                case ItemType.FRUIT:
                case ItemType.FISH:
                    slots[0][cnt0].btn.interactable = true;
                    slots[0][cnt0].img.sprite = ItemManager.LookupItem(item.itemId).ico;
                    slots[0][cnt0].text.text = item.num.ToString();
                    slots[0][cnt0].itemId = item.itemId;

                    temp = cnt0;
                    slots[0][cnt0].onPointerEnter.AddListener(() => PointerEnter(0, temp));
                    slots[0][cnt0].onPointerExit.AddListener(() => PointerExit());

                    cnt0++;
                    break;
                    
                case ItemType.CONSUMABLE:
                case ItemType.SAPLING:
                    break;
                    
                case ItemType.SUNDRY:
                case ItemType.QUESTNEED:
                    break;

                case ItemType.UNCONSUMABLE:
                    break;
                    
                default:
                    break;
            }
        }
    }
```
