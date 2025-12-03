using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button loadGameButton;
    
    void Start()
    {
        // Vérifier s'il y a une sauvegarde
        if (SaveSystem.instance != null)
        {
            bool hasSave = SaveSystem.instance.HasSaveFile();
            
            if (loadGameButton != null)
            {
                loadGameButton.interactable = hasSave;
            }
            else
            {
                Debug.LogError("loadGameButton est NULL ! Assigne le bouton dans l'Inspector.");
            }
        }
        else
        {
            Debug.LogError("SaveSystem.instance est NULL !");
        }
    }
    
    public void NewGame()
    {
        // Supprimer l'ancienne sauvegarde si elle existe
        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.DeleteSave();
            SaveSystem.pickedUpItems.Clear(); // Vider aussi la liste en mémoire
        }
        
        GameLoader.loadingSave = false;
        
        // Changer la musique
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayGameMusic();
        }
        
        SceneManager.LoadScene("GameScene");
    }

    public void LoadGame()
    {
        if (SaveSystem.instance != null)
        {
            bool hasSave = SaveSystem.instance.HasSaveFile();
            
            if (hasSave)
            {
                GameLoader.loadingSave = true;
                
                // Changer la musique
                if (MusicManager.instance != null)
                {
                    MusicManager.instance.PlayGameMusic();
                }
                
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.LogWarning("Aucune sauvegarde disponible !");
            }
        }
        else
        {
            Debug.LogError("SaveSystem.instance est NULL !");
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Ouverture des paramètres");
        if (SettingsManager.instance != null)
        {
            SettingsManager.instance.OpenSettings();
        }
    }

    public void OpenCredits()
    {
        // TODO: Implémenter le menu de crédits
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}