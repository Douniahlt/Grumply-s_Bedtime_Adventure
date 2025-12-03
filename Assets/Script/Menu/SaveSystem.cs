using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Position du joueur
    public float playerX;
    public float playerY;
    public float playerZ;
    
    // Inventaire (noms des items seulement)
    public string[] inventoryItems;
    
    // IDs des objets ramassés (pour ne pas les réafficher)
    public string[] pickedUpItemIDs;
    
    // IDs des portes ouvertes
    public string[] openedDoorIDs;
    
    // État du tutoriel
    public bool hasSeenWelcome;
    public bool hasMovedWithZQSD;
    public bool hasUsedTab;
    public bool hasPickedUpItem;
    public bool hasOpenedInventory;
    public bool hasCraftedItem;
    
    // État du dialogue actif
    public bool hasActiveDialogue;
    public string activeDialogueName;
    public int currentLineIndex;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public static HashSet<string> pickedUpItems = new HashSet<string>();
    public static HashSet<string> openedDoors = new HashSet<string>();
    
    private string savePath;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Charger les HashSet statiques depuis la sauvegarde si elle existe
            LoadStaticDataFromSave();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        savePath = Application.persistentDataPath + "/savegame.json";
    }
    
    // Charger les données statiques (pickedUpItems et openedDoors) depuis la sauvegarde
    private void LoadStaticDataFromSave()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
        
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            if (data != null)
            {
                // Restaurer les objets ramassés
                pickedUpItems.Clear();
                if (data.pickedUpItemIDs != null)
                {
                    foreach (string id in data.pickedUpItemIDs)
                    {
                        pickedUpItems.Add(id);
                    }
                }
                
                // Restaurer les portes ouvertes
                openedDoors.Clear();
                if (data.openedDoorIDs != null)
                {
                    foreach (string id in data.openedDoorIDs)
                    {
                        openedDoors.Add(id);
                    }
                }
            }
        }
    }
    
    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        // Sauvegarder la position du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
            data.playerZ = player.transform.position.z;
        }
        
        // Sauvegarder l'inventaire (noms seulement)
        if (InventoryManager.instance != null)
        {
            data.inventoryItems = InventoryManager.instance.items.ToArray();
        }
        
        // Sauvegarder les objets ramassés
        data.pickedUpItemIDs = new string[pickedUpItems.Count];
        pickedUpItems.CopyTo(data.pickedUpItemIDs);
        
        // Sauvegarder les portes ouvertes
        data.openedDoorIDs = new string[openedDoors.Count];
        openedDoors.CopyTo(data.openedDoorIDs);
        
        // Sauvegarder l'état du tutoriel
        if (TutorialManager.instance != null)
        {
            data.hasSeenWelcome = TutorialManager.instance.GetHasSeenWelcome();
            data.hasMovedWithZQSD = TutorialManager.instance.GetHasMovedWithZQSD();
            data.hasUsedTab = TutorialManager.instance.GetHasUsedTab();
            data.hasPickedUpItem = TutorialManager.instance.GetHasPickedUpItem();
            data.hasOpenedInventory = TutorialManager.instance.GetHasOpenedInventory();
            data.hasCraftedItem = TutorialManager.instance.GetHasCraftedItem();
        }
        
        // Sauvegarder l'état du dialogue
        if (DialogueManager.instance != null)
        {
            data.hasActiveDialogue = DialogueManager.instance.HasActiveDialogue();
            data.activeDialogueName = DialogueManager.instance.GetCurrentDialogueName();
            data.currentLineIndex = DialogueManager.instance.GetCurrentLineIndex();
        }
        
        // Convertir en JSON et sauvegarder
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        
        Debug.Log("✓ Partie sauvegardée");
    }
    
    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            return data;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée à : " + savePath);
            return null;
        }
    }
    
    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
    
    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            pickedUpItems.Clear();
            openedDoors.Clear();
        }
    }
}