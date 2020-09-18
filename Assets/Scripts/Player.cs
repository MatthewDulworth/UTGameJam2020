using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour {
    
    public float speed = 10;
    private Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    // handles movement
    private void HandleMovement() {
        
        // horizontal
        int x = 0;
        if (Input.GetKey(KeyCode.A)) {
            x += -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            x += 1;
        }
        
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    }
}
