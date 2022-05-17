using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utils;

namespace Characters
{
    public class TurnController : MonoBehaviour
    {
        public static Action<int> TurnChanged;

        [SerializeField]
        public int currentTurn = 0;

        private List<Player> turnOrder = new();
        private CurrentTurnText turnText;

        private void Awake()
        {
            Player.PlayerSpawned += HandlePlayerSpawned;
            Player.PlayerDespawned += HandlePlayerDespawned;
            Player.EndTurn += HandleNextTurn;
        }

        public void OnDestroy()
        {
            Player.PlayerSpawned -= HandlePlayerSpawned;
            Player.PlayerDespawned -= HandlePlayerDespawned;
            Player.EndTurn -= HandleNextTurn;
        }

        public void AddPlayer(Player player) { }

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

        private void HandlePlayerSpawned(Player player)
        {
            if (turnOrder.Contains(player))
                return;

            turnOrder.Add(player);
        }

        private void HandlePlayerDespawned(Player player)
        {
            turnOrder.Remove(player);
        }

        private void HandleNextTurn()
        {
            currentTurn++;
            if (currentTurn >= turnOrder.Count)
            {
                currentTurn = 0;
            }
            TurnChanged?.Invoke(currentTurn);
        }
    }
}
