using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : MonoBehaviour
{

    /* 잡는 부분 */
    // 총을 잡는 부분.
    public Transform WeaponHold;
    // 총을 잡는 부분.(오른쪽)
    public Transform WeaponHold_Right;

    /* 원거리 무기 종류 */
    // 초기 시작 총.
    public RangedWeapon startingRangedWeapon;
    // 무기 변환해서 쓸 총.
    public RangedWeapon transRangedWeapon;
    // 소드 오프 건.
    public RangedWeapon SwordOffRangedWeapon;
    // 룬 캐스팅을 위한 특수 발사체
    public RangedWeapon RuneCastingRangedWeapon;

    /* 투사체 종류 */
    // 날리는 투사체 종류.
    public Transform Projectile;

    /* 현재 상태 */
    // 장착중인 총
    public RangedWeapon equippedRangedWeapon;
    // 장착중인 소드오프 건(오른쪽)
    public RangedWeapon equippedRangedWeapon_right;
    // 투사체 초기화 갯수
    public float totProjectileCount;

    // 근거리 무기 위치 벡터
    Vector3 RangedPos;
    Vector3 RangedRot;

    // 원거리 사용 여부 변수
    public bool UseRanged = false;



    void Start()
    {
        // 원거리 무기 소환 위치 설정
        RangedPos = new Vector3(-0.0002437282f, -0.0005343346f, -0.0004673656f);
        RangedRot = new Vector3(-3.283f, 2.276f, -90.436f);

        if (UseRanged == true && startingRangedWeapon != null)
        {
            // 초기 시작 총 장착.
            EquipGun(startingRangedWeapon);
        }

    }


    /* 무기 장착 메소드 */

    // 총 장착을 하는 메소드
    public void EquipGun(RangedWeapon gunToEquip)
    {
        if (equippedRangedWeapon != null)
        {
            Destroy(equippedRangedWeapon.gameObject);
        }

        // WeaponHold에 총 장착.
        equippedRangedWeapon = Instantiate(gunToEquip, WeaponHold.position, WeaponHold.rotation) as RangedWeapon;
        // WeaponHold에 귀속.
        equippedRangedWeapon.transform.parent = WeaponHold;

        // 근거리 무기 오브젝트의 위치를 알맞게 로칼 위치, 회전값으로 조정.
        //equippedRangedWeapon.transform.localPosition = RangedPos;

        equippedRangedWeapon.transform.localRotation = Quaternion.Euler(0, 0, -180);
    }

    // 오른쪽에 총을 장착하는 메소드
    public void EquipGun_right(RangedWeapon gunToEquip)
    {
        // WeaponHold_Right에 총 장착.
        equippedRangedWeapon_right = Instantiate(gunToEquip, WeaponHold_Right.position, WeaponHold_Right.rotation) as RangedWeapon;
        // WeaponHold_Right에 귀속.
        equippedRangedWeapon_right.transform.parent = WeaponHold_Right;

        // 근거리 무기 오브젝트의 위치를 알맞게 로칼 위치, 회전값으로 조정.
        //equippedRangedWeapon.transform.localPosition = RangedPos;

        equippedRangedWeapon_right.transform.localRotation = Quaternion.Euler(0, 0, -180);
    }

    // 소드오프건 장착 메소드
    public void EquiSwordOffGun()
    {
        // 왼쪽 장착.
        EquipGun(SwordOffRangedWeapon);
        // 오른쪽 장착.
        EquipGun_right(SwordOffRangedWeapon);

        // 양 무기 능력치 동기화.
        equippedRangedWeapon_right.BulkBoltMagazieCount = equippedRangedWeapon.BulkBoltMagazieCount;

        equippedRangedWeapon.transform.localRotation = Quaternion.Euler(0, 180, -180);
    }

    // 룬 캐스팅용 빈 오브젝트 장착 메소드
    public void EquipRuneCasting()
    {
        EquipGun(RuneCastingRangedWeapon);
    }

    // 전환된 총 장착 메소드
    public void EquiTransGun()
    {
        EquipGun(transRangedWeapon);
    }

    // 총을 파괴하는 메소드
    public void DestroyGun()
    {
        if (equippedRangedWeapon != null)
        {
            Destroy(equippedRangedWeapon.gameObject);
        }
    }

 
    /* 발사 관련 메소드 */

    // 투사체를 발사하는 메소드
    public void Shoot()
    {
        if (equippedRangedWeapon != null)
        {
            // 투사체 갯수가 넉넉한지 확인.
            if (isProjectileEnough() == true)
            {
                equippedRangedWeapon.Shoot();
            }
        }

        if(equippedRangedWeapon_right != null)
        {
            // 투사체 갯수가 넉넉한지 확인.
            if (isProjectileEnoughRight() == true)
            {
                equippedRangedWeapon_right.Shoot();
            }
        }
    }

    // 장전 갯수와 상관없이 투사체를 발사하는 메소드
    public void LimitlessShoot()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.Shoot();
        }
    }

    // 발사 준비 완료 / 실패 원인 반환 메소드
    public bool ShootReadyCompletedOrNot()
    {
        if (equippedRangedWeapon == null)
        {
            print("== EquipedRandedWeapon ERROR : THERE IS NO RANGED WEAPON");
            return false;
        }

        if (isProjectileEnough() == false)
        {
            print("== 0 Projectile : NO ENOUGH PROJECTILE ");
            return false;
        }

        return true;
    }


    /* 발사체의 사거리, 데미지 관련 메소드들 */


    // 발사체 사거리 관련 제어

    // 발사체의 사거리를 1 단위로 더해주는 메소드
    public void SetAddRangedDst()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.SetAddRangedDst();
        }

    }

    // 발사체의 사거리를 초기화 하는 메소드
    public void SetInitRangedDst()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.SetInitRangedDst();
        }
    }

    // 발사체의 사거리를 퍼센트 만큼 증가 변경 시키는 메소드
    public void ChangeRangedDstToPercent_Inc(float percent)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeRangedDstToPercent_Inc(percent);
        }
    }

    // 발사체의 초기화 사거리 자체를 재설정하는 메소드
    public void ChangeInitRangedDst(float initDst)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeInitRangedDst(initDst);
        }
    }

    // 발사체의 초기화 사거리를 % 만큼 감소시켜 변경하는 메소드
    public void ChangeInitRangedDst_decPercent(float percent)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeInitRangedDst_decPercent(percent);
        }

    }

    // 발사체의 초기화 사거리를 % 만큼 증가시켜 변경하는 메소드
    public void ChangeInitRangedDst_incPercent(float percent)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeInitRangedDst_incPercent(percent);
        }

    }


    // 발사체 데미지 관련 제어

    // 발사체의 데미지를 퍼센트 단위로 곱해주는 메소드
    public void SetMultiplyPercentDamage()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.SetMultiplyPercentDamage();
        }
    }

    // 발사체의 데미지를 퍼센트 단위로 증가 변경하는 메소드
    public void ChangeDamageToPercent_Inc(float percent)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeDamageToPercent_Inc(percent);
        }
    }

    // 발사체의 데미지를 초기화 하는 메소드
    public void SetInitDamage()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.SetInitDamage();
        }
    }

    // 발사체의 데미지 자체를 초기화 하는 메소드
    public void ChageinitDamage(float initDamage)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeInitDamge(initDamage);
        }
    }

    // 발사체의 초기화 데미지를 % 만큼 감소시켜 변경하는 메소드
    public void ChangeInitDamage_decPercent(float percent)
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.ChangeInitDamge_decPercent(percent);
        }
    }

    // 발사체의 적용된 데미지를 반환하는 메소드


    // 특성 관련 특수 처리

    // 발사체의 약점 관통 능력을 설정하는 메소드
    public void AddPenetratingWeakness_BoltProjectile()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.AddPenetratingWeakness_BoltProjectile();
        }
    }

    // 발사체의 생존 주의 능력을 설정하는 메소드
    public void AddAbsorptionDamage_Survivalism()
    {
        if(equippedRangedWeapon != null)
        {
            equippedRangedWeapon.AddAbsorptionDamage_Survivalism();
        }
    }

    /* 장전 관련 메소드들 */

    // 발사체 갯수가 0인지 확인하는 메소드
    public bool isProjectileEnough()
    {
        if (equippedRangedWeapon != null)
        {
            return equippedRangedWeapon.isProjectileEnough();
        }

        return false;
    }

    // 오른쪽 무기의 발사체 갯수가 0인지 확인하는 메소드
    public bool isProjectileEnoughRight()
    {
        if (equippedRangedWeapon_right != null)
        {
            return equippedRangedWeapon_right.isProjectileEnough();
        }

        return false;
    }

    // 발사체 갯수를 최대치까지 회복하는(장전하는) 메소드
    public void RestoreProjectileCount()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.RestoreProjectileCount();
        }
        if (equippedRangedWeapon_right !=  null)
        {
            equippedRangedWeapon_right.RestoreProjectileCount();
        }
    }

    // 장착된 무기에 발사체 갯수를 초기화 하는 메소드
    public void initProjectileCount()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.totProjectileCount = totProjectileCount;
        }
    }


    /* 특성 관련 메소드들 */

    // 엔지니어 스타일 특성 관련
    // 발사체 갯수를 증가시키는 메소드
    public void InctotProjectileCount()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.inctotProjectileCount();
        }
    }

    // 엔지니어 전문가 특성 관련
    // 발사체 갯수를 2배로 증가시키는 메소드
    public void SquareProjectileCount()
    {
        if (equippedRangedWeapon != null)
        {
            equippedRangedWeapon.SquareProjectileCount();
        }
        if (equippedRangedWeapon_right != null)
        {
            equippedRangedWeapon_right.SquareProjectileCount();
        }
    }

}
