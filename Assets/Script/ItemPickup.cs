using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName = "Cube";
    public Sprite itemIcon;
    
    [Header("ID Unique")]
    [Tooltip("ID unique pour cet objet. Laissez vide pour génération automatique.")]
    public string uniqueID;
    
    private bool playerNearby = false;
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material outlineMaterial;
    private GameObject ePrompt;

    void Start()
    {
        // Générer un ID unique si non défini
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = gameObject.name + "_" + transform.position.ToString();
        }
        
        // Vérifier si cet objet a déjà été ramassé
        if (SaveSystem.pickedUpItems.Contains(uniqueID))
        {
            // Détruire le ePrompt s'il existe avant de détruire l'objet
            if (ePrompt != null)
            {
                Destroy(ePrompt);
            }
            Destroy(gameObject);
            return;
        }
        
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        
        CreateOutlineMaterial();
        CreateEPrompt();
    }

    void CreateOutlineMaterial()
    {
        // Dupliquer le matériau original
        outlineMaterial = new Material(originalMaterial);
        
        // Le rendre blanc brillant avec GLOW
        outlineMaterial.color = Color.white;
        
        // Activer l'émission pour le glow
        outlineMaterial.EnableKeyword("_EMISSION");
        outlineMaterial.SetColor("_EmissionColor", Color.white * 6f); // Intensité du glow
        
        // Optionnel : ajuster la métallicité et la brillance
        outlineMaterial.SetFloat("_Metallic", 0f);
        outlineMaterial.SetFloat("_Glossiness", 0.8f);
    }

    void CreateEPrompt()
    {
        // Calculer la hauteur de l'objet
        float objectHeight = objectRenderer.bounds.size.y;
        
        // Créer un GameObject pour le prompt (AVEC parent mais taille indépendante)
        ePrompt = new GameObject("E_Prompt");
        ePrompt.transform.SetParent(transform, false); // worldPositionStays = false
        ePrompt.transform.localPosition = new Vector3(0, objectHeight / 2 + 0.5f, 0);
        
        // Utiliser une échelle ABSOLUE indépendante de la taille de l'objet parent
        ePrompt.transform.localScale = new Vector3(0.05f / transform.lossyScale.x, 0.05f / transform.lossyScale.y, 0.05f / transform.lossyScale.z);
        
        // Créer le cercle de fond
        GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        circle.name = "Circle";
        circle.transform.SetParent(ePrompt.transform);
        circle.transform.localPosition = Vector3.zero;
        circle.transform.localScale = new Vector3(10f, 10f, 0.5f);
        
        // Matériau gris foncé pour le cercle
        Renderer circleRenderer = circle.GetComponent<Renderer>();
        circleRenderer.material = new Material(Shader.Find("Unlit/Color"));
        circleRenderer.material.color = new Color(0.2f, 0.2f, 0.2f);
        
        // Supprimer le collider du cercle
        Destroy(circle.GetComponent<Collider>());
        
        // Ajouter un TextMesh pour le E
        GameObject textObj = new GameObject("E_Text");
        textObj.transform.SetParent(ePrompt.transform);
        textObj.transform.localPosition = new Vector3(0, 0, -0.3f);
        textObj.transform.localScale = Vector3.one;
        
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = "E";
        textMesh.fontSize = 100;
        textMesh.color = Color.green;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.5f;
        
        // Cacher au départ
        ePrompt.SetActive(false);
    }

    void Update()
    {
        // Faire suivre le E au-dessus de l'objet
        if (ePrompt != null && ePrompt.activeSelf)
        {
            float objectHeight = objectRenderer.bounds.size.y;
            ePrompt.transform.position = transform.position + new Vector3(0, objectHeight / 2 + 0.5f, 0);
        }

        // Si le joueur est proche et appuie sur E (ou bouton A manette)
        if (playerNearby)
        {
            bool pickupPressed = false;
            
            if (InputManager.instance != null)
            {
                pickupPressed = InputManager.instance.GetPickupButton();
            }
            else
            {
                pickupPressed = Input.GetKeyDown(KeyCode.E); // Fallback clavier
            }
            
            if (pickupPressed)
            {
                PickupItem();
            }
        }

        // Si on appuie sur Tab (ou bouton X manette), montrer l'outline
        bool highlightPressed = false;
        bool highlightReleased = false;
        
        if (InputManager.instance != null)
        {
            highlightPressed = InputManager.instance.GetHighlightButton();
            highlightReleased = InputManager.instance.GetHighlightButtonUp();
        }
        else
        {
            highlightPressed = Input.GetKeyDown(KeyCode.Tab); // Fallback clavier
            highlightReleased = Input.GetKeyUp(KeyCode.Tab);
        }
        
        if (highlightPressed)
        {
            ShowOutline(true);
        }
        else if (highlightReleased)
        {
            // Si le joueur n'est pas proche, cacher l'outline
            if (!playerNearby)
            {
                ShowOutline(false);
            }
        }
    }

    void ShowOutline(bool show)
    {
        if (objectRenderer == null || originalMaterial == null || outlineMaterial == null)
        {
            return; // Ignorer si pas encore initialisé
        }
        
        if (show)
        {
            objectRenderer.material = outlineMaterial;
        }
        else
        {
            objectRenderer.material = originalMaterial;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Vérifier que tout est bien initialisé
            if (ePrompt == null || objectRenderer == null)
            {
                if (objectRenderer == null)
                {
                    objectRenderer = GetComponent<Renderer>();
                    originalMaterial = objectRenderer.material;
                    CreateOutlineMaterial();
                }

                if (ePrompt == null)
                {
                    CreateEPrompt();
                }
            }

            playerNearby = true;
            ShowOutline(true);

            if (ePrompt != null)
            {
                ePrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            ShowOutline(false);
            
            if (ePrompt != null)
            {
                ePrompt.SetActive(false);
            }
        }
    }

    void PickupItem()
    {
        InventoryManager.instance.AddItem(itemName, itemIcon);

        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnItemPickedUp();
        }

        // Enregistrer que cet objet a été ramassé
        SaveSystem.pickedUpItems.Add(uniqueID);

        // Sauvegarder automatiquement
        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.SaveGame();
        }

        Destroy(ePrompt);
        Destroy(gameObject);
    }
}