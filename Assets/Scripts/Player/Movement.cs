using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Movement physics heavily inspired by Quake
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        const float walkSpeed = 6f;
        const float runSpeed = walkSpeed * 2;
        const float maxSpeed = 20f;
        const float stopSpeed = runSpeed * 0.3125f;
        const float friction = 6f;
        const float groundAcceleration = 10f;
        const float airAcceleration = 2f;
        const float jumpHeight = 7f;

        float speed;
        Vector3 velocity = Vector3.zero;
        bool wishJump;

        CharacterController controller;
        Transform characterTransform;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            controller.minMoveDistance = 0;
            characterTransform = transform;
        }

        void Update()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            var wishSpeed = Input.GetKeyDown(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            if (Input.GetButtonDown("Jump")) wishJump = true;
            if (Input.GetButtonUp("Jump")) wishJump = false;

            var wishDirection = characterTransform.TransformDirection(input).normalized;

            if (controller.isGrounded)
            {
                if (wishJump)
                {
                    velocity.y = jumpHeight;
                    wishJump = false;
                }

                UpdateFriction();
                Accelerate(wishDirection, wishSpeed, groundAcceleration);
            }
            else
            {
                Accelerate(wishDirection, wishSpeed, airAcceleration);
            }

            speed = velocity.magnitude;
            CapSpeed(maxSpeed);

            velocity += 2 * Physics.gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            if (controller.isGrounded) velocity.y = 0;
        }

        void Accelerate(Vector3 wishDirection, float wishSpeed, float acceleration)
        {
            var speedProjection = Vector3.Dot(velocity, wishDirection);
            var addSpeed = wishSpeed - speedProjection;
            if (addSpeed <= 0) return;

            var accelSpeed = acceleration * wishSpeed * Time.deltaTime;

            accelSpeed = Mathf.Min(accelSpeed, addSpeed);
            velocity += accelSpeed * wishDirection;
        }

        void UpdateFriction()
        {
            var currentSpeed = velocity.magnitude;
            if (currentSpeed <= 0) return;

            var control = Mathf.Max(stopSpeed, currentSpeed);
            var newSpeed = currentSpeed - control * friction * Time.deltaTime;

            velocity *= Math.Max(newSpeed, 0) / currentSpeed;
        }

        void CapSpeed(float max)
        {
            if (speed <= max) return;

            var temp = velocity.y;
            velocity.y = 0;
            velocity *= max / speed;
            velocity.y = temp;
        }
    }
}