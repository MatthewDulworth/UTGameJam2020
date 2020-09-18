using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float horizontalMax = 1f;

    [SerializeField]
    private GameObject player;
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject mainCamera;

    private Vector3 cameraOffset;
    private Vector3 targetOffset;
    private Vector3 dummy = new Vector3(2f, 2f, 2f);

    // Start is called before the first frame update
    void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
        cameraOffset = new Vector3(0, 0, 0);
        targetOffset = new Vector3(0, 0, 0);
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey("a"))
        {
            targetOffset.x = -horizontalMax;
        }
        else if (Input.GetKey("d"))
        {
            targetOffset.x = horizontalMax;
        }
        else
        {
            //targetOffset.x = 0;
            //targetOffset.y = 0;
        }

        Vector3 newPos = mainCamera.transform.position;

        //Calculate and apply new camera offset
        newPos -= cameraOffset;
        Vector3 vel = rb.velocity;
        cameraOffset = Vector3.SmoothDamp(cameraOffset, targetOffset, ref dummy, .1f, 2f);
        newPos += cameraOffset;

        mainCamera.transform.position = newPos;
    }
}
