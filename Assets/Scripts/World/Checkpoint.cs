using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Respawner res = collision.gameObject.GetComponent<Respawner>();
        if (res)
        {
            Debug.Log("Checkpoint set");
            res.setRespawnPoint(gameObject.transform.position);
        }
    }
}
