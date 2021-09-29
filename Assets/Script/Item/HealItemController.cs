using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItemController : MonoBehaviour {

    // 해당 아이템 생성을 관리할 transform
    public Transform healController;

    /* 회복 아이템 설정 변수 */
    // 회복 아이템
    public HealingItem HealItem;
    // 회복 아이템 스폰 허용.
    public bool isSpawn;

    /* 스폰 위치 관련 변수 */
    // 스폰 위치 x
    public int x;
    // 스폰 위치 y
    public int y;

    /* 시간 걸쳐 회복시 필요 변수 */
    // 아이템들을 일괄 통제.
    // 즉시회복 하도록 제어하는 변수
    // public bool allisAtOnceHeal = false;
    // 시간 걸쳐서 회복하도록 제어하는 변수
    //public bool allisOverTimeHeal = false;


 
    void Start ()
    {
        if (isSpawn)
        {
            SpawnHealItem();
        }
	}
	

    // 회복 아이템 스폰하는 메소드
    public void SpawnHealItem()
    {
        // 스폰 위치 지정.
        Vector3 SpawnPosition = new Vector3((float)(x), 0, (float)(y));

        // 새로운 회복 아이템 Instant화.
        HealingItem newHealItem = Instantiate(HealItem, SpawnPosition, Quaternion.identity) as HealingItem;
        // 새로운 회복 아이템 부모 지정.
        newHealItem.transform.parent = healController;
    }

}
