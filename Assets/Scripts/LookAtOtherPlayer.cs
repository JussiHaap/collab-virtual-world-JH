
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtOtherPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private ChainIKConstraint lookConstraint;

    private Transform otherHead;

    public override void OnNetworkSpawn()
    {
        TryAssignTarget();
    }

    private void Update()
    {
        if (otherHead == null)
        {
            TryAssignTarget();
        }

        if (otherHead != null && lookTarget != null)
        {
            // Move the existing target to the other player's head.
            lookTarget.position = otherHead.position;
        }
    }


    private void TryAssignTarget()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (NetworkManager.Singleton.SpawnManager == null)
            return;

        var players = NetworkManager.Singleton
            .SpawnManager.SpawnedObjects;

        foreach (var player in players.Values)
        {
            if (player.OwnerClientId == OwnerClientId)
                continue;

            LookAtOtherPlayer other =
                player.GetComponent<LookAtOtherPlayer>();

            if (other == null || other.head == null)
                continue;

            otherHead = other.head;

            Debug.Log(
                $"{OwnerClientId} is now following " +
                $"player {player.OwnerClientId}"
            );

            break;
        }
    }

}
