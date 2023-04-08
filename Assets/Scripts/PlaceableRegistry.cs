using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableRegistry : MonoBehaviour {
	
	//Always keep just one instance of this, accessible from anywhere.
	private static PlaceableRegistry _instance;
	public static PlaceableRegistry Instance {
		get { return _instance; }
	}

	[SerializeField] private GameController controller;
    [SerializeField] private GameObject[] placeables;
    
    void Awake() {
	    if (_instance != null && _instance != this) {
		    Destroy(gameObject);
	    } else {
		    _instance = this;
	    }
    }
    
    public GameObject[] GetRegistry() {
        return placeables;
	}
    
    public void SetHeldObject(int id) {
	    Debug.Log("Setting held object to " + id);
	    controller.SetHeldObject(placeables[id]);
    }

    public void SetSelectedObject(GameObject selected) {
	    controller.SetSelectedObject(selected);
    }
}
