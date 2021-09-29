using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour {

    /* 회복 수치 관련 변수 */
    // 즉시 체력 회복 수치를 나타내는 값.
    public float onceHealValue;
    // 시간 걸쳐서 체력 회복 수치를 나타내는 값.
    public float overTimeHealValue;

    /* 회복 유형 선택 */
    // 즉시회복 하도록 제어하는 변수
    public bool isAtOnceHeal;
    // 시간 걸쳐서 회복하도록 제어하는 변수
    public bool isOverTimeHeal;

    /* 시간 걸쳐 회복시 필요 변수 */
    // 얼마만큼 회복 할지
    public float itemHealBetweenTimes;
    // 전체 회복 시간
    public float itemHealTime;



    private void OnTriggerEnter(Collider other)
    {
        LivingEntity HealObject = other.GetComponent<LivingEntity>();

        if (this.gameObject != null)
        {
            if (other.tag == "Player")
            {
                // 즉시 회복
                if (isAtOnceHeal == true)
                {
                    print("You Healed At once.");
                    HealObject.Heal(onceHealValue);
                    HealObject.PrintHealthPoint();
                    Destroy(this.gameObject);
                }
                // 시간 걸쳐서 회복
                if (isOverTimeHeal == true)
                {
                    print("You Healed Over time.");
                    HealObject.SetHealOverTime(itemHealTime, itemHealBetweenTimes, overTimeHealValue);
                    Destroy(this.gameObject);
                }

            }
        }
    }
}
