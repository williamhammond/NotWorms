﻿using UnityEngine;

namespace Characters
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public float HorizontalMovement => Input.GetAxis("Horizontal");

        public bool NextTurn => Input.GetKey(KeyCode.R);

        public float lastDebouncedActionDTime = Time.time;
        private float debouncedActionThreshold = .5f;

        public bool canDebouncedAction()
        {
            return (Time.time - lastDebouncedActionDTime) > debouncedActionThreshold;
        }
    }
}
