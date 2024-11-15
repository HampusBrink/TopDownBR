using UnityEngine;

public class SkeletonSway3D : MonoBehaviour
{
    public float swayAngle = 15f;
    public float swaySpeed = 2f;
    public float returnSpeed = 1f;
    public float pushForce = 2f;
    public Vector3 swayAxis = Vector3.forward; // Adjust based on your model's orientation

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isSwaying = false;
    private Rigidbody rb;

    void Start()
    {
        initialRotation = transform.rotation;
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        
        // Ensure the Rigidbody is not kinematic
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false; // Disable gravity if you don't want it to fall
        }
    }

    void FixedUpdate()
    {
        if (isSwaying)
        {
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * swaySpeed));
            rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * swaySpeed));
        }
        else
        {
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, initialRotation, Time.fixedDeltaTime * returnSpeed));
            rb.MovePosition(Vector3.Lerp(rb.position, initialPosition, Time.fixedDeltaTime * returnSpeed));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 impactDirection = (collision.contacts[0].point - transform.position).normalized;
            Vector3 crossProduct = Vector3.Cross(impactDirection, swayAxis);
            float angle = Mathf.Sign(crossProduct.y) * swayAngle;
            targetRotation = Quaternion.AngleAxis(angle, swayAxis) * initialRotation;
            
            // Calculate new position
            targetPosition = initialPosition + impactDirection * pushForce;
            
            isSwaying = true;
            
            // Apply immediate force
            rb.AddForce(impactDirection * pushForce, ForceMode.Impulse);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isSwaying = false;
        }
    }
}