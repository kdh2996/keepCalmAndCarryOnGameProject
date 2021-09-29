using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, Idamageable {

    BehaviourResource behaviourResource_LivingEntity;

    /* 살아있는 상태에 대한 변수 */
    // 죽었는지 살았는지
    protected bool dead;
    // 죽었음을 알려줌.
    public event System.Action OnDeath;

    /* 체력 관련 변수 */
    // 전체 체력
    public float totHealthPoint;
    // 현재 체력
    protected float healthPoint;

    /* 상태 관련 변수 */
    public bool isDamageimmunity = false;

    // 에너지 방벽에 대한 특수 처리
    public bool isEnergyBarrierOn = false;


    /*체력 재생 관련 변수 */
    // 체력 재생 판별 변수
    public bool selfRegenerate = false;
    // 자동 체력 주기
    bool restoreCycle = true;
    // 자동 체력 재생 수치.
    public float restoreValue;
    // 자동 체력 재생 속도. (ms 단위)
    public float restoreSpeed;
    // 자동 체력 재생 시간. (ms 단위)
    public float restoreTime;


    /* 방어력 관련 변수 */
    // 고정 수치 방어력 적용
    public bool applySteadyValDefense;
    // 비율 방어력 적용
    public bool applyRateDefense;
    // 고정 수치 방어력
    public float SteadDefensePoint;

    // 비율 방어력
    [Range(0, 100)]
    public float RateDefensePercent;
    // 비율 방어력의 초기 값
    [Range(0, 100)]
    public float initRateDefensePercent;

    // 순간 비율 감소 방어력
    [Range(0, 100)]
    public float RateWeakDefensePercent_atOnce;

    // 고정된 피해 감소량
    public float FixedDamageDecrement = 0;


    // 2018.10.05 부로, 기존의 이동관련 메소드는 사용하지 않아 비활성화 한다.

    /* 이동 관련 변수 */
    /*
    // 처음 속도
    public float startSpeed = 5;
    // 이동속도
    public float moveSpeed = 5;
    // 최대 속도
    public float maxSpeed;

    // 속도를 초기화 하는 변수
    public bool resetSpeed;
    // 속도를 감속시키는 것 허용.
    public bool applyDeceleration;

    // 속도 감소 판별 변수.
    protected bool isSpeedDown = false;
    // 속도 증가 판별 변수.
    protected bool isSpeedUp = true;

    // 일정한 가속도 적용 (단 0이 아님)
    public bool applyConstantAccer;
    // 증가하는 가속도 적용
    public bool applyChangeAccer;
    // 갑작스러운 속도 변화 적용
    public bool applySuddenChangeSpeed;

    // 가속도
    public float acceleration;
    // 가속도 증감 비율
    public float accelerationChangeRate;

    // 입력 속도
    public float inputSpeed;
    */



    // 덮의씌움 방지 virtual 키워드.
    protected virtual void Start()
    {
        healthPoint = totHealthPoint;

        initRateDefensePercent = RateDefensePercent;

        behaviourResource_LivingEntity = GetComponent<BehaviourResource>();
    }



    /* 데미지 적용 관련 메소드 */

    // 충돌 시 데미지 적용
    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    // 데미지 적용 메소드
    public bool TakeDamage(float damage)
    {
        // 데미지 면역 상태 일 경우, 바로 return
        if(isDamageimmunity == true)
        {
            return false;
        }

        // 에너지 방벽 상태 일 경우,
        if (isEnergyBarrierOn == true)
        {
            behaviourResource_LivingEntity.spiritEnergy -= damage * 0.6f;
            behaviourResource_LivingEntity.spiritEnergy = (int)behaviourResource_LivingEntity.spiritEnergy;

            if(behaviourResource_LivingEntity.spiritEnergy < 0)
            {
                behaviourResource_LivingEntity.spiritEnergy = 0;
            }

            print("** SpiritEnergy : " + behaviourResource_LivingEntity.spiritEnergy + " **");

            if (behaviourResource_LivingEntity.spiritEnergy > 0)
            {
                print("== EnergyBarrier prevents Damage ==");
                return false;
            }

        }

        // 실 데미지.
        float RealDamage = 0;


        // 방어력 적용
        // 데미지에 방어력을 적용하여 받음.
        if (applySteadyValDefense == true)
        {
            // 고정 수치 방어력 적용.
            RealDamage = damage - SteadDefensePoint;
       
            print(RealDamage);
        }
        else if (applyRateDefense == true)
        {
            // 비율 방어력에 순간 감소 방어력이 적용
            RateDefensePercent -= RateWeakDefensePercent_atOnce;

            if(RateDefensePercent <= 0)
            {
                RateDefensePercent = 0;
            }

            // 비율 방어력 적용.
            RealDamage = damage - (int)(damage * (RateDefensePercent / 100));

            print(RealDamage);
        }
        else
        {
            RealDamage = damage;
        }


        // 고정된 피해감소량 적용
        RealDamage -= FixedDamageDecrement;


        // 데미지가 음수 값일 경우 조정.
        if (RealDamage < 0)
        {
            RealDamage = 0;
        }

        print("Take Damage : " + RealDamage);

        healthPoint -= RealDamage;

        // 순간 방어력 감소치 초기화
        RateWeakDefensePercent_atOnce = 0;

        if (healthPoint <= 0 && !dead)
        {
            Die();
        }

        return true;
    }


    /* 체력 관련 메소드 */

    // 현재 체력을 얻는 메소드
    public float GetHealthPoint()
    {
        return healthPoint;
    }

    // 현재 체력을 보여주는 메소드
    public void PrintHealthPoint()
    {
        print(healthPoint);
    }


    /* 체력 회복 관련 메소드 */

    // 체력을 회복하는 메소드
    public void Heal(float healPoint)
    {
        if(dead == false)
        {
            healthPoint += healPoint;

            if (healthPoint > totHealthPoint)
            {
                healthPoint = totHealthPoint;
            }
        }
    }

    // 시간 걸쳐서 회복하도록 셋팅하는 메소드.
    public void SetHealOverTime(float totHealTime, float HealBetweenTimes, float healValue)
    {
        StartCoroutine(HealOverTime(totHealTime, HealBetweenTimes, healValue));
    }

    // 시간 걸쳐 회복하는 Routine
    IEnumerator HealOverTime(float totHealTime, float healBetweenTimes, float healValue)
    {
        float HealTime = 0;
        float nextHealTime = 0;

        while (HealTime < totHealTime)
        {
            HealTime += Time.deltaTime;

            if (Time.time > nextHealTime)
            {
                nextHealTime = Time.time + healBetweenTimes / 1000;
                this.Heal(healValue);
                print(this.healthPoint);
            }

            yield return null;
        }

        print("All Heal Completed!");
    }

    // 체력 재생 시작.
    public void RegenerateHealth()
    {
        // 자동 체력 재생.
        if (selfRegenerate == true && restoreCycle == true)
        {
            restoreCycle = false;
            StartCoroutine(OneRestoreCycleOver());
        }
    }

    // 체력 재생 사이클 초기화 Routine
    IEnumerator OneRestoreCycleOver()
    {
        // 체력 재생 사이클 시작.
        SetHealOverTime(restoreTime, restoreSpeed, restoreValue);
        yield return new WaitForSeconds(restoreTime/1000);

        // 사이클 한 번 돌았으므로, 다시 사이클 변수 초기화.
        print(healthPoint);
        restoreCycle = true;
    }


    /* 죽음 관련 메소드 */

    // 객체 죽음 적용
    protected void Die()
    {
        dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject);
    }


    // 2018.10.05 부로, 기존의 이동관련 메소드는 사용하지 않아 비활성화 한다.

    /* 이동 관련 메소드 
     
    public void ControlSpeed()
    {
        // 속도 관련 전부 리셋
        if (resetSpeed == true)
        {
            applyChangeAccer = false;
            applyChangeAccer = false;
            applySuddenChangeSpeed = false;
            resetSpeed = false;

            moveSpeed = startSpeed;

            isSpeedUp = true;
            isSpeedDown = false;
        }


        // 일정한 가속도 적용.
        if (applyConstantAccer == true)
        {
            // 속도 증가 구간.
            if (isSpeedUp == true && moveSpeed < maxSpeed)
            {
                print(moveSpeed);
                moveSpeed = moveSpeed + acceleration * Time.deltaTime;

            }

            // 속도 감소 구간.
            if (isSpeedDown == true && moveSpeed >= startSpeed)
            {
                print(moveSpeed);
                moveSpeed = moveSpeed - acceleration * Time.deltaTime;

            }

            // 속도 감속 허용시 감소 구간으로 진입.
            if (applyDeceleration == true && moveSpeed >= maxSpeed)
            {
                moveSpeed = moveSpeed - acceleration * Time.deltaTime;
                isSpeedDown = true;
                isSpeedUp = false;
            }
        }

        // 가변하는 가속도 적용.
        if (applyChangeAccer == true)
        {
            // 속도 증가 구간.
            if (isSpeedUp == true && moveSpeed < maxSpeed)
            {
                print(moveSpeed);
                print(acceleration);
                acceleration *= accelerationChangeRate;
                moveSpeed = moveSpeed + acceleration * Time.deltaTime;
            }

            // 속도 감소 구간.
            if (isSpeedDown == true && moveSpeed >= startSpeed)
            {
                print("Speed" + moveSpeed);
                print("Accer" + acceleration);
                acceleration *= accelerationChangeRate;
                moveSpeed = moveSpeed - acceleration * Time.deltaTime;
            }

            // 속도 감속 허용시 감소 구간으로 진입.
            if (applyDeceleration == true && moveSpeed >= maxSpeed)
            {
                moveSpeed = moveSpeed - acceleration * Time.deltaTime;
                isSpeedDown = true;
                isSpeedUp = false;
            }
        }

        // 갑작스러운 속도 변화 적용.
        if (applySuddenChangeSpeed == true)
        {
            moveSpeed = inputSpeed;
        }
    }

    */

}
