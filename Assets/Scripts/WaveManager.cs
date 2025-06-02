using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject enemyPrefab;
        public int count;
        public float spawnRate;
    }

    public List<Wave> waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public int enemiesPerLevelIncrease = 5; // Cuántos enemigos extra se añaden por nivel

    private int nextWave = 0;
    private int enemiesRemaining;
    private int totalEnemiesKilledInLevel = 0;
    private int currentLevel = 1;

    public static WaveManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one WaveManager in scene!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        enemiesRemaining = 0;
        StartNextWave();
    }

    void Update()
    {
        if (enemiesRemaining <= 0 && nextWave > 0)
        {
            // Todos los enemigos de la ola actual han sido eliminados
            if (nextWave < waves.Count)
            {
                StartCoroutine(WaveCompleted());
            }
            else
            {
                // Todas las olas completadas para el nivel actual
                CheckForLevelCompletion();
            }
        }
    }

    void StartNextWave()
    {
        if (nextWave < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[nextWave]));
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        enemiesRemaining = _wave.count;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemyPrefab);
            yield return new WaitForSeconds(1f / _wave.spawnRate);
        }
        nextWave++;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
            return;
        }
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }

    public void EnemyDied()
    {
        enemiesRemaining--;
        totalEnemiesKilledInLevel++;
        Debug.Log("Enemies remaining: " + enemiesRemaining);
    }

    IEnumerator WaveCompleted()
    {
        Debug.Log("Wave Completed! Next wave in " + timeBetweenWaves + " seconds.");
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }

    void CheckForLevelCompletion()
    {
        Debug.Log("All waves completed for current level. Total enemies killed: " + totalEnemiesKilledInLevel);

        if (currentLevel < 3) // Si hay más niveles para avanzar
        {
            currentLevel++;
            Debug.Log("Advancing to Level " + currentLevel);
            // Guarda el estado si es necesario, por ejemplo, el número de nivel
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            // Reinicia la cuenta de enemigos y la ola para el nuevo nivel
            totalEnemiesKilledInLevel = 0;
            nextWave = 0; // Reinicia las olas para el nuevo nivel
            IncreaseEnemyCountForNextLevel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena para el nuevo nivel
        }
        else
        {
            Debug.Log("All levels completed! You win!");
            // Puedes cargar una escena de victoria o mostrar un mensaje
            SceneManager.LoadScene("VictoryScene"); // Asume que tienes una escena de victoria
        }
    }

    void IncreaseEnemyCountForNextLevel()
    {
        foreach (Wave wave in waves)
        {
            wave.count += enemiesPerLevelIncrease;
        }
    }
}

