using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    [Header("Audio")]
    public AudioMixer mainMixer;
    [Header("Volume Sliders")]
    public Slider musicVolumeSlider = null;
    public Slider sfxVolumeSlider = null;
    [Header("Dificultad")]
    public Toggle toggleFacil;
    public Toggle toggleNormal;
    public Toggle toggleDificil;


    private List<Resolution> filteredResolutions = new List<Resolution>();

    void Start()
    {
        ShowMainMenu();
        PopulateResolutionDropdown(); // Esto debe ejecutarse antes
        SetupUIListeners();           // Luego conectamos eventos
        LoadSettingsIntoUI();        // Ahora cargamos
        ApplyCurrentSettings();      // Finalmente aplicamos
        SetupDifficultyToggles();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
                BackFromOptions();
            else if (creditsPanel != null && creditsPanel.activeSelf)
                ShowMainMenu();
        }
    }

    private void SetupUIListeners()
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged); 
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
        SceneManager.LoadScene("Nivel1");
    }

    private void PopulateResolutionDropdown()
    {
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


    private void LoadSettingsIntoUI()
    {
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        resolutionDropdown.value        = Mathf.Clamp(resolutionIndex, 0, filteredResolutions.Count - 1);
        fullscreenToggle.isOn           = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        musicVolumeSlider.value         = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolumeSlider.value           = PlayerPrefs.GetFloat("sfxVolume", 1f);
    }

    private void ApplyCurrentSettings()
    {
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        ApplyResolution(resolutionIndex, isFullscreen);
        Screen.fullScreen = isFullscreen;
        mainMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Clamp(musicVolume, 0.0001f, 1f)) * 20f);
        mainMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20f);

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

    public void OnMusicVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
        // Aquí debes aplicar ese volumen a tu audio de música
        mainMixer.SetFloat("musicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void OnSfxVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
        PlayerPrefs.Save();
        // Aquí debes aplicar ese volumen a tus SFX
        mainMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
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

    private void SetupDifficultyToggles()
    {
        // Leer dificultad guardada
        int dificultadGuardada = PlayerPrefs.GetInt("GameDifficulty", 1); // 0=Fácil, 1=Normal, 2=Difícil

        toggleFacil.isOn = (dificultadGuardada == 0);
        toggleNormal.isOn = (dificultadGuardada == 1);
        toggleDificil.isOn = (dificultadGuardada == 2);

        toggleFacil.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(0); });
        toggleNormal.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(1); });
        toggleDificil.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(2); });
    }

    private void SetDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("GameDifficulty", difficulty);
        PlayerPrefs.Save();
        Debug.Log("Dificultad seleccionada: " + (difficulty == 0 ? "Fácil" : difficulty == 1 ? "Normal" : "Difícil"));
    }

}
