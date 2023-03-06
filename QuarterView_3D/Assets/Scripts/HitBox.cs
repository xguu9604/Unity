using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
// NavMeshAgent�� ����ϱ� ���ؼ� �ҷ��;� �ϴ� ģ��
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
        // material�� ��� MeshRenderer�� �����ؼ� material�� ������
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();


        if (enemyType != Type.D) 
            // ���� �� 2���Ŀ� ����ٴϱ� ����
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
            // ������ ��ǥ ��ġ�� �������ִ� �Լ�
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }


    // �÷��̾�� �ε����� ���� �и��� �Ѿ��� �и��⸸ �ϴ� ���� ����
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
        // ���ݸ���� ��¦ �����̰� �����ϱ� �ǰ� �ð��� ���� ����
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
                // ���� ��Ű��
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                // ���� ���� Ȱ��ȭ
                meleeArea.enabled = true;

                // ��� ��������
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                // �̻����� �ٴڿ� ������ ������ �������� ��
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
            // ���⿡�� �ǰݴ��� ����
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector, false));

        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // ���⿡�� �ǰݴ��� ����
            Vector3 reactVector = transform.position - other.transform.position;
            // �Ѿ˿� �¾����� �Ѿ� ���ֱ�
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
            // ���� ó���� ���� ���� ���� ������ 14��° ���̾�� ��ȯ
            gameObject.layer = 14;
            isDead = true;
            isChase = false;
            // ���� Ƣ�°� ������ ����
            nav.enabled = false;
            anim.SetTrigger("doDie");
            if (isGrenade)
            {
                // ���� ũ�⸦ 1�� ��հ����� �����
                reactVector = reactVector.normalized;
                // �� ū ȿ���� ����
                reactVector += Vector3.up * 3;

                rigid.freezeRotation = false;
                // ���� �������� ���� �� �߰�
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVector * 15, ForceMode.Impulse);
            }
            else
            {
                // ���� ũ�⸦ 1�� ��հ����� �����
                reactVector = reactVector.normalized;
                // ��¦ ���� �ߴ� �����߰�
                reactVector += Vector3.up;

                // ���� �������� ���� �� �߰�
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
            }

            if (enemyType != Type.D)
                Destroy(gameObject, 3);
        }
    }
}
