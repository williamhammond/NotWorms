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
        private CurrentTurnText turnText;
        private PlayerInput _playerInput;

        public float lastDebouncedActionDTime = Time.time;
        private float debouncedActionThreshold = .5f;

        public bool canDebouncedAction()
        {
            return (Time.time - lastDebouncedActionDTime) > debouncedActionThreshold;
        }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Player/EndTurn"].performed += HandleNextTurn;

            Player.PlayerSpawned += HandlePlayerSpawned;
            Player.PlayerDespawned += HandlePlayerDespawned;
        }

        public void OnDestroy()
        {
            Player.PlayerSpawned -= HandlePlayerSpawned;
            Player.PlayerDespawned -= HandlePlayerDespawned;
        }

        private void HandleNextTurn(InputAction.CallbackContext context)
        {
            if (canDebouncedAction())
            {
                currentTurn++;
                if (currentTurn >= turnOrder.Count)
                {
                    currentTurn = 0;
                }
                TurnChanged?.Invoke(currentTurn);
            }
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
    }
}
