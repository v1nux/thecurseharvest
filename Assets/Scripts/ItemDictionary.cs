using UnityEngine;
using System.Collections.Generic;

public class ItemDictionary : MonoBehaviour
{
    public List<GameObject> itemPrefabs;
    private Dictionary<int, GameObject> itemsDictionary;

    void Awake()
    {
        itemsDictionary = new Dictionary<int, GameObject>();

        foreach (GameObject prefab in itemPrefabs)
        {
            if (prefab == null) continue;

            Item item = prefab.GetComponent<Item>();

            if (item != null)
            {
                if (!itemsDictionary.ContainsKey(item.ID))
                    itemsDictionary.Add(item.ID, prefab);
                else
                    Debug.LogWarning("Duplicate item ID: " + item.ID);
            }
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemsDictionary.TryGetValue(itemID, out GameObject prefab);

        if (prefab == null)
            Debug.LogWarning("Item with ID " + itemID + " not found in dictionary.");

        return prefab;
    }
}