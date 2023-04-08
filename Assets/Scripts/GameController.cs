using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject heldObject;
    [SerializeField] private GameObject selectedObject;
    
    //General non-movement controls
    public KeyCode SnapMode = KeyCode.F;
    public KeyCode FreeMouse = KeyCode.LeftAlt;
    
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



        //Objects we need access to
    [SerializeField] private Material editMaterial;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private ItemPanelController itemInfoPanel;
    [SerializeField] private TextMeshProUGUI snapInfoPanel;

    private bool _isSelecting = false;
    private bool _snapEnabled = false;
    private bool _editingObject = false;
    private bool mouseLocked = true;
    
    private Vector3 _selectedStartPos;
    private Vector3 _selectedStartRot;
    private Vector3 _selectedStartScale;
    private Material _selectedStartMat;

    void Update() {
        if (Input.GetKeyDown(OpenSelection)) {
            ToggleSelPanel(!_isSelecting);
            Destroy(heldObject);
            heldObject = null;
        }

        if (Input.GetKeyDown(SnapMode)) {
            _snapEnabled = !_snapEnabled;
            snapInfoPanel.SetText(_snapEnabled ? "Enabled" : "Disabled");
        }

        if (Input.GetKeyDown(FreeMouse)) {
            Cursor.lockState = mouseLocked ? CursorLockMode.Confined : CursorLockMode.Locked;
            mouseLocked = !mouseLocked;
        }

        
        if (heldObject != null) {
            PlaceNewObject();
            
        //We have selected an existing object in the world to edit    
        } else if (selectedObject != null) {
            HandleSelectedObject();
            
        //We have no current object interactions, and can select an object.    
        } else {
            if (Input.GetKeyDown(MakeSelection)) {
                int layerMask = 1 << 7; //only target ground layers and existing placed objects
            
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
                    Debug.Log("Hit: " + hit.transform.gameObject.name);
                    SetSelectedObject(hit.transform.gameObject);
                }
            }
        }
    }

    //We have an object being held, ready for placement.
    private void PlaceNewObject() {
        HandleKeyRotation(heldObject);
        HandleKeyScaling(heldObject);
        
        Quaternion transformRotation = heldObject.transform.rotation;
        int layerMask = (1 << 6) | (1 << 7); //only target ground layers and existing placed objects
            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
            Vector3 pos = new Vector3(hit.point.x, hit.point.y + (heldObject.transform.localScale.y/2), hit.point.z);

            if (_snapEnabled) {
                pos = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
            }

            heldObject.transform.position = pos;

            if (Input.GetKeyDown(PlaceObject) || Input.GetKeyDown(ContinuousPlace)) {
                GameObject placed = Instantiate(heldObject, pos, transformRotation);
                placed.name = heldObject.name;
                placed.layer = 7;
                placed.GetComponent<PlaceableObject>().SetMaterial(MaterialsRegistry.Instance.GetMaterial(0));
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
                heldObject.GetComponent<PlaceableObject>().SetMaterial(_selectedStartMat);
                heldObject.layer = 7;
            } else {
                Destroy(heldObject);
            }
            heldObject = null;
            itemInfoPanel.gameObject.SetActive(false);
        }
    }

    private void HandleSelectedObject() {
        itemInfoPanel.UpdatePanel(selectedObject);

        HandleKeyRotation(selectedObject);
        HandleKeyScaling(selectedObject);
        selectedObject.GetComponent<PlaceableObject>().SetMaterial(editMaterial);
        selectedObject.layer = 0;
        if (Input.GetKeyDown(EditSelection)) {
            heldObject = selectedObject;
            selectedObject = null;
            _editingObject = true;
        }

        if (Input.GetKeyDown(DeleteSelection)) {
            Destroy(selectedObject);
            _editingObject = false;
        }

        if (Input.GetKeyDown(CancelSelection)) {
            selectedObject.transform.position = _selectedStartPos;
            selectedObject.transform.eulerAngles = _selectedStartRot;
            selectedObject.transform.localScale = _selectedStartScale;
            selectedObject.GetComponent<PlaceableObject>().SetMaterial(_selectedStartMat);
            selectedObject.layer = 7;
            selectedObject = null;
            itemInfoPanel.gameObject.SetActive(false);
        }

        if ((Input.GetKeyDown(ConfirmSelection) && mouseLocked) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            selectedObject.GetComponent<PlaceableObject>().SetMaterial(_selectedStartMat);
            selectedObject.layer = 7;
            selectedObject = null;
            itemInfoPanel.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
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
        if (go != null) {
            Vector3 scale = go.transform.localScale;
            float mod = 0.01f; //How much to change by
            if (Input.GetKey(ScaleUpUniform)) { go.transform.localScale = new Vector3(scale.x+mod, scale.y+mod, scale.z+mod); }
            if (Input.GetKey(ScaleDownUniform)) { go.transform.localScale = new Vector3(scale.x-mod, scale.y-mod, scale.z-mod); }
        
            if (Input.GetKey(ScaleUpX)) { go.transform.localScale = new Vector3(scale.x+mod, scale.y, scale.z); }
            if (Input.GetKey(ScaleUpY)) { go.transform.localScale = new Vector3(scale.x, scale.y+mod, scale.z); }
            if (Input.GetKey(ScaleUpZ)) { go.transform.localScale = new Vector3(scale.x, scale.y, scale.z+mod); }
        
            if (Input.GetKey(ScaleDownX)) { go.transform.localScale = new Vector3(scale.x-mod, scale.y, scale.z); }
            if (Input.GetKey(ScaleDownY)) { go.transform.localScale = new Vector3(scale.x, scale.y-mod, scale.z); }
            if (Input.GetKey(ScaleDownZ)) { go.transform.localScale = new Vector3(scale.x, scale.y, scale.z-mod); }
        }
        else {
            Debug.LogWarning("why is it null in handlekeyscaling uh oh");
        }
        
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
    public void ToggleSelPanel(bool open) {
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

    public void SetSelectedObject(GameObject selected) {
        heldObject = null;
        selectedObject = selected;
        _selectedStartPos = selected.transform.position;
        _selectedStartRot = selected.transform.eulerAngles;
        _selectedStartScale = selected.transform.localScale;
        _selectedStartMat = selected.GetComponent<PlaceableObject>().GetMaterial();
        itemInfoPanel.gameObject.SetActive(true);
    }
}
