using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    public List<string> items = new List<string>();
    public List<Sprite> itemIcons = new List<Sprite>();
    
    public GameObject inventoryUI;
    
    [Header("Database")]
    public ItemDatabase itemDatabase; // NOUVEAU
    
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
        // Initialiser la database
        if (itemDatabase != null)
        {
            itemDatabase.Initialize();
        }
        
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
    }

    void Update()
    {
        // Appuyer sur I (ou bouton B manette) pour afficher/cacher l'inventaire
        bool inventoryPressed = false;
        
        if (InputManager.instance != null)
        {
            inventoryPressed = InputManager.instance.GetInventoryButton();
        }
        else
        {
            inventoryPressed = Input.GetKeyDown(KeyCode.I); // Fallback clavier
        }
        
        if (inventoryPressed)
        {
            ToggleInventory();
        }
    }

    public void AddItem(string itemName, Sprite icon)
    {
        items.Add(itemName);
        itemIcons.Add(icon);

        if (inventoryUI != null && inventoryUI.activeSelf)
        {
            UpdateInventoryUI();
        }
    }

    void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            UpdateInventoryUI();
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI != null)
        {
            InventoryDisplay display = inventoryUI.GetComponent<InventoryDisplay>();
            if (display != null)
            {
                display.UpdateDisplay();
            }
        }
    }
    
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            itemIcons.RemoveAt(index);
            UpdateInventoryUI();
        }
    }
    
    // NOUVEAU : Récupérer une icône depuis la database
    public Sprite GetIconForItem(string itemName)
    {
        if (itemDatabase != null)
        {
            return itemDatabase.GetIcon(itemName);
        }
        
        Debug.LogWarning("ItemDatabase non assignée !");
        return null;
    }
}