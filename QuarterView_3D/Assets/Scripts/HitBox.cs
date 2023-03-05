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
        // material�� ��� MeshRenderer�� �����ؼ� material�� ������
        material = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // ���⿡�� �ǰݴ��� ����
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector, false));

        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // ���⿡�� �ǰݴ��� ����
            Vector3 reactVector = transform.position - other.transform.position;
            // �Ѿ˿� �¾����� �Ѿ� ���ֱ�
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
            // ���� ó���� ���� ���� ���� ������ 14��° ���̾�� ��ȯ
            gameObject.layer = 14;

            if (isGrenade)
            {
                // ���� ũ�⸦ 1�� ��հ����� �����
                reactVector = reactVector.normalized;
                // �� ū ȿ���� ����
                reactVector += Vector3.up * 3;

                rigid.freezeRotation = false;
                // ���� �������� ���� �� �߰�
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVector * 15, ForceMode.Impulse);
            }
            else
            {
                // ���� ũ�⸦ 1�� ��հ����� �����
                reactVector = reactVector.normalized;
                // ��¦ ���� �ߴ� �����߰�
                reactVector += Vector3.up;

                // ���� �������� ���� �� �߰�
                rigid.AddForce(reactVector * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 3);
        }
    }
}
