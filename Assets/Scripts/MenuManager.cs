using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD
=======
using UnityEngine.SceneManagement;
>>>>>>> origin/dev/Nelson
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

<<<<<<< HEAD
    private Resolution[] availableResolutions;
    private int pendingResolutionIndex;
    private bool pendingFullscreen;
    private float pendingVolume;
=======
    private List<Resolution> filteredResolutions = new List<Resolution>();
>>>>>>> origin/dev/Nelson

    void Start()
    {
        ShowMainMenu();
<<<<<<< HEAD
        PopulateResolutionDropdown();
        LoadSettingsIntoUI();
        SettingsManager.ApplySavedSettings();
=======
        PopulateResolutionDropdown(); // Esto debe ejecutarse antes
        SetupUIListeners();           // Luego conectamos eventos
        LoadSettingsIntoUI();        // Ahora cargamos
        ApplyCurrentSettings();      // Finalmente aplicamos
>>>>>>> origin/dev/Nelson
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
<<<<<<< HEAD
            {
                BackFromOptions();
            }
            else if (creditsPanel != null && creditsPanel.activeSelf)
            {
=======
                BackFromOptions();
            else if (creditsPanel != null && creditsPanel.activeSelf)
>>>>>>> origin/dev/Nelson
                ShowMainMenu();
        }
    }

<<<<<<< HEAD
=======
    private void SetupUIListeners()
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

>>>>>>> origin/dev/Nelson
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        LoadSettingsIntoUI();
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
<<<<<<< HEAD
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
=======
        SceneManager.LoadScene("Nivel1");
>>>>>>> origin/dev/Nelson
    }

    private void PopulateResolutionDropdown()
    {
<<<<<<< HEAD
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + "x" + availableResolutions[i].height + " (" + (availableResolutions[i].refreshRateRatio.numerator / availableResolutions[i].refreshRateRatio.denominator) + "Hz)";
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
    }

    private void LoadSettingsIntoUI()
    {
        int currentSystemResolutionIndex = 0;
        Resolution currentSystemResolution = Screen.currentResolution;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i].width == currentSystemResolution.width &&
                availableResolutions[i].height == currentSystemResolution.height &&
                availableResolutions[i].refreshRateRatio.numerator == currentSystemResolution.refreshRateRatio.numerator &&
                availableResolutions[i].refreshRateRatio.denominator == currentSystemResolution.refreshRateRatio.denominator)
            {
                currentSystemResolutionIndex = i;
                break;
            }
        }

        pendingResolutionIndex = PlayerPrefs.GetInt(SettingsManager.RESOLUTION_INDEX_KEY, currentSystemResolutionIndex);
        if (pendingResolutionIndex < 0 || pendingResolutionIndex >= availableResolutions.Length)
        {
            pendingResolutionIndex = currentSystemResolutionIndex;
        }
        resolutionDropdown.value = pendingResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        pendingFullscreen = PlayerPrefs.GetInt(SettingsManager.FULLSCREEN_KEY, 1) == 1;
        fullscreenToggle.isOn = pendingFullscreen;

        pendingVolume = PlayerPrefs.GetFloat(SettingsManager.VOLUME_KEY, 1f);
        volumeSlider.value = pendingVolume;
    }

    public void OnResolutionChanged(int index)
    {
        pendingResolutionIndex = index;
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        pendingFullscreen = isFullscreen;
    }

    public void OnVolumeChanged(float volume)
    {
        pendingVolume = volume;
    }
=======
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();
        List<string> options = new List<string>();

        int savedWidth = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        int matchedIndex = 0;

        foreach (Resolution res in Screen.resolutions)
        {
            // Solo agregamos si no hay ya una resolución con ese ancho y alto
            bool alreadyExists = filteredResolutions.Exists(r => r.width == res.width && r.height == res.height);
            if (!alreadyExists)
            {
                filteredResolutions.Add(res);
                options.Add(res.width + "x" + res.height);

                if (res.width == savedWidth && res.height == savedHeight)
                {
                    matchedIndex = filteredResolutions.Count - 1;
                }
            }
        }


        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = matchedIndex;
        resolutionDropdown.RefreshShownValue();

        // Guardamos el índice válido también
        PlayerPrefs.SetInt("resolutionIndex", matchedIndex);
    }

>>>>>>> origin/dev/Nelson

    private void LoadSettingsIntoUI()
    {
<<<<<<< HEAD
        SettingsManager.SaveSettings(pendingResolutionIndex, pendingFullscreen, pendingVolume);
        SettingsManager.ApplySavedSettings();
    }

    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        LoadSettingsIntoUI();
    }
}
=======
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        resolutionDropdown.value = Mathf.Clamp(resolutionIndex, 0, filteredResolutions.Count - 1);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
    }

    private void ApplyCurrentSettings()
    {
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        float volume = PlayerPrefs.GetFloat("volume", 1f);

        ApplyResolution(resolutionIndex, isFullscreen);
        Screen.fullScreen = isFullscreen;
        AudioListener.volume = volume;
    }

    public void OnResolutionChanged(int index)
    {
        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.SetInt("resolutionWidth", filteredResolutions[index].width);
        PlayerPrefs.SetInt("resolutionHeight", filteredResolutions[index].height);
        PlayerPrefs.Save();
        ApplyResolution(index, fullscreenToggle.isOn);
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
        if (index >= 0 && index < filteredResolutions.Count)
        {
            Resolution selected = filteredResolutions[index];
            Screen.SetResolution(selected.width, selected.height, fullscreen);
        }
    }

    public void BackFromOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        LoadSettingsIntoUI();
    }
}
>>>>>>> origin/dev/Nelson
