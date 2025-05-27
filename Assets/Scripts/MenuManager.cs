using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel; // Opcional, si lo usas

    [Header("Options Controls")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private Resolution[] resolutions;
    private int pendingResolutionIndex;
    private bool pendingFullscreen;
    private float pendingVolume;

    void Start()
    {
        // Setup menú principal
        ShowMainMenu();

        // Setup resoluciones en dropdown
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
        // Permitir volver al menú principal con ESC desde opciones o créditos
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf || (creditsPanel != null && creditsPanel.activeSelf))
            {
                ShowMainMenu();
            }
        }
    }

    #region Menu Navigation

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
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
        Debug.Log("Aplicar cambios llamado");
        PlayerPrefs.SetInt("resolutionIndex", pendingResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("volume", pendingVolume);
        PlayerPrefs.Save();

        Resolution res = resolutions[pendingResolutionIndex];
        Screen.fullScreenMode = pendingFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        AudioListener.volume = pendingVolume;
    }


    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    #endregion
}
