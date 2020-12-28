using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCountDisplay : MonoBehaviour
{
    private int nbDeath = 0;
    [SerializeField] private Text deathText;

    // Update is called once per frame
    void Update()
    {
        nbDeath = GameManager.Instance.NbBrouted;
        deathText.text = "x " + nbDeath;
    }
}
