using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // 공전을 하는 기준점
    public Transform target;
    // 공전 속도
    public float orbitSpeed;
    // 목표와의 거리
    Vector3 offset;


    void Start()
    {
        // 현재 수류탄 위치 - 타겟 위치
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
        // 한 대상 주변을 돌게 해주는 메서드
        transform.RotateAround(target.position,
                                Vector3.up,
                                orbitSpeed * Time.deltaTime);

        offset = transform.position - target.position;
    }
}
