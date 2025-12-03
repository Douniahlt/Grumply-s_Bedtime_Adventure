using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    private AudioSource audioSource;
    
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public float volume = 0.5f;
    
    void Awake()
    {
        // Singleton - ne pas détruire entre les scènes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        
        // Configuration de base
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.volume = volume;
        }
    }
    
    void Start()
    {
        // Démarrer avec la musique de menu
        PlayMenuMusic();
    }
    
    public void PlayMenuMusic()
    {
        if (audioSource != null && menuMusic != null)
        {
            // Changer seulement si ce n'est pas déjà la musique jouée
            if (audioSource.clip != menuMusic)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
            }
        }
    }
    
    public void PlayGameMusic()
    {
        if (audioSource != null && gameMusic != null)
        {
            // Changer seulement si ce n'est pas déjà la musique jouée
            if (audioSource.clip != gameMusic)
            {
                audioSource.clip = gameMusic;
                audioSource.Play();
            }
        }
    }
    
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}