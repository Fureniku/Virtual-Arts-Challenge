using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableChild : MonoBehaviour {

	[SerializeField] private GameObject parent;

	public GameObject GetParent() {
		return parent;
	}
}
