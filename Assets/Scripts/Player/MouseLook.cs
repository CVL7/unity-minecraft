using UnityEngine;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] float sensitivity = 10f;

        [SerializeField] Transform viewCamera;
        Transform characterTransform;

        static bool mouseLook
        {
            set
            {      
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            }

            get
            {
                return Cursor.lockState == CursorLockMode.Locked;
            }
        }

        void Start()
        {
            characterTransform = transform;
            mouseLook = true;
        }

        void Update()
        {
           // if (!mouseLook) return;

            var yRot = Input.GetAxisRaw("Mouse X") * sensitivity;
            var xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;

            characterTransform.localRotation *= Quaternion.Euler(0f, yRot, 0f);
            viewCamera.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);
            LimitLook(xRot);
        }

        void LimitLook(float xRot)
        {
            if (Quaternion.Angle(Quaternion.identity, viewCamera.localRotation) > 90)
            {
                if (xRot > 0)
                {
                    viewCamera.localRotation = Quaternion.Euler(-90, 0, 0);
                }
                else
                {
                    viewCamera.localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            mouseLook = hasFocus;
        }
    }
}
