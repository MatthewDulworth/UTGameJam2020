using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class CanvasFader : MonoBehaviour
{

    public Image target;
    public float time = 1f;

    // Start is called before the first frame update
    void Start()
    {
        target.CrossFadeAlpha(0f, 0f, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.ToLower().Equals("player"))
        {
            Debug.Log("Entry");
            target.CrossFadeAlpha(1f, time, true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.ToLower().Equals("player"))
        {
            Debug.Log("Exit");
            target.CrossFadeAlpha(0f, time, true);
        }

    }
}
