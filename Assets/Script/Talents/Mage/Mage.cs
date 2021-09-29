using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage {

    // 2018.10.03 변경

    public bool MageBaseTalents_On;

    public bool MageBasePassiveSkill_On;
    //public bool MageBaseActiveSkill_SpellBlaze_On;
    public bool MageBaseActiveSkill_Roll_On;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }


    // 기본 패시브
    // [천갑]
    // 이동속도 10%증가
    public void MageBasePassiveSkill()
    {
        //m_player.MageBasePassiveSkill();
    }
    
    // 기본 액티브
    // [주문 불꽃 Spell blaze] 
    // 1.5초간 정신집중하여 원하는 위치에 RAD=4 짜리 불꽃을 소환.
    // 그 위를 지나는 모든 오브젝트는 3초간 초당 공격력의 60%의 마법피해를 준다.
    public void MageBaseActiveSkill_SpellBlaze()
    {

    }

    // 기본 액티브
    // [구르기 Tumble]
    // 진행방향으로 단숨에 도약합니다. 
    // (500ms 만에 1초만에 뛰어서 갈수있는 거리만큼 이동, 선딜레이 x, 후딜레이 3000ms)
    public void MageBaseActiveSkill_Tumble()
    {

    }

}
