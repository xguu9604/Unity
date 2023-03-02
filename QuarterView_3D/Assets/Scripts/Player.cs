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

    // 메인 카메라 변수
    public Camera followCamera;

    // 최대치
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;


    float horizontalAxis;
    float verticalAxis;

    bool walkDown;
    bool jumpDown;

    // 공격 실행
    bool attackDown;
    // 장전
    bool reloadDown;

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
    bool isReload;

    // 공격 가능 여부 확인
    bool isReadyToAttack = true;


    Vector3 moveVector;
    // 회피하는 동안 방향 전환 막기 위해 사용
    Vector3 dodgeVector;
    // 물리 효과를 위해 RigidBody 변수 선언
    Rigidbody rigid;
    Animator anim;

    // 트리거 된 아이템을 저장하기 위한 변수 선언
    GameObject nearObject;
    // 현재 장착중인 무기 정보를 저장
    Weapon equipWeapon;
    // 첫번째 무기는 index = 0이니까 막기
    int equipWeaponIndex = -1;
    // 공격 딜레이 시간
    float attackDelay;

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
        Attack();
        Reload();
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
        attackDown = Input.GetButton("Fire1");
        reloadDown = Input.GetButtonDown("Reload");
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
        // 공격중에는 움직임 막기
        if (isSwap || !isReadyToAttack || isReload)
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
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVector);

        // 마우스에 의한 회전
        if (attackDown)
        {
            // 위의 if문이 없으면 그냥 계속 마우스 따라 캐릭터가 회전
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            // RaycastHit 정보를 저장할 변수 
            RaycastHit rayHit;
            // out : return 처럼 반환값을 주어진 변수에 저장하는 키워드
            // 아래의 경우 반환값이 rayHit에 저장
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                // point : rayHit이 찍어진 위치
                Vector3 nextVector = rayHit.point - transform.position;
                // y값이 존재하면 벽이나 위쪽을 보게 되면 애가 기울어짐
                nextVector.y = 0;
                transform.LookAt(transform.position + nextVector);
            }
        }
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

    void Attack()
    {
        // 손에 무기가 없으면 공격 못함
        if (equipWeapon == null) return;

        attackDelay += Time.deltaTime;
        // 공속이 딜레이 시간보다 작은 경우에 공격이 가능
        isReadyToAttack = equipWeapon.rate < attackDelay;

        // 공격 키를 누르고 공격 준비가 되고 피하거나 무기 교환 중이 아닌경우
        if (attackDown && isReadyToAttack && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            attackDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (ammo == 0) return;

        if (reloadDown && !isJump && !isDodge && !isSwap && isReadyToAttack)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1f);
        }
    }

    void ReloadOut()
    {
        // 재장전되는 탄환의 개수를 구해서 넣자
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
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
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            // 해당 무기의 인덱스 값을 활성화 시키기
            equipWeapon.gameObject.SetActive(true);

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
                    if (hasGrenades == maxHasGrenades) return;
                    grenades[hasGrenades].SetActive(true);
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
