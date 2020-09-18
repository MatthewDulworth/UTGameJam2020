using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public float speed;
    [SerializeField]
    private Transform endPoint;
    private Vector2 end;
    private Vector2 start;
    private bool reverse = false;
    
    // Start is called before the first frame update
    void Start()
    {
        end = endPoint.transform.position;
        start = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 target = end;
        if (reverse)
        {
            target = start;
        }

        Vector2 current = gameObject.transform.position;
        Vector2 newPos = Vector2.MoveTowards(current, target, speed * Time.deltaTime);
        transform.position = newPos;

        //upon reaching end or start, go in the opposite direction
        if(Vector2.Distance(newPos, target) < .1f)
        {
            reverse = !reverse;
        }
    }
}
