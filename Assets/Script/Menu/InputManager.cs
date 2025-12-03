using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    
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
    
    // Vérifier si on utilise la manette
    public bool IsUsingGamepad()
    {
        return SettingsManager.instance != null && SettingsManager.instance.IsUsingGamepad();
    }
    
    // Action : Ramasser / Interagir
    public bool GetPickupButton()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonDown("Fire1"); // Bouton A (South)
        }
        else
        {
            return Input.GetKeyDown(KeyCode.E);
        }
    }
    
    // Action : Ouvrir inventaire
    public bool GetInventoryButton()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonDown("Fire2"); // Bouton B (East)
        }
        else
        {
            return Input.GetKeyDown(KeyCode.I);
        }
    }
    
    // Action : Surligner objets
    public bool GetHighlightButton()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonDown("Fire3"); // Bouton X (West)
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Tab);
        }
    }
    
    public bool GetHighlightButtonUp()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonUp("Fire3");
        }
        else
        {
            return Input.GetKeyUp(KeyCode.Tab);
        }
    }
    
    // Action : Passer dialogue
    public bool GetContinueButton()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonDown("Fire1"); // Bouton A (South)
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
    
    // Action : Retour / Pause
    public bool GetBackButton()
    {
        if (IsUsingGamepad())
        {
            return Input.GetButtonDown("Cancel"); // Bouton Start
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
    
    // Vibrations
    public void Vibrate(float duration = 0.2f, float intensity = 0.5f)
    {
        if (IsUsingGamepad())
        {
            StartCoroutine(VibrateCoroutine(duration, intensity));
        }
    }
    
    System.Collections.IEnumerator VibrateCoroutine(float duration, float intensity)
    {
        // Unity ne supporte pas directement les vibrations de manette
        // Il faut utiliser Input.SetGamepadState mais c'est limité
        
        // Solution basique avec les moteurs de vibration
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // Utiliser XInput pour Windows (nécessite un package)
        // Pour l'instant on log juste
        Debug.Log("Vibration : " + intensity + " pendant " + duration + "s");
        #endif
        
        yield return new WaitForSeconds(duration);
        
        // Arrêter la vibration
        Debug.Log("Vibration arrêtée");
    }
}