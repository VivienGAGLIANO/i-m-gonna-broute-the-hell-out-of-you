using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporteur : MonoBehaviour
{
    public float disableTimeAfterUse = 2f;
    public Teleporteur tpCible;
    public bool isNotDisabled = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isNotDisabled)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Carotte"))
            {
                GameObject go = collision.gameObject;
                StartCoroutine(TpPlayerOrCarotte(go));
            }
        }
    }

    IEnumerator TpPlayerOrCarotte(GameObject machinATeleporter)
    {
        if (tpCible && tpCible.isNotDisabled)
        {
            PlayerManager.TurnAroundKeepVelocity(machinATeleporter, tpCible.transform);
            //machinATeleporter.transform.rotation = tpCible.transform.rotation;
            machinATeleporter.transform.position = tpCible.transform.position;
            isNotDisabled = false;
            yield return new WaitForSeconds(disableTimeAfterUse);
            isNotDisabled = true;
        }
    }
}
