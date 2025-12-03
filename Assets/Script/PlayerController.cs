using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;
        
        // Vérifier si on utilise la manette
        bool usingGamepad = SettingsManager.instance != null && SettingsManager.instance.IsUsingGamepad();
        
        if (usingGamepad)
        {
            // Contrôles manette (Joystick gauche)
            moveX = Input.GetAxis("Horizontal"); // Joystick gauche X
            moveZ = Input.GetAxis("Vertical");   // Joystick gauche Y
        }
        else
        {
            // Contrôles clavier (ZQSD ou flèches)
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }

        Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}