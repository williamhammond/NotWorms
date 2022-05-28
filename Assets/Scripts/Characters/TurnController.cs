using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Characters
{
    public class TurnController : MonoBehaviour
    {
        public static Action<int> TurnChanged;

        [SerializeField]
        public int currentTurn = 0;

        private List<Player> turnOrder = new();
        private readonly CurrentTurnText turnText;
        private PlayerInput _playerInput;

        public float lastDebouncedActionDTime;
        private readonly float debouncedActionThreshold = .5f;

        private bool CanDebouncedAction()
        {
            return (Time.time - lastDebouncedActionDTime) > debouncedActionThreshold;
        }

        private void Awake()
        {
            lastDebouncedActionDTime = Time.time;

            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Player/EndTurn"].performed += HandleNextTurn;

            Player.ServerOnPlayerSpawned += HandleServerOnPlayerSpawned;
            Player.ServerOnPlayerDespawned += HandleServerOnPlayerDespawned;
        }

        private void OnDestroy()
        {
            Player.ServerOnPlayerSpawned -= HandleServerOnPlayerSpawned;
            Player.ServerOnPlayerDespawned -= HandleServerOnPlayerDespawned;
        }

        private void HandleNextTurn(InputAction.CallbackContext context)
        {
            if (CanDebouncedAction())
            {
                currentTurn++;
                if (currentTurn >= turnOrder.Count)
                {
                    currentTurn = 0;
                }
                TurnChanged?.Invoke(currentTurn);
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

        private void HandleServerOnPlayerSpawned(Player player)
        {
            if (turnOrder.Contains(player))
                return;

            turnOrder.Add(player);
        }

        private void HandleServerOnPlayerDespawned(Player player)
        {
            turnOrder.Remove(player);
        }
    }
}
