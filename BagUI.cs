using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagUI : UIBase
{

    [Header("背包")]
    //背包页相关
    public Transform pageParent;
    private Transform[] pages;
    private int page;
    //显示物品的内容页
    private Transform[] contents;
    //toggle相关
    private UIManager uiManager;

    //背包格子
    private BagSlot[][] slots;
    //物品详情
    public GameObject itemInfo;
    public Canvas canvas;//rect适配

    void Start()
    {
        //初始化背包格子
        slots = new BagSlot[pageParent.childCount][];
        pages = new Transform[pageParent.childCount];
        contents = new Transform[pageParent.childCount];
        for (int i = 0; i < pageParent.childCount; i++) {
            pages[i] = pageParent.GetChild(i);
            contents[i] = pages[i].GetChild(0).GetChild(0);

            slots[i] = new BagSlot[contents[i].childCount];
            slots[i] = contents[i].GetComponentsInChildren<BagSlot>();
            for (int j = 0; j < slots[i].Length; j++) {
                slots[i][j].btn.onClick.AddListener(() => UseItem());
            }
        }

        //组件获取
        uiManager = GetComponentInParent<UIManager>();
        itemInfoRect = itemInfo.GetComponent<RectTransform>();
        itemInfo_itemName = itemInfo.transform.Find("Text_ItemName").GetComponent<Text>();
        itemInfo_intro = itemInfo.transform.Find("Text_Intro").GetComponent<Text>();

    }

    RectTransform itemInfoRect;//物品介绍
    void Update()
    {
        //单个物品介绍信息跟随鼠标
        if (itemInfo.activeSelf) {
            itemInfo.transform.position = Input.mousePosition + new Vector3(itemInfoRect.rect.width * canvas.scaleFactor / 2, -itemInfoRect.rect.height * canvas.scaleFactor / 2, 0);
        }
    }

    /// <summary>
    /// 打开背包时加载
    /// </summary>
    int cnt0, cnt1, cnt2, cnt3;//每页的计数
    protected override void OnOpenPanel()
    {
        UpdateBagDisplay();
    }
    /// <summary>
    /// 更新背包ui，将数据同步到显示
    /// </summary>
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
                    slots[1][cnt1].btn.interactable = true;
                    slots[1][cnt1].img.sprite = ItemManager.LookupItem(item.itemId).ico;
                    slots[1][cnt1].text.text = item.num.ToString();
                    slots[1][cnt1].itemId = item.itemId;

                    temp = cnt1;
                    slots[1][cnt1].onPointerEnter.AddListener(() => PointerEnter(1, temp));
                    slots[1][cnt1].onPointerExit.AddListener(() => PointerExit());

                    cnt1++;
                    break;
                case ItemType.SUNDRY:
                case ItemType.QUESTNEED:
                    //slots[1][cnt1].btn.interactable = true;
                    slots[2][cnt2].img.sprite = ItemManager.LookupItem(item.itemId).ico;
                    slots[2][cnt2].text.text = item.num.ToString();
                    slots[2][cnt2].itemId = item.itemId;

                    temp = cnt2;
                    slots[2][cnt2].onPointerEnter.AddListener(() => PointerEnter(2, temp));
                    slots[2][cnt2].onPointerExit.AddListener(() => PointerExit());

                    cnt2++;
                    break;

                case ItemType.UNCONSUMABLE:
                    //slots[2][cnt2].btn.interactable = true;
                    slots[3][cnt3].img.sprite = ItemManager.LookupItem(item.itemId).ico;
                    slots[3][cnt3].text.text = item.num.ToString();
                    slots[3][cnt3].itemId = item.itemId;

                    temp = cnt3;
                    slots[3][cnt3].onPointerEnter.AddListener(() => PointerEnter(3, temp));
                    slots[3][cnt3].onPointerExit.AddListener(() => PointerExit());

                    cnt3++;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 使用物品
    /// </summary>
    private int currItemIndex;//当前物品，在该页的序号
    public void UseItem()
    {
        switch (ItemManager.LookupItem(slots[page][currItemIndex].itemId).type) {
            case ItemType.SAPLING:
                yardManager.Seed(slots[page][currItemIndex].itemId);
                //CloseItemDetail();
                uiManager.CloseAllUIPanel();
                return;
        }
        EmoDataManager.BagController.UseItem(slots[page][currItemIndex].itemId);
        UpdateBagDisplay();
        //itemDetail.gameObject.SetActive(false);
    }

    /// <summary>
    /// 切换物品内容页
    /// </summary>
    int currToggleIndex;//当前背包页的toggle
    public void ChangePageCurrToggle(int index) { currToggleIndex = index; }
    public void ChangePage(bool isOn)
    {
        if (isOn) {
            try {
                page = currToggleIndex;
                for (int i = 0; i < pages.Length; i++) {
                    if (i == currToggleIndex) {
                        pages[i].gameObject.SetActive(true);
                    } else {
                        pages[i].gameObject.SetActive(false);
                    }
                }
            } catch (System.Exception) { }
            //print(currToggleIndex);
        }
    }

    //hoverslot函数
    Text itemInfo_itemName;
    Text itemInfo_intro;
    Item tempitem;
    public void PointerEnter(int p, int index)
    {
        currItemIndex = index;
        tempitem = ItemManager.LookupItem(slots[p][index].itemId);
        itemInfo_itemName.text = tempitem.name;
        itemInfo_intro.text = tempitem.description;

        itemInfo.SetActive(true);
    }

    public void PointerExit()
    {
        itemInfo.SetActive(false);
    }
}
