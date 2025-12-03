using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int itemIndex;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;
    private InteractableObject currentHighlightedObject;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvas = GetComponentInParent<Canvas>();
        
        Image[] childImages = GetComponentsInChildren<Image>();
        foreach (Image img in childImages)
        {
            if (img.gameObject != gameObject)
            {
                img.raycastTarget = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;
        startParent = transform.parent;
        
        canvasGroup.alpha = 0.7f;
        rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
        
        canvasGroup.blocksRaycasts = false;
        
        HorizontalLayoutGroup layoutGroup = startParent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }
        
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.ignoreLayout = true;
        
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
        
        // Vérifier si on survole un objet interactable
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null)
            {
                // Si on survole un nouvel objet différent
                if (interactable != currentHighlightedObject)
                {
                    // Enlever le highlight du précédent objet
                    if (currentHighlightedObject != null)
                    {
                        currentHighlightedObject.SetHighlight(false);
                    }
                    
                    // Activer le highlight du nouvel objet
                    currentHighlightedObject = interactable;
                    currentHighlightedObject.SetHighlight(true);
                }
            }
            else
            {
                // On a touché un objet mais ce n'est pas un InteractableObject
                if (currentHighlightedObject != null)
                {
                    currentHighlightedObject.SetHighlight(false);
                    currentHighlightedObject = null;
                }
            }
        }
        else
        {
            // Si on ne survole plus rien, enlever le highlight
            if (currentHighlightedObject != null)
            {
                currentHighlightedObject.SetHighlight(false);
                currentHighlightedObject = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Enlever le highlight quand on arrête de drag
        if (currentHighlightedObject != null)
        {
            currentHighlightedObject.SetHighlight(false);
            currentHighlightedObject = null;
        }
        
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
        canvasGroup.blocksRaycasts = true;
        
        HorizontalLayoutGroup layoutGroup = startParent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
        }
        
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = false;
        }
        
        // Vérifier si on a déposé sur un objet 3D dans la scène
        bool usedOnSceneObject = TryUseOnSceneObject(eventData);
        
        if (!usedOnSceneObject && transform.parent == canvas.transform)
        {
            transform.SetParent(startParent, true);
        }
    }

    bool TryUseOnSceneObject(PointerEventData eventData)
    {
        // Raycast depuis la souris vers la scène 3D
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        
        // Augmenter la distance
        float maxDistance = 1000f;
        
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Chercher un composant InteractableObject sur l'objet touché
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null)
            {
                // Récupérer le nom de l'item depuis l'inventaire
                string itemName = InventoryManager.instance.items[itemIndex];
                
                // Essayer d'utiliser l'item sur l'objet
                bool success = interactable.UseItem(itemName);
                
                if (success)
                {
                    // Supprimer l'item de l'inventaire
                    InventoryManager.instance.RemoveItem(itemIndex);
                    return true;
                }
            }
        }
        
        return false;
    }
}