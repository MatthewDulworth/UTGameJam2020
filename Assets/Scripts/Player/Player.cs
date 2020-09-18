using System;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public float speed = 10;
    
    public float clickRadius = 1;
    public float grappleRadius = 5;
    public float grappleSpeed;
    
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;
    
    private Rigidbody2D rb;

    private bool grappling = false;
    private Vector2 grapplePoint;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        HandleMovement();
        Grapple();
        //HandleGrapple();
    }

    private void Grapple() {
        
        if (Input.GetKeyDown(KeyCode.J)) {
            Vector2 origin = transform.position;
            Vector2 target = ClosestGrappleInRange(origin);

            if (target != origin) {
                rb.AddForce((target - origin) * grappleSpeed, ForceMode2D.Impulse);
            }
        }
    }

    private Vector2 ClosestGrappleInRange(Vector2 origin) {
        Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(transform.position, grappleRadius, targetLayer);
        
        if (grapplePoints.Length > 0) {
            float closestDist = Mathf.Infinity;
            Vector2 closestPoint = grapplePoints[0].transform.position;

            foreach (Collider2D collide in grapplePoints) {
                Vector2 point = collide.transform.position;
                Vector2 direction = (point - origin).normalized;
                float dist = Vector2.Distance(point, origin);

                if (dist < closestDist && !Physics2D.Raycast(origin, direction, dist, obstacleLayer)) {
                    closestDist = dist;
                    closestPoint = collide.transform.position;
                }
            }
            return closestPoint;
        }
        return origin;
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
    
     // private void HandleGrapple() {
    //     if (!grappling && Input.GetKeyDown(KeyCode.J)) {
    //         Transform point = fov.ClosestTarget();
    //         if (point) {
    //             grappling = true;
    //             grapplePoint = point.position;
    //             Debug.Log("start");
    //         }
    //     }
    //     else if (Input.GetKeyUp(KeyCode.J)) {
    //         grappling = false;
    //         Debug.Log("end");
    //     }
    //
    //     if (grappling) {
    //         Vector2 dir = (grapplePoint - (Vector2) transform.position).normalized;
    //         rb.AddForce(speed * dir, ForceMode2D.Force);
    //         Debug.Log("on-going");
    //         if (Vector2.Distance(transform.position, grapplePoint) < 0.1) {
    //             grappling = false;
    //             Debug.Log("end");
    //         }
    //     }
    // }
}