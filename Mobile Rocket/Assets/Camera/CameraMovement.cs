using UnityEngine;

namespace Assets.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform LookAt;
        public Transform CameraRoot;

        private void LateUpdate()
        {
            CameraRoot.localPosition = new Vector3(LookAt.position.x, LookAt.position.y + 3.57f, CameraRoot.position.z);
            transform.localPosition = new Vector3(Mathf.Sin(Time.fixedTime / 2) * 4, Mathf.Sin(Time.fixedTime / 2) / 4);
            transform.rotation = Quaternion.LookRotation(LookAt.position - transform.position, Vector3.up);
        }
    }
}
