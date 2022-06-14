using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private Button buttonHost;

        [SerializeField]
        private Button buttonServer;

        [SerializeField]
        private Button buttonClient;

        [SerializeField]
        private Button buttonStop;

        [SerializeField]
        private GameObject panelStart;

        [SerializeField]
        private GameObject panelStop;

        [SerializeField]
        private TMP_InputField inputFieldAddress;

        [SerializeField]
        private TMP_Text serverText;

        [SerializeField]
        private TMP_Text clientText;

        private void Start()
        {
            if (NetworkManager.singleton.networkAddress != "localhost")
            {
                inputFieldAddress.text = NetworkManager.singleton.networkAddress;
            }

            inputFieldAddress.onValueChanged.AddListener(
                delegate
                {
                    ValueChangeCheck();
                }
            );

            buttonHost.onClick.AddListener(ButtonHost);
            buttonServer.onClick.AddListener(ButtonServer);
            buttonClient.onClick.AddListener(ButtonClient);
            buttonStop.onClick.AddListener(ButtonStop);

            SetupCanvas();
        }

        private void ValueChangeCheck()
        {
            NetworkManager.singleton.networkAddress = inputFieldAddress.text;
        }

        private void ButtonHost()
        {
            NetworkManager.singleton.StartHost();
            SetupCanvas();
        }

        private void ButtonServer()
        {
            NetworkManager.singleton.StartServer();
            SetupCanvas();
        }

        private void ButtonClient()
        {
            NetworkManager.singleton.StartClient();
            SetupCanvas();
        }

        private void ButtonStop()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }

            SetupCanvas();
        }

        private void SetupCanvas()
        {
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                if (NetworkClient.active)
                {
                    panelStart.SetActive(false);
                    panelStop.SetActive(true);
                    clientText.text =
                        "Connecting to " + NetworkManager.singleton.networkAddress + "..";
                }
                else
                {
                    panelStart.SetActive(true);
                    panelStop.SetActive(false);
                }
            }
            else
            {
                panelStart.SetActive(false);
                panelStop.SetActive(true);

                if (NetworkServer.active)
                {
                    serverText.text = "Server: active. Transport: " + Transport.activeTransport;
                }

                if (NetworkClient.isConnected)
                {
                    clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
                }
            }
        }
    }
}
