using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Updraftable : MonoBehaviour
{

    public Boolean willUpdraft = true;
    [SerializeField]
    private Boolean isPlayer = false;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space") && isPlayer)
        {
            willUpdraft = true;
        }
        else if(isPlayer)
        {
            willUpdraft = false;
        }
    }

    public Rigidbody2D rigidBody()
    {
        return rb;
    }
}
