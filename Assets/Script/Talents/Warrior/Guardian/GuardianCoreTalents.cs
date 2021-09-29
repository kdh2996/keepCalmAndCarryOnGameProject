using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianCoreTalents : Warrior {

    // 2018.10.03 변경

    public bool GuardianCoreTalents_Taunt_On;
    public bool GuardianCoreTalents_Roar_On;


    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // [도발 Taunt] 
    // : 방패를 두드리며 RAD=8 주변의 적이 자신을 공격하게 함. (채널링 없음.)
    public void GuardianCoreTalentsSkill_Taunt()
    {
        //m_player.GuardianCoreTalentsSkill_Taunt();
    }

    // [포효] [Roar] 
    // : [방어] 스킬을 사용중일때만 사용가능한 연계기. 
    // 자신을 기점으로 RAD=2안에 있는 적들이
    // 각각 플레이어를 기점으로 RAD=5까지 넉백됨
    public void GuardianCoreTalentsSkill_Roar()
    {
        //m_player.GuardianCoreTalentsSkill_Roar();
    }


}
