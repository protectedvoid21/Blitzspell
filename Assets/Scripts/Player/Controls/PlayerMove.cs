using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float footstepsSpeed = 0.5f;

    private Vector2 horizontal;
    private bool isGrounded;
    private bool jump;
    private Vector3 verticalVelocity = Vector3.zero;
    private bool playingFootsteps;

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
        if (isGrounded) verticalVelocity.y = 0f;

        var horizontalVelocity = (transform.right * horizontal.x + transform.forward * horizontal.y) * moveSpeed;
        controller.Move(horizontalVelocity * Time.deltaTime);

        if (jump)
        {
            if (isGrounded) verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
            jump = false;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);

        switch (horizontalVelocity.magnitude)
        {
            case > 0f when !playingFootsteps && isGrounded:
                StartFootsteps();
                break;
            case > 0f when !isGrounded:
            case 0f:
                StopFootsteps();
                break;
        }
    }

    public void ReciveInput(Vector2 horizontalInput)
    {
        horizontal = horizontalInput;
    }

    public void OnJumpPressed()
    {
        jump = true;
    }

    private void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootsteps), 0f, footstepsSpeed);
    }

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