using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianExpertTalents : Warrior {

    // 2018.10.03 변경

    public bool GuardianExpertTalents_HammerSecurities_On;
    public bool GuardianExpertTalents_HeavySmash_On;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // [고유: 망치 경호대] [Hammer Securities] 
    // : 일반무기가 망치로 변경됩니다.
    // 기본공격 마지막 콤보에 맞은 적은 넉백됩니다.
    public void GuardianExpertTalentsSkill_HammerSecurities()
    {
        //m_player.GuardianExpertTalentsSkill_HammerSecurities();
    }

    // [육중한 강타] 
    // : 플레이어가 바라보는 방향으로 WID=2 RANGE=3 만큼 일직선으로 공격력의 80%만큼 
    // 광역 물리피해 4초에 1개씩 충전됨 최대 장전수 5개
    public void GuardianExpertTalentsSkill_HeavySmash()
    {
        //m_player.GuardianExpertTalentsSkill_HeavySmash();
    }

}
