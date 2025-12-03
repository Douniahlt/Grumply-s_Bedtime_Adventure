using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject itemSlotPrefab; // Un prefab pour chaque slot
    public Transform itemsParent; // Le parent où on va créer les slots

    public void UpdateDisplay()
    {
        // Nettoyer les anciens slots
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
    
        // Créer un slot pour chaque item
        for (int i = 0; i < InventoryManager.instance.items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemsParent);
            
            // Ajouter un Image component si le slot n'en a pas
            if (slot.GetComponent<Image>() == null)
            {
                slot.AddComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            
            // Chercher seulement l'icône
            Image icon = null;
            
            foreach (Transform child in slot.transform)
            {
                if (child.name == "Icon")
                {
                    icon = child.GetComponent<Image>();
                }
                // Cacher le texte s'il existe
                else if (child.name == "Name")
                {
                    child.gameObject.SetActive(false);
                }
            }
            
            // Configurer l'icône
            if (icon != null && InventoryManager.instance.itemIcons[i] != null)
            {
                icon.sprite = InventoryManager.instance.itemIcons[i];
            }
            
            // Ajouter le composant draggable
            DraggableItem draggable = slot.AddComponent<DraggableItem>();
            draggable.itemIndex = i;
            
            // Ajouter le composant drop
            ItemSlot itemSlot = slot.AddComponent<ItemSlot>();
            itemSlot.slotIndex = i;
        }
    }
}
