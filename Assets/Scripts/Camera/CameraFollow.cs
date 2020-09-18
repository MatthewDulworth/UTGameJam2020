using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float horizontalMax = 1f;
    public float verticalMax = .5f;

    [SerializeField]
    private Rigidbody2D player;
    [SerializeField]
    [Tooltip("Make sure the camera is a child of the player!")]
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
    }

    // Update is called once per frame
    void Update()
    {
        
        if (player.velocity.x < 0)
        {
            targetOffset.x = -horizontalMax;
        }
        else if (player.velocity.x > 0)
        {
            targetOffset.x = horizontalMax;
        }

        if (player.velocity.y < 0)
        {
            targetOffset.y = -verticalMax;
        }
        else if (player.velocity.y > 0)
        {
            targetOffset.y = verticalMax;
        }


        Vector3 newPos = mainCamera.transform.position;

        //Calculate and apply new camera offset
        newPos -= cameraOffset;
        cameraOffset = Vector3.SmoothDamp(cameraOffset, targetOffset, ref dummy, .1f, 2f);
        newPos += cameraOffset;

        mainCamera.transform.position = newPos;
    }
}
