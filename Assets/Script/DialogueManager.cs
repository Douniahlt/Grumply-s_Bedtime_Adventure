using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string text;
    
    public bool waitForAction = false;
    public string requiredAction = "";
}

[System.Serializable]
public class Dialogue
{
    public string dialogueName;
    public List<DialogueLine> lines;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    [Header("UI References")]
    public GameObject dialoguePanel;
    public GameObject dialogueTextObject;
    public GameObject continuePromptObject;
    
    [Header("Dialogues")]
    public List<Dialogue> dialogues = new List<Dialogue>();
    
    private Queue<DialogueLine> currentDialogueLines;
    private DialogueLine currentLine;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool waitingForAction = false;
    private string currentRequiredAction = "";
    private string currentFullText = "";
    
    // Suivi du dialogue actuel
    private string currentDialogueName = "";
    private int currentLineIndex = 0;
    
    // Suivi des actions déjà effectuées
    private HashSet<string> actionsPerformed = new HashSet<string>();
    
    // Références internes aux composants TextMeshPro
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI continuePrompt;
    
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
        
        // Récupérer les composants TextMeshProUGUI
        if (dialogueTextObject != null)
        {
            dialogueText = dialogueTextObject.GetComponent<TextMeshProUGUI>();
            if (dialogueText == null)
            {
                Debug.LogError("DialogueTextObject n'a pas de composant TextMeshProUGUI !");
            }
        }
        else
        {
            Debug.LogError("DialogueTextObject n'est pas assigné !");
        }
        
