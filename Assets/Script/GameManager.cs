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
        bool backPressed = false;
        
        if (InputManager.instance != null)
        {
            backPressed = InputManager.instance.GetBackButton();
        }
        else
        {
            backPressed = Input.GetKeyDown(KeyCode.Escape); // Fallback clavier
        }
        
        if (backPressed)
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