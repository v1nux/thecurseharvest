using System.Collections.Generic;
using UnityEngine;

public class PropsSpawner : MonoBehaviour
{
    [Header("Spawn Zone")]
    [SerializeField] private PolygonCollider2D spawnZone;
    [SerializeField] private string areaId = "Forest";


    [Header("Props")]
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] stonePrefabs;
    [SerializeField] private GameObject[] vegetationPrefabs;

    [Header("Density")]
    [Range(0f, 1f)]
    [SerializeField] private float treeDensity = 0.3f;

    [Range(0f, 1f)]
    [SerializeField] private float stoneDensity = 0.2f;

    [Range(0f, 1f)]
    [SerializeField] private float vegetationDensity = 0.15f;

    [SerializeField] private int maxTreeCount = 40;
    [SerializeField] private int maxStoneCount = 35;
    [SerializeField] private int maxVegetationCount = 25;

    [Header("Spacing")]
    [SerializeField] private float minDistanceBetweenProps = 1.5f;
    [SerializeField] private int maxAttempts = 30;

    [Header("House Exclusion")]
    [SerializeField] private Transform houseTransform;
    [SerializeField] private float houseRadius = 4f;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnStart = true;

    [HideInInspector] public bool hasSpawned = false;

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<GameObject> spawnedProps = new List<GameObject>();
    private List<PropSaveData> propDataList = new List<PropSaveData>();

    void Start()
    {
        if (spawnOnStart && !hasSpawned)
            SpawnNewForest();
    }

    public void SpawnNewForest()
    {
        if (hasSpawned) return;

        int treeCount = Mathf.RoundToInt(maxTreeCount * treeDensity);
        int stoneCount = Mathf.RoundToInt(maxStoneCount * stoneDensity);
        int vegCount = Mathf.RoundToInt(maxVegetationCount * vegetationDensity);

        SpawnGroup(treePrefabs, treeCount, "Tree");
        SpawnGroup(stonePrefabs, stoneCount, "Stone");
        SpawnGroup(vegetationPrefabs, vegCount, "Vegetation");

        hasSpawned = true;
    }

    void SpawnGroup(GameObject[] prefabs, int count, string propType)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        int spawned = 0;
        int attempts = 0;

        while (spawned < count && attempts < count * maxAttempts)
        {
            attempts++;

            Vector3 randomPos = GetRandomPointInPolygon();

            if (!spawnZone.OverlapPoint(randomPos)) continue;
            if (!IsFarEnough(randomPos)) continue;
            if (IsTooCloseToHouse(randomPos)) continue;

            int prefabIndex = Random.Range(0, prefabs.Length);
            GameObject prefab = prefabs[prefabIndex];

            string id = System.Guid.NewGuid().ToString();

            GameObject prop = Instantiate(prefab, randomPos, Quaternion.identity, transform);

            HarvestableProp harvestable = prop.GetComponent<HarvestableProp>();
            if (harvestable != null)
                harvestable.Init(id, this);

            PropSaveData data = new PropSaveData()
            {
                propId = id,
                areaId = areaId,
                propType = propType,
                prefabIndex = prefabIndex,
                position = randomPos,
                destroyed = false
            };

            propDataList.Add(data);
            spawnedPositions.Add(randomPos);
            spawnedProps.Add(prop);

            spawned++;
        }
    }

    Vector3 GetRandomPointInPolygon()
    {
        Bounds bounds = spawnZone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(x, y, 0f);
    }

    bool IsFarEnough(Vector3 pos)
    {
        foreach (Vector3 existing in spawnedPositions)
        {
            if (Vector3.Distance(pos, existing) < minDistanceBetweenProps)
                return false;
        }

        return true;
    }

    bool IsTooCloseToHouse(Vector3 pos)
    {
        if (houseTransform == null) return false;

        return Vector3.Distance(pos, houseTransform.position) < houseRadius;
    }

    public void MarkDestroyed(string propId)
    {
        foreach (PropSaveData data in propDataList)
        {
            if (data.propId == propId)
            {
                data.destroyed = true;
                break;
            }
        }
    }

    public List<PropSaveData> GetSaveData()
    {
        return propDataList;
    }

    public void LoadFromSave(List<PropSaveData> savedData)
    {
        ClearSpawnedProps();

        List<PropSaveData> myAreaData = new List<PropSaveData>();

        if (savedData != null)
        {
            foreach (PropSaveData data in savedData)
            {
                if (data.areaId == areaId)
                    myAreaData.Add(data);
            }
        }

        // If this area has no saved props yet, generate new props
        if (myAreaData.Count == 0)
        {
            SpawnNewForest();
            return;
        }

        propDataList = new List<PropSaveData>(myAreaData);

        foreach (PropSaveData data in propDataList)
        {
            if (data.destroyed) continue;

            GameObject prefab = GetPrefab(data.propType, data.prefabIndex);
            if (prefab == null) continue;

            GameObject prop = Instantiate(prefab, data.position, Quaternion.identity, transform);

            HarvestableProp harvestable = prop.GetComponent<HarvestableProp>();
            if (harvestable != null)
                harvestable.Init(data.propId, this);

            spawnedProps.Add(prop);
            spawnedPositions.Add(data.position);
        }

        hasSpawned = true;
    }

    GameObject GetPrefab(string propType, int index)
    {
        if (propType == "Tree")
        {
            if (index >= 0 && index < treePrefabs.Length)
                return treePrefabs[index];
        }

        if (propType == "Stone")
        {
            if (index >= 0 && index < stonePrefabs.Length)
                return stonePrefabs[index];
        }

        if (propType == "Vegetation")
        {
            if (index >= 0 && index < vegetationPrefabs.Length)
                return vegetationPrefabs[index];
        }

        return null;
    }

    void ClearSpawnedProps()
    {
        foreach (GameObject prop in spawnedProps)
        {
            if (prop != null)
                Destroy(prop);
        }

        spawnedProps.Clear();
        spawnedPositions.Clear();
        propDataList.Clear();
        hasSpawned = false;
    }

    public void DespawnAll()
    {
        foreach (GameObject prop in spawnedProps)
        {
            if (prop != null)
                prop.SetActive(false);
        }
    }

    public void ShowAll()
    {
        foreach (GameObject prop in spawnedProps)
        {
            if (prop != null)
                prop.SetActive(true);
        }
    }
}