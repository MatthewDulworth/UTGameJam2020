﻿using UnityEngine;
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
    private Vector2 startPos;
    
    // mouse input
    private bool grapplePressed;
    private bool grappleHeld;
    private Vector2 mousePos;
    
    // direction input
    private bool airBoostPressed;
    private bool UpHeld;
    private bool LeftHeld;
    private bool RightHeld;
    private bool DownHeld;

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

    private void UpdateInputs() {
        Vector3 pos = Input.mousePosition;
        pos.z = Camera.main.nearClipPlane;
        mousePos = Camera.main.ScreenToWorldPoint(pos);
        grapplePressed = Input.GetMouseButtonDown(0) || grapplePressed;
        grappleHeld = Input.GetMouseButton(0);

        airBoostPressed = Input.GetKeyDown(KeyCode.LeftShift) || airBoostPressed;
        UpHeld = Input.GetKey(KeyCode.W);
        DownHeld = Input.GetKey(KeyCode.S);
        LeftHeld = Input.GetKey(KeyCode.A);
        RightHeld = Input.GetKey(KeyCode.D);
    }


    // --------------------------------------------------------------
    // Air Movement
    // --------------------------------------------------------------
    private void AirMovement() {
        int x = 0;
        if (LeftHeld) {
            x += -1;
        }

        if (RightHeld) {
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

        else if (airBoostPressed && Physics2D.OverlapCircle(playerPos, boostRadius, boostLayer)) {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            boostWaitRemaining = boostWait;
            airBoostPressed = false;
        }
    }

    private Vector2 boostDirection() {
        // up right
        if (UpHeld && RightHeld) {
            return new Vector2(1, 1);
        }

        // down right 
        if (RightHeld && DownHeld) {
            return new Vector2(1, -1);
        }

        // down left
        if (DownHeld && LeftHeld) {
            return new Vector2(-1, -1);
        }

        // up left
        if (LeftHeld && UpHeld) {
            return new Vector2(-1, 1);
        }

        // up
        if (UpHeld) {
            return Vector2.up;
        }

        // right 
        if (RightHeld) {
            return Vector2.right;
        }

        // down
        if (DownHeld) {
            return Vector2.down;
        }

        // left
        if (LeftHeld) {
            return Vector2.left;
        }

        return Vector2.zero;
    }


    // --------------------------------------------------------------
    // Grounded Movement
    // --------------------------------------------------------------
    private void GroundMovement() {
        int x = 0;
        if (LeftHeld) {
            x += -1;
        }

        if (RightHeld) {
            x += 1;
        }

        rb.velocity = new Vector2(x * groundSpeed, rb.velocity.y);
    }

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