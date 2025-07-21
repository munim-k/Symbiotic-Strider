using System;
using UnityEngine;

public class MinionHandler : MonoBehaviour
{
    public static MinionHandler Instance { get; private set; }
    public Action<Minion> OnMinionSpawned;
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private Transform minion;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    int spawnIndex = 0;
    public void SpawnMinion()
    {
        Transform newMinionTransform = Instantiate(minion, minionSpawnPoints[spawnIndex].position, Quaternion.identity);
        Minion newMinion = newMinionTransform.GetComponent<Minion>();
        spawnIndex++;
        spawnIndex %= minionSpawnPoints.Length;
        OnMinionSpawned?.Invoke(newMinion);
    }
}
