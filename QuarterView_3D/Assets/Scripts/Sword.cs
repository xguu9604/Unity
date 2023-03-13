using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;

    public BoxCollider meleeArea;


    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);

        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.7f);
        meleeArea.enabled = false;

        yield break;
    }
}
