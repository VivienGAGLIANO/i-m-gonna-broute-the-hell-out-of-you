using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScript : MonoBehaviour
{
    public GameObject upbtn;
    public GameObject dobtn;
    public GameObject lebtn;
    public GameObject ribtn;

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.IsOnCrouchJumpMode)
        {
            upbtn.SetActive(true);
            dobtn.SetActive(true);
            lebtn.SetActive(false);
            ribtn.SetActive(false);
        }
        else
        {
            upbtn.SetActive(false);
            dobtn.SetActive(false);
            lebtn.SetActive(true);
            ribtn.SetActive(true);
        }
    }
}
