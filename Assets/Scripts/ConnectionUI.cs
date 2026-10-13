using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using TMPro;

namespace CollaborativeVirtualWorlds
{
    public class ConnectionUI : MonoBehaviour
    {
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startServerButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private TextMeshProUGUI connectionStatusText;

        void Start()
        {
            StartCoroutine(WaitForNetWorkManagerInitialization());
        }

        IEnumerator WaitForNetWorkManagerInitialization()
        {
            yield return new WaitWhile(() => NetworkManager.Singleton == null);

            NetworkManager.Singleton.OnServerStarted += UpdateConnectionStatusText;

            startHostButton.onClick.AddListener(OnStartHostButtonClick);
            startServerButton.onClick.AddListener(OnStartServerButtonClick);
            startClientButton.onClick.AddListener(OnStartClientButtonClick);

            UpdateConnectionStatusText();

        }
        void OnDestroy()
        {
            startHostButton.onClick.RemoveListener(OnStartHostButtonClick);
            startServerButton.onClick.RemoveListener(OnStartServerButtonClick);
            startClientButton.onClick.RemoveListener(OnStartClientButtonClick);
        }
        void OnStartHostButtonClick()
        {
            if (NetworkManager.Singleton == null)
                return;

            ConnectionManager.StartHost();
        }

        void OnStartServerButtonClick()
        {
            if (NetworkManager.Singleton == null)
                return;

            ConnectionManager.StartServer();
        }

        void OnStartClientButtonClick()
        {
            if (NetworkManager.Singleton == null)
                return;

            ConnectionManager.StartClient();
        }

        string GetStatusText()
        {
            if (NetworkManager.Singleton == null || (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer))
            {
                return "NotConnected";
            }
            if (NetworkManager.Singleton.IsHost)
            {
                return "Type: Host";
            }
            if (NetworkManager.Singleton.IsServer)
            {
                return "Type: Server";
            }
            if (NetworkManager.Singleton.IsClient)
            {
                return "Type: Client";
            }
            else
            {
                return "error";
            }
        }

        void UpdateConnectionStatusText()
        {
            if (connectionStatusText == null)
                return;

            connectionStatusText.text = GetStatusText();
        }

    }
}
