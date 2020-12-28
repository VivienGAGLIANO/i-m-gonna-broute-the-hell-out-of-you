using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    private static CustomSceneManager _instance;
    [SerializeField] private int idLastLevel = 2;
    public AudioSource ass;


    public void SayBonjour()
    {
        Debug.Log("Bonjour");
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        ass = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if(!Instance.ass.isPlaying)
            ass.Play();
    }

    public static CustomSceneManager Instance { get => _instance; set => _instance = value; }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel()
    {

        int curScene = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Going to next level from scene  " + curScene);
        if (curScene != idLastLevel)
        {
            /// TODO : changer ça, c'est pour tester

            SceneManager.LoadScene(curScene +1);
        }
        else
        {
            GoToMenu();
        }
    }
}
