using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.Animation
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator animator;
        private PlayerMovement playerMovement;

        void Start()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            HandleAnimations();
        }

        private void HandleAnimations()
        {
            bool isWalking = playerMovement.IsWalking;
            bool isJumping = playerMovement.IsJumpping;
            //Debug.LogWarning("isWalking:" + isWalking + ",isJumping:" + isJumping);
            animator.SetBool(AnimationConstant.Walk, isWalking);
            animator.SetBool(AnimationConstant.Jump, isJumping);
        }
    }
}
