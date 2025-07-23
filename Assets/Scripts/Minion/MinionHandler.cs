using System;
using UnityEngine;

public class MinionHandler : MonoBehaviour
{
    public static MinionHandler Instance { get; private set; }
    public Action<Minion> OnMinionSpawned;
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private Transform minion;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerStats playerStats;

    private float damage = 10f;

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
    private void Start()
    {
        UpgradeUI.Instance.OnDamageUpgraded += DamageUpgraded;
        GlyphsUI.Instance.OnLanceButtonClicked += DamageUpgradedBetter;
    }

    private void DamageUpgraded()
    {
        damage *= 1.2f;
    }
    private void DamageUpgradedBetter()
    {
        damage = 17.5f;
    }

    private void OnDestroy()
    {
        UpgradeUI.Instance.OnDamageUpgraded -= DamageUpgraded;
        GlyphsUI.Instance.OnLanceButtonClicked -= DamageUpgradedBetter;
    }


    int spawnIndex = 0;
    public void SpawnMinion()
    {
        if (playerStats.currentStamina < 10f)
            return;
        playerStats.currentStamina -= 10f;
        Transform newMinionTransform = Instantiate(minion, minionSpawnPoints[spawnIndex].position, Quaternion.identity);
        newMinionTransform.localScale = player.transform.localScale;
        Minion newMinion = newMinionTransform.GetComponent<Minion>();
        MinionAI minionAI = newMinion.GetComponent<MinionAI>();
        Debug.Log(damage);
        minionAI.damage = damage;
        spawnIndex++;
        spawnIndex %= minionSpawnPoints.Length;
        OnMinionSpawned?.Invoke(newMinion);
    }
}
