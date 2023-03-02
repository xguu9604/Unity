using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // 카메라가 따라다닐 친구
    public Transform target;
    // 따라갈 목표와 위치 오프셋을 public 변수로 선언
    public Vector3 offset;


    void Update()
    {
        transform.position = target.position + offset;
    }
}
