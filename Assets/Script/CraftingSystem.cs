using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CraftingRecipe
{
    public string item1;
    public string item2;
    public string result;
    public Sprite resultIcon;
}

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem instance;
    
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();
    
    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TryCraft(int index1, int index2)
    {
        // Éviter de fusionner un item avec lui-même
        if (index1 == index2)
        {
            return;
        }
        
        // Vérifier que les index sont valides
        if (index1 >= InventoryManager.instance.items.Count || index2 >= InventoryManager.instance.items.Count)
        {
            return;
        }
        
        string item1 = InventoryManager.instance.items[index1];
        string item2 = InventoryManager.instance.items[index2];
        
        // Chercher une recette qui correspond
        CraftingRecipe matchingRecipe = FindRecipe(item1, item2);
        
        if (matchingRecipe != null)
        {
            // Supprimer les deux items (du plus grand au plus petit index)
            int maxIndex = Mathf.Max(index1, index2);
            int minIndex = Mathf.Min(index1, index2);
            
            InventoryManager.instance.items.RemoveAt(maxIndex);
            InventoryManager.instance.itemIcons.RemoveAt(maxIndex);
            
            InventoryManager.instance.items.RemoveAt(minIndex);
            InventoryManager.instance.itemIcons.RemoveAt(minIndex);
            
            // Ajouter le nouvel item
            InventoryManager.instance.items.Add(matchingRecipe.result);
            InventoryManager.instance.itemIcons.Add(matchingRecipe.resultIcon);
            
            // Notifier le tutoriel
            if (TutorialManager.instance != null)
            {
                TutorialManager.instance.OnItemCrafted();
            }
            
            // Forcer la mise à jour de l'affichage avec un délai
            StartCoroutine(RefreshInventoryUI());
        }
    }
    
    IEnumerator RefreshInventoryUI()
    {
        yield return null; // Attendre 1 frame
        
        InventoryDisplay display = InventoryManager.instance.inventoryUI.GetComponent<InventoryDisplay>();
        if (display != null)
        {
            display.UpdateDisplay();
        }
    }
    
    CraftingRecipe FindRecipe(string item1, string item2)
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            // Vérifier les deux ordres possibles
            if ((recipe.item1 == item1 && recipe.item2 == item2) || 
                (recipe.item1 == item2 && recipe.item2 == item1))
            {
                return recipe;
            }
        }
        return null;
    }
}