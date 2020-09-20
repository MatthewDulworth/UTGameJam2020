using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D)]
public class LevelFinish : MonoBehaviour
{
    [Tooltip("Check the build settings for the indexs")]
    public int targetSceneIndex;

    public float fadeInTime = 2f;
    public float fadeOutTime = 1f;

    public float delay = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.ToLower().Equals("player"))
        {
            StartCoroutine(switchDelay());
        }
    }

    IEnumerator switchDelay()
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneSwitcher.Instance().loadScene(targetSceneIndex, fadeOutTime, fadeInTime);
    }
}
