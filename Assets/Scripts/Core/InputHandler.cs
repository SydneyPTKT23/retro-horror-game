using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InputHandler : MonoBehaviour
    {
        private Vector2 m_inputVector;

        public Vector2 InputVector => m_inputVector;
        public bool InputDetected => m_inputVector != Vector2.zero;

        private void Update()
        {
            GetMovement();
        }


        private void GetMovement()
        {
            m_inputVector.x = Input.GetAxisRaw("Horizontal");
            m_inputVector.y = Input.GetAxisRaw("Vertical");



        }
    }
}