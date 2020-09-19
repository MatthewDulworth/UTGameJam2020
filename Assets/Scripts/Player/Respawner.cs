using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public float respawnHeight = -40f;
    private Vector3 respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y <= respawnHeight)
        {
            respawn();
        }
    }

    public void respawn()
    {
        gameObject.transform.position = respawnPoint;
    }

    public void setRespawnPoint(Vector3 point)
    {
        respawnPoint = point;
    }
}
