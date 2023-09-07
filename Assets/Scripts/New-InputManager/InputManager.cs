using Photon.Pun;
using UnityEngine;

namespace FPS
{
    public class InputManager : MonoBehaviour
    {
        public PhotonView photonView;
        private PlayerInput playerInput;
        private PlayerInput.OnFootActions OnFoot;

        private PlayerController player;
   
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            player = GetComponent<PlayerController>();

            playerInput = new PlayerInput();
            OnFoot = playerInput.OnFoot;
            OnFoot.Jump.performed += Jump => player.Jump();
            OnFoot.Fire.performed += Fire => player.Fire(); 
            OnFoot.Reload.performed += Reload => player.Reload();
        }

        private void OnEnable()
        {
            OnFoot.Enable();
        }

        private void OnDisable()
        {
            OnFoot.Disable(); 
        }

        private void FixedUpdate()
        {
            player.Move(OnFoot.Movement.ReadValue<Vector2>());
        }
    }
}