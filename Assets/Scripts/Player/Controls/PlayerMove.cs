using Sound;
using UnityEngine;

namespace Player.Controls
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float footstepsSpeed = 0.5f;
        [SerializeField] private float groundCheckDistance = 0.15f;

        private Vector2 horizontal;
        private bool isGrounded;
        private bool playingFootsteps;

        private void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, out var hit, groundCheckDistance, groundMask);
            
            var moveDirection = transform.right * horizontal.x + transform.forward * horizontal.y;
            // Jeśli uderzyliśmy w ziemię i mamy normalną, dostosowujemy kierunek ruchu do powierzchni.
            if (isGrounded && hit.normal != Vector3.zero)
            {
                // Projekcja kierunku ruchu na płaszczyznę ziemi
                moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
            } else {
                moveDirection = moveDirection.normalized;
            }
            controller.Move(moveDirection * (moveSpeed * Time.deltaTime));
            
            if (!isGrounded)
                controller.Move(Physics.gravity * Time.deltaTime);

            switch (controller.velocity.magnitude)
            {
                case > 0f when !playingFootsteps && isGrounded:
                    StartFootsteps();
                    break;
                case > 0.1f when !isGrounded:
                case <= 0.1f:
                    StopFootsteps();
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
        }

        public void ReciveInput(Vector2 horizontalInput)
        {
            horizontal = horizontalInput;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void StartFootsteps()
        {
            playingFootsteps = true;
            InvokeRepeating(nameof(PlayFootsteps), 0f, footstepsSpeed);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void StopFootsteps()
        {
            playingFootsteps = false;
            CancelInvoke(nameof(PlayFootsteps));
        }

        private void PlayFootsteps()
        {
            SoundEffectManager.PlaySoundEffect("Player", true);
        }
    }
}