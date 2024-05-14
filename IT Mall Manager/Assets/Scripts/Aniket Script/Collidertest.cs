using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Diagnostics;
public class Collidertest : MonoBehaviour
{
    public bool PlayerInRange=false;
    public float deductAmount=100;

    void Update()
    {
        if(PlayerInRange){
            CashMovement.instance.cashReachedPlayer -= deductAmount * Time.deltaTime;
            CashMovement.instance.cashReachedPlayer = Mathf.Max(CashMovement.instance.cashReachedPlayer,0);
            CashMovement.instance.UpdateMoneyUi();
        }
        if(CashMovement.instance.cashReachedPlayer<=0){
            //Debug.Log("Player is out of cash");
        }
    }
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            PlayerInRange=true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
    }
}
