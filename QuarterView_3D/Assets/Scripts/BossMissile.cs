using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// �Ѿ��� ��� ���� ��� �޾ƿ��� ���� ��ɸ� ���� �߰��ϱ� ����
public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        nav.SetDestination(target.position);
    }
}
