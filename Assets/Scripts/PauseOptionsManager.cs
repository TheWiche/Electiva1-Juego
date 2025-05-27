using UnityEngine;
using UnityEngine.UI;

public class PauseOptionsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenu;
    public GameObject optionsPanel;

    [Header("Pause Buttons")]
    public Button resumeButton;
    public Button quitButton;
    public Button optionsButton;

    [Header("Options Controls")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private bool isPaused = false;

    private Resolution[] resolutions;
    private int pendingResolutionIndex;
    private bool pendingFullscreen;
    private float pendingVolume;

    void Start()
    {
        // Aplica la configuración guardada al iniciar
        SettingsManager.ApplySavedSettings();

        // Inicializar menú pausa
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);

        // Setup botones
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        optionsButton.onClick.AddListener(ShowOptions);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Setup opciones resolución
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        LoadSettingsFromPrefs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
            {
                // Si estoy en options, vuelvo a pausa, si no, reanudo
                if (optionsPanel.activeSelf)
                    BackFromOptions();
                else
                    ResumeGame();
            }
        }
    }

    #region Pause Menu

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        optionsPanel.SetActive(false);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menú Juego");
    }

    public void ShowOptions()
    {
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(true);
        LoadSettingsFromPrefs();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel1");
    }

    #endregion

    #region Options Menu

    private void LoadSettingsFromPrefs()
    {
        pendingResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        resolutionDropdown.value = pendingResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        pendingFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = pendingFullscreen;

        pendingVolume = PlayerPrefs.GetFloat("volume", 1f);
        volumeSlider.value = pendingVolume;
    }

    public void OnResolutionChanged(int index) => pendingResolutionIndex = index;
    public void OnFullscreenChanged(bool isFullscreen) => pendingFullscreen = isFullscreen;
    public void OnVolumeChanged(float volume) => pendingVolume = volume;

    public void ApplyChanges()
    {
        PlayerPrefs.SetInt("resolutionIndex", pendingResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("volume", pendingVolume);
        PlayerPrefs.Save();

        SettingsManager.ApplySavedSettings();
    }

    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }

    #endregion
}
