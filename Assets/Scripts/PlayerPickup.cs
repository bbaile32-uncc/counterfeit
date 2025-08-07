using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 5f;
    public LayerMask pickupMask;
    public float followSpeed = 10f;

    private GameObject heldObject;
    private Rigidbody heldRB;
    private FPSInputActions input;
    private Camera cam;
    private Vector3 holdOffset;
    private float holdDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<FPSInputActions>();
        cam = Camera.main;

        input.InputActions.Player.Interact.performed += ctx => TryInteract();
    }
    void TryInteract()
    {
        if (heldObject == null)
        {
            Debug.Log("Trying to pick up");
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
            {
                Debug.Log("Hit " + hit.collider.name);
                Pickup(hit.collider.gameObject);
            }
        }
        else
        {
            Drop();
        }
    }

    void Pickup(GameObject obj)
    {
        heldObject = obj;
        heldRB = obj.GetComponent<Rigidbody>();

        if (heldRB)
        {
            heldRB.useGravity = false;
            heldRB.isKinematic = false;
            heldRB.freezeRotation = true;
            heldRB.angularVelocity = Vector3.zero;
            heldRB.Sleep();

        }

        // Record offset and distance from camera

        holdOffset = obj.transform.position - cam.transform.position;
        holdDistance = holdOffset.magnitude;
    }

    void Drop()
    {
        if (heldRB)
        {
            heldRB.useGravity = true;
            heldRB.freezeRotation = false;

        }

        heldObject = null;
        heldRB = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (heldObject && heldRB)
        {
            Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;
            Vector3 toTarget = targetPos - heldRB.position;

            float smoothFactor = 15f;

            Vector3 desiredVelocity = toTarget * smoothFactor;

            heldRB.linearVelocity = Vector3.ClampMagnitude(desiredVelocity, 25f);

        }
    }
}
