using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasterExpertTalents : Mage {

    // 2018.10.03 변경

    public bool SpellCasterExpertTalents_CoreMedium_On;
    public bool SpellCasterExpertTalents_TornadoBlaze_On;

    Player m_player;



    void Start()
    {
        //m_player = GetComponent<Player>();
    }

    // [고유: 핵심매개체] 
    // : 플레이어를 따라다니는 구체의 위습형태의 소환물을 소환.
    // 스테이지 시작때부터 이미 영구소환 상태이며, 죽지도 않음.
    // 플레이어가 공격할때마다 똑같은 방향으로 원거리 투사체 공격.
    // (소환사의 공격력의 15%)을 하며 RAD = 15 안의 우호적인 대상에게
    // 초당 25의 체력을 회복시켜준다.
    public void SpellCasterExpertTalentsSkill_CoreMedium()
    {

    }

    // [회오리불꽃] [Tornado Blaze] 
    // : 회오리치는 불기둥을 발사함.  
    // RAD=3 짜리 불기둥은 2초동안 12m를 전진한 후 4초동안 지속됨.
    // 불기둥은 경로에 있는 오브젝트를 전부 끌어당김.
    // 불길에 닿는 적은 0.3초당 공격력의 25%의 마법피해를 줌.
    // 이 공격에 3번 적중할때마다 공격력의 75%만큼 추가 마법피해를 줌.
    public void SpellCasterExpertTalentsSkill_TornadoBlaze()
    {

    }




}
