using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    void OnCollisionEnter(Collision collision)
    {
        // ź�ǰ� �ٴڿ� �������� ������
        if (collision.gameObject.tag == "Floor")
        {
            // 1�ʵڿ� �ٴڿ��� �Ѿ� ���ֱ�
            Destroy(gameObject, 1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ���� ������ ��쿡�� ���ָ� ���� �ֵ��� ���� ���ϰ� ��;;
        if (other.gameObject.tag == "Wall" && !isMelee)
        {
            Destroy(gameObject);
        }
    }
}