        if (continuePromptObject != null)
        {
            continuePrompt = continuePromptObject.GetComponent<TextMeshProUGUI>();
            if (continuePrompt == null)
            {
                Debug.LogError("ContinuePromptObject n'a pas de composant TextMeshProUGUI !");
            }
        }
        else
        {
            Debug.LogError("ContinuePromptObject n'est pas assigné !");
        }
    }

    void Start()
    {
        // Cacher le dialogue au démarrage
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Espace (ou bouton A manette) fonctionne TOUJOURS pour afficher le texte complet
        if (isDialogueActive && InputManager.instance != null && InputManager.instance.GetContinueButton())
        {
            if (isTyping)
            {
                StopAllCoroutines();
                DisplayFullLine();
            }
            else if (!waitingForAction)
            {
                DisplayNextLine();
            }
        }
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    // Getters pour la sauvegarde
    public bool HasActiveDialogue()
    {
        return isDialogueActive;
    }
    
    public string GetCurrentDialogueName()
    {
        return currentDialogueName;
    }
    
    public int GetCurrentLineIndex()
    {
        // Retourner l'index de la ligne actuellement affichée (pas la suivante)
        return currentLineIndex - 1;
    }
    
    // Restaurer un dialogue à une ligne spécifique
    public void RestoreDialogue(string dialogueName, int lineIndex)
    {
        Dialogue dialogue = dialogues.Find(d => d.dialogueName == dialogueName);
        if (dialogue != null && lineIndex >= 0 && lineIndex < dialogue.lines.Count)
        {
            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            
            currentDialogueName = dialogueName;
            currentDialogueLines = new Queue<DialogueLine>();
            
            // Ajouter les lignes à partir de la ligne sauvegardée
            for (int i = lineIndex; i < dialogue.lines.Count; i++)
            {
                currentDialogueLines.Enqueue(dialogue.lines[i]);
            }
            
            // Définir currentLineIndex pour qu'après l'incrément il soit à lineIndex
            currentLineIndex = lineIndex - 1;
            DisplayNextLine();
        }
    }

    public void TriggerDialogueByName(string dialogueName)
    {
        // Ne pas déclencher si un dialogue est déjà actif
        if (isDialogueActive)
        {
            return;
        }
        
        Dialogue dialogue = dialogues.Find(d => d.dialogueName == dialogueName);
        if (dialogue != null)
        {
            StartDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning("Dialogue '" + dialogueName + "' non trouvé !");
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        currentDialogueName = dialogue.dialogueName;
        currentLineIndex = 0;
        
        currentDialogueLines = new Queue<DialogueLine>();
        
        foreach (DialogueLine line in dialogue.lines)
        {
            currentDialogueLines.Enqueue(line);
        }
        
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (currentDialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        currentLine = currentDialogueLines.Dequeue();
        currentLineIndex++;
        
        // Vérifier si cette ligne attend une action
        if (currentLine.waitForAction)
        {
            // Vérifier si l'action a déjà été effectuée
            if (actionsPerformed.Contains(currentLine.requiredAction))
            {
                // L'action a déjà été faite, passer directement à la ligne suivante après affichage
                waitingForAction = false;
                currentRequiredAction = "";
                StopAllCoroutines();
                StartCoroutine(TypeSentenceAndSkip(currentLine.text));
                return;
            }
            else
            {
                waitingForAction = true;
                currentRequiredAction = currentLine.requiredAction;
            }
        }
        else
        {
            waitingForAction = false;
            currentRequiredAction = "";
        }
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.text));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        
        // Adapter le texte selon le mode de contrôle
        sentence = AdaptTextForInput(sentence);
        currentFullText = sentence;
        
        if (dialogueText == null)
        {
            Debug.LogError("DialogueText est null !");
            yield break;
        }
        
        if (continuePrompt == null)
        {
            Debug.LogError("ContinuePrompt est null !");
            yield break;
        }
        
        dialogueText.text = "";
        continuePrompt.gameObject.SetActive(false);
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        
        isTyping = false;
        
        // Afficher le prompt SEULEMENT si on n'attend PAS d'action
        if (!waitingForAction)
        {
            // Adapter aussi le prompt
            if (InputManager.instance != null && InputManager.instance.IsUsingGamepad())
            {
                continuePrompt.text = "Appuyez sur Bouton A...";
            }
            else
            {
                continuePrompt.text = "Appuyez sur Espace...";
            }
            continuePrompt.gameObject.SetActive(true);
        }
        else
        {
            // Cacher complètement le prompt quand on attend une action
            continuePrompt.gameObject.SetActive(false);
        }
    }
    
    // Coroutine spéciale qui affiche le texte puis passe automatiquement à la suite
    IEnumerator TypeSentenceAndSkip(string sentence)
    {
        isTyping = true;
        
        // Adapter le texte
        sentence = AdaptTextForInput(sentence);
        currentFullText = sentence;
        
        if (dialogueText == null || continuePrompt == null)
        {
            yield break;
        }
        
        dialogueText.text = "";
        continuePrompt.gameObject.SetActive(false);
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        
        isTyping = false;
        
        // Attendre un peu pour que le joueur puisse lire
        yield return new WaitForSeconds(1f);
        
        // Passer automatiquement à la ligne suivante
        DisplayNextLine();
    }

    void DisplayFullLine()
    {
        isTyping = false;
        
        // Afficher le texte complet immédiatement
        if (dialogueText != null)
        {
            dialogueText.text = currentFullText; // Déjà adapté
        }
        
        // Afficher le prompt SEULEMENT si on n'attend PAS d'action
        if (continuePrompt != null)
        {
            if (!waitingForAction)
            {
                // Adapter le prompt
                if (InputManager.instance != null && InputManager.instance.IsUsingGamepad())
                {
                    continuePrompt.text = "Appuyez sur Bouton A...";
                }
                else
                {
                    continuePrompt.text = "Appuyez sur Espace...";
                }
                continuePrompt.gameObject.SetActive(true);
            }
            else
            {
                // Cacher le prompt quand on attend une action
                continuePrompt.gameObject.SetActive(false);
            }
        }
    }

    // Adapter le texte selon le mode de contrôle
    string AdaptTextForInput(string text)
    {
        if (InputManager.instance != null && InputManager.instance.IsUsingGamepad())
        {
            // Remplacer les touches clavier par les boutons manette
            text = text.Replace("ZQSD", "Joystick gauche");
            text = text.Replace("zqsd", "joystick gauche");
            text = text.Replace("Z Q S D", "Joystick gauche");
            text = text.Replace("flèches", "joystick gauche");
            text = text.Replace("Tab", "Bouton X");
            text = text.Replace("E", "Bouton A");
            text = text.Replace("I", "Bouton B");
            text = text.Replace("Espace", "Bouton A");
        }
        
        return text;
    }

    // Appelée quand une action est effectuée
    public void OnActionPerformed(string actionName)
    {
        // Enregistrer que cette action a été effectuée
        actionsPerformed.Add(actionName);
        
        if (isDialogueActive && waitingForAction && currentRequiredAction == actionName)
        {
            waitingForAction = false;
            currentRequiredAction = "";
            DisplayNextLine();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        waitingForAction = false;
        currentRequiredAction = "";
        currentDialogueName = "";
        currentLineIndex = 0;
        // Ne PAS réinitialiser actionsPerformed pour garder la trace des actions entre les dialogues
        dialoguePanel.SetActive(false);
    }
}