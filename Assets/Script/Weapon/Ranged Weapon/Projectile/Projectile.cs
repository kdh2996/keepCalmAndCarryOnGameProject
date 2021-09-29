using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour {

    Rigidbody projectileRigidbody;


    /* 운동에 대한 변수 */

    // 발사체 속도
    public float speed = 1f;

    // 발사체 첫 생성 위치
    Vector3 firstPos;
    // 발사체 현재 위치
    Vector3 currentPos;

    // 발사체 현재 이동 거리
    float sqrDstToStartPos;

    // 발사체 초기화 사거리.
    public float initRangedDst;
    // 발사체 이동 사거리
    public float rangedDst;


    /* 데미지에 대한 변수 */

    // 무기 초기화 데미지.
    public float initDamage = 1;
    // 무기 데미지
    public float damage = 1;

    // 데미지 후, 회복
    public bool isAbsorptionDamage = false;
    // 적용된 데미지
    public float appliedDamage = 0;


    /* 피격에 대한 변수 */

    // 플레이어에 대한 Transform
    Transform PlayerTransform;
    // 적에 대한 Transform.
    Transform EnemyTransform;

    // 피격 대상 관련 정보.
    Material SkinMaterial;
    Color OriginColor;

    // 피격 후 겉 색깔 변화
    bool skinColorChange = false;
    // 피격 후 겉 색깔 회복
    bool skinRecove = false;

    // 적 색깔 저장.
    Color EnemyColor;
    // 플레이어 색깔 저장.
    Color PlayerColor;


    /* 특성에 따른 특수한 변수 */

    // 약점 관통 스킬 적용을 위한 특수 처리
    public bool isPenetratingWeaknessOn = false;

    private void Start()
    {
        projectileRigidbody = GetComponent<Rigidbody>();

        // 처음 생성 위치 저장.
        firstPos = transform.position;

        // 데미지 초기화
        initDamage = damage;
        damage = initDamage;
    }

    private void FixedUpdate()
    {   
        // rigidbody로, 탄환 발사 처리 (깔끔한 피격처리를 위함)
        projectileRigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
    }

    void Update ()
    {
        // 현재 위치에 대한 저장.
        currentPos = transform.position;

        // 사거리 만큼 갔는지 확인하는 메소드 호출
        if (isReachedAllDst(currentPos) == true)
        {
            // 사거리 만큼 도달했으므로, 투사체 파괴.
            Destroy(this.gameObject);
        }


        // 수정 필요. (매번 적 태그로 검사)
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            EnemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
            EnemyColor = EnemyTransform.GetComponent<Renderer>().material.color;
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (PlayerTransform != null)
        {
            PlayerColor = PlayerTransform.GetComponent<Renderer>().material.color;
        }

        // 피격후 색깔 변경
        if (skinColorChange == true)
        {
            StartCoroutine(ChangeSkin());
        }

        // 색 변경 후 복구
        if (skinRecove == true)
        {
            StartCoroutine(ReCoverSkin());
        }

        print("bullet Damage :: " + damage);
        print("bullet Ranged Dst :: " + rangedDst);

        // 투사체 직진 발사. (FiexedUpdate에서 Rigidbody로 물리 거리로 계산.)
        //transform.Translate(Vector3.forward * Time.deltaTime * speed);

    }


    /* 피격 후 처리 */

    // 피격 후 색깔 변경
    IEnumerator ChangeSkin()
    {
        yield return null;
        // 빨갠색으로 변경.
        SkinMaterial.color = Color.red;
        skinRecove = true;
    }

    // 피격 후 색깔 복구
    IEnumerator ReCoverSkin()
    {
        yield return new WaitForSeconds(0.5f);
        SkinMaterial.color = OriginColor;

        skinColorChange = false;
        skinRecove = false;
    }


    /* 운동 처리 */

    // 새로운 속도 지정 메소드
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }


    /* 사거리 처리 */

    // 이동 거리 계산 후, 사거리까지 닿았는지 판단하는 메소드
    public bool isReachedAllDst(Vector3 pos)
    {
        sqrDstToStartPos = (firstPos - pos).sqrMagnitude;

        // 이동 거리가 사거리 보다 큰지 작은지 판단.
        if (sqrDstToStartPos < Mathf.Pow(rangedDst, 2))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 사거리를 지정해주는 메소드
    public void SetRangedDst(float dst)
    {
        rangedDst = dst;
    }

    // 사거리를 퍼센트 만큼 증가 변경하는 메소드
    public void ChangeRangedDstToPercent_Inc(float percent)
    {
        rangedDst = rangedDst * (1 + percent / 100);

        rangedDst = (int)rangedDst;
    }

    // 1만큼 사거리가 증가하는 메소드
    public void SetAddRangedDst()
    {
        float setDst = rangedDst + 1;
        rangedDst = (int)setDst;

        // 40 초과시 다시 제어.
        if (rangedDst >= initRangedDst + 20)
        {
            rangedDst = initRangedDst + 20;
        }
    }

    // 사거리를 초기화 시키는 메소드
    public void SetInitRangedDst()
    {
        rangedDst = initRangedDst;
    }

    // 초기화 사거리 자체를 변경 시키는 메소드
    public void ChagneInitRangedDst(float initDst)
    {
        initRangedDst = initDst;
    }

    // 초기화 사거리 자체를 %만큼 감소 시키는 메소드
    public void ChagneInitRangedDst_Percent_Down(float percent)
    {
        initRangedDst = initRangedDst * (1 - percent / 100);

        initRangedDst = (int)initRangedDst;
    }

    // 초기화 사거리 자체를 %만큼 증가 시키는 메소드
    public void ChagneInitRangedDst_Percent_Up(float percent)
    {
        initRangedDst = initRangedDst * (1 + percent / 100);

        initRangedDst = (int)initRangedDst;
    }


    /* 데미지 처리 */

    // 1%씩 퍼센트 데미지 만큼 데미지가 증가하는 메소드
    public void SetMultiplyPercentDamage()
    {
        float setDamage = 1.01f * damage;
        damage = (int)setDamage;

        // 150% 초과시 다시 제어.
        if (damage >= 1.5f * initDamage)
        {
            damage = 1.5f * initDamage;
        }
    }

    // 데미지를 퍼센트 만큼 증가 변경하는 메소드
    public void ChangeDamageToPercent_Inc(float percent)
    {
        damage = damage * (1 + percent / 100);

        damage = (int)damage;
    }

    // 데미지를 초기화 시키는 메소드
    public void SetInitDamage()
    {
        damage = initDamage;
    }

    // 초기화 데미지 자체를 변경 시키는 메소드
    public void ChagneInitDamage(float damage)
    {
        initDamage = damage;
    }

    // 초기화 데미지 자체를 %만큼 변경 시키는 메소드
    public void ChagneInitDamage_Percent(float percent)
    {
        initDamage = initDamage * (1 - percent / 100);

        initDamage = (int)initDamage;
    }

    // 피해를 반환하는 메소드
    public float GetAppliedDamage()
    {
        return appliedDamage;
    }


    /* 특성에 따른 특수 처리 */

    // 약점 관통 효과
    public void AddPenetratingWeakness()
    {
        // 값을 true로 변경
        isPenetratingWeaknessOn = true;
        print(isPenetratingWeaknessOn);
    }

    // 생존 주의 효과
    public void AddAbsorptionDamage_Survivalism()
    {
        isAbsorptionDamage = true;
    }


    /* 피격 처리 */

    /* 투사체에 직접적으로 피격 판정 1) OnCollsion 사용 */
    private void OnCollisionEnter(Collision collision)
    {
        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(collision.collider.tag == "Enemy" || collision.collider.tag == "Player" || collision.collider.tag == "Transporter" || collision.collider.tag == "Boss1" || collision.collider.tag == "Boss1_HallWall"))
        {
            return;
        }

        Idamageable damageableObject = collision.collider.GetComponent<Idamageable>();
        LivingEntity damageableLivingEntity = collision.collider.GetComponent<LivingEntity>();
        SkinMaterial = collision.collider.GetComponent<Renderer>().material;

        // 피격 대상이 'Enemy'일 경우
        if (collision.collider.tag == "Enemy" && this.gameObject.tag == "PlayerArrow")
        {
            Debug.Log("(ProjectileAttack) Enemy Hit!");
            OriginColor = EnemyColor;

            // 약점관통 스킬 사용시, 특수하게 방어력 감소.
            // 볼트에만 적용되도록 해야함 (현재 볼트일 경우 라는 특수처리 누락)
            if(isPenetratingWeaknessOn == true)
            {
                Debug.Log("(PenetratingWeakness Applied)");
                damageableLivingEntity.RateWeakDefensePercent_atOnce = 30f;
            }

            damageableObject.TakeDamage(damage);

            // 만약 피해 후 체력 회복 시스템이 적용 되면, 특수하게 처리
            if(isAbsorptionDamage == true)
            {
                if(GameObject.FindGameObjectWithTag("Player") != null)
                {
                    print("== Absorption Damage == ");
                    if(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Survivalism_On == true)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Process_Survivalism_AbsorptionDamage(damage);
                    }
                }
                else
                {
                    print("== Absorption Damage Exception ==");
                    print("== (Can't Find Player Object) ==");
                }
                
                //appliedDamage = damage;
            }

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Boss1'일 경우
        if (collision.collider.tag == "Boss1" && this.gameObject.tag == "PlayerArrow")
        {
            Debug.Log("(ProjectileAttack) Boss1 Hit!");

            // 약점관통 스킬 사용시, 특수하게 방어력 감소.
            if (isPenetratingWeaknessOn == true)
            {
                Debug.Log("(PenetratingWeakness Applied)");
                damageableLivingEntity.RateWeakDefensePercent_atOnce = 30f;
            }

            damageableObject.TakeDamage(damage);

            // 만약 피해 후 체력 회복 시스템이 적용 되면, 특수하게 처리
            if (isAbsorptionDamage == true)
            {
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    print("== Absorption Damage == ");
                    if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Survivalism_On == true)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Process_Survivalism_AbsorptionDamage(damage);
                    }
                }
                else
                {
                    print("== Absorption Damage Exception ==");
                    print("== (Can't Find Player Object) ==");
                }

                //appliedDamage = damage;
            }

            //OriginColor = EnemyColor;


            // 피격으로 색깔 변경.
            //skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        if (collision.collider.tag == "Player" && this.gameObject.tag == "EnemyArrow")
        {
            Debug.Log("(ProjectTileAttack) Player Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = PlayerColor;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Transporter'일 경우
        if (collision.collider.tag == "Transporter" && this.gameObject.tag == "EnemyArrow")
        {
            Debug.Log("(ProjectTileAttack) Transporter Hit!");
            damageableObject.TakeDamage(damage);
        }

    }


    /* 투사체에 직접적으로 피격 판정 2) OnTrigger 사용
    private void OnTriggerEnter(Collider other)
    {
        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        // 피격 대상이 'Enemy'일 경우
        if (other.tag == "Enemy")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;


            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        if (other.tag == "Player")
        {
            Debug.Log("Player Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = PlayerColor;
            
            // 피격으로 색깔 변경.
            skinColorChange = true;
        }
    }
    */

}
