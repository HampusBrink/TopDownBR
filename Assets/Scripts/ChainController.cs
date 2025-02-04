using UnityEngine;

public class ChainController : MonoBehaviour
{
    public Transform skeletonTransform;
    public Transform anchorPoint;
    public ConfigurableJoint[] chainJoints;
    public Camera mainCamera;
    public bool lockXAxis = true;
    public bool lockYAxis = false;
    public bool lockZAxis = true;

    private Vector3[] initialLocalPositions;
    private Vector3[] originalRotations;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        initialLocalPositions = new Vector3[chainJoints.Length];
        originalRotations = new Vector3[chainJoints.Length];

        for (int i = 0; i < chainJoints.Length; i++)
        {
            initialLocalPositions[i] = chainJoints[i].transform.localPosition;
            originalRotations[i] = chainJoints[i].transform.rotation.eulerAngles;
        }
    }

    void LateUpdate()
    {
        // Update the position of the chain relative to the skeleton
        Vector3 offset = skeletonTransform.position - transform.position;
        transform.position += offset;

        // Keep the anchor point fixed
        chainJoints[0].connectedAnchor = anchorPoint.position;

        // Update the local positions of the chain links and apply billboarding
        for (int i = 0; i < chainJoints.Length; i++)
        {
            Transform linkTransform = chainJoints[i].transform;

            // Update local position
            if (i > 0) // Skip the first link as it's anchored
            {
                linkTransform.localPosition = initialLocalPositions[i];
            }

            // Apply billboarding
            linkTransform.LookAt(linkTransform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);

            Vector3 eulerAngles = linkTransform.eulerAngles;

            if (lockXAxis) eulerAngles.x = originalRotations[i].x;
            if (lockYAxis) eulerAngles.y = originalRotations[i].y;
            if (lockZAxis) eulerAngles.z = originalRotations[i].z;

            linkTransform.eulerAngles = eulerAngles;
        }
    }
}