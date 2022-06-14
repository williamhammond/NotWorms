using Characters;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class NotWormsNetworkManager : NetworkManager
    {
        [SerializeField]
        private TurnController turnControllerPrefab;

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Network"))
            {
                TurnController turnControllerInstance = Instantiate(turnControllerPrefab);
                NetworkServer.Spawn(turnControllerInstance.gameObject);
            }
        }
    }
}
