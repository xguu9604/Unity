using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ���� Ÿ�� ������ �� enum
    public enum Type { Melee, Range };
    // ���� Ÿ��
    public Type type;
    // ���� ������
    public int damage;
    // ����
    public float rate;
    // �ִ� źâ
    public int maxAmmo;
    // ���� źâ
    public int currentAmmo;

    // ���� ������ ����
    public BoxCollider meleeArea;
    // ���� ����Ʈ
    // TrailRenderer: �ܻ��� �׷��ִ� ������Ʈ
    public TrailRenderer trailEffect;
    // �Ѿ� �������� �������� ��ġ
    public Transform bulletPosition;
    // �Ѿ� �������� �޾ƿ� ����
    public GameObject bullet;
    // ź��
    public Transform bulletCasePosition;
    public GameObject bulletCase;

    
    // ���⸦ �������
    public void Use()
    {
        if (type == Type.Melee)
        {
            // ���� �ڷ�ƾ�̶� ���� �����ϰ� �ִ� �ڷ�ƾ�� ������Ų��
            // ������ ���̴� ��츦 ���� ����
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if (type == Type.Range && currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine("Shot");
        }
    }

    // ���⸦ �ֵθ�
    IEnumerator Swing()
    {
        // 2
        // yield return null; // 1������ ���
        // 3 
        // yield return new WaitForSeconds(0.1f); // 0.1�� ���

        // 1
        yield return new WaitForSeconds(0.1f); // 0.1�� ���, �ϴ� ���� ��Ű��!

        // BoxCollider Ȱ��ȭ
        meleeArea.enabled = true;
        // ȿ���� Ȱ��ȭ
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield break; // �ڷ�ƾ Ż��
    }

    IEnumerator Shot()
    {
        // 1. �Ѿ� �߻�
        // Instantiate() �Լ��� �Ѿ� �ν��Ͻ�ȭ �ϱ�
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        // 50�� �ӷ�
        bulletRigid.velocity = bulletPosition.forward * 50;
        yield return null;

        // 2. ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVector = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        // ������� �� impulse
        caseRigid.AddForce(caseVector, ForceMode.Impulse);
        // ź���� ȸ��
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
