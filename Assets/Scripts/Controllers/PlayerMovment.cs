using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private CharacterController characterController;
       
        [SerializeField] private float speed = 6f;
        [SerializeField] private float rotateSpeed = 3f;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float jumpHeight = 1.5f;

        private bool isGrounded;
        private Vector3 moveDirection = Vector3.zero;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        //Move and rotate with new Input system
        internal void ProcessMove(Vector2 input)
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded)
            {
                moveDirection = new Vector3(0, 0, input.y);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
            }
            moveDirection.y += gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, input.x, 0);
        }

        // Jump on space key
        internal void Jump()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded)
            {
                moveDirection.y = jumpHeight;
            }
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
}
