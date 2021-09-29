using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourResource : MonoBehaviour {


    /* 자원 관련 변수 */
    // 전체 행동력 수치
    public float totBehaviourPoint = 100;
    // 현재 행동력 수치
    public float behaviourPoint;

    // 행동력 사용 판단 변수
    public bool isBehaving = false;

    // 회복 수치
    public float resourceRecoveryVal = 40;
    // 회복 주기
    public float resourceRecoveryRate = 1000;
    // 회복 사이클 판단 (중첩 회복 방지)
    public bool IsResourceCycle = false;


    /* 영혼 에너지 관련 변수 */
    // 전체 영혼 에너지 수치
    public float totspiritEnergy = 100f;
    // 현재 영혼 에너지 수치
    public float spiritEnergy;

    // 영혼 에너지 사용 판단 변수
    public bool isSpiritEnergyApplied = false;



    /* 각 행동별 자원 코스트 */
    // 근접 공격시 소모되는 수치
    public float MeleeActionCost = 10;
    // 원거리 공격시 소모되는 수치
    public float RangedActionCost = 10;

    // 아처 - 암살자 특성
    // 돌진 스킬 사용시 소모되는 수치
    public float Assassin_RushAtkCost = 25;

    // 메이지 - 스펠케스터 특성
    // 주문 불꽃 사용시 소모되는 수치
    public float SpellCaster_SpellBlazeCost = 75;



    void Start()
    {
        behaviourPoint = totBehaviourPoint;
        spiritEnergy = totspiritEnergy;
    }


    /* 행동력 소모, 회복을 직접 하는 메소드 */
    // 행동력을 소모하는 메소드
    public void UseBehaviourResource(float cost)
    {
        // 영혼 에너지 적용시 먼저 소비
        if(isSpiritEnergyApplied == true && spiritEnergy > 0)
        {
            UseSpiritEnergy(cost);
            return;
        }

        if (behaviourPoint >= 0)
        {
            behaviourPoint -= cost;
        }

        // 음수 수치 방지.
        if (behaviourPoint < 0)
        {
            behaviourPoint = 0;
        }
    }

    // 행동력을 회복하는 메소드
    public void RecoverBehaviourResource(float val)
    {
        behaviourPoint += val;

        // 전체 수치를 넘었을 경우, 자동으로 조정.
        if (behaviourPoint > totBehaviourPoint)
        {
            behaviourPoint = totBehaviourPoint;
        }

    }

    /* 영혼 에너지 소모를 하는 메소드 */
    // 영혼 에너지를 초기화 하는 메소드
    public void InitSpiritEnergy()
    {
        spiritEnergy = totspiritEnergy;
    }

    // 영혼 에너지를 소모하는 메소드
    public void UseSpiritEnergy(float cost)
    {
        if (spiritEnergy >= 0)
        {
            spiritEnergy -= cost;
        }

        // 음수 수치 방지.
        if (spiritEnergy < 0)
        {
            spiritEnergy = 0;
        }

        print("== Spirit Energy is consumed");
        print("** Sprit Enrgy : " + spiritEnergy + " **");

    }


    /* 행동력 회복을 하는 메소드 */
    // 시간 걸쳐서 자원량을 회복하도록 셋팅하는 메소드.
    public void SetRecoverOverTime(float totRecoveryTimes, float recoveryRate, float recoverValue)
    {
        // 행동력을 소모하는 행동을 하지 않을 경우에,
        if (isBehaving == false)
        {
            StartCoroutine(ResourceRecoverOverTime(totRecoveryTimes, recoveryRate, recoverValue));
        }
    }

    // 시간 걸쳐 회복하는 Routine
    IEnumerator ResourceRecoverOverTime(float totRecoveryTimes, float recoveryRate, float recoverValue)
    {
        float recoverTime = 0;
        float nextHealTime = 0;

        // 사이클 돌 경우,
        IsResourceCycle = true;

        while (recoverTime < totRecoveryTimes)
        {
            recoverTime += Time.deltaTime;

            if (Time.time > nextHealTime)
            {
                nextHealTime = Time.time + recoveryRate / 1000;
                this.RecoverBehaviourResource(recoverValue);
                //print(this.behaviourPoint);
            }

            yield return null;
        }

        // 사이클 해제.
        IsResourceCycle = false;
        //print("Behaviour Resource Recovered");
    }

    // 원거리 공격시 행동력을 %만큼 감소시키는 메소드
    public void DecByPercent_RangedWeaponBehaviour(float percent)
    {
        RangedActionCost = RangedActionCost * (percent / 100);

        // 소수점 제거.
        RangedActionCost = (int)RangedActionCost;
    }

}
