using System;
using UnityEngine;

namespace Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private Transform projectile;

        [SerializeField]
        private float timer;

        [SerializeField]
        private float cooldown;

        private void Awake()
        {
            timer = Mathf.Infinity;
        }

        private void Update()
        {
            timer += Time.deltaTime;
        }

        public bool OnCooldown()
        {
            return timer < cooldown;
        }

        public void Fire()
        {
            if (!OnCooldown())
            {
                timer = 0;
                Transform clone = Instantiate(projectile, firePoint.position, firePoint.rotation);
                clone.GetComponent<Combat.Projectile>().Setup(Mathf.Sign(transform.localScale.x));
            }
        }
    }
}
