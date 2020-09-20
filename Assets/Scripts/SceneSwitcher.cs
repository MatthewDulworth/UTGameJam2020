using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("The image that will be used to fade in and out")]
    public Image fadeout;

    private static SceneSwitcher INSTANCE = null;

    // Start is called before the first frame update
    void Start()
    {   
        if(INSTANCE != null)
        {
            Destroy(this);
        }

        fadeout.CrossFadeAlpha(0f, 0, true);
        DontDestroyOnLoad(fadeout.gameObject.transform.root);
        DontDestroyOnLoad(gameObject);

        INSTANCE = this;
    }

    public static SceneSwitcher Instance()
    {
        return INSTANCE;
    } 

    public void loadIntro()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 99;
        loadScene(1, 2, 1);
    }

    public void loadScene(int sceneIndex, float fadeOutTime, float fadeInTime)
    {
        StartCoroutine(switcherCoroutine(sceneIndex, fadeOutTime, fadeInTime));
    }

    IEnumerator switcherCoroutine(int sceneIndex, float fadeOutTime, float fadeInTime)
    {
        fadeout.CrossFadeAlpha(1f, fadeOutTime, true);
        yield return new WaitForSecondsRealtime(fadeOutTime);
        SceneManager.LoadScene(sceneIndex);
        fadeout.CrossFadeAlpha(0f, fadeInTime, true);
    }

}
