using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carotte : MonoBehaviour
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Throw(Vector2 positionSpawnCarotte, Vector2 forceAAppliquerALaCarotte)
    {
        //transform.position = positionSpawnCarotte;
        rb.AddForce(forceAAppliquerALaCarotte, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Cheval"))
        {
            Debug.Log("Carotte a touche un cheval !");
            collider.GetComponent<PlayerFollower>().SePrendreUneCarotte();
            Destroy(this.gameObject);
        }
        else if (!(collider.CompareTag("Player")))
        {
            Debug.Log("Carotte a touche un mur !");
            Destroy(this.gameObject);
        }
    }
}
