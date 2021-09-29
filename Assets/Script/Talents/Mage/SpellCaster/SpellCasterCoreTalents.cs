using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasterCoreTalents : Mage {

    // 2018.10.03 변경

    public bool SpellCasterCoreTalents_RuneCasiting_On;
    public bool SpellCasterCoreTalents_Blink_On;
    public bool SpellCasterCoreTalents_ManaCollection_On;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // [룬캐스팅] [Rune Casting] 
    // 일반공격이 연사력이 좋은 원거리 투사체 공격으로 전환됨
    // 가장 가까운적에게 자동 조준되며, 공격력의 12%의 마법피해를 3번 줌.
    public void SpellCasterCoreTalentsSkill_RuneCasiting()
    {

    }

    // [순간이동] [Blink]
    // [구르기]가 [순간이동]으로 대체됨
    // [순간이동]은 지형을 무시하여 통과할 수 있음.
    public void SpellCasterCoreTalentsSkill_Blink()
    {

    }

    // [마나수집]
    // 1초간 정신집중상태 이후 초당 마나의 15%를 회복함.채널링이 끊기면 캔슬됨.
    public void SpellCasterCoreTalentsSkill_ManaCollection()
    {

    }

}
