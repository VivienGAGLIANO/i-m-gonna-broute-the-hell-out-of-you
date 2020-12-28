using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightleft : MonoBehaviour
{
    private bool isToggled = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isToggled = !isToggled;
            this.gameObject.SetActive(isToggled);
        }
    }
}
