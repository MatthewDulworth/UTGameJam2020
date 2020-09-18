using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updraft : MonoBehaviour
{

    public float maxforce = 100f;
    public float minforce = 20f;
    public float height = 1f;
    List<Updraftable> affectedObjects;

    // Start is called before the first frame update
    void Start()
    {
        affectedObjects = new List<Updraftable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach(Updraftable up in affectedObjects)
        {
            if (up.willUpdraft)
            {
                float force = maxforce * ((height - Vector2.Distance(up.gameObject.transform.position, gameObject.transform.position)) / height);
                force = Math.Max(force, minforce);
                up.rigidBody().AddForce(Vector2.up * force);
            }
            
        }
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        Updraftable up = other.gameObject.GetComponent<Updraftable>();
        if (up != null){
            affectedObjects.Add(up);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Updraftable up = other.gameObject.GetComponent<Updraftable>();

        if (affectedObjects.Contains(up))
        {
            affectedObjects.Remove(up);
        }

        
    }
}
