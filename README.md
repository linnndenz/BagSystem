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



<h2 id="ui">UI显示</h2>
