using System.Data.SqlTypes;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 10;
    public float grappleRange = 10;
    public float grappleSpeed;
    public float clickRadius = 1;
    public float grappleRadius;
    public LayerMask targetLayer;

    private FieldOfView fov;
    private Rigidbody2D rb;

    private bool grappling = false;
    private Vector2 grapplePoint;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FieldOfView>();
    }

    private void FixedUpdate() {
        //HandleMovement();
        Grapple();
        //HandleGrapple();
    }

    private void Grapple() {
        if (Input.GetKeyDown(KeyCode.J)) {
            Transform target = fov.ClosestTarget();
            if (target) {
                Debug.Log("grappling");
                Vector2 point = target.position;
                Vector2 dir = (grapplePoint - (Vector2) transform.position).normalized;
                float dist = Vector2.Distance(transform.position, point);
                rb.AddForce(dir * dist, ForceMode2D.Impulse);
            }
        }
    }

    private void HandleGrapple() {
        if (!grappling && Input.GetKeyDown(KeyCode.J)) {
            Transform point = fov.ClosestTarget();
            if (point) {
                grappling = true;
                grapplePoint = point.position;
                Debug.Log("start");
            }
        }
        else if (Input.GetKeyUp(KeyCode.J)) {
            grappling = false;
            Debug.Log("end");
        }

        if (grappling) {
            Vector2 dir = (grapplePoint - (Vector2) transform.position).normalized;
            rb.AddForce(speed * dir, ForceMode2D.Force);
            Debug.Log("on-going");
            if (Vector2.Distance(transform.position, grapplePoint) < 0.1) {
                grappling = false;
                Debug.Log("end");
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

    // private Vector2 GrappelPoint() {
    //     Vector3 pos = Input.mousePosition;
    //     pos.z = Camera.main.nearClipPlane;
    //     Vector2 mousePos = Camera.main.ScreenToWorldPoint(pos);
    //
    //     // mouse pos within grapple radius
    //     if (Vector2.Distance(mousePos, transform.position) <= grappleRadius) {
    //          Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(mousePos, clickRadius, targetLayer);
    //          if (grapplePoints.Length > 0) {
    //             Targeting.GetClosestTarget(grapplePoints, mousePos) 
    //          }
    //     }
    // }
}