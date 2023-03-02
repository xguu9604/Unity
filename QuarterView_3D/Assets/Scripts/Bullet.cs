using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        // 탄피가 바닥에 떨어지면 없애자
        if (collision.gameObject.tag == "Floor")
        {
            // 1초뒤에 바닥에서 총알 없애기
            Destroy(gameObject, 1);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
