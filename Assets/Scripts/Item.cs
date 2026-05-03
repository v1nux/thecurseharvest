using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public ItemType itemType = ItemType.Misc;

    [Header("Stacking")]
    public bool isStackable = true;
    public int amount = 1;
    public int maxStack = 99;

    [Header("UI")]
    public TMP_Text amountText;

    public enum ItemType
    {
        Misc,
        Tool,
        Seed,
        Food,
        Resource,
        Weapon
    }

    public CropData cropData;

    void Awake()
    {
        if (amountText == null)
            amountText = GetComponentInChildren<TMP_Text>();

        UpdateAmountText();
    }

    public void UpdateAmountText()
    {
        if (amountText == null) return;

        if (amount > 1)
            amountText.text = amount.ToString();
        else
            amountText.text = "";
    }

    public virtual void UseItem()
    {
        Debug.Log("Using item: " + Name);
    }
}