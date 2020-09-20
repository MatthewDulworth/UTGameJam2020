using System;
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

    // layers
    public LayerMask boostLayer;
    public LayerMask grapplePointLayer;
    public LayerMask groundLayer;

    // audio
    public AudioClip leftStep;
    public AudioClip rightStep;
    public float timeBetweenSteps;
    private AudioSource footSteps;
    private float stepTimeLeft;
    private bool step;

    // --- private vars --- //
    private bool grappling;
    private double boostWaitRemaining;
    private Vector2 grapplePoint;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private Animator anim;
    private GameObject[] grapplePoints;
    private GameObject[] boostPoints;
    private Respawner respawner;
    public bool alive = true;

    // mouse input
    private bool grapplePressed;
    private bool grappleHeld;
    private Vector2 mousePos;

    // direction input
    private bool upHeld;
    private bool leftHeld;
    private bool rightHeld;
    private bool downHeld;
    private bool airBoostPressed;
    private static readonly int Walking = Animator.StringToHash("Walking");

    // --------------------------------------------------------------
    // Mono Methods
    // --------------------------------------------------------------

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
        respawner = GetComponent<Respawner>();
        footSteps = GetComponent<AudioSource>();
    }

    private void Start() {
        grapplePoints = GameObject.FindGameObjectsWithTag("GrapplePoint");
        boostPoints = GameObject.FindGameObjectsWithTag("BoostPoint");
    }

    private void Update() {
        UpdateInputs();
        GrappleInRangeIndicator();
        BoostInRangeIndicator();
    }

    private void FixedUpdate() {
        if (alive) {
            AirBoost();
            Grapple();
            if (Grounded() && !grappling) {
                GroundMovement();
            }
            else {
                AirMovement();
                if (anim.GetBool(Walking)) {
                    anim.SetBool(Walking, false);
                }

                stepTimeLeft = 0;
            }
        }
    }

    // --------------------------------------------------------------
    // Input
    // --------------------------------------------------------------
    private void UpdateInputs() {
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;
        mousePos = Camera.main.ScreenToWorldPoint(pos);
        // grapplePressed = Input.GetMouseButtonDown(0) || grapplePressed;
        // grappleHeld = Input.GetMouseButton(0);
        grapplePressed = Input.GetKeyDown(KeyCode.Space) || grapplePressed;
        grappleHeld = Input.GetKey(KeyCode.Space);

        upHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        downHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        leftHeld = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightHeld = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        airBoostPressed = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||
                          Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
                          Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
                          Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
                          airBoostPressed;
    }

    public void FootSteps() {
        if (stepTimeLeft <= 0) {
            footSteps.clip = (step) ? leftStep : rightStep;
            footSteps.Play();
            step = !step;
            stepTimeLeft = timeBetweenSteps;
        }
        else {
            stepTimeLeft -= Time.deltaTime;
        }
    }

    // --------------------------------------------------------------
    // Grappling
    // --------------------------------------------------------------


    private void Grapple() {
        Vector2 origin = transform.position;

        if (!grappling && grapplePressed) {
            // grapplePoint = ClosestGrappleMouse();
            grapplePoint = ClosestGrapplePoint();

            if (grapplePoint != origin) {
                grappling = true;
                grapplePressed = false;
            }
            
            anim.SetBool(Grapple1, true);
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
            anim.SetBool(Grapple1, false);
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

    private Vector2 ClosestGrapplePoint() {
        Vector2 playerPos = transform.position;
        Collider2D[] grappleColliders = Physics2D.OverlapCircleAll(playerPos, grappleRadius, grapplePointLayer);

        float minDist = Mathf.Infinity;
        Vector2 closestPoint = playerPos;

        foreach (Collider2D gc in grappleColliders) {
            Vector2 point = gc.transform.position;
            Vector2 direction = (point - playerPos).normalized;
            float distance = Vector2.Distance(playerPos, point);

            if (distance < minDist && !Physics2D.Raycast(playerPos, direction, distance, groundLayer)) {
                minDist = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
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
            return Vector2.up * 1.5f;
        }

        // right 
        if (rightHeld) {
            return Vector2.right * 1.5f;
        }

        // down
        if (downHeld) {
            return Vector2.down * 1.5f;
        }

        // left
        if (leftHeld) {
            return Vector2.left * 1.5f;
        }

        return Vector2.zero;
    }

    private void GrappleInRangeIndicator() {
        Vector2 playerPos = transform.position;

        foreach (GameObject gp in grapplePoints) {
            SpriteRenderer sr = gp.GetComponent<SpriteRenderer>();
            Vector2 point = gp.transform.position;
            float distance = Vector2.Distance(playerPos, point);
            bool inRange = distance <= grappleRadius;

            if (inRange && sr.color.a != 1f) {
                Vector2 direction = (point - playerPos).normalized;
                if (!Physics2D.Raycast(playerPos, direction, distance, groundLayer)) {
                    Color c = sr.color;
                    c.a = 1f;
                    sr.color = c;
                    gp.transform.localScale = new Vector3(1f, 1f);
                }
            }
            else if (!inRange && sr.color.a == 1f) {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
                gp.transform.localScale = new Vector3(0.5f, 0.5f);
            }
        }
    }

    private void BoostInRangeIndicator() {
        Vector2 playerPos = transform.position;
        foreach (GameObject bp in boostPoints) {
            SpriteRenderer sr = bp.GetComponent<SpriteRenderer>();
            bool inRange = Vector2.Distance(playerPos, bp.transform.position) <= boostRadius;

            if (inRange && sr.color.a != 1f) {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
            else if (!inRange && sr.color.a == 1f) {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
            }
        }
    }

    // --------------------------------------------------------------
    // Movement
    // --------------------------------------------------------------
    public bool facingRight;
    private static readonly int Grapple1 = Animator.StringToHash("Grappling");

    private void GroundMovement() {
        int x = 0;
        if (leftHeld) {
            x += -1;
        }

        if (rightHeld) {
            x += 1;
        }

        rb.velocity = new Vector2(x * groundSpeed, rb.velocity.y);

        if (x != 0 && !anim.GetBool(Walking)) {
            anim.SetBool(Walking, true);
        }
        else if (x == 0 && anim.GetBool(Walking)) {
            anim.SetBool(Walking, false);
        }

        if (!facingRight && x < 0 || facingRight && x > 0) {
            Flip();
        }

        if (x != 0) {
            FootSteps();
        }
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

        if (!facingRight && x < 0 || facingRight && x > 0) {
            Flip();
        }
    }

    private void Flip() {
        Transform cam = transform.GetChild(0);
        Transform bg = transform.GetChild(1);
        cam.parent = null;
        bg.parent = null;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        cam.parent = transform;
        bg.parent = transform;
        facingRight = !facingRight;
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
            rb.velocity = Vector2.zero;
            respawner.Die();
            grappling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        if (obj.tag.ToLower().Equals("movingplatform")) {
            transform.parent = null;
        }
    }
}