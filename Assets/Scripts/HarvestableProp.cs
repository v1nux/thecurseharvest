using UnityEngine;

public class HarvestableProp : MonoBehaviour
{
    public string propId;

    [SerializeField] private int hitsToBreak = 3;
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private int dropAmount = 2;

    private int currentHits = 0;
    private PropsSpawner ownerSpawner;

    void DropItems()
    {
        if (dropItemPrefab == null) return;

        for (int i = 0; i < dropAmount; i++)
        {
            Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;

            Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void Init(string id, PropsSpawner spawner)
    {
        propId = id;
        ownerSpawner = spawner;
    }

    public void HitProp()
    {
        currentHits++;

        Debug.Log("Hit prop: " + currentHits + "/" + hitsToBreak);

        if (currentHits >= hitsToBreak)
            DestroyProp();
    }

    public void DestroyProp()
    {
        if (ownerSpawner != null)
            ownerSpawner.MarkDestroyed(propId);

        DropItems();

        Destroy(gameObject);
    }
}