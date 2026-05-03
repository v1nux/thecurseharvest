using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Slot originalSlot;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalSlot = originalParent.GetComponent<Slot>();

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

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

        Slot dropSlot = eventData.pointerEnter?.GetComponentInParent<Slot>();

        if (dropSlot == null)
        {
            ReturnToOriginalSlot();
            return;
        }

        // empty slot
        if (dropSlot.currentItem == null)
        {
            originalSlot.currentItem = null;

            transform.SetParent(dropSlot.transform);
            FitItemToSlot(transform);

            dropSlot.currentItem = gameObject;
            return;
        }

        // if same stackable item, merge
        Item draggedItem = GetComponent<Item>();
        Item targetItem = dropSlot.currentItem.GetComponent<Item>();

        if (draggedItem != null && targetItem != null &&
            draggedItem.ID == targetItem.ID &&
            draggedItem.isStackable)
        {
            int space = targetItem.maxStack - targetItem.amount;
            int moveAmount = Mathf.Min(space, draggedItem.amount);

            targetItem.amount += moveAmount;
            draggedItem.amount -= moveAmount;

            targetItem.UpdateAmountText();
            draggedItem.UpdateAmountText();

            if (draggedItem.amount <= 0)
            {
                originalSlot.currentItem = null;
                Destroy(gameObject);
            }
            else
            {
                ReturnToOriginalSlot();
            }

            return;
        }

        // swap different items
        GameObject otherItem = dropSlot.currentItem;

        otherItem.transform.SetParent(originalSlot.transform);
        FitItemToSlot(otherItem.transform);

        transform.SetParent(dropSlot.transform);
        FitItemToSlot(transform);

        originalSlot.currentItem = otherItem;
        dropSlot.currentItem = gameObject;
    }

    void ReturnToOriginalSlot()
    {
        transform.SetParent(originalParent);
        FitItemToSlot(transform);
    }

    void FitItemToSlot(Transform itemTransform)
    {
        RectTransform rt = itemTransform.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }
        else
        {
            itemTransform.localPosition = Vector3.zero;
            itemTransform.localScale = Vector3.one;
            itemTransform.localRotation = Quaternion.identity;
        }

        TMPro.TMP_Text text = itemTransform.GetComponentInChildren<TMPro.TMP_Text>();

        if (text != null)
        {
            text.raycastTarget = false;
            text.enableAutoSizing = false;
            text.fontSize = 10;
            text.alignment = TMPro.TextAlignmentOptions.BottomRight;

            RectTransform textRT = text.GetComponent<RectTransform>();
            textRT.anchorMin = new Vector2(1, 0);
            textRT.anchorMax = new Vector2(1, 0);
            textRT.pivot = new Vector2(1, 0);
            textRT.sizeDelta = new Vector2(20, 20);
            textRT.anchoredPosition = new Vector2(-3, 3);
            textRT.localScale = Vector3.one;
        }

        Item item = itemTransform.GetComponent<Item>();
        if (item != null)
        {
            item.UpdateAmountText();
        }
    }
}