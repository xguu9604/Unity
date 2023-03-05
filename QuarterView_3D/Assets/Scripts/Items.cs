using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    // 아이템 종류를 받아올 변수
    public Type type;
    public int value;


    Rigidbody rigid;
    // 여기서 sphercollider를 선언하는데 두개중에 무조건 위에 있는 친구 하나만 선언이 되는 방식
    // 따라서 아이템 기준 습득을 위해 설정한 범위가 언제나 하위에 존재해야함
    SphereCollider sphereCollider;


    // 아이템이 생성되고 대상 컴포넌트들을 불러오자
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }


    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }


    // 아이템이 충돌할 때 생기는 로직들
    void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿으면 운동로직만 트루하고 콜라이더를 없애자
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
