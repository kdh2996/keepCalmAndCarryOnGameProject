using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    // 근거리 무기 잡는 부분
    public Transform weaponHold;
    // 방패 잡는 부분 (왼쪽)
    public Transform weaponHold_Left;

    /* 근거리 무기 */

    // 처음 무기
    public MeleeWeapon startingMeleeWeapon;
    // 무기 변환시 쓰일 무기
    public MeleeWeapon transMeleeWeapon;

    // Warrior 전용 근접 무기
    public MeleeWeapon WarriorMeleeWeapon;
    // Mage 전용 근접 무기
    public MeleeWeapon MageMeleeWeapon;

    // 장착중인 근거리 무기
    public MeleeWeapon equippedMeleeWeapon;

    // 근거리 사용 여부 변수
    public bool UseMelee = false;

    // 근거리 무기 종류 판별 변수
    private bool isWarriorKnife = false;
    private bool isMageSword = false;

    // 근거리 무기 위치 벡터
    Vector3 MeleePos;
    Vector3 MeleeRot;

    // Warrior 전용 근거리 무기 위치 벡터 (단검)
    Vector3 WarriorKnifePos;

    // Mage 전용 근거리 무기 위치 벡터 (양손검)
    Vector3 MageSwordPos;
    Vector3 MageSwordRot;

    /* 방패 */

    // 쓰일 방패 종류
    public Shield startingShield;

    // 장착중인 방패
    public Shield equippedShield;

    // 방패 위치 벡터
    Vector3 shiledPos;
    Vector3 shiledRot;

    // 방패 데미지 퍼센트
    float shieldDamagePercent = 30;

    /* 망치 */

    // 쓰일 망치 종류
    public Hammer startingHammer;



    private void Start()
    {
        // 근접 무기 소환 위치 설정.
        MeleePos = new Vector3(-0.00016f, 0.00265f, 0.00188f); 
        MeleeRot = new Vector3(-5.507f, 62.046f, 87.33801f);

        WarriorKnifePos = new Vector3(-0.002919583f, 0.002908814f, 0.007088697f);

        MageSwordPos = new Vector3(-0.00092f, 0.00112f, 0.00773f);
        MageSwordRot = new Vector3(-8.868f, 80.967f, -79.90401f);


        shiledPos = new Vector3(-0.0002437282f, -0.0005343346f, -0.0004673656f);
        shiledRot = new Vector3(-3.283f, 2.276f, -90.436f);

        if (UseMelee == true && startingMeleeWeapon != null)
        {
            EquipStick(startingMeleeWeapon);
        }
    }


    /* 무기를 장착과 해제하는 메소드 */

    // 근거리 무기 장착 메소드
    public void EquipStick(MeleeWeapon stickToEquip)
    {

        // 이미 장착중일 경우, 장착 중인 무기 파괴.
        if (equippedMeleeWeapon != null)
        {
            Destroy(equippedMeleeWeapon.gameObject);
        }

        equippedMeleeWeapon = Instantiate(stickToEquip, MeleePos, Quaternion.Euler(MeleeRot.x, MeleeRot.y, MeleeRot.z)) as MeleeWeapon;

        // 근거리 무기 오브젝트가 플레이어를 따라 움직이도록.
        equippedMeleeWeapon.transform.parent = weaponHold;

        // 근거리 무기 오브젝트의 위치를 알맞게 로칼 위치, 회전값으로 조정.
        equippedMeleeWeapon.transform.localPosition = MeleePos;
        equippedMeleeWeapon.transform.localRotation = Quaternion.Euler(MeleeRot.x, MeleeRot.y, MeleeRot.z);

        if(isWarriorKnife == true)
        {
            equippedMeleeWeapon.transform.localPosition = WarriorKnifePos;
            equippedMeleeWeapon.transform.localRotation = Quaternion.Euler(MeleeRot.x, MeleeRot.y, MeleeRot.z);
        }

        if(isMageSword == true)
        {
            equippedMeleeWeapon.transform.localPosition = MageSwordPos;
            equippedMeleeWeapon.transform.localRotation = Quaternion.Euler(MageSwordRot.x, MageSwordRot.y, MageSwordRot.z);
        }

    }

    // Warrior 전용 근접 무기를 장착하는 메소드
    public void EquipWarriorMeleeWeapon_Knife()
    {
        if (WarriorMeleeWeapon != null)
        {
            isWarriorKnife = true;
            isMageSword = false;

            EquipStick(WarriorMeleeWeapon);
            
        }
    }

    // Mage 전용 근접 무기를 장착하는 메소드
    public void EquipMageMeleeWeapon_TwoHandsSword()
    {
        if (MageMeleeWeapon != null)
        {
            isMageSword = true;
            isWarriorKnife = false;

            EquipStick(MageMeleeWeapon);
        }
        
    }

    // 망치를 장착하는 메소드
    public void EquipHammer()
    {
        if(startingHammer != null)
        {
            EquipStick(startingHammer);
        }
    }

    // 전환된 근거리 무기 장착 메소드
    public void EquiTransStick()
    {
        if (transMeleeWeapon != null)
        {
            EquipStick(transMeleeWeapon);
        }
    }

    // 근거리 무기 파괴 메소드
    public void DestroyStick()
    {
        if (equippedMeleeWeapon != null)
        {
            Destroy(equippedMeleeWeapon.gameObject);
        }
    }


    /* 히트 카운트 와 콤보와 관련된 메소드 */

    // 히트 카운트를 증가시키는 메소드
    public void IncHitCount()
    {
        equippedMeleeWeapon.IncHitCount();
    }

    // 돌진에 대한 특수한 처리를 하는 메소드
    public void Assassin_RushAttack()
    {
        equippedMeleeWeapon.Assassin_RushAttack();
    }

    // 돌진 처리 완료 후 초기화 하는 메소드
    public void Assassin_RushAttack_Off()
    {
        equippedMeleeWeapon.Assassin_RushAttack_Off();
    }

    // 현재 장착 무기의 콤보를 불러오는 메소드
    public int getCurrentWeaponCombo()
    {
        int combo;
        combo = equippedMeleeWeapon.getCurrentCombo();

        return combo;
    }


    /* 방패 사용 관련*/
    public void EquipShield()
    {
        // 이미 장착중인 방패가 있으면 파괴.
        if (equippedShield != null)
        {
            Destroy(equippedShield.gameObject);
        }

        equippedShield = Instantiate(startingShield, shiledPos, Quaternion.Euler(shiledRot.x, shiledRot.y, shiledRot.z)) as Shield;

        // 방패 오브젝트가 플레이어를 따라 움직이도록.
        equippedShield.transform.parent = weaponHold_Left;

        // 방패 오브젝트의 위치를 알맞게 로칼 위치, 회전값으로 조정.
        equippedShield.transform.localPosition = shiledPos;
        equippedShield.transform.localRotation = Quaternion.Euler(shiledRot.x, shiledRot.y, shiledRot.z);
    }


    /* 특성 관련 처리 */

    // 넉백 스킬 사용.
    public void KnockBackEnemy()
    {
        if (equippedShield != null)
        {
            equippedShield.IncHitCount();
            // 데미지 설정.
            ShieldSetDamage();
            print("Knock to enemy");
        }
    }

    // 넉백 효과 추가
    public void AddKnockBack()
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.AddKnockBack();
        }
    }

    // 넉백 효과 제거
    public void SubtractKnockBack()
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.SubtractKnockBack();
        }
    }

    // 도발 스킬 사용.
    public void TauntEnemy(float dst)
    {
        if(equippedShield != null)
        {
            equippedShield.Taunt(dst);
            print("Taunt to Enemy");
        }
    }

    // 포효 스킬 사용.
    public void Roar(float dst)
    {
        if (equippedShield != null)
        {
            equippedShield.Roar(dst);
            print("Roar to Enemy");
        }
    }

    // 방패 데미지 적용.
    public void ShieldSetDamage()
    {
        if(equippedMeleeWeapon !=  null)
        {
            if(equippedShield != null)
            {
                equippedShield.setDamage(equippedMeleeWeapon.damage * shieldDamagePercent/100);
            }
        }
    }

    // 생존 주의 효과 적용
    public void AddAbsorptionDamage_Survivalism()
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.AddAbsorptionDamage_Survivalism();
        }
    }

    // 솔선 효과 추가
    public void AddLeadAbility()
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.AddLeadAbility();
        }

    }

    // 데미지를 증가시키는 메소드
    public void AddDamage(float damage)
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.AddDamage(damage);
        }
    }

    // 데미지를 감소시키는 메소드
    public void SubtractDamage(float damage)
    {
        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.SubtractDamage(damage);
        }
    }


    /* 미사용 더미 코드 */

    // MeleeController의 근거리 무기 운동 명령부분
    // 2018.08.18 애니메이션 모션 사용으로 미사용.

    // 근거리 무기를 휘두루도록 명령하는 메소드
    public void Swing()
    {
        if (equippedMeleeWeapon != null)
        {
            //equippedMeleeWeapon.MeleeSwing();
        }
    }

    // 근거리 무기를 내려치도록 명령하는 메소드
    public void Chop()
    {
        if (equippedMeleeWeapon != null)
        {
            //equippedMeleeWeapon.Chop();
        }
    }

    // 근거리 무기로 찌르도록 명령하는 메소드
    public void Pierce()
    {
        if (equippedMeleeWeapon != null)
        {
            //equippedMeleeWeapon.Pierce();
        }
    }

    // 근거리 무기로 회전 공격을 명령하는 메소드
    public void RotAttack()
    {
        if (equippedMeleeWeapon != null)
        {
            //equippedMeleeWeapon.RotAttack();
        }
    }

}
