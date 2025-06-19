using Menu.PauseMenu;
using Player.Abilities;
using UnityEngine;

namespace Player.Controls
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerMove movement;
        [SerializeField] private PlayerLook look;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private PlayerShield shield;
        
        [SerializeField] private GameObject pauseMenu;

        private PlayerControls controls;

        private Vector2 horizontal;
        private Vector2 mouseInput;
        private PlayerControls.PlayerActions player;

        private void Awake()
        {
            controls = new PlayerControls();
            player = controls.Player;
            var pause = pauseMenu.GetComponent<PauseMenu>();

            player.Move.performed += ctx => horizontal = ctx.ReadValue<Vector2>();
            player.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            player.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
            player.Attack.performed += _ => spellCaster.OnPrimarySpellCast();
            player.SecondaryAttack.performed += _ => spellCaster.OnSecondarySpellCast();
            player.Shield.performed += _ => shield.OnShieldActive();
            player.Pause.performed += _ => pause.Pause();
            player.Resume.performed += _ => pause.Resume();
        }

        private void Update()
        {
            if (PauseScript.GetIsGamePaused()) return;
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
}