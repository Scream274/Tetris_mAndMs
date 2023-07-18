using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public AudioSource audioSource;

    public void Start()
    {
        GameObject targetObject = GameObject.Find("MusicLayer");

        if (targetObject != null)
        {
            DontDestroyOnLoad(targetObject);
        }

    }

    //Start game
    public void Play()
    {
        audioSource.Play();
        StartCoroutine(LoadSceneWithDelay("Tetris", 0.2f));
    }

    //Back to main menu
    public void Main()
    {
        audioSource.Play();

        StartCoroutine(LoadSceneWithDelay("Menu", 0.2f));
    }

    //Exit game
    public void Exit()
    {
        Debug.Log("GAME OVER!");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

}