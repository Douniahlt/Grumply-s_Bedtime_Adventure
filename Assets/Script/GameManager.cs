using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // Retour au menu avec Échap (ou Start manette)
        if (InputManager.instance != null && InputManager.instance.GetBackButton())
        {
            ReturnToMenu();
        }
    }
    
    public void ReturnToMenu()
    {
        // Sauvegarder avant de quitter
        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.SaveGame();
        }
        
        // Changer la musique
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayMenuMusic();
        }
        
        // Charger le menu
        SceneManager.LoadScene("MainMenu");
    }
    
    void OnApplicationQuit()
    {
        // Sauvegarder quand on ferme le jeu
        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.SaveGame();
        }
    }
}