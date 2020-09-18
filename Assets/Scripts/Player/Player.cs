
using UnityEngine;

public class Player : MonoBehaviour {
    
    public float speed = 10;
    public float grappleRange = 10;
    public float grappleSpeed;
    private FieldOfView fov;
    private Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleGrapple();
    }
    
    private void HandleGrapple() {
        if (Input.GetKeyDown(KeyCode.J)) {
            if (fov.ClosestTarget() != null) {
                Debug.Log("yeet");
            }
            else {
                Debug.Log("no");
            }
        }
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
