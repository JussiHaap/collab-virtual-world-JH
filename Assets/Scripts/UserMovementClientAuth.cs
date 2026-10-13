using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.InputSystem;

namespace CollaborativeVirtualWorlds
{
    [RequireComponent(typeof(NetworkTransform))]
    public class UserMovementClientAuth : NetworkBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 10f;

        private void Awake()
        {
            NetworkTransform networkTransform = GetComponent<NetworkTransform>();
            networkTransform.AuthorityMode = NetworkTransform.AuthorityModes.Owner;
        }

        void Update()
        {
            if (!IsOwner)
                return;


        }

        void ProduceMoveInput(out Vector2 moveInput)
        {
            Keyboard keyboard = Keyboard.current;
            moveInput = Vector2.zero;

            if (keyboard.aKey.isPressed)
            {
                moveInput.x += -1f;
            }
            if (keyboard.dKey.isPressed)
            {
                moveInput.x += 1f;
            }
            if (keyboard.wKey.isPressed)
            {
                moveInput.y += 1f;
            }
            if (keyboard.sKey.isPressed)
            {
                moveInput.y -= 1f;
            }
        }

        void Move(Vector2 moveInput)
        {
            moveInput = moveInput.normalized;
            Vector3 moveDelta = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
            transform.Translate(moveDelta * Time.deltaTime);
         }
    }
}
