using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private Resolution[] availableResolutions;
    private int pendingResolutionIndex;
    private bool pendingFullscreen;
    private float pendingVolume;

    void Start()
    {
        ShowMainMenu();
        PopulateResolutionDropdown();
        LoadSettingsIntoUI();
        SettingsManager.ApplySavedSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
            {
                BackFromOptions();
            }
            else if (creditsPanel != null && creditsPanel.activeSelf)
            {
                ShowMainMenu();
            }
        }
    }

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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Nivel1");
    }

    private void PopulateResolutionDropdown()
    {
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

    public void ApplyChanges()
    {
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