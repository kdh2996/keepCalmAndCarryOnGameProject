using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedController : MonoBehaviour
{

    /* 소환 위치 */
    // 소환 위치.
    public Transform summonedPos;

    /* 소환 종류 */
    // 소환물
    public SummonedEntity summonedObject;

    /* 소환 상태 */
    // 현재 소환된 것.
    public SummonedEntity currentSummonedObject;

    /* 소환물의 행동 */



    void Start()
    {

    }

    void Update()
    {
        // 소환수가 파괴되면, 마찬가지로 구체 역시 파괴된다.
        if (summonedPos == null)
        {
            Destroy(currentSummonedObject.gameObject);
        }

    }


    // 소환물을 생성하는 메소드
    public void CreateSummonedObject()
    {
        if (currentSummonedObject != null)
        {
            Destroy(currentSummonedObject.gameObject);
        }

        // 소환 하는 부분.
        currentSummonedObject = Instantiate(summonedObject, summonedPos.position, summonedPos.rotation) as SummonedEntity;
        // 소환 위치를 부모로 지정.
        currentSummonedObject.transform.parent = summonedPos;

    }

    // 투사체를 발사하는 메소드
    public void Shoot()
    {
        if (currentSummonedObject != null)
        {
            currentSummonedObject.Shoot();
        }
    }

}
