using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    // --- public vars --- //
    public float groundSpeed;
    public float airAcceleration;
    public float maxSpeed;

    // grapple 
    public float grappleAccel;
    public float grappleRadius;
    public float mouseRadius;
    public float detachRadius;

    // boost
    public float boostRadius;
    public float boostForce;
    public float boostWait;

    // ground detection
    public float collisionRadius;
    public Vector2 bottomOffset;

    public LayerMask boostLayer;
    public LayerMask grapplePointLayer;
    public LayerMask groundLayer;

    // --- private vars --- //
    private bool grappling;
    private double boostWaitRemaining;
    private Vector2 grapplePoint;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.R)) {
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FixedUpdate() {
        AirBoost();
        // Grapple();
        //
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

            if (Vector2.Distance(origin, grapplePoint) <= detachRadius) {
                grappling = false;
            }
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
        Collider2D[] grappleColliders = Physics2D.OverlapCircleAll(mousePos, mouseRadius, grapplePointLayer);

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
        Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(transform.position, grappleRadius, grapplePointLayer);

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
    // Air Boost
    // --------------------------------------------------------------


    private void AirBoost() {
        Vector2 playerPos = transform.position;

        if (boostWaitRemaining > 0) {
            boostWaitRemaining -= Time.deltaTime;

            if (boostWaitRemaining <= 0) {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.AddForce(boostDirection() * boostForce, ForceMode2D.Impulse);
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift) && Physics2D.OverlapCircle(playerPos, boostRadius, boostLayer)) {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            boostWaitRemaining = boostWait;
        }
    }

    private Vector2 boostDirection() {
        // up right
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) {
            return new Vector2(1, 1);
        }
        // down right 
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
            return new Vector2(1, -1);
        }
        // down left
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) {
            return new Vector2(-1, -1);
        }
        // up left
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) {
            return new Vector2(-1, 1);
        }
        // up
        if (Input.GetKey(KeyCode.W)) {
            return Vector2.up;
        }
        // right 
        if (Input.GetKey(KeyCode.D)) {
            return Vector2.right;
        }
        // down
        if (Input.GetKey(KeyCode.S)) {
            return Vector2.down;
        }
        // left
        if (Input.GetKey(KeyCode.A)) {
            return Vector2.left;
        }
        return Vector2.zero;
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