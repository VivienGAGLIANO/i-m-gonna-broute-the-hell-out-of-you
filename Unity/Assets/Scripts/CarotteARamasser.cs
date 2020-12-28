using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarotteARamasser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerManager>().NbCarottesDuNiveau++;
            Destroy(this.gameObject);
        }
    }
}
