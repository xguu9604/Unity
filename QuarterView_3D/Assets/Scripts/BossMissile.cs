using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// 총알의 모든 것을 상속 받아오고 유도 기능만 따로 추가하기 위함
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
