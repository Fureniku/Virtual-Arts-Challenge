using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsRegistry : MonoBehaviour {
    
    //Always keep just one instance of this, accessible from anywhere.
    private static MaterialsRegistry _instance;
    public static MaterialsRegistry Instance {
        get { return _instance; }
    }
    
    [SerializeField] private GameController controller;
    [SerializeField] private Material[] materials;
    
    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    public Material GetMaterial(int id) {
        return materials[id];
    }
}
