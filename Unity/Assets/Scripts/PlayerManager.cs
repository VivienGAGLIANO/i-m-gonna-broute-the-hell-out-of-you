using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    public KeyCode goLeftKey = KeyCode.X;
    public KeyCode goRightKey = KeyCode.C;

    public KeyCode jumpKey = KeyCode.X;
    public float jumpForce = 300f;
    public int maxJumps = 1;
    private int remainingJumps;
    private bool shouldCheckGroundedForJumps = false;

    public KeyCode crouchKey = KeyCode.C;
    [SerializeField] private bool crouched = false;
    private BoxCollider2D bc2D;
    private bool doCrouchFixedUpdate = false;
    private bool doUnCrouchFixedUpdate = false;

    public KeyCode pauseKey = KeyCode.P;

    private KeyCode switchKey = KeyCode.LeftControl;
    private bool onCrouchJumpMode = false;
    private static bool isOnCrouchJumpMode = false;

    [Space(10)]
    [Header("Crampons")]
    public bool canUseCrampons = false;
    public KeyCode cramponKey = KeyCode.N;
    [SerializeField] private bool onCrampon = false;
    [SerializeField ]private LayerMask maskTerrain;
    [SerializeField] private float cramponGravityFactor = 10f;
    [SerializeField] private float angleGoForward = 45f;
    // raycast au cas où, bon, on est sur du concave
    [SerializeField] private float angleRaycastDeSecours = 45f;
    [SerializeField] private float lengthRaycastDeSecours = .1f;
    [SerializeField] private bool isCloseEnoughWithCrampons = false;
    [SerializeField] private float distCloseEnoughWithCrampons = .5f;
    [SerializeField] private float accelSpeedCrampons = 70f;
    [SerializeField] private float SpeedMaxCrampons = 4f;

    [Space(10)]
    [Header("Carrottes")]
    //carotte
    public KeyCode carotteKey = KeyCode.M;
    private int nbCarottesDuNiveau = 0;
    public Carotte carottePrefab;
    public Vector2 spawnCarotte = Vector2.zero;
    public float speedCarotte = 1f;

    private static int nbCarottesDuNiveauStatic;

    private Rigidbody2D rb;


    [Space(10)]
    [Header("Movement")]
    public float hMoveSpeed = 2f;
    public float accelSpeed = .1f;
    // double force applied to turn around
    private bool shouldBurstBrake = false;

    // le niveau a commencé, doit courir maintenant.
    [SerializeField] private bool startedLevel = true;
    public bool goingRight = true;

    //isGrounded
    [SerializeField] private bool isGrounded = false;

    // Animator reference
    private Animator animator;



    //debug
    [Space(20)]
    [Header("Debug")]
    [SerializeField] private bool useMoveDebug = false;
    [SerializeField] private bool rightDebug = false;
    [SerializeField] private bool leftDebug = false;
    [SerializeField] private float myAccelFactorDebug = 10f;

    Vector3 mydebugAverageNormal = new Vector3();
    Vector3 mydebugAveragePoint = new Vector3();

    private RaycastHit2D leftRH;
    private RaycastHit2D rightRH;
    private Ray2D leftR2D;
    private Ray2D rightR2D;
    private Ray2D leftR2DSecours;
    private Ray2D rightR2DSecours;
    private Vector2 leftNormal;
    private Vector2 rightNormal;
    private Vector2 avgNormal;
    private Vector2 avgNormalpos;
    private Vector2 leftNormalpos;
    private Vector2 rightNormalpos;
    [SerializeField] private float lenghtRays = 1f;
    [SerializeField] private float offsetX = .2f;
    [SerializeField] private float offsetY = .1f;
    [SerializeField] private float angleRays = 45f;
    [SerializeField] private bool shouldDrawGizmo = false;
    private bool shouldCramponning = false;

    [Range(0f,1f)] public float timescalelol = 1f;

    public int NbCarottesDuNiveau { get => nbCarottesDuNiveau; set => nbCarottesDuNiveau = value; }
    public static int NbCarottesDuNiveauStatic { get => nbCarottesDuNiveauStatic; set => nbCarottesDuNiveauStatic = value; }
    public bool OnCrouchJumpMode { get => onCrouchJumpMode; set => onCrouchJumpMode = value; }
    public static bool IsOnCrouchJumpMode { get => isOnCrouchJumpMode; set => isOnCrouchJumpMode = value; }

    public TrucSauvegarded GiveMeLeTrucSauvegarded()
    {
        return new TrucSauvegarded(crouched, isGrounded, rb.velocity, transform.position, transform.rotation);
    }

    private bool FireRays (out RaycastHit2D lrh, out RaycastHit2D rrh)
    {
        Vector3 selfpos = transform.position;
        Vector3 originLeft = transform.TransformPoint(new Vector3(-offsetX, offsetY));
        Vector3 originRight = transform.TransformPoint(new Vector3(offsetX, offsetY));
        leftR2D = new Ray2D(originLeft, Quaternion.AngleAxis(-angleRays, Vector3.back) * -transform.up * lenghtRays);
        rightR2D = new Ray2D(originRight, Quaternion.AngleAxis(angleRays, Vector3.back) * -transform.up * lenghtRays);

        leftR2DSecours = new Ray2D(originLeft, Quaternion.Euler(new Vector3(0f, 0f, angleRaycastDeSecours)) * transform.right * lengthRaycastDeSecours);
        rightR2DSecours = new Ray2D(originRight, Quaternion.Euler(new Vector3(0f, 0f, -angleRaycastDeSecours)) * -transform.right * lengthRaycastDeSecours);

        if (!goingRight)
        {
            lrh = Physics2D.Raycast(leftR2DSecours.origin, leftR2DSecours.direction);
            if (!lrh)
                lrh = Physics2D.Raycast(leftR2D.origin, leftR2D.direction);
            rrh = Physics2D.Raycast(rightR2D.origin, rightR2D.direction);
        }
        else
        {
            rrh = Physics2D.Raycast(rightR2DSecours.origin, rightR2DSecours.direction);
            if (!rrh)
                rrh = Physics2D.Raycast(rightR2D.origin, rightR2D.direction);
            lrh = Physics2D.Raycast(leftR2D.origin, leftR2D.direction);
        }

        

        if(lrh && rrh)
        {
            //Debug.Log("Hit leftRH : " + lrh.collider.tag + " / Hit rightRH : " + rrh.collider.tag);

            leftNormal = lrh.normal;
            rightNormal = rrh.normal;
            avgNormal = leftNormal + rightNormal;
            leftNormalpos = lrh.point;
            rightNormalpos = rrh.point;
            avgNormalpos = (leftNormalpos + rightNormalpos) / 2;

            float angleToRotate = Vector2.SignedAngle(transform.up, avgNormal);
            transform.rotation = Quaternion.Euler(0f, 0f, angleToRotate + transform.eulerAngles.z);
            return true;
        }
        return false;
        //transform.Rotate(new Vector3(0f, 0f, angleToRotate), Space.Self);
    }

    private void ThrowCarotte()
    {
        if(NbCarottesDuNiveau > 0)
        {
            --NbCarottesDuNiveau;

            Carotte c = GameObject.Instantiate<Carotte>(carottePrefab, transform.TransformPoint(spawnCarotte), transform.rotation);
            c.Throw(spawnCarotte, transform.right * speedCarotte);

            animator.SetTrigger("ThrowCarrot");
        }
    }

    private void OnDrawGizmos()
    {
        if (shouldDrawGizmo)
        {
            Vector3 sizeCube = new Vector3(.1f, .1f, .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(leftR2D.origin, leftR2D.direction);
            Gizmos.DrawRay(rightR2D.origin, rightR2D.direction);
            Gizmos.DrawCube(leftR2D.origin, sizeCube);
            Gizmos.DrawCube(rightR2D.origin, sizeCube);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(leftNormalpos, leftNormal * lenghtRays);
            Gizmos.DrawRay(rightNormalpos, rightNormal * lenghtRays);
            Gizmos.color = Color.white;
            Gizmos.DrawRay(avgNormalpos, avgNormal * lenghtRays);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(new Vector3(0f, 0f, angleGoForward)) * transform.right * 10f);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(new Vector3(0f, 0f, -angleGoForward)) * -transform.right * 10f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(leftR2D.origin, leftR2DSecours.direction * lengthRaycastDeSecours /* Quaternion.Euler(new Vector3(0f, 0f, angleRaycastDeSecours)) * transform.right * lengthRaycastDeSecours*/);
            Gizmos.DrawRay(rightR2D.origin, Quaternion.Euler(new Vector3(0f, 0f, -angleRaycastDeSecours)) * -transform.right * lengthRaycastDeSecours);
        }
    }
    

    // Start is called before the first frame update
    void Awake()
    {
        bc2D = GetComponent<BoxCollider2D>();
        bc2D.size = new Vector2(.5f, 1f);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        remainingJumps = maxJumps;
        maskTerrain = LayerMask.GetMask("Terrain");
    }

    private void Update()
    {
        NbCarottesDuNiveauStatic = nbCarottesDuNiveau;
        IsOnCrouchJumpMode = OnCrouchJumpMode;
        //Time.timeScale = timescalelol;
        isGrounded = CheckGrounded();

        if (onCrampon)
        {
            isCloseEnoughWithCrampons = CheckProximityForCrampons();
        }

        if (remainingJumps < maxJumps && shouldCheckGroundedForJumps)
        {
            if (isGrounded)
            {
                // Debug.Log("[playerManager] Reset sauts !");
                remainingJumps = maxJumps;
                shouldCheckGroundedForJumps = false;
            }
        }
        MonitorKeyboard();


        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            shouldCramponning = true;
            shouldDrawGizmo = true;
            StartCoroutine(DoCramponningMagic());
        }        
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            shouldDrawGizmo = false;
            shouldCramponning = false;
        }

        UpdateAnimator();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startedLevel)
        {
            MovePlayer();
        }
    }

    IEnumerator DoCramponningMagic()
    {
        while (shouldCramponning)
        {
            FireRays(out leftRH, out rightRH);
            yield return new WaitForSeconds(.5f);
        }
    }


    private void MonitorKeyboard()
    {
        /// TODO : gérer d'envoyer vers la gauche/droite par rapport à l'orientation du player

        if (Input.GetKeyDown(switchKey))
        {
            OnCrouchJumpMode = !OnCrouchJumpMode;
            animator.SetTrigger("ChangeRunMode");
            animator.SetBool("OnCrouchJumpMode", OnCrouchJumpMode);
        }

        // TODO : faire une rotation sur Y de 180 du player pour tourner
        if (!OnCrouchJumpMode)
        {
            if (Input.GetKeyDown(goLeftKey))
            {
                if (goingRight)
                {
                    goingRight = false;
                    shouldBurstBrake = true;
                    transform.Rotate(transform.up, 180f);
                }
            }
            if (Input.GetKeyDown(goRightKey))
            {
                if(!goingRight)
                {
                    goingRight = true;
                    shouldBurstBrake = true;
                    transform.Rotate(transform.up, -180f);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(crouchKey))
            {
                if (!crouched)
                {
                    crouched = true;
                    //transform.localScale = new Vector3(1f, .5f, 1f);
                    bc2D.size = new Vector2(.5f, .5f);
                    bc2D.offset = new Vector2(0f, -.25f);
                    //transform.position = transform.TransformPoint(new Vector3(0f, -transform.localScale.y / 2, transform.position.z));
                }
            }
            if (Input.GetKeyUp(crouchKey))
            {
                if (crouched)
                {
                    if (canUncrouch())
                    {
                        crouched = false;
                        //transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z);
                        //transform.localScale = new Vector3(1f, 1f, 1f);
                        bc2D.size = new Vector2(.5f, 1f);
                        bc2D.offset = new Vector2(0f, 0f);
                    }
                    else /// TODO : lancer batterie de tests pour uncrouch dès que possible
                    {
                        crouched = false;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
            }
            if (Input.GetKeyDown(jumpKey))
            {
                if (remainingJumps > 0 && !onCrampon)
                {
                    rb.AddForce(new Vector2(0f, jumpForce));
                    --remainingJumps;
                    StartCoroutine(WaitThenCheckableGrounded(.2f));
                }
            }
        }


        // debug avec fleche droite et fleche gauche
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightDebug = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftDebug = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightDebug = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftDebug = false;
        }

        if (canUseCrampons){

        if (Input.GetKeyDown(cramponKey))
        {
            // crampon switching
            CramponSwitch();
        }

        if (Input.GetKeyDown(carotteKey))
        {
            ThrowCarotte();
        }
        }
    }

    private void CramponSwitch()
    {
        onCrampon = !onCrampon;

        if (onCrampon)
        {
            //if()
            //disable gravity, enable customForceGravity
            // check quand tu peux l'activer (en étant au sol) -> changer l'ui mais pas la physique avant d'avoir atterri
            StartCoroutine(WaitForIsGroundedToMettreLesCrampons());
            rb.constraints = RigidbodyConstraints2D.FreezeRotation /* | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY */;
        }
        else
        {
            //enable gravity, disable customForceGravity, rotate towards down accordingly
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    IEnumerator WaitForIsGroundedToMettreLesCrampons()
    {
        while (!isGrounded && onCrampon)
        {
            yield return 0;
        }
        if(onCrampon)
            rb.gravityScale = 0f;
    }

    // cast raycast to know if able to stand up or should stay crouched (i.e. in small passage)
    // else should uncrouch when possible from the first call
    private bool canUncrouch()
    {
        return true;
    }

    /// reset le nb de sauts si grounded
    private bool CheckGrounded()
    {
        RaycastHit2D rh = Physics2D.Raycast(this.transform.position, -this.transform.up, (this.transform.localScale.y / 2 + .1f));
        if (rh)
        {
            string t = rh.collider.tag;
            return (!(t == "Player" || t == "Carotte"));
        }
        return false;
    }

    private bool CheckProximityForCrampons()
    {
        RaycastHit2D rh = Physics2D.Raycast(this.transform.position, -this.transform.up, (this.transform.localScale.y / 2 + distCloseEnoughWithCrampons));
        if (rh)
        {
            string t = rh.collider.tag;
            return (!(t == "Player" || t == "Carotte"));
        }
        return false;
    }

    private IEnumerator WaitThenCheckableGrounded(float waittimebeforeallowchecking)
    {
        yield return new WaitForSeconds(waittimebeforeallowchecking);
        shouldCheckGroundedForJumps = true;
    }

    public static void TurnAroundKeepVelocity(GameObject gototurn, Transform newRotation)
    {
        if(newRotation.eulerAngles.y == gototurn.transform.eulerAngles.y)
        {
            // osef
        }
        else //demitour
        {
            gototurn.transform.Rotate(gototurn.transform.up, 180f);
            Rigidbody2D r = gototurn.GetComponent<Rigidbody2D>();
            r.velocity = -r.velocity;
            if (gototurn.CompareTag("Player"))
            {
                PlayerManager p = gototurn.GetComponent<PlayerManager>();
                p.goingRight = !p.goingRight;
            }
        }
    }

    private void MovePlayer()
    {
        if (!onCrampon)
        {
            float speedH = shouldBurstBrake ? 2 * accelSpeed : accelSpeed;
            if (shouldBurstBrake) // demitour
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                // Debug.Log("BurstBrake " + Time.frameCount);
            }
            shouldBurstBrake = false;
            if (goingRight)
            {
                if (rb.velocity.magnitude < hMoveSpeed)
                {
                    //rb.AddForce(new Vector2(speedH, 0f));
                    rb.AddForce(transform.right * speedH * 2f);
                }
            }
            else //left
            {
                if (rb.velocity.magnitude < hMoveSpeed)
                {
                    //rb.AddRelativeForce(Vector3.right * speedH * 2f);
                    rb.AddForce(transform.right * speedH * 2f);
                    //rb.AddForce(new Vector2(-speedH, 0f));
                }
            }
        }
        else // on crampon
        {
            if (isCloseEnoughWithCrampons)
            {
                if(FireRays(out leftRH, out rightRH))
                {
                    rb.AddForce(-avgNormal * cramponGravityFactor, ForceMode2D.Force);
                }
            }

            // test // TODO check que c'est un demi tour
            float speedH = shouldBurstBrake ? 4 * accelSpeedCrampons : accelSpeedCrampons;
            ForceMode2D mf = shouldBurstBrake ? ForceMode2D.Impulse : ForceMode2D.Force;
            if (shouldBurstBrake)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                // Debug.Log("BurstBrake " + Time.frameCount);
            }
            shouldBurstBrake = false;
            if (goingRight)
            {
                if (rb.velocity.magnitude < SpeedMaxCrampons)
                {
                    //rb.AddForce(new Vector2(speedH, 0f));
                    rb.AddForce(Quaternion.Euler(new Vector3(0f,0f,angleGoForward)) * transform.right * speedH, mf);
                }
            }
            else //left
            {
                if (rb.velocity.magnitude < SpeedMaxCrampons)
                {
                    rb.AddForce(Quaternion.Euler(new Vector3(0f, 0f, -angleGoForward)) * -transform.right * speedH, mf);
                    //rb.AddForce(new Vector2(-speedH, 0f));
                }
            }
            // endtest

            //if (DoubleRaycastDown(1f, out RaycastHit2D leftHitInfo, out RaycastHit2D rightHitInfo))
            //{
            //    Debug.Log("Bonjour les kheys");
            //    PositionOnTerrain(leftHitInfo, rightHitInfo, 180f, transform.localScale.y / 2);
            //}
        }
    }

    /*

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawRay(this.transform.position, -this.transform.up * (this.transform.localScale.y / 2 + .03f));

        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;

        Ray2D leftRay = new Ray2D(transform.position + originOffsetRayCramponY * transformUp + originOffsetRayCramponX * transformRight, -transformUp);
        Ray2D rightRay = new Ray2D(transform.position + originOffsetRayCramponY * transformUp + -originOffsetRayCramponX * transformRight, -transformUp);

        Gizmos.DrawRay(leftRay.origin, leftRay.direction);
        Gizmos.DrawRay(rightRay.origin, rightRay.direction);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mydebugAveragePoint, mydebugAverageNormal * myAccelFactorDebug);

    }

    [SerializeField] float originOffsetRayCramponY = .2f;
    [SerializeField] float originOffsetRayCramponX = .2f;

    bool DoubleRaycastDown( float rayLength, out RaycastHit2D leftHitInfo, out RaycastHit2D rightHitInfo)
    {
        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;
        Ray2D leftRay = new Ray2D(transform.position + originOffsetRayCramponY * transformUp + originOffsetRayCramponX * transformRight, -transformUp);
        Ray2D rightRay = new Ray2D(transform.position + originOffsetRayCramponY * transformUp + -originOffsetRayCramponX * transformRight, -transformUp);

        leftHitInfo = Physics2D.Raycast(leftRay.origin, leftRay.direction, rayLength, maskTerrain);
        rightHitInfo = Physics2D.Raycast(rightRay.origin, rightRay.direction, rayLength, maskTerrain);

        return leftHitInfo && rightHitInfo;
    }

    void PositionOnTerrain(RaycastHit2D leftHitInfo, RaycastHit2D rightHitInfo, float maxRotationDegrees, float positionOffsetY)
    {
        Vector3 averageNormal = (leftHitInfo.normal + rightHitInfo.normal) / 2;
        Vector3 averagePoint = (leftHitInfo.point + rightHitInfo.point) / 2;

        mydebugAverageNormal = averageNormal;
        mydebugAveragePoint = averagePoint;

        rb.AddForce(-averageNormal * myAccelFactorDebug);
        
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
        Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDegrees);
        transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);

        //transform.position = averagePoint + transform.up * positionOffsetY;
        
    }
    */

    /*********************************************** Mayo ***********************************************/

    private void UpdateAnimator()
    {
        animator.SetBool("IsCrouched", crouched);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("HorizontalVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }
}
