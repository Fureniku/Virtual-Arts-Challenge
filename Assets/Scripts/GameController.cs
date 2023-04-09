using TMPro;
using UI;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject heldObject;
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject parentObject;
    
    //General non-movement controls
    public KeyCode SnapMode = KeyCode.F;
    public KeyCode FreeMouse = KeyCode.LeftAlt;
    public KeyCode PlayerType = KeyCode.F2;
    
    //When something isn't selected
    public KeyCode OpenSelection = KeyCode.Q;
    public KeyCode MakeSelection = KeyCode.Mouse0;
    
    //When something is selected
    public KeyCode EditSelection = KeyCode.E;
    public KeyCode DeleteSelection = KeyCode.Delete;
    public KeyCode ConfirmSelection = KeyCode.Mouse0;
    public KeyCode CancelSelection = KeyCode.Escape;
    
    //When something is being placed
    public KeyCode CancelPlacement = KeyCode.Escape;
    public KeyCode PlaceObject = KeyCode.Mouse0;
    public KeyCode ContinuousPlace = KeyCode.Mouse1;
    
    //In both edit modes
    public KeyCode RotateLeft = KeyCode.Z;
    public KeyCode RotateRight = KeyCode.X;
    public KeyCode RotateUp = KeyCode.C;
    public KeyCode RotateDown = KeyCode.V;
    public KeyCode ScaleUpUniform = KeyCode.RightBracket;
    public KeyCode ScaleDownUniform = KeyCode.LeftBracket;
    public KeyCode ScaleUpX = KeyCode.U;
    public KeyCode ScaleUpY = KeyCode.I;
    public KeyCode ScaleUpZ = KeyCode.O;
    public KeyCode ScaleDownX = KeyCode.J;
    public KeyCode ScaleDownY = KeyCode.K;
    public KeyCode ScaleDownZ = KeyCode.L;
    public KeyCode TogglePhysics = KeyCode.P;
    
    //Objects we need access to
    [SerializeField] private Material editMaterial;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private ItemPanelController itemInfoPanel;
    [SerializeField] private TextMeshProUGUI cameraInfoPanel;
    [SerializeField] private TextMeshProUGUI mouseInfoPanel;
    [SerializeField] private TextMeshProUGUI snapInfoPanel;
    [SerializeField] private TextMeshProUGUI physicsInfoPanel;
    [SerializeField] private GameObject controlsBasePanel;
    [SerializeField] private GameObject controlsPlacePanel;
    [SerializeField] private GameObject controlsEditPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CharacterController characterController;
    

    private bool _isSelecting;
    private bool _snapEnabled;
    private bool _editingObject;
    private bool _mouseLocked = true;
    private bool _enablePhysicsOnPlace;
    private bool _cameraControl = true;
    private bool _isPaused;
    
    private Vector3 _selectedStartPos;
    private Vector3 _selectedStartRot;
    private Vector3 _selectedStartScale;
    private Material _selectedStartMat;
    private Camera _camera;

    private void Awake() {
        _camera = Camera.main;
    }

    void Update() {
        if (!_isPaused) {
            if (Input.GetKeyDown(PlayerType)) {
                _cameraControl = !_cameraControl;
            
                cameraController.enabled = _cameraControl;
                characterController.enabled = !_cameraControl;

                if (!_cameraControl) {
                    _camera.transform.parent = characterController.transform;
                    _camera.transform.position = characterController.transform.position;
                }

                cameraInfoPanel.text = _cameraControl ? "Free Cam" : "Player Mode";
            }
        
            if (Input.GetKeyDown(OpenSelection)) {
                ToggleSelPanel(!_isSelecting);
                Destroy(heldObject);
                heldObject = null;
            }

            if (Input.GetKeyDown(SnapMode)) {
                _snapEnabled = !_snapEnabled;
                snapInfoPanel.SetText(_snapEnabled ? "Enabled" : "Disabled");
            }
        
            if (Input.GetKeyDown(TogglePhysics)) {
                _enablePhysicsOnPlace = !_enablePhysicsOnPlace;
                physicsInfoPanel.SetText(_enablePhysicsOnPlace ? "Enabled" : "Disabled");
            }

            if (Input.GetKeyDown(FreeMouse)) {
                SetMouseLockState(!_mouseLocked);
            }
            if (heldObject != null) {
                controlsBasePanel.SetActive(false);
                controlsPlacePanel.SetActive(true);
                controlsEditPanel.SetActive(false);
                PlaceNewObject();
            } else if (selectedObject != null) {
                controlsBasePanel.SetActive(false);
                controlsPlacePanel.SetActive(false);
                controlsEditPanel.SetActive(true);
                HandleSelectedObject();
            } else {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    TogglePause();
                }
                controlsBasePanel.SetActive(true);
                controlsPlacePanel.SetActive(false);
                controlsEditPanel.SetActive(false);
                SelectObject();
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                TogglePause();
            }
        }
    }

    public void TogglePause() {
        _isPaused = !_isPaused;
        pauseMenuPanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    //We have an object being held, ready for placement.
    private void PlaceNewObject() {
        HandleKeyRotation(heldObject);
        HandleKeyScaling(heldObject);
        
        Quaternion transformRotation = heldObject.transform.rotation;
        int layerMask = (1 << 6) | (1 << 7); //only target ground layers and existing placed objects
            
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
            Vector3 pos = new Vector3(hit.point.x, hit.point.y + (heldObject.transform.localScale.y/2), hit.point.z);

            if (_snapEnabled) {
                pos = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
            }

            heldObject.transform.position = pos;

            if (Input.GetKeyDown(PlaceObject) || Input.GetKeyDown(ContinuousPlace)) {
                GameObject placed = Instantiate(heldObject, pos, transformRotation);
                placed.name = heldObject.name;
                placed.transform.parent = parentObject.transform;
                placed.layer = 7;
                placed.GetComponent<PlaceableObject>().SetPlaced(MaterialsRegistry.Instance.GetMaterial(0), _enablePhysicsOnPlace);
                if (Input.GetKeyDown(PlaceObject) || _editingObject) {
                    Destroy(heldObject);
                    heldObject = null;
                    _editingObject = false;
                }
            }
        }

        if (Input.GetKeyDown(CancelPlacement)) {
            if (_editingObject) {
                heldObject.transform.position = _selectedStartPos;
                heldObject.transform.eulerAngles = _selectedStartRot;
                heldObject.transform.localScale = _selectedStartScale;
                heldObject.GetComponent<PlaceableObject>().SetPlaced(_selectedStartMat, _enablePhysicsOnPlace);
            } else {
                Destroy(heldObject);
            }
            heldObject = null;
            itemInfoPanel.gameObject.SetActive(false);
        }
    }

    //We have selected an existing object in the world to edit
    private void HandleSelectedObject() {
        itemInfoPanel.UpdatePanel(selectedObject);

        HandleKeyRotation(selectedObject);
        HandleKeyScaling(selectedObject);
        selectedObject.GetComponent<PlaceableObject>().SetPickedUp(editMaterial);
        if (Input.GetKeyDown(EditSelection)) {
            heldObject = selectedObject;
            selectedObject = null;
            _editingObject = true;
        }

        if (Input.GetKeyDown(DeleteSelection)) {
            Destroy(selectedObject);
            _editingObject = false;
            itemInfoPanel.gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(CancelSelection)) {
            selectedObject.transform.position = _selectedStartPos;
            selectedObject.transform.eulerAngles = _selectedStartRot;
            selectedObject.transform.localScale = _selectedStartScale;
            selectedObject.GetComponent<PlaceableObject>().SetPlaced(_selectedStartMat, _enablePhysicsOnPlace);
            selectedObject = null;
            itemInfoPanel.gameObject.SetActive(false);
            return;
        }

        if ((Input.GetKeyDown(ConfirmSelection) && _mouseLocked) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            selectedObject.GetComponent<PlaceableObject>().SetPlaced(_selectedStartMat, _enablePhysicsOnPlace);
            selectedObject = null;
            itemInfoPanel.gameObject.SetActive(false);
            SetMouseLockState(true);
        }
    }

    //We have no current object interactions, and can select an object.    
    private void SelectObject() {
        if (Input.GetKeyDown(MakeSelection)) {
            int layerMask = 1 << 7; //only target ground layers and existing placed objects
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
                heldObject = null;
                GameObject selected = hit.transform.gameObject;
                
                //If we've clicked just part of the whole object, get the objects parent.
                if (selected.GetComponent<PlaceableChild>() != null) {
                    selected = selected.GetComponent<PlaceableChild>().GetParent();
                }

                selectedObject = selected;
                _selectedStartPos = selected.transform.position;
                _selectedStartRot = selected.transform.eulerAngles;
                _selectedStartScale = selected.transform.localScale;
                _selectedStartMat = selected.GetComponent<PlaceableObject>().GetMaterial();
                itemInfoPanel.gameObject.SetActive(true);
            }
        }
    }

    private void HandleKeyRotation(GameObject go) {
        if (_snapEnabled) {
            //Quickly rotate by 45 degrees
            if (Input.GetKeyDown(RotateLeft)) { go.transform.Rotate(0, -45, 0, Space.Self); }
            if (Input.GetKeyDown(RotateRight)) { go.transform.Rotate(0, 45, 0, Space.Self); }
            
            if (Input.GetKeyDown(RotateUp)) { go.transform.Rotate(-45, 0, 0, Space.Self); }
            if (Input.GetKeyDown(RotateDown)) { go.transform.Rotate(45, 0, 0, Space.Self); }
        } else {
            //Slowly rotate while key is held
            if (Input.GetKey(RotateLeft)) { go.transform.Rotate(0, -0.1f, 0, Space.Self); }
            if (Input.GetKey(RotateRight)) { go.transform.Rotate(0, 0.1f, 0, Space.Self); }
            
            if (Input.GetKey(RotateUp)) { go.transform.Rotate(-0.1f, 0, 0, Space.Self); }
            if (Input.GetKey(RotateDown)) { go.transform.Rotate(0.1f, 0, 0, Space.Self); }
        }
    }

    private void HandleKeyScaling(GameObject go) {
        Vector3 scale = go.transform.localScale;
        float mod = 0.01f; //How much to change by
        
        if (Input.GetKey(ScaleUpUniform)) { scale.x += mod; scale.y += mod; scale.z += mod; }
        if (Input.GetKey(ScaleDownUniform)) { scale.x -= mod; scale.y -= mod; scale.z -= mod; }
    
        if (Input.GetKey(ScaleUpX)) { scale.x += mod; }
        if (Input.GetKey(ScaleUpY)) { scale.y += mod; }
        if (Input.GetKey(ScaleUpZ)) { scale.z += mod; }
    
        if (Input.GetKey(ScaleDownX)) { scale.x -= mod; }
        if (Input.GetKey(ScaleDownY)) { scale.y -= mod; }
        if (Input.GetKey(ScaleDownZ)) { scale.z -= mod; }

        if (scale.x < 0.1f) { scale.x = 0.1f; }
        if (scale.y < 0.1f) { scale.y = 0.1f; }
        if (scale.z < 0.1f) { scale.z = 0.1f; }
        
        go.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }

    //Apply changes from the UI menu
    public void UpdateObjectFromUI(Vector3 pos, Vector3 rot, Vector3 scale) {
        if (selectedObject != null) {
            selectedObject.transform.position = pos;
            selectedObject.transform.eulerAngles = rot;
            selectedObject.transform.localScale = scale;
        } else if (heldObject != null) {
            heldObject.transform.position = pos; //Kind of pointless as mouse will move it back, but if they confirm without touching mouse it matters.
            heldObject.transform.eulerAngles = rot;
            heldObject.transform.localScale = scale;
        }
    }

    //Close the selection panel from other places, e.g. when we've selected an item from the UI we can close it.
    private void ToggleSelPanel(bool open) {
        _isSelecting = open;
        selectionPanel.SetActive(open);
        Cursor.lockState = open ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void SetHeldObject(GameObject heldIn) {
        ToggleSelPanel(false);
        heldObject = Instantiate(heldIn);
        heldObject.name = heldIn.name;
        selectedObject = null;
    }

    private void SetMouseLockState(bool locked) {
        _mouseLocked = locked;
        Cursor.lockState = _mouseLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
        Time.timeScale = _mouseLocked ? 1.0f : 0.0f;
        mouseInfoPanel.text = _mouseLocked ? "Locked" : "Unlocked";
    }
}
