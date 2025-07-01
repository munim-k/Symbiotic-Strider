using UnityEngine;

public class MinionHandler : MonoBehaviour
{
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private Transform minion;
    int spawnIndex = 0;
    public void SpawnMinion() {
        Instantiate(minion,minionSpawnPoints[spawnIndex].position,Quaternion.identity);
        spawnIndex++;
        spawnIndex%= minionSpawnPoints.Length;
    }
}
