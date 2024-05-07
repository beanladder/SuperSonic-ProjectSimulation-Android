using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CashOutflow : MonoBehaviour
{
    [SerializeField] Transform[] CashHolder = new Transform[6];
    [SerializeField] GameObject CashPrefab;
    [SerializeField] float CashDeliveryTime,Yaxis;
    void Start()
    {
        for(int i = 0; i < CashHolder.Length; i++)
        {
            CashHolder[i]= transform.GetChild(0).GetChild(i);
        }
        StartCoroutine(CashSpawn(CashDeliveryTime));
    }

    public IEnumerator CashSpawn(float time)
    {
        int CashIndex = 0;
        int CountCash = 0;
        while (CountCash < 100)
        {
            GameObject newCash = Instantiate(CashPrefab, new Vector3(transform.position.x, -3f, transform.position.z), Quaternion.identity, transform.GetChild(1));

            newCash.transform.DOJump(new Vector3(CashHolder[CashIndex].position.x, CashHolder[CashIndex].position.y + Yaxis, CashHolder[CashIndex].position.z), 2f, 1, 0.5f).SetEase(Ease.OutQuad);
            if(CashIndex < 5)
            {
                CashIndex++;
            }
            else
            {
                CashIndex = 0;
                Yaxis += 0.05f;
            }
            yield return new WaitForSeconds(time);
        }
    }
}
