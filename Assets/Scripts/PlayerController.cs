using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private bool isTouching;
    public bool isGrounded;
    public bool isJumping;
    public Transform groundCheckLimit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointA = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            pointB = Input.mousePosition;
            isTouching = true;
            
        }
        else
            isTouching = false;

        an.SetBool("Grounded", isGrounded);
        an.SetBool("Running", isTouching);



        GroundCheck();
    }
    private void FixedUpdate()
    {
        if (isTouching)
        {
            pivot.LookAt(target);
            Vector2 offset = pointA - pointB;
            Vector2 direction = Vector2.ClampMagnitude(offset, 2f);
            direction = direction * -1;
            Vector3 targetNewPosition = new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
            target.position = Vector3.Lerp(target.position, targetNewPosition, 0.1f);
            MovePlayer();
        }
    }
    void MovePlayer() {
        if (isGrounded)
        {
        transform.position = Vector3.MoveTowards(transform.position,target.position, moveSpeed * Time.fixedDeltaTime);
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f), 0.1f);

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
   
    void MoveTarget(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, targetLayerMask))
        {
            Debug.DrawRay(ray.origin, hit.point, Color.green);
            target.position = hit.point;
        }
    }

    void Jump()
    {
        isJumping = true;
        an.SetTrigger("Jump");
        rb.AddForce(Vector3.up * jumpForce);
        rb.AddForce(pivot.forward * jumpLenght);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            target.position = transform.position;
            StartCoroutine(JumpReset());

        }
    }

    IEnumerator JumpReset()
    {
        yield return new WaitForSeconds(0.2f);
        isJumping = false;

    }
}