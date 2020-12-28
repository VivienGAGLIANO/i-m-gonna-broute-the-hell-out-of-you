using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortailDeFinDeNiveau : MonoBehaviour
{
    [SerializeField] private float timeTransitionBetweenScenes;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Pliz ?");
        string gotag = other.tag;
        if(gotag == "Player")
        {
            Debug.Log("Wesh marche");
            //lancer des particules
            StartCoroutine(CompterJusqua4());
            // choses de fin de niveau
        }
    }

    private IEnumerator CompterJusqua4()
    {
        yield return new WaitForSeconds(timeTransitionBetweenScenes);
        CustomSceneManager.Instance.GoToNextLevel();
    }
}
