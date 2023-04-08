using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour {

    [SerializeField] private int switchTime = 60;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool active = true;
    
    private bool _extended;
    private int _timer;
    private Vector3 target;

    void Awake() {
        Vector3 pos = GetComponent<PlaceableChild>().GetParent().transform.position;
        target = new Vector3(pos.x, pos.y + 1.3f, pos.z);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_timer < switchTime) {
            _timer++;
        } else {
            _timer = 0;
            _extended = !_extended;

            Vector3 pos = GetComponent<PlaceableChild>().GetParent().transform.position;
            target = new Vector3(pos.x, pos.y + (_extended ? 1.3f : 0.4f), pos.z);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
