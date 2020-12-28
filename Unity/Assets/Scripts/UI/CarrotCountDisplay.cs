using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarrotCountDisplay : MonoBehaviour
{
    private int nbCarrot = 0;
    [SerializeField] private Text carrotText;
    // Update is called once per frame
    void Update()
    {
        nbCarrot = PlayerManager.NbCarottesDuNiveauStatic;
        carrotText.text = "x " + nbCarrot;
    }
}
