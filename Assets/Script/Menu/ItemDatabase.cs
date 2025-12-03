using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemIcon;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();
    
    private Dictionary<string, Sprite> itemDictionary;
    
    public void Initialize()
    {
        itemDictionary = new Dictionary<string, Sprite>();
        
        foreach (ItemData item in items)
        {
            if (!itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary[item.itemName] = item.itemIcon;
            }
        }
    }
    
    public Sprite GetIcon(string itemName)
    {
        if (itemDictionary == null)
        {
            Initialize();
        }
        
        if (itemDictionary.ContainsKey(itemName))
        {
            return itemDictionary[itemName];
        }
        
        Debug.LogWarning("Icône non trouvée pour : " + itemName);
        return null;
    }
}