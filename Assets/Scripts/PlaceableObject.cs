using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour {

    [SerializeField] private GameObject[] subObjects;
    private Material finalMaterial;
    
    public void SetMaterial(Material mat) {
        if (GetComponent<Renderer>() != null) { //Set material on the current object
            GetComponent<Renderer>().material = mat;
        }
        
        for (int i = 0; i < subObjects.Length; i++) { //Set the material on any sub-objects
            subObjects[i].GetComponent<Renderer>().material = mat;
        }
    }

    public Material GetMaterial() {
        if (GetComponent<Renderer>() != null) { //If this has a material, use that.
            return GetComponent<Renderer>().material;
        }
        return subObjects[0].GetComponent<Renderer>().material; //Use the first sub-object instead.
    }

    public void SetPlaced(Material material) {
        gameObject.layer = 7;
        for (int i = 0; i < subObjects.Length; i++) {
            subObjects[i].layer = 7;
        }
        SetMaterial(material);
    }

    public void SetPickedUp(Material material) {
        gameObject.layer = 0;
        for (int i = 0; i < subObjects.Length; i++) {
            subObjects[i].layer = 0;
        }
        SetMaterial(material);
    }
}
