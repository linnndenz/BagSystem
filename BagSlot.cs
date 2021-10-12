using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//��ӦUI�ϵ�slot
public class BagSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //�ȴ�BagUI�ڶ�̬����
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
