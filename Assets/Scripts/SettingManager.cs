using UnityEngine;

public static class SettingsManager
{
    public static void ApplySavedSettings()
    {
        int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        bool fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        float volume = PlayerPrefs.GetFloat("volume", 1f);

        Resolution[] resolutions = Screen.resolutions;
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
            resolutionIndex = 0; // fallback

        Resolution res = resolutions[resolutionIndex];
        
        // Mejor control del modo pantalla
        Screen.fullScreenMode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        
        AudioListener.volume = volume;
    }
}
