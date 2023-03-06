using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
// NavMeshAgent를 사용하기 위해서 불러와야 하는 친구
using UnityEngine.AI;

public class HitBox : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshes;
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // material의 경우 MeshRenderer에 접근해서 material로 가야함
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();


        if (enemyType != Type.D) 
            // 생성 후 2초후에 따라다니기 시작
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            // 도착할 목표 위치를 지정해주는 함수
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }


    // 플레이어와 부딪히고 나서 밀리면 한없이 밀리기만 하는 버그 방지
    void FreezeVelocity ()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 6f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

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
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        // 공격모션이 살짝 딜레이가 있으니까 피격 시간에 텀을 주자
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(12f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                // 돌진 시키기
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                // 공격 범위 활성화
                meleeArea.enabled = true;

                // 잠시 멈춰주자
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                // 미사일이 바닥에 박혀서 나가서 보정값을 줌
                Vector3 startVector = transform.position + Vector3.up * 1.5f;
                GameObject instantBullet = Instantiate(bullet, startVector, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // 무기에게 피격당한 방향
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector, false));

        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // 무기에게 피격당한 방향
            Vector3 reactVector = transform.position - other.transform.position;
            // 총알에 맞았을때 총알 없애기
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVector, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPosition)
    {
        curHealth -= 100;
        Vector3 reactVector = transform.position - explosionPosition;
        StartCoroutine(OnDamage(reactVector, true));
    }


    IEnumerator OnDamage(Vector3 reactVector, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshes)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0) 
        {
            foreach (MeshRenderer mesh in meshes)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshes)
                mesh.material.color = Color.gray;
            // 죽음 처리를 위해 죽은 적을 선언한 14번째 레이어로 교환
            gameObject.layer = 14;
            isDead = true;
            isChase = false;
            // 위로 튀는걸 막은걸 해제
            nav.enabled = false;
            anim.SetTrigger("doDie");
            if (isGrenade)
            {
                // 방향 크기를 1로 평균값으로 만들기
                reactVector = reactVector.normalized;
                // 더 큰 효과를 주자
                reactVector += Vector3.up * 3;

                rigid.freezeRotation = false;
                // 공격 방향으로 스윽 힘 추가
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVector * 15, ForceMode.Impulse);
            }
            else
            {
                // 방향 크기를 1로 평균값으로 만들기
                reactVector = reactVector.normalized;
                // 살짝 위로 뜨는 방향추가
                reactVector += Vector3.up;

                // 공격 방향으로 스윽 힘 추가
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
            }

            if (enemyType != Type.D)
                Destroy(gameObject, 3);
        }
    }
}
