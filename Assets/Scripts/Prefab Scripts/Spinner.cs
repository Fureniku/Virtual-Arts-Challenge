using UnityEngine;

namespace Prefab_Scripts {
    public class Spinner : MonoBehaviour {

        [SerializeField] private float speed = 100f;
        [SerializeField] private bool clockwise = true;
        
        void FixedUpdate() {
            transform.Rotate(Vector3.up, (clockwise ? 1 : -1) * speed * Time.deltaTime);
        }
    }
}
