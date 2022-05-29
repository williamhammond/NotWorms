using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private float maxLifetime = 10;

        [SerializeField]
        private int damage;

        private Animator _animator;
        private BoxCollider2D _boxCollider;

        private bool _isHit = false;
        private float _direction;
        private static readonly int ExplodeID = Animator.StringToHash("explode");
        private float _explosionTime;

        private static readonly float Tolerance = 1f;

        #region Server


        public void Setup(float direction)
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (ExplodeID == Animator.StringToHash(clip.name))
                {
                    _explosionTime = clip.length;
                }
            }
            SetDirection(direction);
        }

        public override void OnStartServer()
        {
            Invoke(nameof(Destroy), maxLifetime);
        }

        [Server]
        public void Destroy()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void Update()
        {
            if (_isHit)
            {
                return;
            }

            float velocity = speed * Time.deltaTime * _direction;
            transform.Translate(velocity, 0, 0);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            _isHit = true;
            _boxCollider.enabled = false;
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                player.TakeDamage(damage);
            }
            _animator.SetTrigger(ExplodeID);
        }

        #endregion


        private void SetDirection(float direction)
        {
            _direction = direction;

            float localScaleX = transform.localScale.x;
            if (Math.Abs(Mathf.Sign(localScaleX) - _direction) > Tolerance)
            {
                localScaleX = -localScaleX;
            }

            var localScale = transform.localScale;
            localScale = new Vector3(localScaleX, localScale.y, localScale.z);
            transform.localScale = localScale;
        }
    }
}
