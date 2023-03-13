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
    bool jumpDown;
    bool spinDown;

    bool isReadyToAttack = true;

    bool isBorder;

    bool isDamaged;

    Vector3 moveVector;
    Vector3 dodgeVector;
    Rigidbody rigid;
    Animator anim;

    float attackDelay;
    float attackRate = 0.7f;

    public Sword sword;

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
        SpinAttack();
    }

    void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        jumpDown = Input.GetButtonDown("Jump");
        attackDown = Input.GetButtonDown("Fire1");
        spinDown = Input.GetButtonDown("Fire2");
    }

    void Move()
    {
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        // if (!isReadyToAttack)
        //    moveVector = Vector3.zero;

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

        isReadyToAttack = attackRate < attackDelay;

        if (attackDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doAttack");
            attackDelay = 0;
        }
    }

    void Dodge()
    {
        if (jumpDown)
        {
            dodgeVector = moveVector;
            speed *= 4;
            anim.SetTrigger("doDodge");

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.25f;
    }

    void SpinAttack()
    {
        attackDelay += Time.deltaTime;

        float spinAttackRate = 1.0f;

        isReadyToAttack = spinAttackRate < attackDelay;

        if (spinDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doSpinAttack");
            attackDelay = 0;
        }
    }
}
