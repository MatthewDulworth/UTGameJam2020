using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class CanvasFader : MonoBehaviour
{

    public Image target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.ToLower().Equals("player"))
        {
            Debug.Log("Entry");
            target.CrossFadeAlpha(1f, 2f, true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.ToLower().Equals("player"))
        {
            Debug.Log("Exit");
            target.CrossFadeAlpha(0f, 2f, true);
        }

    }
}
