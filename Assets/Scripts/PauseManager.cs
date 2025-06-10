using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    [Header("Other UI References")]
    public GameObject congratulationsPanel;

    private List<Resolution> filteredResolutions = new List<Resolution>();

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
        if (WaveManager.instance != null && WaveManager.instance.IsGameCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else if (inOptions) BackFromOptions();
            else ResumeGame();
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
        filteredResolutions.Clear();
        List<string> options = new List<string>();

        int savedWidth  = PlayerPrefs.GetInt("resolutionWidth",  Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);

        int matchedIndex = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            var res = Screen.resolutions[i];
            if (!filteredResolutions.Exists(r => r.width == res.width && r.height == res.height))
            {
                filteredResolutions.Add(res);
                options.Add(res.width + "x" + res.height);

                if (res.width == savedWidth && res.height == savedHeight)
                    matchedIndex = filteredResolutions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        // No guardamos aquí: solo inicializamos visualmente
        resolutionDropdown.value = matchedIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadSettingsIntoUI()
    {
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        resolutionDropdown.value        = Mathf.Clamp(resolutionIndex, 0, filteredResolutions.Count - 1);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn           = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        volumeSlider.value              = PlayerPrefs.GetFloat("volume", 1f);
    }

    private void ApplyCurrentSettings()
    {
        int  resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        bool isFullscreen    = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        float volume         = PlayerPrefs.GetFloat("volume", 1f);

        ApplyResolution(resolutionIndex, isFullscreen);
        Screen.fullScreen = isFullscreen;
        AudioListener.volume = volume;
    }

    public void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= filteredResolutions.Count) return;

        var selected = filteredResolutions[index];
        Screen.SetResolution(selected.width, selected.height, fullscreenToggle.isOn);

        PlayerPrefs.SetInt("resolutionIndex",  index);
        PlayerPrefs.SetInt("resolutionWidth",  selected.width);
        PlayerPrefs.SetInt("resolutionHeight", selected.height);
        PlayerPrefs.Save();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Screen.fullScreen = isFullscreen;
        ApplyResolution(resolutionDropdown.value, isFullscreen);
    }

    public void OnVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
        AudioListener.volume = volume;
    }

    private void ApplyResolution(int index, bool fullscreen)
    {
        if (index < 0 || index >= filteredResolutions.Count) return;
        var sel = filteredResolutions[index];
        Screen.SetResolution(sel.width, sel.height, fullscreen);
    }

    #endregion
}
