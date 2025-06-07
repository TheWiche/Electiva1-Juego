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
        public int baseCount;
        public float spawnRate;
        public Transform[] spawnPoints; // <<--- Puntos de spawn específicos para esta wave
    }

    public List<Wave> waves;
    public float timeBetweenWaves = 5f;

    private int nextWave = 0;
    private int enemiesRemaining;
    private bool levelCompleted = false;

    public static WaveManager instance;

    [Header("UI")]
    public GameObject congratulationsPanel;
    public GameObject nextButton;
    public GameObject exitButton;

    private enum Difficulty { Facil = 0, Normal = 1, Dificil = 2 }
    private Difficulty selectedDifficulty;
    private bool isWaitingForNextWave = false;

    // Propiedad pública para que otros scripts sepan si el nivel ya terminó
    public bool IsGameCompleted => levelCompleted;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one WaveManager in scene!");
            return;
        }
        instance = this;

        selectedDifficulty = (Difficulty)PlayerPrefs.GetInt("GameDifficulty", 1);
    }

    void Start()
    {
        enemiesRemaining = 0;
        StartNextWave();
    }

    void Update()
    {
        if (!levelCompleted && enemiesRemaining <= 0 && !isWaitingForNextWave)
        {
            if (nextWave < waves.Count)
            {
                StartCoroutine(WaveCompleted());
            }
            else
            {
                ShowCongratulationsPanel();
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

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Spawning Wave: " + wave.name);

        int enemyCount = GetEnemyCountByDifficulty(wave.baseCount);
        enemiesRemaining = enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefab, wave.spawnPoints);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        nextWave++;
    }

    void SpawnEnemy(GameObject enemy, Transform[] spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points set for this wave.");
            return;
        }

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, sp.position, sp.rotation);
    }

    public void EnemyDied()
    {
        enemiesRemaining--;
        Debug.Log("Enemies remaining: " + enemiesRemaining);
    }

    IEnumerator WaveCompleted()
    {
        isWaitingForNextWave = true;
        Debug.Log("Wave Completed! Next wave in " + timeBetweenWaves + " seconds.");
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
        isWaitingForNextWave = false;
    }

    void ShowCongratulationsPanel()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        StartCoroutine(ShowCongratulationsPanelDelayed());
    }

    IEnumerator ShowCongratulationsPanelDelayed()
    {
        // Espera 3 segundos en tiempo real para que las animaciones se terminen
        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 0f;

        if (congratulationsPanel != null)
        {
            congratulationsPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (nextButton != null) nextButton.SetActive(true);
            if (exitButton != null) exitButton.SetActive(true);
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Nivel2");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menú Juego");
    }

    private int GetEnemyCountByDifficulty(int baseCount)
    {
        switch (selectedDifficulty)
        {
            case Difficulty.Facil: return Mathf.RoundToInt(baseCount * 0.5f);
            case Difficulty.Dificil: return Mathf.RoundToInt(baseCount * 1.5f);
            default: return baseCount;
        }
    }
}
