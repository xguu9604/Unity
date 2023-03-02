using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        // ź�ǰ� �ٴڿ� �������� ������
        if (collision.gameObject.tag == "Floor")
        {
            // 1�ʵڿ� �ٴڿ��� �Ѿ� ���ֱ�
            Destroy(gameObject, 1);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
