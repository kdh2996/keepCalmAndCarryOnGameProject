using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianStyleTalents : Warrior {

    // 2018.10.03 변경

    public bool GuardianStyleTalents_KnockBack_On;

    public bool GuardianStyleTalents_DefenseVanguard_On;

    public bool GuardianStyleTalents_DefenseReinforcement;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // [넉백] [Knock-back] 
    // : [방어]스킬이 발동중일때만 사용가능 
    // 방패로 적을(3/4/5)만큼 뒤로 밀쳐내면서
    // 공격력의(30%/40%/50%)만큼 마법피해를 입힘. - 2티어
    public void GuardianStyleTalentsSkill_KnockBack()
    {
        //m_player.GuardianStyleTalentsSkill_KnockBack();
    }

}
