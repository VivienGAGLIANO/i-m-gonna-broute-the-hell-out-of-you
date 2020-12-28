using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerFollower : MonoBehaviour
{
    private Queue<TrucSauvegarded> playerPositionsQueue = new Queue<TrucSauvegarded>();

    public PlayerManager playerToFollow;

    public float delayBetweenChecks = .5f;
    public float delayGivenToPlayer = 3f;

    public bool enTrainDeMachouillerLeJoueur = false;

    private Vector2 nextPosition = new Vector2();

    private Collider2D c2d2;

    public bool beingCarotted = false;
    public float timeCarotting = 2f;

    // Anitmator reference
    Animator animator;

    private void Awake()
    {
        c2d2 = GetComponent<Collider2D>();
        c2d2.isTrigger = true;
        c2d2.enabled = false;
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PrepareChase());
    }

    // a lancer quand le joueur bouge
    IEnumerator PrepareChase()
    {
        StartCoroutine(TraceTheFuckOutOfThePlayer());
        yield return new WaitForSeconds(delayGivenToPlayer);
        StartCoroutine(ChaseTheFuckOutOfThePlayer());
    }

    /// relève la position du joueur toutes les delayBetweenChecks secondes
    IEnumerator TraceTheFuckOutOfThePlayer()
    {
        while (!enTrainDeMachouillerLeJoueur)
        {
            playerPositionsQueue.Enqueue(playerToFollow.GiveMeLeTrucSauvegarded());
            yield return new WaitForSeconds(delayBetweenChecks);
        }
    }

    IEnumerator ChaseTheFuckOutOfThePlayer()
    {
        c2d2.enabled = true;
        while (!enTrainDeMachouillerLeJoueur)
        {
            if (beingCarotted)
            {
                //Debug.Log("je bouge passsssssssssssssssssssssssssssssssssssssssssssssss");
                yield return 0;
                continue;
            }
            TrucSauvegarded truc = playerPositionsQueue.Dequeue();

            animator.SetBool("IsGrounded", truc.IsGrounded);
            animator.SetBool("IsCrouched", truc.Crouched);
            animator.SetFloat("HorizontalVelocity", Mathf.Abs(truc.Velocity.x));
            animator.SetFloat("VerticalVelocity", truc.Velocity.y);

            nextPosition = truc.ChasePosition;
            transform.position = nextPosition;
            transform.rotation = truc.ChaseRotation;
            yield return new WaitForSeconds(delayBetweenChecks);
        }
    }

    IEnumerator MeCarotted()
    {
        beingCarotted = true;
        animator.SetTrigger("GetCarroted");
        
        yield return new WaitForSeconds(timeCarotting);
        beingCarotted = false;
    }

    public void SePrendreUneCarotte()
    {
        StartCoroutine(MeCarotted());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddBrouted();
            GameManager.Instance.ResetLevel();
        }
    }
}

public struct TrucSauvegarded
{
    public TrucSauvegarded(bool crouched, bool isGrounded, Vector2 velocity, Vector2 chasePosition, Quaternion chaseRotation)
    {
        Crouched = crouched;
        IsGrounded = isGrounded;
        Velocity = velocity;
        ChasePosition = chasePosition;
        ChaseRotation = chaseRotation;
    }

    public bool Crouched { get; }
    public bool IsGrounded { get; }
    public Vector2 Velocity { get; }
    public Vector2 ChasePosition { get; }
    public Quaternion ChaseRotation { get; }

    //public override string ToString() => $"({X}, {Y})";
}
