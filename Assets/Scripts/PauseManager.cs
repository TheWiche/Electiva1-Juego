using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenu;
    public GameObject optionsPanel;

    [Header("Pause Buttons")]
    public Button resumeButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Options UI")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private bool isPaused = false;
    private bool inOptions = false;

    private Resolution[] resolutions;

    [Header("Other UI References")]
    public GameObject congratulationsPanel;  // asignar en inspector el panel de felicitaciones

    void Start()
    {
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        optionsButton.onClick.AddListener(ShowOptions);
        quitButton.onClick.AddListener(QuitGame);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetupUIListeners();
        PopulateResolutionDropdown();
        LoadSettingsIntoUI();
        ApplyCurrentSettings();
    }

    void Update()
    {
        // Si el panel de felicitaciones está activo, no permitir abrir pausa
        if (WaveManager.instance != null && WaveManager.instance.IsGameCompleted)
        return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                if (inOptions)
                    BackFromOptions();
                else
                    ResumeGame();
            }
        }
    }


    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        inOptions = false;
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel1");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menú Juego");
    }

    public void ShowOptions()
    {
        inOptions = true;
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(true);
        LoadSettingsIntoUI();
    }

    public void BackFromOptions()
    {
        inOptions = false;
        optionsPanel.SetActive(false);
        pauseMenu.SetActive(true);
        LoadSettingsIntoUI();
    }

    #region Settings Management

    private void SetupUIListeners()
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        int savedWidth = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = res.width + "x" + res.height;
            options.Add(option);

            if (res.width == savedWidth && res.height == savedHeight)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadSettingsIntoUI()
    {
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", 0);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
    }

    private void ApplyCurrentSettings()
    {
        OnResolutionChanged(PlayerPrefs.GetInt("resolutionIndex", 0));
        OnFullscreenChanged(fullscreenToggle.isOn);
        OnVolumeChanged(volumeSlider.value);
    }

    public void OnResolutionChanged(int index)
    {
        Resolution selected = resolutions[index];
        Screen.SetResolution(selected.width, selected.height, fullscreenToggle.isOn);

        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.SetInt("resolutionWidth", selected.width);
        PlayerPrefs.SetInt("resolutionHeight", selected.height);
        PlayerPrefs.Save();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    #endregion
}
