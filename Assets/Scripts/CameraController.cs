using UnityEngine;

public class CameraController : MonoBehaviour {

    public float sensX;
    public float sensY;

    [Range(0, 89)] public float maxXAngle = 85f;

    public float maximumMovementSpeed = 1f;
    public float sprintModifier = 4f;
    
    [Header("Controls")]

    public KeyCode Forwards = KeyCode.W;
    public KeyCode Backwards = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Up = KeyCode.Space;
    public KeyCode Down = KeyCode.LeftControl;
    public KeyCode Sprint = KeyCode.LeftShift;

    private float _maxMove;
    private Vector3 _moveSpeed;
    private bool _sprint;
    private float _rotX;

    private void Start() {
        _moveSpeed = Vector3.zero;
        _maxMove = maximumMovementSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            HandleMouseRotation();
        }
    }
    
    private void FixedUpdate() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            HandleKeyInput();
        } else {
            _moveSpeed = Vector3.zero;
        }

        // clamp the move speed
        _maxMove = _sprint ? maximumMovementSpeed * 4 : maximumMovementSpeed;

        if (_moveSpeed.magnitude > _maxMove) {
            _moveSpeed = _moveSpeed.normalized * _maxMove;
        }

        transform.Translate(_moveSpeed);
    }

    private void HandleKeyInput() {
        _sprint = Input.GetKey(Sprint);
        bool keyDown = false;

        //key input detection
        if (Input.GetKey(Forwards)) {
            keyDown = true;
            _moveSpeed.z += 0.01f * (_sprint ? sprintModifier : 1);
        } else {
            if (_moveSpeed.z > 0) _moveSpeed.z -= 0.02f;
        }

        if (Input.GetKey(Backwards)) {
            keyDown = true;
            _moveSpeed.z -= 0.01f;
        } else {
            if (_moveSpeed.z < 0) _moveSpeed.z += 0.02f;
        }

        if (Input.GetKey(Left)) {
            keyDown = true;
            _moveSpeed.x -= 0.01f;
        } else {
            if (_moveSpeed.x < 0) _moveSpeed.x += 0.02f;
        }

        if (Input.GetKey(Right)) {
            keyDown = true;
            _moveSpeed.x += 0.01f;
        } else {
            if (_moveSpeed.x > 0) _moveSpeed.x -= 0.02f;
        }

        if (Input.GetKey(Up)) {
            keyDown = true;
            _moveSpeed.y += 0.01f;
        } else {
            if (_moveSpeed.y > 0) _moveSpeed.y -= 0.02f;
        }

        if (Input.GetKey(Down)) {
            keyDown = true;
            _moveSpeed.y -= 0.01f;
        } else {
            if (_moveSpeed.y < 0) _moveSpeed.y += 0.02f;
        }

        // Apply sprint modifier
        if (_sprint) {
            _moveSpeed *= sprintModifier;
        }

        // Reset _moveSpeed if no key is down
        if (!keyDown) {
            _moveSpeed = Vector3.zero;
        }
    }

    private void HandleMouseRotation() {
        float rotHori = sensX * Input.GetAxis("Mouse X");
        float rotVert = sensY * Input.GetAxis("Mouse Y");

        // always rotate Y in global world space to avoid gimbal lock
        transform.Rotate(Vector3.up * rotHori, Space.World);

        float rotY = transform.localEulerAngles.y;

        _rotX += rotVert;
        _rotX = Mathf.Clamp(_rotX, -maxXAngle, maxXAngle);

        transform.localEulerAngles = new Vector3(-_rotX, rotY, 0);
    }
}
