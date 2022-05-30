using System.Collections.Generic;
using Mirror;
using UI;
using UnityEngine;
using Utils;

namespace Characters
{
    public class TurnController : NetworkBehaviour
    {
        [SerializeField]
        public int currentTurn = 0;

        private List<Player> turnOrder = new();
        private CurrentTurnText turnText;

        public float lastDebouncedActionDTime;
        private readonly float debouncedActionThreshold = .5f;

        private bool CanDebouncedAction()
        {
            return (Time.time - lastDebouncedActionDTime) > debouncedActionThreshold;
        }

        private void Awake()
        {
            lastDebouncedActionDTime = Time.time;
            turnText = FindObjectOfType<CurrentTurnText>();
        }

        #region Server

        public override void OnStartServer()
        {
            Player.ServerOnPlayerSpawned += ServerHandleOnPlayerSpawned;
            Player.ServerOnPlayerDespawned += ServerHandleOnPlayerDespawned;

            Player.ServerPlayerEndedTurn += ServerHandleNextTurn;
        }

        public override void OnStopServer()
        {
            Player.ServerOnPlayerSpawned -= ServerHandleOnPlayerSpawned;
            Player.ServerOnPlayerDespawned -= ServerHandleOnPlayerDespawned;

            Player.ServerPlayerEndedTurn -= ServerHandleNextTurn;
        }

        [Server]
        private void ServerHandleOnPlayerSpawned(Player player)
        {
            Debug.Log($"Adding player {player} to the turn controller");
            if (turnOrder.Contains(player))
            {
                return;
            }

            turnOrder.Add(player);
        }

        [Server]
        private void ServerHandleOnPlayerDespawned(Player player)
        {
            turnOrder.Remove(player);
        }

        [Server]
        private void ServerHandleNextTurn()
        {
            if (CanDebouncedAction())
            {
                currentTurn++;
                if (currentTurn > turnOrder.Count - 1)
                {
                    currentTurn = 0;
                }

                Debug.Log($"New Turn: {currentTurn}");
                RpcSetTurnText(currentTurn);
            }
        }

        public void ResetTurnOrder()
        {
            turnOrder = (List<Player>)turnOrder.Shuffle();
            currentTurn = 0;
        }

        public Player CurrentPlayer()
        {
            if (turnOrder.Count < 1)
            {
                return null;
            }

            return turnOrder[currentTurn];
        }

        #endregion


        #region Client

        [ClientRpc]
        private void RpcSetTurnText(int turn)
        {
            turnText.SetTurn(turn);
        }

        #endregion
    }
}
