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

    // boost
    public float boostRadius;
    public float boostForce;

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
    private Vector2 startPos;

    // mouse input
    private bool grapplePressed;
    private bool grappleHeld;
    private Vector2 mousePos;

    // direction input
    private bool upHeld;
    private bool leftHeld;
    private bool rightHeld;
    private bool downHeld;

    // --------------------------------------------------------------
    // Mono Methods
    // --------------------------------------------------------------

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        startPos = transform.position;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        UpdateInputs();
    }

    private void FixedUpdate() {
        AirBoost();
        Grapple();
        if (Grounded() && !grappling) {
            GroundMovement();
        }
        else {
            AirMovement();
        }
    }

    // --------------------------------------------------------------
    // Input
    // --------------------------------------------------------------
    private void UpdateInputs() {
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;
        mousePos = Camera.main.ScreenToWorldPoint(pos);
        grapplePressed = Input.GetMouseButtonDown(0) || grapplePressed;
        grappleHeld = Input.GetMouseButton(0);

        upHeld = Input.GetKey(KeyCode.W);
        downHeld = Input.GetKey(KeyCode.S);
        leftHeld = Input.GetKey(KeyCode.A);
        rightHeld = Input.GetKey(KeyCode.D);
    }

    // --------------------------------------------------------------
    // Grappling
    // --------------------------------------------------------------
    private void Grapple() {
        Vector2 origin = transform.position;

        if (!grappling && grapplePressed) {
            grapplePoint = ClosestGrappleMouse();

            if (grapplePoint != origin) {
                grappling = true;
                grapplePressed = false;
            }
        }

        if (grappleHeld && grappling) {
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

    // --------------------------------------------------------------
    // Air Boost
    // --------------------------------------------------------------

    private void AirBoost() {
        Vector2 playerPos = transform.position;
        Vector2 dir = BoostDirection();
        if (dir != Vector2.zero && Physics2D.OverlapCircle(playerPos, boostRadius, boostLayer)) {
            rb.velocity = Vector2.zero;
            rb.AddForce(BoostDirection() * boostForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 BoostDirection() {
        // up right
        if (upHeld && rightHeld) {
            return new Vector2(1, 1);
        }

        // down right 
        if (rightHeld && downHeld) {
            return new Vector2(1, -1);
        }

        // down left
        if (downHeld && leftHeld) {
            return new Vector2(-1, -1);
        }

        // up left
        if (leftHeld && upHeld) {
            return new Vector2(-1, 1);
        }

        // up
        if (upHeld) {
            return Vector2.up;
        }

        // right 
        if (rightHeld) {
            return Vector2.right;
        }

        // down
        if (downHeld) {
            return Vector2.down;
        }

        // left
        if (leftHeld) {
            return Vector2.left;
        }

        return Vector2.zero;
    }

    // --------------------------------------------------------------
    // Movement
    // --------------------------------------------------------------
    private void GroundMovement() {
        int x = 0;
        if (leftHeld) {
            x += -1;
        }

        if (rightHeld) {
            x += 1;
        }

        rb.velocity = new Vector2(x * groundSpeed, rb.velocity.y);
    }

    private void AirMovement() {
        int x = 0;
        if (leftHeld) {
            x += -1;
        }

        if (rightHeld) {
            x += 1;
        }

        rb.AddForce(new Vector2(x, 0) * airAcceleration);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    // --------------------------------------------------------------
    // Collisions
    // --------------------------------------------------------------
    private bool Grounded() {
        return Physics2D.OverlapCircle((Vector2) transform.position + bottomOffset, collisionRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        if (obj.tag.ToLower().Equals("movingplatform")) {
            gameObject.transform.parent = obj.transform;
        }

        if (obj.tag.ToLower().Equals("hazard")) {
            transform.position = startPos;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        if (obj.tag.ToLower().Equals("movingplatform")) {
            transform.parent = null;
        }
    }
}