using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    [Header("Items requis")]
    public List<string> acceptedItems = new List<string>(); // Les items qui fonctionnent
    
    [Header("Action à effectuer")]
    public string actionType = "OpenDoor"; // Type d'action
    
    private string uniqueID;
    
    [Header("Paramètres de la porte")]
    public GameObject leftDoor; // Cube gauche
    public GameObject rightDoor; // Cube droit
    public float doorOpenDistance = 3f; // Distance d'ouverture
    public float doorOpenSpeed = 2f; // Vitesse d'ouverture
    
    private bool isOpen = false;
    private Vector3 leftDoorStartPos;
    private Vector3 rightDoorStartPos;
    private Vector3 leftDoorTargetPos;
    private Vector3 rightDoorTargetPos;
    private float openProgress = 0f;
    
    private Renderer leftDoorRenderer;
    private Renderer rightDoorRenderer;
    private Material leftOriginalMaterial;
    private Material rightOriginalMaterial;
    private Material highlightMaterial;

    void Start()
    {
        // Générer un ID unique pour cet objet
        uniqueID = gameObject.name + "_" + transform.position.ToString();
        
        // Récupérer les renderers des portes pour le highlight
        if (leftDoor != null)
        {
            leftDoorRenderer = leftDoor.GetComponent<Renderer>();
            if (leftDoorRenderer != null)
            {
                leftOriginalMaterial = leftDoorRenderer.material;
            }
        }
        
        if (rightDoor != null)
        {
            rightDoorRenderer = rightDoor.GetComponent<Renderer>();
            if (rightDoorRenderer != null)
            {
                rightOriginalMaterial = rightDoorRenderer.material;
            }
        }
        
        // Créer un matériau de highlight (blanc brillant)
        if (leftOriginalMaterial != null)
        {
            highlightMaterial = new Material(leftOriginalMaterial);
            highlightMaterial.color = Color.white;
            highlightMaterial.EnableKeyword("_EMISSION");
            highlightMaterial.SetColor("_EmissionColor", Color.white * 0.5f);
        }
        
        if (leftDoor != null && rightDoor != null)
        {
            // Sauvegarder les positions de départ
            leftDoorStartPos = leftDoor.transform.position;
            rightDoorStartPos = rightDoor.transform.position;
            
            // Calculer les positions cibles (éloignées)
            leftDoorTargetPos = leftDoorStartPos + Vector3.left * doorOpenDistance;
            rightDoorTargetPos = rightDoorStartPos + Vector3.right * doorOpenDistance;
            
            // Vérifier si cette porte était déjà ouverte
            if (SaveSystem.openedDoors.Contains(uniqueID))
            {
                isOpen = true;
                openProgress = 1f;
                // Positionner directement les portes en position ouverte
                leftDoor.transform.position = leftDoorTargetPos;
                rightDoor.transform.position = rightDoorTargetPos;
            }
        }
    }
    
    public void SetHighlight(bool highlight)
    {
        if (leftDoorRenderer != null)
        {
            leftDoorRenderer.material = highlight ? highlightMaterial : leftOriginalMaterial;
        }
        
        if (rightDoorRenderer != null)
        {
            rightDoorRenderer.material = highlight ? highlightMaterial : rightOriginalMaterial;
        }
    }

    public bool UseItem(string itemName)
    {
        // Vérifier si l'item est accepté
        if (acceptedItems.Contains(itemName))
        {
            // Vibration quand l'item fonctionne !
            if (InputManager.instance != null)
            {
                InputManager.instance.Vibrate(0.3f, 0.7f); // 0.3s de vibration à 70% d'intensité
            }
            
            PerformAction();
            return true;
        }
        
        return false;
    }

    void PerformAction()
    {
        if (actionType == "OpenDoor" && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        
        // Enregistrer que cette porte a été ouverte
        SaveSystem.openedDoors.Add(uniqueID);
        
        // Sauvegarder immédiatement
        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.SaveGame();
        }
    }

    void Update()
    {
        // Animer l'ouverture de la porte
        if (isOpen && openProgress < 1f)
        {
            openProgress += Time.deltaTime * doorOpenSpeed;
            openProgress = Mathf.Clamp01(openProgress);
            
            if (leftDoor != null)
            {
                leftDoor.transform.position = Vector3.Lerp(leftDoorStartPos, leftDoorTargetPos, openProgress);
            }
            
            if (rightDoor != null)
            {
                rightDoor.transform.position = Vector3.Lerp(rightDoorStartPos, rightDoorTargetPos, openProgress);
            }
        }
    }
}