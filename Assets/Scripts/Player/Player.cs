using UnityEngine;

public class Player : MonoBehaviour {
    // --- public vars --- //
    public float groundSpeed;
    public float airAcceleration;
    public float grappleAccel;
    public float maxSpeed;
    public float grappleRadius;
    public float mouseRadius;
    public float collisionRadius;
    public Vector2 bottomOffset;

    public LayerMask targetLayer;
    public LayerMask groundLayer;

    // --- private vars --- //
    private bool grappling;
    private Vector2 grapplePoint;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate() {
        Grapple();

        if (Grounded()) {
            GroundMovement();
        }
        else {
            // AirMovement();
        }
    }


    // --------------------------------------------------------------
    // Air Movement
    // --------------------------------------------------------------
    private void AirMovement() {
        int x = 0;
        if (Input.GetKey(KeyCode.A)) {
            x += -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            x += 1;
        }

        rb.AddForce(new Vector2(x, 0) * airAcceleration);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    // --------------------------------------------------------------
    // Grappling
    // --------------------------------------------------------------
    private void Grapple() {
        Vector2 origin = transform.position;

        if (!grappling && Input.GetMouseButtonDown(0)) {
            grapplePoint = ClosestGrappleMouse();

            if (grapplePoint != origin) {
                grappling = true;
            }
        }

        if (Input.GetMouseButton(0) && grappling) {
            Vector2 dir = (grapplePoint - origin).normalized;
            rb.AddForce(dir * grappleAccel);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, grapplePoint);
        }
        else {
            grappling = false;
            lineRenderer.positionCount = 0;
        }
    }

    private Vector2 ClosestGrappleMouse() {
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(pos);
        Collider2D[] grappleColliders = Physics2D.OverlapCircleAll(mousePos, mouseRadius, targetLayer);
        
        if (grappleColliders.Length > 0) {
            Vector2 origin = transform.position;
            Vector2 point = grappleColliders[0].transform.position;
            Vector2 direction = (point - origin).normalized;
            float distance = Vector2.Distance(point, origin);
            
            if (distance < grappleRadius && !Physics2D.Raycast(origin, direction, distance, groundLayer)) {
                return point;
            }
        }
        return transform.position;
    }

    private Vector2 ClosestGrappleInRange(Vector2 origin) {
        Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(transform.position, grappleRadius, targetLayer);

        if (grapplePoints.Length > 0) {
            float closestDist = Mathf.Infinity;
            Vector2 closestPoint = transform.position;

            foreach (Collider2D collide in grapplePoints) {
                Vector2 point = collide.transform.position;
                Vector2 direction = (point - origin).normalized;
                float dist = Vector2.Distance(point, origin);

                if (dist < closestDist && !Physics2D.Raycast(origin, direction, dist, groundLayer)) {
                    closestDist = dist;
                    closestPoint = collide.transform.position;
                }
            }

            return closestPoint;
        }

        return transform.position;
    }

    // --------------------------------------------------------------
    // Grounded Movement
    // --------------------------------------------------------------
    private void GroundMovement() {
        int x = 0;
        if (Input.GetKey(KeyCode.A)) {
            x += -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            x += 1;
        }

        rb.velocity = new Vector2(x * groundSpeed, rb.velocity.y);
    }

    private bool Grounded() {
        return Physics2D.OverlapCircle((Vector2) transform.position + bottomOffset, collisionRadius, groundLayer);
    }
}