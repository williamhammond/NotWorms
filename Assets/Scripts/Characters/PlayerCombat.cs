using System;
using Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Animator _animator;
        private Weapon _weapon;
        private Rigidbody2D _body;

        private static readonly int AttackID = Animator.StringToHash("attack");

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.actions["Player/Fire"].performed += HandleFire;

            _animator = GetComponent<Animator>();
            _weapon = GetComponent<Weapon>();
            _body = GetComponent<Rigidbody2D>();
        }

        private void HandleFire(InputAction.CallbackContext context)
        {
            if (CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }
        }

        private bool CanAttack()
        {
            return _body.velocity.x < 0.01f && _body.velocity.y < 0.01f && !_weapon.OnCooldown();
        }
    }
}
