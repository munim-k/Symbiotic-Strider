using System;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public Action<Enemy, float> OnEnemySpawned;
    public Action<float, float> OnTimerChanged;
    [SerializeField] private Enemy aggressiveEnemyPrefab;
    [SerializeField] private Enemy defensiveEnemyPrefab;
    [SerializeField] private Enemy hybridEnemyPrefab;
    [SerializeField] private float enemySpawnTime = 15f;
    [SerializeField] private float value = 0.25f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float numberOfEnemies = 2f;
    private float updatePerSecond = 0.01f;
    private float enemySpawnTimer;
    private bool shouldStartSpawning = false;
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
        enemySpawnTimer = enemySpawnTime;
        GlyphsUI.Instance.OnCometButtonClicked += StartSpawning;
        GlyphsUI.Instance.OnFortressButtonClicked += StartSpawning;
        GlyphsUI.Instance.OnLanceButtonClicked += StartSpawning;
        GlyphsUI.Instance.OnMountainButtonClicked += StartSpawning;
    }
    private void OnDestroy()
    {
        GlyphsUI.Instance.OnCometButtonClicked -= StartSpawning;
        GlyphsUI.Instance.OnFortressButtonClicked -= StartSpawning;
        GlyphsUI.Instance.OnLanceButtonClicked -= StartSpawning;
        GlyphsUI.Instance.OnMountainButtonClicked -= StartSpawning;
    }

    private void StartSpawning()
    {
        shouldStartSpawning = true;
    }

    private void Update()
    {
        if (!shouldStartSpawning)
            return;
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= enemySpawnTime)
        {
            enemySpawnTimer = 0f;
            //spawn enemies
            for (int i = 0; i < (int)numberOfEnemies; i++)
            {
                //get random x and z position based on range
                float randomX = UnityEngine.Random.Range(-spawnRadius, spawnRadius);
                float randomZ = UnityEngine.Random.Range(-spawnRadius, spawnRadius);

                int random = UnityEngine.Random.Range(0, 3);
                Enemy enemy = Instantiate(random == 0 ? aggressiveEnemyPrefab : random == 1 ? defensiveEnemyPrefab : hybridEnemyPrefab, new Vector3(randomX, transform.position.y, randomZ), Quaternion.identity);

                OnEnemySpawned?.Invoke(enemy, value);
            }
        }
        value += updatePerSecond * Time.deltaTime;
        if (spawnRadius < 30f)
            spawnRadius += updatePerSecond * Time.deltaTime;
        if (numberOfEnemies <= 5f)
            numberOfEnemies += updatePerSecond * Time.deltaTime;

        OnTimerChanged?.Invoke(enemySpawnTimer, enemySpawnTime);
    }
}
