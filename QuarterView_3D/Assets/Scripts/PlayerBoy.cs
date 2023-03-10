using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoy : MonoBehaviour
{
    public float speed;

    public int health;
    public int score;
    public int coin;

    public Camera followCamera;
    public GameManager manager;

    public int maxHealth;
    public int maxCoin;

    float horizontalAxis;
    float verticalAxis;

    bool walkDown;
    bool attackDown;

    bool isReadyToAttack = true;

    bool isBorder;

    bool isDamaged;

    Vector3 moveVector;
    Vector3 dodgeVector;
    Rigidbody rigid;
    Animator anim;

    float attackDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Attack();
        Dodge();
    }

    void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
    }

    void Move()
    {
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        if (!isReadyToAttack)
            moveVector = Vector3.zero;

        if (!isBorder)
            transform.position += moveVector * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVector != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVector);

        if (attackDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;
                transform.LookAt(transform.position + nextVector);
            }
        }
    }

    void Attack()
    {
        attackDelay += Time.deltaTime;

        if (attackDown && isReadyToAttack)
        {
            anim.SetTrigger("doAttack");
        }
    }

    void Dodge()
    {
        dodgeVector = moveVector;
        speed *= 2;
        anim.SetTrigger("doDodge");

        Invoke("DodgeOut", 0.4f);
    }

    void DodgeOut()
    {
        speed *= 0.5f;
    }
}
