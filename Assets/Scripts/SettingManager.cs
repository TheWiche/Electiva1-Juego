using UnityEngine;

public static class SettingsManager
{
    public const string RESOLUTION_INDEX_KEY = "resolutionIndex";
    public const string FULLSCREEN_KEY = "fullscreen";
    public const string VOLUME_KEY = "volume";

    public static void ApplySavedSettings()
    {
        int resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, 0);
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        Resolution[] availableResolutions = Screen.resolutions;

        if (availableResolutions.Length == 0)
        {
            return;
        }

        if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Length)
        {
            resolutionIndex = 0;
        }

        Resolution targetResolution = availableResolutions[resolutionIndex];

        FullScreenMode screenMode = fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        int preferredRefreshRate = 0;

        Screen.SetResolution(targetResolution.width, targetResolution.height, screenMode, preferredRefreshRate);

        AudioListener.volume = volume;
    }

    public static void SaveSettings(int resolutionIndex, bool fullscreen, float volume)
    {
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, resolutionIndex);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}