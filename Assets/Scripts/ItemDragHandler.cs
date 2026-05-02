using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform orginalParent;
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        orginalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        Slot originalSlot = orginalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            if (dropSlot.currentItem == null)
            {
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;

                if (originalSlot != null)
                    originalSlot.currentItem = null;
            }
            else
            {
                GameObject tempItem = dropSlot.currentItem;

                tempItem.transform.SetParent(originalSlot.transform);
                tempItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
            }
        }
        else
        {
            transform.SetParent(orginalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}