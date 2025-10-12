using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask doorLayer;
    public KeyCode interactKey = KeyCode.E;


    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, doorLayer))
            {
                Animator animator = hit.collider.GetComponent<Animator>();
                if (animator != null)
                {
                    bool isOpen = animator.GetBool("character_nearby");
                    animator.SetBool("character_nearby", !isOpen);
                }
            }
        }
    }
}
