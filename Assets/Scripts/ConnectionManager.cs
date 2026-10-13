using Unity.Netcode;
using UnityEngine;

namespace CollaborativeVirtualWorlds
{
    public class ConnectionManager : MonoBehaviour
    {
        private static ConnectionManager instance;

        private void Awake()
        {
            instance = this;
        }

        public static void StartHost()
        {
            Debug.Log("Host started.");
            NetworkManager.Singleton.StartHost();
        }

        public static void StartServer()
        {
            Debug.Log("Server started.");
            NetworkManager.Singleton.StartServer();
        }

        public static void StartClient()
        {
            Debug.Log("Client started.");
            NetworkManager.Singleton.StartClient();
        }
    }
}
