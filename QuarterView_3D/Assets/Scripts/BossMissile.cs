using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// �Ѿ��� ��� ���� ��� �޾ƿ��� ���� ��ɸ� ���� �߰��ϱ� ����
public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;
    public GameObject meshObject;
    public GameObject effectObject;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(LimitTime());
    }


    void Update()
    {
        nav.SetDestination(target.position);
    }

    IEnumerator LimitTime()
    {
        yield return new WaitForSeconds(5f);
        meshObject.SetActive(false);
        effectObject.SetActive(true);
        nav.isStopped = true;

        Destroy(gameObject, 1f);
    }
}
