using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public LayerMask targetLayerMask;
    public Transform pivot;
    public GameObject playerModel;
    public Animator an;
    public Rigidbody rb;
    public float jumpForce;
    public float jumpLenght;
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector3 storeTarget;
    public bool isTouching;
    public bool attacked;
    public bool isRunning;
    public bool isGrounded;
    public bool isJumping;
    public bool isAttacking;
    public Transform groundCheckLimit;
    Vector2 direction;
    Vector3 targetNewPosition;
    Vector2 offset;
    public float attackCount = 0;
    public GameObject swordHit;
    public bool useButtonToAttack;
    public GameObject attackButton;
    float targetSpeed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!attacked)
            {
                pointA = Input.mousePosition;

            }
            if (!useButtonToAttack)
            {
                StartCoroutine(Attack());
            }


        }

        if (Input.GetMouseButton(0))
        {
            if (!isTouching)
            {
                StartCoroutine(MouseHold());
            }

            if (isTouching)
            {
                pointB = Input.mousePosition;
                pivot.LookAt(target,Vector3.up);
                offset = pointA - pointB;
                direction = Vector2.ClampMagnitude(offset, 1f);
                direction = direction * -1;
                if (!isAttacking)
                {
                    targetNewPosition = new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
                    target.position = Vector3.Lerp(target.position, targetNewPosition, 0.5f);
                }
            }
        }
        else
        {
            pointB = pointA;
            isTouching = false;
        }

        an.SetBool("Touching", isTouching);
        an.SetBool("Grounded", isGrounded);
        an.SetBool("Running", isRunning);
        an.SetBool("Attacking", isAttacking);
        an.SetFloat("AttackCount", attackCount);

        if (pointB != pointA)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        

        

    }
    private void FixedUpdate()
    {
        

        if (isTouching)
        {
            MovePlayer();
        }
        else
        {
            //offset = Vector2.zero;
            
            //target.position = transform.position;
        }
        GroundCheck();
    }
    void MovePlayer() {
        if (isGrounded)
        {
        transform.position = Vector3.MoveTowards(transform.position,target.position, moveSpeed * Time.fixedDeltaTime);
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f), 0.2f);

        }

    }

    void GroundCheck()
    {
        isGrounded = Physics.Linecast(pivot.position, groundCheckLimit.position, targetLayerMask);
        Debug.DrawLine(pivot.position, groundCheckLimit.position, Color.blue);

        if (!isGrounded && !isJumping)
        {
            Jump();
        }

        
    }
   /*
    void MoveTarget(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, targetLayerMask))
        {
            Debug.DrawRay(ray.origin, hit.point, Color.green);
            target.position = hit.point;
        }
    }
    */
    void Jump()
    {
        isGrounded = false;
        isJumping = true;
        an.SetTrigger("Jump");
        rb.AddForce(Vector3.up * jumpForce);
        rb.AddForce(playerModel.transform.forward * jumpLenght);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            StartCoroutine(JumpReset());

        }
    }

    IEnumerator JumpReset()
    {
        yield return new WaitForSeconds(0.3f);
        isJumping = false;
    }

    public void StartAttack()
    {
        attacked = true;
        StartCoroutine(Attack());
        targetSpeed = 0.1f;
    }
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.03f);
        if (isGrounded && !isAttacking)
        {
            pointA = Input.mousePosition;
            target.Translate(playerModel.transform.forward * 2f);
            rb.AddForce(Vector3.up * 100f);
            rb.AddForce(playerModel.transform.forward * 100f);
        }
        attacked = false;
        isAttacking = true;
        attackCount += 1;
        an.SetTrigger("Attack");
        if (attackCount == 1)
        {
            yield return new WaitForSeconds(0.5f);
            attackCount = 0;
            isAttacking = false;
            targetSpeed = 0.3f;
        }
    }
    IEnumerator MouseHold()
    {
        yield return new WaitForSeconds(0.05f);
        isTouching = true;
    }
    public void ActivateAttackButton(Toggle ok)
    {
        useButtonToAttack = ok.isOn;

        if (useButtonToAttack)
        {
            attackButton.SetActive(true);
        }
        else
        {
            attackButton.SetActive(false);
        }
    }


}
