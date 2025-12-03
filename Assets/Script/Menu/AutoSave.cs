using UnityEngine;

public class AutoSave : MonoBehaviour
{
    public float saveInterval = 30f; // Sauvegarder toutes les 30 secondes
    private float saveTimer = 0f;
    
    void Update()
    {
        saveTimer += Time.deltaTime;
        
        if (saveTimer >= saveInterval)
        {
            saveTimer = 0f;
            
            if (SaveSystem.instance != null)
            {
                SaveSystem.instance.SaveGame();
            }
        }
    }
}