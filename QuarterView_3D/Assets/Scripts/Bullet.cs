using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision collision)
    {
        // 탄피가 바닥에 떨어지면 없애자
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            // 1초뒤에 바닥에서 총알 없애기
            Destroy(gameObject, 1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 근접 공격의 경우에도 없애면 이제 애들이 공격 못하게 됨;;
        if (other.gameObject.tag == "Wall" && !isMelee)
        {
            Destroy(gameObject);
        }
    }
}
