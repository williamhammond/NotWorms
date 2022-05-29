using System;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private GameObject projectile;

        [SerializeField]
        private float timer;

        [SerializeField]
        private float cooldown;

        #region Server

        public override void OnStartServer()
        {
            timer = Mathf.Infinity;
        }

        [ServerCallback]
        private void Update()
        {
            timer += Time.deltaTime;
        }

        public bool OnCooldown()
        {
            return timer < cooldown;
        }

        [Server]
        public void Fire()
        {
            if (!OnCooldown())
            {
                timer = 0;
                GameObject clone = Instantiate(projectile, firePoint.position, firePoint.rotation);
                clone.GetComponent<Combat.Projectile>().Setup(Mathf.Sign(transform.localScale.x));
                NetworkServer.Spawn(clone, connectionToClient);
            }
        }

        #endregion
    }
}
