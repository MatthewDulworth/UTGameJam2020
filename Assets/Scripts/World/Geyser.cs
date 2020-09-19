using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Geyser : MonoBehaviour
{
    public float interval;
    public float force;
    public ParticleSystem eruption;
    private float timer;
    private List<Rigidbody2D> affected;
    // Start is called before the first frame update
    void Start()
    {
        affected = new List<Rigidbody2D>();
        timer = interval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            eruption.gameObject.SetActive(true);
            foreach(Rigidbody2D rb in affected)
            {
                rb.AddForce(Vector2.up * force);
            }

            timer = interval;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            affected.Add(rb);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

        if (affected.Contains(rb))
        {
            affected.Remove(rb);
        }


    }
}
