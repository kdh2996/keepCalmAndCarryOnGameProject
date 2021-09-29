using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]

public class Warrior {

    // 2018.10.03 변경

    public bool WarriorBaseTalents_On;

    public bool WarriorBasePassiveSkill_On;
    public bool WarriorBaseActiveSkill_On;
    public bool WarriorBaseActiveSkill_Roll_On;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // 기본 패시브
    // [중장갑]
    // 일반공격에 의한 피해량 10% 무시.
    // 이동속도 감소 5%
    public void WarriorBasePassiveSkill()
    {
        //m_player.WarriorBasePassive();
    }

    // 기본 액티브
    // [방어 Defense] 
    // 스킬 - 방패를 사용하는 동안 받는 물리 피해의 70% 가량을 무시함. 
    // 지속시간동안 이동불가.
    public void WarriorBaseActiveSkill_Defense()
    {
        //m_player.WarriorBaseActiveSkill_Defense();
    }

    // 기본 액티브
    // [구르기 Tumble] 
    // 스킬 - 진행방향으로 단숨에 도약합니다. 
    // (500ms 만에 3만큼 이동, 선딜레이 x, 후딜레이 3000ms)
    public void WarriorBaseActiveSkill_Tumble()
    {
        //m_player.WarriorBaseActiveSkill_Tumble();
    }

}
