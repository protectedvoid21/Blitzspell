using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerMove movement;
    [SerializeField] private PlayerLook look;
    [SerializeField] private SpellCaster spellCaster;
    [SerializeField] private PlayerShield shield;

    private PlayerControls controls;

    private Vector2 horizontal;
    private Vector2 mouseInput;
    private PlayerControls.PlayerActions player;

    private void Awake()
    {
        controls = new PlayerControls();
        player = controls.Player;

        player.Move.performed += ctx => horizontal = ctx.ReadValue<Vector2>();
        player.Jump.performed += _ => movement.OnJumpPressed();
        player.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        player.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        player.Attack.performed += _ => spellCaster.OnPrimarySpellCast();
        player.SecondaryAttack.performed += _ => spellCaster.OnSecondarySpellCast();
        player.Shield.performed += _ => shield.OnShieldActive();
    }

    private void Update()
    {
        movement.ReciveInput(horizontal);
        look.ReciveInput(mouseInput);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}