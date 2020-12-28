using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhichPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject slime;

    private SpriteRenderer playerRenderer;
    private SpriteRenderer slimeRenderer;
    // private BoxCollider2D playerCollider;
    // private BoxCollider2D slimeCollider;

    private void Awake()
    {
        playerRenderer = player.GetComponent<SpriteRenderer>();
        slimeRenderer = slime.GetComponent<SpriteRenderer>();
        // playerCollider = player.GetComponent<BoxCollider2D>();
        // slimeCollider = slime.GetComponent<BoxCollider2D>();

        playerRenderer.enabled = true;
        slimeRenderer.enabled = false;
        // playerCollider.enabled = true;
        // slimeCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerRenderer.enabled = !playerRenderer.enabled;
            slimeRenderer.enabled = !slimeRenderer.enabled;
            player.tag = playerRenderer.enabled ? "Player" : "Untagged";
            slime.tag = slimeRenderer.enabled ? "Player" : "Untagged";
            // playerCollider.enabled = !playerCollider.enabled;
            // slimeCollider.enabled = !slimeCollider.enabled;
        }        
    }
}
