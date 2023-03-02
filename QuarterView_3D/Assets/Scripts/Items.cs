using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    // 아이템 종류를 받아올 변수
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
