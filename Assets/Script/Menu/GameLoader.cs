using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    public static bool loadingSave = false;
    
    void Start()
    {
        if (loadingSave && SaveSystem.instance != null)
        {
            LoadGameData();
            loadingSave = false;
        }
    }
    
    void LoadGameData()
    {
        SaveData data = SaveSystem.instance.LoadGame();

        if (data != null)
        {
            // Restaurer les objets ramassés
            if (data.pickedUpItemIDs != null)
            {
                SaveSystem.pickedUpItems.Clear();
                foreach (string id in data.pickedUpItemIDs)
                {
                    SaveSystem.pickedUpItems.Add(id);
                }
            }
            
            // Restaurer les portes ouvertes
            if (data.openedDoorIDs != null)
            {
                SaveSystem.openedDoors.Clear();
                foreach (string id in data.openedDoorIDs)
                {
                    SaveSystem.openedDoors.Add(id);
                }
            }

            // Restaurer la position du joueur
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
            }
            else
            {
                Debug.LogWarning("Player non trouvé pour restaurer la position !");
            }

            // Restaurer l'inventaire avec les icônes depuis la database
            if (InventoryManager.instance != null && data.inventoryItems != null)
            {
                InventoryManager.instance.items.Clear();
                InventoryManager.instance.itemIcons.Clear();

                // Ajouter les items avec leurs icônes depuis la database
                foreach (string itemName in data.inventoryItems)
                {
                    InventoryManager.instance.items.Add(itemName);

                    // Récupérer l'icône depuis la database
                    Sprite icon = InventoryManager.instance.GetIconForItem(itemName);
                    InventoryManager.instance.itemIcons.Add(icon);
                }

                // Mettre à jour l'affichage
                InventoryManager.instance.UpdateInventoryUI();
            }

            // Restaurer l'état du tutoriel
            if (TutorialManager.instance != null)
            {
                TutorialManager.instance.SetHasSeenWelcome(data.hasSeenWelcome);
                TutorialManager.instance.SetHasMovedWithZQSD(data.hasMovedWithZQSD);
                TutorialManager.instance.SetHasUsedTab(data.hasUsedTab);
                TutorialManager.instance.SetHasPickedUpItem(data.hasPickedUpItem);
                TutorialManager.instance.SetHasOpenedInventory(data.hasOpenedInventory);
                TutorialManager.instance.SetHasCraftedItem(data.hasCraftedItem);
            }

            // Restaurer le dialogue actif avec un délai
            if (data.hasActiveDialogue)
            {
                StartCoroutine(RestoreDialogueDelayed(data.activeDialogueName, data.currentLineIndex));
            }
        }
        else
        {
            Debug.LogError("Échec du chargement des données !");
        }
    }

    // Coroutine pour restaurer le dialogue avec un délai
    IEnumerator RestoreDialogueDelayed(string dialogueName, int lineIndex)
    {
        // Attendre 2 frames pour être sûr que tout est initialisé
        yield return null;
        yield return null;

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.RestoreDialogue(dialogueName, lineIndex);
        }
        else
        {
            Debug.LogError("DialogueManager.instance est null lors de la restauration du dialogue !");
        }
    }
}