using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region singleton
    private static GameManager _instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
    public static GameManager Instance { get => _instance; set => _instance = value; }
    public int NbBrouted { get => nbBrouted; set => nbBrouted = value; }

    #endregion


    // doit appuyer 2 fois sur R en moins de timeCheckRestart secondes pour restart
    public KeyCode restartKey = KeyCode.R;
    public float timeCheckRestart = 2f;
    private bool hitKeyRestart = false;

    public KeyCode menuKey = KeyCode.M;

    public KeyCode pauseKey = KeyCode.P;
    [SerializeField] private bool gamePaused = false;

    public int nbTotalCarottes = 0;
    private int nbBrouted = 0;

    public void AddBrouted()
    {
        NbBrouted++;
    }

    public void AddCarottesTotal(int nbCarottesToAdd)
    {
        nbTotalCarottes += nbCarottesToAdd;
    }

    public void ResetLevel()
    {
        CustomSceneManager.Instance.ReloadScene();
    }

    IEnumerator DoTimeCheckRestart()
    {
        hitKeyRestart = true;
        yield return new WaitForSeconds(timeCheckRestart);
        hitKeyRestart = false;
    }

    private void PauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    private void GoToMenu()
    {
        CustomSceneManager.Instance.GoToMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            GoToMenu();
        }

        if (Input.GetKeyDown(pauseKey))
        {
            PauseGame();
        }

        if (Input.GetKeyDown(restartKey)){
            if (hitKeyRestart)
            {
                ResetLevel();
            }
            else
            {
                StartCoroutine(DoTimeCheckRestart());
            }
        }
    }
}
