using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // material의 경우 MeshRenderer에 접근해서 material로 가야함
        material = GetComponent<MeshRenderer>().material;
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
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0) 
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            // 죽음 처리를 위해 죽은 적을 선언한 14번째 레이어로 교환
            gameObject.layer = 14;

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

            Destroy(gameObject, 3);
        }
    }
}
