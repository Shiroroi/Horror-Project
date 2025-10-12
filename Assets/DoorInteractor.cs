using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance to interact with doors")]
    public float interactRange = 3f;
    
    [Tooltip("Layer that doors are on")]
    public LayerMask doorLayer;
    
    [Tooltip("Key to press to interact with doors")]
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        // Check if player pressed the interact key
        if (Input.GetKeyDown(interactKey))
        {
            // Cast a ray from the camera forward
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            
            // Check if the ray hits something on the door layer within range
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, doorLayer))
            {
                // Try to get the Animator component from what we hit
                Animator animator = hit.collider.GetComponent<Animator>();
                
                if (animator != null)
                {
                    // Get current state of the door
                    bool isOpen = animator.GetBool("character_nearby");
                    
                    // Toggle the door state
                    animator.SetBool("character_nearby", !isOpen);
                    
                    // Optional: Debug feedback
                    Debug.Log($"Door {(isOpen ? "closing" : "opening")}");
                }
                else
                {
                    Debug.LogWarning("Hit object has no Animator component!");
                }
            }
        }
    }

    // Optional: Draw the interaction range in the Scene view for debugging
    void OnDrawGizmos()
    {
        if (Camera.main != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactRange);
        }
    }
}