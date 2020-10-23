using UnityEditor;
using UnityEngine;

namespace Assets.Rocket
{
    public class RocketController : MonoBehaviour
    {
        public Rigidbody RocketRigidbody;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var input = Input.GetTouch(0).deltaPosition;
                RocketRigidbody.AddForce(input, 0);
            }
        }
    }
}
