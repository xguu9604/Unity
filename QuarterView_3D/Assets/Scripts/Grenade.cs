using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // 수류탄 오브젝트
    public GameObject meshObject;
    // 폭발 이펙트 오브젝트
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

        // 수류탄 피격용
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, // 터지는 위치
                                                     15, // 터지는 범위
                                                     Vector3.up, // ray가 쏘아지는 방향
                                                     0f, // 여기에 값을 주면 위의 방향만큼 이동하고 쏜다
                                                     LayerMask.GetMask("Enemy")); // 체크할 대상

        foreach(RaycastHit hitObject in rayHits)
        {
            hitObject.transform.GetComponent<HitBox>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
