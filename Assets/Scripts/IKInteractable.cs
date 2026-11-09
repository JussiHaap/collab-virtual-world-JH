
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class IKInteractable : MonoBehaviour
{
    [Header("IK References")]
    public Transform handTarget;
    public Transform handBone;
    public Transform gripPoint;
    public Transform shoulder;

    [Header("Interaction")]
    public float reachDuration = 0.8f;
    public float maxReach = 0.8f;

    private bool isInteracting;
    private bool isHeld;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            !isInteracting && !isHeld)
        {
            // Check that all references exist
            if (handTarget == null || handBone == null ||
                gripPoint == null || shoulder == null)
            {
                Debug.LogWarning("IK references are missing!");
                return;
            }

            // Check distance from shoulder to grip point
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
    }

    IEnumerator ReachAndGrab()
    {
        isInteracting = true;

        Vector3 startPosition = handTarget.position;
        Quaternion startRotation = handTarget.rotation;

        float elapsed = 0f;

        while (elapsed < reachDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / reachDuration);

            t = Mathf.SmoothStep(0f, 1f, t);

            handTarget.position = Vector3.Lerp(
                startPosition, gripPoint.position, t);

            handTarget.rotation = Quaternion.Slerp(
                startRotation, gripPoint.rotation, t);

            yield return null;
        }

        handTarget.position = gripPoint.position;
        handTarget.rotation = gripPoint.rotation;

        // Attach object to hand
        transform.SetParent(handBone, true);

        isHeld = true;
        isInteracting = false;
    }
}
