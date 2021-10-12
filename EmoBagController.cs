using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

//精灵背包资产数据，函数外部引用只用id
[Serializable]
public class EmoBagController : System.Object
{
    public List<Slot> bagContent;

    private Slot GetSlot(int id)
    {
        for (int i = 0; i < bagContent.Count; i++) {
            if (id == bagContent[i].itemId) {
                return bagContent[i];
            }
        }
        Debug.Log("背包内不含id为 " + id.ToString() + " 名字为 " + ItemManager.LookupItem(id).name + " 的物品");
        return null;
    }

    /// <summary>
    /// 减少物品-1出错，0物品为0，1还剩物品
    /// </summary>
    private int ReduceItem(Slot slot, int n = 1)
    {
        if (slot == null) {
            Debug.LogError("未获取到要减少的物品");
            return -1;
        }
        if (slot.num > n) {
            slot.num -= n;
            return 1;
        } else if (slot.num == n) {
            bagContent.Remove(slot);
            return 0;
        } else {
            Debug.LogError("物品减少数量超出库存");
            return -1;
        }
    }
    public int ReduceItem(int id, int n = 1)
    {
        return ReduceItem(GetSlot(id), n);
    }

    /// <summary>
    /// 获得物品
    /// </summary>
    public void AddItem(int id, int n = 1)
    {
        if (n <= 0) return;
        //原本有物品则直接增加数量
        if (GetSlot(id) != null) {
            GetSlot(id).num += n;
        } else {
            Slot tempSlot = new Slot(id);
            tempSlot.num += n - 1;
            bagContent.Add(tempSlot);
        }

    }

    /// <summary>
    /// 使用物品
    /// </summary>
    public int UseItem(int id)
    {
        Slot slot = GetSlot(id);
        if (slot == null) {
            return -1;
        }
        //使用物品
        switch (ItemManager.LookupItem(id).type) {
            case ItemType.FRUIT:
                EmoDataManager.Full += ((Fruit)ItemManager.LookupItem(id)).full;
                EmoDataManager.Enegy += ((Fruit)ItemManager.LookupItem(id)).enegy;
                EmoDataManager.Pressure += ((Fruit)ItemManager.LookupItem(id)).pressure;
                return ReduceItem(slot);
            case ItemType.FISH:
                EmoDataManager.Full += ((Fish)ItemManager.LookupItem(id)).full;
                EmoDataManager.Enegy += ((Fish)ItemManager.LookupItem(id)).enegy;
                EmoDataManager.Pressure += ((Fish)ItemManager.LookupItem(id)).pressure;
                return ReduceItem(slot);

            case ItemType.SAPLING:
                return ReduceItem(slot);
        }

        return -1;
    }

    /// <summary>
    /// 卖出物品
    /// </summary>
    public int SellItem(int id, int n = 1)
    {
        int result = ReduceItem(GetSlot(id), n);
        if (result >= 0) {
            EmoDataManager.Money += ItemManager.LookupItem(id).price;
            return result;
        }

        return result;
    }
    /// <summary>
    /// 买入物品
    /// </summary>
    public bool BuyItem(int id, int n=1)
    {
        if (ItemManager.LookupItem(id).price * n > EmoDataManager.Money) {
            Debug.Log("钱不够");
            return false;
        } else {
            AddItem(id, n);
            EmoDataManager.Money -= ItemManager.LookupItem(id).price * n;
            return true;

        }
    }

    /// <summary>
    /// 检索是否拥有某物，主要是床、车等非消耗品用
    /// </summary>
    public bool IsOwnItem(int itemid)
    {
        foreach (var slot in bagContent) {
            if (slot.itemId == itemid) {
                return true;
            }
        }
        return false;
    }
}

