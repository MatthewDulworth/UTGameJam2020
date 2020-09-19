using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Booster : MonoBehaviour
{

    public float boostForce = 1000f;
    private Boolean canBoost = false;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp("space") && canBoost)
        {
            rb.AddForce(Vector2.up * boostForce);
            Debug.Log("Woooosh!");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("UpdraftTop"))
        {
            canBoost = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("UpdraftTop"))
        {
            canBoost = false;
        }
    }
}
