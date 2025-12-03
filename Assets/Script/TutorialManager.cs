using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    
    // Étapes du tutoriel
    private bool hasSeenWelcome = false;
    private bool hasMovedWithZQSD = false;
    private bool hasUsedTab = false;
    private bool hasPickedUpItem = false;
    private bool hasOpenedInventory = false;
    private bool hasCraftedItem = false;
    
    // File d'attente pour les dialogues en attente
    private string pendingDialogue = null;
    
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

    void Start()
    {
        // Démarrer le tutoriel avec le message de bienvenue seulement si pas déjà vu
        if (!hasSeenWelcome && !GameLoader.loadingSave)
        {
            Invoke("ShowWelcome", 1f);
        }
    }

    void Update()
    {
        // Détecter si le joueur bouge avec ZQSD ou flèches
        if (!hasMovedWithZQSD && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Q) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            OnPlayerMoved();
        }
        
        // Détecter si le joueur appuie sur Tab
        if (!hasUsedTab && Input.GetKeyDown(KeyCode.Tab))
        {
            OnTabUsed();
        }
        
        // Détecter si le joueur ouvre l'inventaire
        if (!hasOpenedInventory && Input.GetKeyDown(KeyCode.I))
        {
            OnInventoryOpened();
        }
        
        // Vérifier si un dialogue est en attente et si on peut le lancer
        if (pendingDialogue != null && DialogueManager.instance != null)
        {
            // Vérifier si le dialogue actuel est terminé
            if (!DialogueManager.instance.IsDialogueActive())
            {
                DialogueManager.instance.TriggerDialogueByName(pendingDialogue);
                pendingDialogue = null;
            }
        }
    }

    void ShowWelcome()
    {
        if (!hasSeenWelcome)
        {
            hasSeenWelcome = true;
            TriggerDialogue("Welcome");
        }
    }

    public void OnPlayerMoved()
    {
        if (!hasMovedWithZQSD)
        {
            hasMovedWithZQSD = true;
            
            // Notifier le DialogueManager
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.OnActionPerformed("Move");
            }
        }
    }

    public void OnTabUsed()
    {
        if (!hasUsedTab)
        {
            hasUsedTab = true;
            
            // Notifier le DialogueManager
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.OnActionPerformed("Tab");
            }
            
            TriggerDialogue("TabTutorial");
        }
    }

    public void OnItemPickedUp()
    {
        if (!hasPickedUpItem && hasUsedTab)
        {
            hasPickedUpItem = true;
            
            // Notifier le DialogueManager
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.OnActionPerformed("Pickup");
            }
            
            TriggerDialogue("PickupTutorial");
        }
    }

    public void OnInventoryOpened()
    {
        if (!hasOpenedInventory && hasPickedUpItem)
        {
            hasOpenedInventory = true;
            
            // Notifier le DialogueManager
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.OnActionPerformed("Inventory");
            }
            
            TriggerDialogue("InventoryTutorial");
        }
    }

    public void OnItemCrafted()
    {
        if (!hasCraftedItem && hasOpenedInventory)
        {
            hasCraftedItem = true;
            
            // Notifier le DialogueManager
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.OnActionPerformed("Craft");
            }
            
            TriggerDialogue("CraftTutorial");
        }
    }
    
    // Méthode pour déclencher un dialogue (avec attente si nécessaire)
    void TriggerDialogue(string dialogueName)
    {
        if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive())
        {
            // Si un dialogue est actif, mettre en attente
            pendingDialogue = dialogueName;
        }
        else
        {
            // Sinon lancer directement
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.TriggerDialogueByName(dialogueName);
            }
        }
    }
    
    // Getters pour la sauvegarde
    public bool GetHasSeenWelcome() { return hasSeenWelcome; }
    public bool GetHasMovedWithZQSD() { return hasMovedWithZQSD; }
    public bool GetHasUsedTab() { return hasUsedTab; }
    public bool GetHasPickedUpItem() { return hasPickedUpItem; }
    public bool GetHasOpenedInventory() { return hasOpenedInventory; }
    public bool GetHasCraftedItem() { return hasCraftedItem; }
    
    // Setters pour le chargement
    public void SetHasSeenWelcome(bool value) { hasSeenWelcome = value; }
    public void SetHasMovedWithZQSD(bool value) { hasMovedWithZQSD = value; }
    public void SetHasUsedTab(bool value) { hasUsedTab = value; }
    public void SetHasPickedUpItem(bool value) { hasPickedUpItem = value; }
    public void SetHasOpenedInventory(bool value) { hasOpenedInventory = value; }
    public void SetHasCraftedItem(bool value) { hasCraftedItem = value; }
}