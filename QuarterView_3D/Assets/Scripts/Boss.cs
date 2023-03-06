using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : HitBox
{
    public GameObject missile;
    public Transform missilePort1;
    public Transform missilePort2;

    // �÷��̾ �̵��� ��ġ�� �̸� �ľ��ϱ� ���� ����
    Vector3 lookVector;
    // ������⸦ ���� �ʿ��� ����
    Vector3 tauntVector;
    // �÷��̾ �ٶ󺸴� �÷��� ����
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // �켱 �������� ���⼭ ����
        nav.isStopped = true;
        StartCoroutine(Think());
    }

    
    void Update()
    {
        if (isDead)
        {
            // ������ ���� �ϰ� �ִ� ��� �ڷ�ƾ ����
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            // �÷��̾��� �Է°��� ������� �����ϱ�
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(h, 0, v) * 3f;
            transform.LookAt(target.position + lookVector);
        }
        else
        {
            nav.SetDestination(tauntVector);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.2f);

        // int�� �����ϸ� ������ int ���������� ����
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            // 0, 1 �̻��� �߻� ����
            case 0:
            case 1:
                StartCoroutine(MissileShoot());
                break;
            // 2, 3 �� �������� ����
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            // 4 ������� ����
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShoot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePort1.position, missilePort1.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePort2.position, missilePort2.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        // ������� ������ ������ ���⸸ �ٶ󺸱�
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVector = target.position + lookVector;

        // �����ϸ鼭 �÷��̾ �ٶ󺸴� ���� ��������
        isLook = false;
        nav.isStopped = false;
        // �����ÿ� �÷��̾�� �ε����� ���� �浹 �߻��ϴ� ���� ����
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
