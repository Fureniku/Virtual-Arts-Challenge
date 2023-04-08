using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableManager : MonoBehaviour {
	
    [SerializeField] private GameObject[] placeables;
    
    void Start() {
        
    }

    void Update() {
        
    }

    public GameObject[] GetRegistry() {
        return placeables;
	}
}
