using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedEntity : LivingEntity {

    // 원거리 공격 컨트롤러
    RangedWeaponController rangedController;

    /* 오브젝트 관련 변수 */
    // Player
    Player player;
    // Player와의 제곱 거리
    float SqrDstToPlayer;

    // Transporter
    Transporter transporter;
    // Transporter과의 제곱 거리
    float SqrDstToTransporter;


    /* 핵심 매개체 관련 변수 */
    // 핵심매개체로 회복 시킬 경우, 회복량
    public float CoreMedium_healPoint;
    // 핵심매개체 회복 루틴 사이클
    bool isOneCycle_Heal = false;
    // 핵심매개체 회복 거리
    public float CoreMedium_healRangeDst = 15f;



    protected override void Start ()
    {
        base.Start();

        rangedController = GetComponent<RangedWeaponController>();
	}
	
	void Update ()
    {
        // 회복 적용 중 한 사이클이 끝나면,
        if(isOneCycle_Heal == true)
        {
            isOneCycle_Heal = false;

            HealToAlliance();
        }
		
	}


    // 투사체를 발사하는 메소드
    public void Shoot()
    {
        // 무한 투사체 상태로 발사.
        rangedController.equippedRangedWeapon.ApplyLimitlessShoot();
        rangedController.LimitlessShoot();
    }

    // 아군에게 회복을 적용하는 메소드
    public void HealToAlliance()
    {

        /* 아군을 찾음. */
        // 플레이어
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // 플레이어와의 거리 계산.
        SqrDstToPlayer = (player.transform.position - transform.position).sqrMagnitude;

        // 운송체
        transporter = GameObject.FindGameObjectWithTag("Transporter").GetComponent<Transporter>();
        // 운송체와의 거리 계산.
        SqrDstToTransporter = (transporter.transform.position - transform.position).sqrMagnitude;

        // 회복 Routine 시작.
        StartCoroutine(HealRoutine(CoreMedium_healPoint));
    }

    // 힐 하는 Rountine
    IEnumerator HealRoutine(float healPoint)
    {
        // 플레이어에게 힐 적용.
        if(player != null)
        {
            if(SqrDstToPlayer <= Mathf.Pow(CoreMedium_healRangeDst,2))
            {
                player.Heal(healPoint);
                // print("==YOU HEALED BY CORE MEDIUM==");
            }
        }

        // 운송체에게 힐 적용.
        if(transporter != null)
        {
            if (SqrDstToTransporter <= Mathf.Pow(CoreMedium_healRangeDst, 2))
            {
                transporter.Heal(healPoint);
                print("==Transporter HEALED BY CORE MEDIUM==");
            }
        }

        yield return new WaitForSeconds(1f);

        // 사이클 한 번 끝났음을 알림.
        isOneCycle_Heal = true;
    }
}
