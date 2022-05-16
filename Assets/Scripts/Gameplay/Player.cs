using System;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private float speed = 5f;

        [SerializeField]
        private float health = 100f;

        [SerializeField]
        private float energy = 100f;

        [SerializeField]
        private bool isPlayer;

        [SerializeField]
        private TurnController turnController;

        private CurrentTurnText turnText;
        private EnergyLabel energyLabel;

        public PlayerInput playerInput { get; set; }

        private Rigidbody2D _body;
        private Animator _animator;
        private Weapon _weapon;
        private float _deathAnimationTime;

        private bool _isJumping = false;

        private static readonly int IsRunningID = Animator.StringToHash("isRunning");
        private static readonly int IsJumpingID = Animator.StringToHash("isJumping");
        private static readonly int AttackID = Animator.StringToHash("attack");
        private static readonly int DeathID = Animator.StringToHash("death");

        private void Awake()
        {
            if (isPlayer)
            {
                playerInput = new PlayerInput();
                energyLabel = FindObjectOfType<EnergyLabel>();
                turnText = FindObjectOfType<CurrentTurnText>();
            }

            _body = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _weapon = GetComponent<Weapon>();

            turnController.AddPlayer(this);

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (DeathID == Animator.StringToHash(clip.name))
                {
                    _deathAnimationTime = clip.length;
                }
            }
        }

        void Update()
        {
            MoveHorizontal();
            HandleOrientation();

            _animator.SetBool(IsJumpingID, _isJumping);
            if (isPlayer && playerInput.Jump && !_isJumping)
            {
                if (energy > 0)
                {
                    Jump();
                }
            }

            if (isPlayer && playerInput.Fire && CanAttack())
            {
                _animator.SetTrigger(AttackID);
                _weapon.Fire();
            }

            if (isPlayer && playerInput.ResetEnergy)
            {
                energy = 100f;
                UpdateEnergyLabel();
            }

            if (isPlayer && playerInput.NextTurn && playerInput.canDebouncedAction())
            {
                NextTurn();
            }

            _weapon.IncrementTimer(Time.deltaTime);
        }

        void MoveHorizontal()
        {
            if (CanMove() && isPlayer)
            {
                _body.velocity = new Vector2(
                    playerInput.HorizontalMovement * speed,
                    _body.velocity.y
                );

                if (Math.Abs(playerInput.HorizontalMovement) > 0f)
                {
                    energy -= .1f;
                    UpdateEnergyLabel();
                }
            }

            if (isPlayer)
            {
                _animator.SetBool(IsRunningID, playerInput.HorizontalMovement != 0);
            }
        }

        void Jump()
        {
            _body.velocity = new Vector2(_body.velocity.x, speed);
            _isJumping = true;
            energy -= 10f;
            UpdateEnergyLabel();
        }

        private bool CanAttack()
        {
            return playerInput.HorizontalMovement == 0 && !_isJumping && !_weapon.OnCooldown();
        }

        private void HandleOrientation()
        {
            if (!isPlayer)
            {
                return;
            }

            if (playerInput.HorizontalMovement > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (playerInput.HorizontalMovement < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.CompareTag("Ground"))
            {
                _isJumping = false;
            }
        }

        public float GetHealth()
        {
            return health;
        }

        public void UpdateEnergyLabel()
        {
            if (isPlayer)
            {
                energyLabel.UpdateEnergy(energy);
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log($"Player health is {health}");
            if (!IsAlive())
            {
                _animator.SetTrigger(DeathID);
                Destroy(gameObject, _deathAnimationTime);
            }
        }

        void NextTurn()
        {
            Debug.Log("NextTurn");
            turnController.NextTurn();
            turnText.UpdateTurn(turnController.currentTurn);
            this.playerInput.lastDebouncedActionDTime = Time.time;
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public bool CanMove()
        {
            return energy > 0;
        }
    }
}
