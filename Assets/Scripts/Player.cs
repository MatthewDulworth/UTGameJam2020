
using UnityEngine;

public class Player : MonoBehaviour {
    
    public float speed = 10;
    public float grappleRange = 10;
    public float grappleSpeed;
    private Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleGrapple();
    }
    
    private void HandleGrapple() {
        
    }
    
    private void HandleMovement() {
        
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
