
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class IKInteractable : MonoBehaviour
{
    [Header("IK References")]
    public Transform handTarget;
    public Transform handBone;
    public Transform gripPoint;
    public Transform shoulder;
    public TwoBoneIKConstraint interactIK;

    [Header("Interaction")]
    public float reachDuration = 0.8f;
    public float maxReach = 0.8f;

    private bool isInteracting;
    private bool isHeld;

    // Store the hand's original LOCAL pose
    private Vector3 restLocalPosition;
    private Quaternion restLocalRotation;

    // Store the object's original parent
    private Transform originalParent;

    void Start()
    {
        if (handTarget == null || interactIK == null)
        {
            Debug.LogError("Missing IK references!");
            enabled = false;
            return;
        }

        restLocalPosition = handTarget.localPosition;
        restLocalRotation = handTarget.localRotation;

        originalParent = transform.parent;

        // Start with IK disabled
        interactIK.weight = 0f;
    }

    void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.eKey.wasPressedThisFrame ||
            isInteracting)
            return;

        // Release if already holding the object
        if (isHeld)
        {
            StartCoroutine(ReleaseObject());
            return;
        }

        // Check references
        if (handTarget == null || handBone == null ||
            gripPoint == null || shoulder == null ||
            interactIK == null)
        {
            Debug.LogWarning("IK references are missing!");
            return;
        }

        // Check reachability
        float distance = Vector3.Distance(
            shoulder.position,
            gripPoint.position
        );

        if (distance > maxReach)
        {
            Debug.Log("Object is outside arm reach!");
            return;
        }

        StartCoroutine(ReachAndGrab());
    }

    IEnumerator ReachAndGrab()
    {
        isInteracting = true;

        Vector3 startPosition = handTarget.position;
        Quaternion startRotation = handTarget.rotation;

        float startWeight = interactIK.weight;

        float elapsed = 0f;

        // Move hand towards grip point and blend in IK
        while (elapsed < reachDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / reachDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            handTarget.position = Vector3.Lerp(
                startPosition,
                gripPoint.position,
                t
            );

            handTarget.rotation = Quaternion.Slerp(
                startRotation,
                gripPoint.rotation,
                t
            );

            interactIK.weight = Mathf.Lerp(
                startWeight,
                1f,
                t
            );

            yield return null;
        }

        // Final hand position
        handTarget.position = gripPoint.position;
        handTarget.rotation = gripPoint.rotation;
        interactIK.weight = 1f;

        // Attach object to hand
        transform.SetParent(handBone, true);

        isHeld = true;
        isInteracting = false;
    }

    IEnumerator ReleaseObject()
    {
        isInteracting = true;

        // Detach object, preserving world position
        transform.SetParent(originalParent, true);

        isHeld = false;

        Vector3 startPosition = handTarget.position;
        Quaternion startRotation = handTarget.rotation;

        float startWeight = interactIK.weight;

        float elapsed = 0f;

        // Return hand to its local resting pose
        // while gradually fading out IK
        while (elapsed < reachDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / reachDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Calculate the resting pose relative to
            // the character's current position and rotation
            Transform targetParent = handTarget.parent;

            Vector3 restWorldPosition = targetParent != null
                ? targetParent.TransformPoint(restLocalPosition)
                : restLocalPosition;

            Quaternion restWorldRotation = targetParent != null
                ? targetParent.rotation * restLocalRotation
                : restLocalRotation;

            handTarget.position = Vector3.Lerp(
                startPosition,
                restWorldPosition,
                t
            );

            handTarget.rotation = Quaternion.Slerp(
                startRotation,
                restWorldRotation,
                t
            );

            interactIK.weight = Mathf.Lerp(
                startWeight,
                0f,
                t
            );

            yield return null;
        }

        // Restore exact local resting pose
        handTarget.localPosition = restLocalPosition;
        handTarget.localRotation = restLocalRotation;

        interactIK.weight = 0f;

        isInteracting = false;
    }
}
