using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour {

    [SerializeField] private int switchTime = 60;
    [SerializeField] private float speed = 3f;

    private float _closed = 0.45f;
    private float _open = 1.3f;
    
    private bool _extended;
    private int _timer;
    private Vector3 target;

    void Awake() {
        target = new Vector3(0, _open, 0);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_timer < switchTime) {
            _timer++;
        } else {
            _timer = 0;
            _extended = !_extended;

            target = new Vector3(0, _extended ? _open : _closed, 0);
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
    }
}
