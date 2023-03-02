using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 공격 타입 설정해 줄 enum
    public enum Type { Melee, Range };
    // 무기 타입
    public Type type;
    // 무기 데미지
    public int damage;
    // 공속
    public float rate;
    // 최대 탄창
    public int maxAmmo;
    // 현재 탄창
    public int currentAmmo;

    // 근접 공격의 범위
    public BoxCollider meleeArea;
    // 공격 이펙트
    // TrailRenderer: 잔상을 그려주는 컴포넌트
    public TrailRenderer trailEffect;
    // 총알 프리팹을 생성해줄 위치
    public Transform bulletPosition;
    // 총알 프리팹을 받아올 변수
    public GameObject bullet;
    // 탄피
    public Transform bulletCasePosition;
    public GameObject bulletCase;

    
    // 무기를 사용하자
    public void Use()
    {
        if (type == Type.Melee)
        {
            // 같은 코루틴이라도 지금 동작하고 있는 코루틴을 중지시킨다
            // 로직이 꼬이는 경우를 방지 가능
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if (type == Type.Range && currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine("Shot");
        }
    }

    // 무기를 휘두름
    IEnumerator Swing()
    {
        // 2
        // yield return null; // 1프레임 대기
        // 3 
        // yield return new WaitForSeconds(0.1f); // 0.1초 대기

        // 1
        yield return new WaitForSeconds(0.1f); // 0.1초 대기, 일단 실행 시키자!

        // BoxCollider 활성화
        meleeArea.enabled = true;
        // 효과도 활성화
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield break; // 코루틴 탈출
    }

    IEnumerator Shot()
    {
        // 1. 총알 발사
        // Instantiate() 함수로 총알 인스턴스화 하기
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        // 50은 속력
        bulletRigid.velocity = bulletPosition.forward * 50;
        yield return null;

        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVector = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        // 즉발적인 힘 impulse
        caseRigid.AddForce(caseVector, ForceMode.Impulse);
        // 탄피의 회전
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
