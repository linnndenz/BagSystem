using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//消耗品(consume)：树苗、红茶 //非消耗品(unconsume)：床车等 //杂物(sundry)：木材
public enum ItemType { FRUIT = 1, FISH = 2, CONSUMABLE = 3, SUNDRY = 4, QUESTNEED = 5, UNCONSUMABLE = 6, SAPLING = 7, }

//物品管理类******************************************************************************************************
public static class ItemManager
{
    private static readonly Dictionary<int, Item> itemDic = new Dictionary<int, Item>();

    public static void Init()
    {
        //所有物品载入
        var fruitlist = JsonUtility.FromJson<Response<Fruit>>(Resources.Load<TextAsset>("ItemData/Fruits").text);//注意编码为UTF-8不带BOM
        foreach (var item in fruitlist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }

        var fishlist = JsonUtility.FromJson<Response<Fish>>(Resources.Load<TextAsset>("ItemData/Fishes").text);//注意编码为UTF-8不带BOM
        foreach (var item in fishlist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }

        var consumelist = JsonUtility.FromJson<Response<Consumable>>(Resources.Load<TextAsset>("ItemData/Consumables").text);//注意编码为UTF-8不带BOM
        foreach (var item in consumelist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }

        var sundrylist = JsonUtility.FromJson<Response<Sundry>>(Resources.Load<TextAsset>("ItemData/Sundrys").text);//注意编码为UTF-8不带BOM
        foreach (var item in sundrylist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }

        var saplinglist = JsonUtility.FromJson<Response<Sapling>>(Resources.Load<TextAsset>("ItemData/Saplings").text);//注意编码为UTF-8不带BOM
        foreach (var item in saplinglist.list) {
            item.ico = Resources.Load<Sprite>(item.icoPath);
            itemDic.Add(item.id, item);
        }
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

//物品类**********************************************************
//物品类（物品类继承至此）**************************
[Serializable]
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
//水果类（衍生自物品类）**************************
[Serializable]
public class Fruit : Item
{
    public int full;
    public int enegy;
    public int pressure;
}
//鱼类（衍生自物品类）****************************
[Serializable]
public class Fish : Item
{
    public int full;
    public int enegy;
    public int pressure;
}
//消耗品类（衍生自物品类）************************
[Serializable]
public class Consumable : Item
{
}
//杂物类（衍生自物品类）**************************
[Serializable]
public class Sundry : Item
{

}
//树苗类（衍生自物品类）*************************
[Serializable]
public class Sapling : Item
{
    public int full;
    public int enegy;
    public int pressure;
    public float ripeTime;
    public float fruitTime;
    public int woodNum;//砍树后获得木材数量
    public int fruitNum;//每次采摘获得的水果数量

}


//背包资产类***********************************************
//单个物品（附带拥有数量）类********************
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
