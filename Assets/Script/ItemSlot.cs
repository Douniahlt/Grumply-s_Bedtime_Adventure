using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // L'index de cet item dans l'inventaire

    public void OnDrop(PointerEventData eventData)
    {
        // Récupérer l'item qu'on a glissé
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        
        if (draggedItem != null)
        {
            // Essayer de fusionner les items
            CraftingSystem.instance.TryCraft(draggedItem.itemIndex, slotIndex);
        }
    }
}