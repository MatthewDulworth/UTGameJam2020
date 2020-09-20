using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutscenePlayer : MonoBehaviour
{

    public Text[] messages;
    public AudioSource[] sounds;
    public Canvas panel;
    public CutscenePlayer next;
    public int nextSceneIndex = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("PlayCutscene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayCutscene()
    {

        panel.gameObject.SetActive(true);

        //print out all text and play sounds if applicable
        for(int i = 0; i < messages.Length; i++)
        {
            messages[i].CrossFadeAlpha(0, 0, true);
            messages[i].gameObject.SetActive(true);
            messages[i].CrossFadeAlpha(1f, 2.5f, true);
            if(sounds[i] != null)
            {
                sounds[i].gameObject.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(3);
        }

        yield return new WaitForSecondsRealtime(1.5f);
        

        //trigger next set of messages, otherwise go to next scene
        if (next != null)
        {
            panel.gameObject.SetActive(false);
            next.enabled = true;
            this.enabled = false;
        }
        else
        {
            SceneSwitcher.Instance().loadScene(nextSceneIndex, 3, 3);
        }
    }
}
