using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    // 플레이어가 가질 수 있는 무기 종류의 배열
    public GameObject[] weapons;
    // 플레이어가 가지고 있는 무기를 알려주는 배열
    public bool[] hasWeapons;
    // 수류탄 배열 => 캐릭터 주위에서 둥둥 떠다닐 예정
    public GameObject[] grenades;

    // 탄약, 동전, 체력, 수류탄 변수
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

    // 최대치
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;


    float horizontalAxis;
    float verticalAxis;

    bool walkDown;
    bool jumpDown;

    // 상호작용 여부(아이템 기준)
    bool interactionDown;

    // 무기 교환 버튼
    bool swapDown1;
    bool swapDown2;
    bool swapDown3;

    // 무한 점프를 막기 위해 변수를 선언하자
    bool isJump;
    bool isDodge;
    bool isSwap;


    Vector3 moveVector;
    // 회피하는 동안 방향 전환 막기 위해 사용
    Vector3 dodgeVector;
    // 물리 효과를 위해 RigidBody 변수 선언
    Rigidbody rigid;
    Animator anim;

    // 트리거 된 아이템을 저장하기 위한 변수 선언
    GameObject nearObject;
    // 현재 장착중인 무기 정보를 저장
    GameObject equipWeapon;
    // 첫번째 무기는 index = 0이니까 막기
    int equipWeaponIndex = -1;

    void Awake()
    {
        // 초기화 작업
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        // Edit -> Project Settings -> Input Manager 에 미리 선언되어 있음
        // 뭐가? Horizontal과 Vertical이
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        // q키로 설정했다.
        interactionDown = Input.GetButtonDown("Interaction");
        // 무기 교환 키
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        
        // 회피 중에 다른 방향으로 이동 못하게 방지
        if (isDodge)
            moveVector = dodgeVector;


        // 아이템 스왑중에는 움직임 막기
        if (isSwap)
            moveVector = Vector3.zero;


        // if (walkDown)
        //     transform.position += moveVector * speed * 0.3f * Time.deltaTime;
        // else
        //     transform.position += moveVector * speed * Time.deltaTime;
        transform.position += moveVector * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;


        // SetBool()값으로 파라미터 값을 설정
        anim.SetBool("isRun", moveVector != Vector3.zero);
        anim.SetBool("isWalk", walkDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVector);
    }

    void Jump()
    {
        if (jumpDown && !isJump && moveVector == Vector3.zero && !isDodge && !isSwap) {
            // 중력값을 크게 주면 점프 크기가 작아진다
            // 따라서 중력을 늘리면서 점프 크기를 유지하려면 여기서 더 큰 값을 곱해주면 됨
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);

            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jumpDown && !isJump && moveVector != Vector3.zero && !isDodge && !isSwap) {
            dodgeVector = moveVector;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차로 함수를 호출해줌
            Invoke("DodgeOut", 0.4f);
        }
    }


    void DodgeOut()
    {
        // 속도를 원위치하기 위함
        // 구르기 할 때 속도가 2배가 되었음을 기억하자
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        // 해당 무기를 가지고 있지 않거나 그 무기를 손에 들고 있을때는 스왑 막기
        if (swapDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (swapDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (swapDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;


        // 무기의 인덱스 값을 얻기 위함
        int weaponIndex = -1;
        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if (swapDown3) weaponIndex = 2;

        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge)
        {
            // 빈손인 경우에 조건을 추가하자
            if (equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            // 해당 무기의 인덱스 값을 활성화 시키기
            equipWeapon.SetActive(true);

            // unity에서 animator에서 설정한 움직임을 트리거하는 코드
            anim.SetTrigger("doSwap");

            isSwap = true;

            // 무기를 다 바꿨으면 바꿔주자
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        // 아이템 근처에서 q키를 눌렀을 경우를 조건으로 설정
        // 점프나 회피 중에는 아이템 못먹게 막기
        if (interactionDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                // 아이템의 정보를 가져오자
                Items item = nearObject.GetComponent<Items>();
                // 가져온 아이템의 인덱스값을 얻어오자
                int weaponIndex = item.value;
                // 아이템 정보를 가져와서 해당 무기를 입수했다고 체크
                hasWeapons[weaponIndex] = true;

                // 무기를 먹었으니 무기를 없애자
                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }


    // 소모 아이템 집으러 가기
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Items item = other.GetComponent<Items>();

            switch (item.type)
            {
                case Items.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo) ammo = maxAmmo;
                    break;
                case Items.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) coin = maxCoin;
                    break;
                case Items.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) health = maxHealth;
                    break;
                case Items.Type.Grenade:
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                    break;
            }

            Destroy(other.gameObject);
        }
    }


    // 무기 주변에 갔을 때 
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }


    // 무기 주변에서 벗어났을때
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
