using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : HitBox
{
    public GameObject missile;
    public Transform missilePort1;
    public Transform missilePort2;

    // 플레이어가 이동할 위치를 미리 파악하기 위한 벡터
    Vector3 lookVector;
    // 내려찍기를 위해 필요한 벡터
    Vector3 tauntVector;
    // 플레이어를 바라보는 플래그 변수
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // 우선 움직임을 여기서 막자
        nav.isStopped = true;
        StartCoroutine(Think());
    }

    
    void Update()
    {
        if (isDead)
        {
            // 죽으면 지금 하고 있는 모든 코루틴 막기
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            // 플레이어의 입력값을 기반으로 예측하기
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

        // int로 선언하면 무조건 int 범위에서만 돈다
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            // 0, 1 미사일 발사 패턴
            case 0:
            case 1:
                StartCoroutine(MissileShoot());
                break;
            // 2, 3 돌 굴러가는 패턴
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            // 4 내려찍기 패턴
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
        // 기모으는 동안은 모으는 방향만 바라보기
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

        // 점프하면서 플레이어를 바라보는 것을 방지하자
        isLook = false;
        nav.isStopped = false;
        // 점프시에 플레이어와 부딪히면 물리 충돌 발생하는 것을 방지
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
