using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Archer {

    // 2018.10.03 변경

    public bool ArcherBaseTalents_On;

    public bool ArcherBasePassiveSkill_On;
    public bool ArcherBaseActiveSkill_On;
    public bool ArcherBaseActiveSkill_Roll_On;



	void Start ()
    {
        // m_player = GetComponent<Player>();
    }

    // 기본 패시브
    // [경장갑]
    // 이동속도 5% 증가
    // 일반공격에 의한 피해량 5% 무시
    public void ArcherBasePassiveSkill()
    {
        //m_player.ArcherBasePassive();
    }

    // 기본 액티브
    // [볼트 Bolt]
    // 최대 2초동안 조준하여 명중시 물리피해를 주는 투사체공격.
    // 조준을 오래할수록 피해량(공격력의 100%~150%)과 사거리(20~40) 증가.
    // 장전시 2.5초간 이동속도 30% 감소.
    public void ArcherBaseAcitveSkill_Bolt()
    {
        //m_player.ArcherBaseActiveSkill_Bolt_On();
    }

    // 기본 액티브
    // [구르기 Roll]
    public void ArcherBaseActiveSkill_Roll()
    {
        //m_player.ArcherBaseActiveSkill_Roll();
    }
}
