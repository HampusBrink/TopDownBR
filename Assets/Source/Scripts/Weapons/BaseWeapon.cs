using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{

    public BoxCollider2D weaponCol;
    
    [Header("Stats")]
    public float baseDamage = 10f;
    public float baseAttackSpeed = 2f;

    public void WeaponPerformAttack()
    {
        //ActivateAttack();
    }
    
    // private IEnumerator ActivateAttack()
    // {
    //     weaponCol.enabled = true;
    //     //yield return new WaitForSeconds()
    // }
}
