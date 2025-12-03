using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    
    [Header("UI References")]
    public GameObject settingsPanel;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle gamepadToggle;
    
    [Header("Audio")]
    public AudioMixerGroup masterMixer;
    
    // Paramètres
    private float masterVolume = 0.5f;
    private float musicVolume = 0.5f;
    private float sfxVolume = 0.5f;
    private bool useGamepad = false;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // NE PAS utiliser DontDestroyOnLoad pour éviter les conflits
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        LoadSettings();
        
        // Cacher le panel au départ
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        // Lier les sliders aux méthodes
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        if (gamepadToggle != null)
        {
            gamepadToggle.isOn = useGamepad;
            gamepadToggle.onValueChanged.AddListener(SetGamepadMode);
        }
        
        // Appliquer les volumes
        ApplyVolumes();
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        SaveSettings();
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        ApplyVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        // Appliquer aux effets sonores
    }
    
    public void SetGamepadMode(bool enabled)
    {
        useGamepad = enabled;
        Debug.Log("Mode manette : " + (enabled ? "Activé" : "Désactivé"));
    }
    
    void ApplyVolumes()
    {
        // Appliquer le volume à la musique
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetVolume(masterVolume * musicVolume);
        }
    }
    
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("UseGamepad", useGamepad ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Paramètres sauvegardés.");
    }
    
    void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        useGamepad = PlayerPrefs.GetInt("UseGamepad", 0) == 1;
        Debug.Log("Paramètres chargés.");
    }
    
    public bool IsUsingGamepad()
    {
        return useGamepad;
    }
    
    public float GetSFXVolume()
    {
        return masterVolume * sfxVolume;
    }
}