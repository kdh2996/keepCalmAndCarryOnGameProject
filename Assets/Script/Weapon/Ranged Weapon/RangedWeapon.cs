using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour {

    // 발사 위치
    public Transform muzzle;
    // 발사체
    public Projectile projectile;


    /* 격발 관련 변수 */
    // 연사력 (격발 사이 간격)
    public float msBetweenShot;
    // 발사체 순간 속력 (발사 순간 초기 속력)
    public float muzzleVelocity = 35;

    // 다음 격발 시간.
    float nextShotTime;

    // 처음에 회전 값 저장.
    Quaternion tmprotation;
    // 지상으로 부터 수직으로 세울 회전 값.
    Quaternion groundVerticalRotation = Quaternion.Euler(-2.857f, 29.889f, 174.828f);


    /* 발사체 장전 관련 변수*/

    // 전체 발사체 최대치
    public float totProjectileCount;
    // 현재 발사체 갯수
    float currentProjectileCount;

    // 무한 투사체 쏘기 판별 변수
    public bool limitlessProjectile = false;


    /* 특성 관련 */

    // 석궁인지 확인하는 변수
    public bool isCrossBow = false;

    // 대용량 볼트 탄창
    public float BulkBoltMagazieCount;

    // 약점 관통 적용
    public bool PenetratingWeakness_BoltProjectile_On = false;

    // 생존 주의 적용
    public bool AddAbsorptionDamage_Survivalism_On = false;


    private void Start()
    {
        tmprotation = transform.localRotation;

        // 발사체 갯수 초기화
        currentProjectileCount = totProjectileCount;
    }


    /* 원거리 무기 기본 설정 */

    // 무기의 땅 기준 회전 값 조절.
    public void ChangeGroundedRotation()
    {
        transform.localRotation = tmprotation;
    }

    // 무한 투사체 상태로 변경.
    public void ApplyLimitlessShoot()
    {
        limitlessProjectile = true;
    }

    // 석궁임을 확인
    public void ConfirmCrossBow()
    {
        isCrossBow = true;
    }


    // 발사 메소드.
    public void Shoot()
    {
        if(isCrossBow == true)
        {
            // 석궁일 경우
            // 항상 땅으로부터 수직이도록 조정.
            transform.localRotation = groundVerticalRotation;
        }

        // 현재 시간이 다음 격발 시간보다 클 경우,
        if (Time.time > nextShotTime)
        {

            nextShotTime = Time.time + msBetweenShot / 1000;
            // 발사체 인스턴스화.
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            // 약점 주의 적용
            if(PenetratingWeakness_BoltProjectile_On == true)
            {
                newProjectile.isPenetratingWeaknessOn = true;
            }

            // 생존 주의 적용
            if(AddAbsorptionDamage_Survivalism_On == true)
            {
                newProjectile.AddAbsorptionDamage_Survivalism();
            }


            // 투사체 갯수 하나 소비
            if(limitlessProjectile == true)
            {
                // 무한 투사체 상태면 투사체 갯수를 소비하지 않음. (발사체 갯수)
            }
            else
            {
                ConsumeOneProjcetile();
            }
        }

    }


    /* 투사체의 사거리, 데미지를 변경시키는 메소드들 */

    // 사거리 관련

    // 투사체에 1만큼 사거리를 더함.
    public void SetAddRangedDst()
    {
        if (projectile != null)
        {
            projectile.SetAddRangedDst();
        }
    }

    // 투사체 사거리 자체를 퍼센트 만큼 증가 변경
    public void ChangeRangedDstToPercent_Inc(float percent)
    {
        if (projectile != null)
        {
            projectile.ChangeRangedDstToPercent_Inc(percent);
        }
    }

    // 투사체의 사거리를 초기화.
    public void SetInitRangedDst()
    {
        if (projectile != null)
        {
            projectile.SetInitRangedDst();
        }
    }

    // 투사체의 초기화 사거리 자체를 변경.
    public void ChangeInitRangedDst(float initDst)
    {
        if (projectile != null)
        {
            projectile.ChagneInitRangedDst(initDst);
        }
    }

    // 투사체의 초기화 사거리 자체를 %만큼 감소시켜 변경.
    public void ChangeInitRangedDst_decPercent(float percent)
    {
        if (projectile != null)
        {
            projectile.ChagneInitRangedDst_Percent_Down(percent);
        }
    }

    // 투사체의 초기화 사거리 자체를 %만큼 증가시켜 변경.
    public void ChangeInitRangedDst_incPercent(float percent)
    {
        if (projectile != null)
        {
            projectile.ChagneInitRangedDst_Percent_Up(percent);
        }
    }


    // 데미지 관련

    // 투사체에 퍼센트 만큼 데미지를 곱함.
    public void SetMultiplyPercentDamage()
    {
        if (projectile != null)
        {
            projectile.SetMultiplyPercentDamage();
        }
    }

    // 투사체의 데미지를 퍼센트만큼 증가 변경함.
    public void ChangeDamageToPercent_Inc(float percent)
    {
        if (projectile != null)
        {
            projectile.ChangeDamageToPercent_Inc(percent);
        }
    }

    // 투사체의 데미지를 초기화.
    public void SetInitDamage()
    {
        if (projectile != null)
        {
            projectile.SetInitDamage();
        }
    }

    // 투사체의 초기화 데미지 자체를 변경.
    public void ChangeInitDamge(float initDamage)
    {
        if (projectile != null)
        {
            projectile.ChagneInitDamage(initDamage);
        }
    }

    // 투사체의 초기화 데미지 자체를 %만큼 감소 시켜 변경.
    public void ChangeInitDamge_decPercent(float percent)
    {
        if (projectile != null)
        {
            projectile.ChagneInitDamage_Percent(percent);
        }
    }

    // 투사체의 데미지를 반환하는 메소드
    public float GetAppliedDamage()
    {
        if (projectile != null)
        {
            return projectile.GetAppliedDamage();
        }
        else
        {
            return 0;
        }
    }


    // 특성에 따른 특수 처리

    // 투사체에 약점 관통 효과를 추가.
    public void AddPenetratingWeakness_BoltProjectile()
    {
        if (projectile != null)
        {
            PenetratingWeakness_BoltProjectile_On = true;
        }
    }

    // 투사체에 생존 주의 효과를 추가.
    public void AddAbsorptionDamage_Survivalism()
    {
        if(projectile != null)
        {
            AddAbsorptionDamage_Survivalism_On = true;
        }
    }


    /* 투사체 장전 관련 메소드들 */

    // 투사체 갯수를 하나 소비하는 메소드
    public void ConsumeOneProjcetile()
    {
        if(currentProjectileCount > 0)
        {
            currentProjectileCount--;
            print("== curProjCount : " + currentProjectileCount);
        }
    }

    // 투사체 갯수를 초기 최대치 값만큼 회복하는 메소드
    public void RestoreProjectileCount()
    {
        currentProjectileCount = totProjectileCount;
        print("==RELOAD COMPLETED");
    }

    // 투사체 갯수가 0인지 확인하는 메소드
    public bool isProjectileEnough()
    {
        if (currentProjectileCount <= 0)
        {
            return false;
        }

        return true;
    }


    /* 특성 관련 */

    // 엔지니어 스타일 특성 관련
    // 투사체 갯수를 증가시키는 메소드
    public void inctotProjectileCount()
    {
        totProjectileCount = BulkBoltMagazieCount;
    }

    // 엔지니어 전문가 특성 관련
    // 투사체 갯수를 두배로 증가시키는 메소드
    public void SquareProjectileCount()
    {
        totProjectileCount = totProjectileCount * 2;
    }

}
