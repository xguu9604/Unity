using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public int score;

    public BoxCollider meleeArea;
    public Transform target;
    public GameManager manager;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        Invoke("ChaseStart", 3);
    }


    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }    
    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    void Targeting()
    {
        float targetRadius = 2f;
        float targetRange = 5f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
                                    targetRadius,
                                    transform.forward,
                                    targetRange,
                                    LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        yield return null;
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.2f);

        meleeArea.enabled = true;

        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;

        yield return null;

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamaged(reactVector, false));
        }   

        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVector = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamaged(reactVector, false));
        }
    }

    public void HitByGrenades(Vector3 explosionPosition)
    {
        curHealth -= 100;
        Vector3 reactVector = transform.position - explosionPosition;
        StartCoroutine(OnDamaged(reactVector, true));
    }

    IEnumerator OnDamaged(Vector3 reactVector, bool isGrenade)
    {
        anim.SetBool("isWalk", false);
        yield return new WaitForSeconds(0.1f);
        
        if (!isDead)
        {
            if (curHealth > 0)
            {
                nav.enabled = false;
                anim.SetTrigger("doDamaged");

                yield return new WaitForSeconds(1f);
                nav.enabled = true;
                anim.SetBool("isWalk", true);
            }
            else
            {
                gameObject.layer = 14;
                isDead = true;
                isChase = false;

                nav.enabled = false;
                anim.SetTrigger("doDie");

                if (isGrenade)
                {
                    reactVector = reactVector.normalized;
                    reactVector += Vector3.up * 3;

                    rigid.freezeRotation = false;
                    rigid.AddForce(reactVector * 5, ForceMode.Impulse);
                    rigid.AddTorque(reactVector * 10, ForceMode.Impulse);
                }
                else
                {
                    reactVector = reactVector.normalized;
                    reactVector += Vector3.up;

                    rigid.AddForce(reactVector * 3, ForceMode.Impulse);
                    rigid.AddTorque(reactVector * 5, ForceMode.Impulse);
                }

                Destroy(gameObject, 3);
            }
        }
    }
}
