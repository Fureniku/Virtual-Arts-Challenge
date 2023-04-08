using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    //When something isn't selected
    public KeyCode OpenSelection = KeyCode.Q;
    public KeyCode EditTarget = KeyCode.E;
    public KeyCode MakeSelection = KeyCode.Mouse1;
    
    //When something is selected
    
    //Objects we need access to
    [SerializeField] private GameObject selectionPanel;

    private bool _isSelecting = false;
    void Start() {
        
    }

    void Update() {
        if (Input.GetKeyDown(OpenSelection)) {
            _isSelecting = !_isSelecting;
            selectionPanel.SetActive(_isSelecting);
            Cursor.lockState = _isSelecting ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
}
