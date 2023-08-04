using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialization : MonoBehaviour
{
   
    void Start()
    {
        //Load next scene on start
        // SceneManager.LoadScene(1);
        StartCoroutine(nameof(LoadDelay));

    }

    private IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

   
}
