using UnityEngine;

public class HarvestableProp : MonoBehaviour
{
    public string propId;

    [SerializeField] private int hitsToBreak = 3;

    private int currentHits = 0;
    private PropsSpawner ownerSpawner;

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

        Destroy(gameObject);
    }
}