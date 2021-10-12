using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//对应UI上的slot
public class BagSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //等待BagUI内动态加载
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    public Image img;
    public Text text;
    public int itemId;
    public Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }


}
