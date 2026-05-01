using UnityEngine;

public class HarvestableProp : MonoBehaviour
{
    public string propId;

    private PropsSpawner ownerSpawner;

    public void Init(string id, PropsSpawner spawner)
    {
        propId = id;
        ownerSpawner = spawner;
    }

    public void DestroyProp()
    {
        if (ownerSpawner != null)
            ownerSpawner.MarkDestroyed(propId);

        Destroy(gameObject);
    }
}