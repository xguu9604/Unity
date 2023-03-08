using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // ����ź ������Ʈ
    public GameObject meshObject;
    // ���� ����Ʈ ������Ʈ
    public GameObject effectObject;
    public Rigidbody rigid;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        // ����ź �ǰݿ�
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, // ������ ��ġ
                                                     15, // ������ ����
                                                     Vector3.up, // ray�� ������� ����
                                                     0f, // ���⿡ ���� �ָ� ���� ���⸸ŭ �̵��ϰ� ���
                                                     LayerMask.GetMask("Enemy")); // üũ�� ���

        foreach(RaycastHit hitObject in rayHits)
        {
            hitObject.transform.GetComponent<HitBox>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
