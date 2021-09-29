using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeWeapon : MonoBehaviour {

    /* 충돌 관련 처리 변수 */

    // 무기 데미지
    public float damage = 1;
    public float initDamage = 1;

    public float tmpinitDamage = 1;

    public float lastComboDamage = 1;

    // 무기 휘둘러서 때린 횟수 카운트.
    public int HitCount = 0;

    // 데미지 후, 회복
    public bool isAbsorptionDamage = false;
    // 돌진 상태 인지 판별
    public bool isRushOn = false;

    // 넉백 상태 인지 판별
    public bool isKnockBack = false;

    // 피격 대상 관련 정보.
    protected Material SkinMaterial;
    protected Color OriginColor;

    // 적 색깔 저장.
    protected Color EnemyColor;
    // 플레이어 색깔 저장.
    protected Color PlayerColor;

    // 피격 후 겉 색깔 변화
    protected bool skinColorChange = false;
    // 피격 후 겉 색깔 회복
    protected bool skinRecove = false;


    /* 태그 관련 Transform 선언 */

    // 플레이어에 대한 Transform
    Transform PlayerTransform;
    // 적에 대한 Transform.
    Transform EnemyTransform;


    /* 콤보 관련 변수 */

    // 근접 무기 콤보 카운트
    public int meleeAtkComboCount = 0;
    // 근접 무기 콤보 최대 카운트
    public int meleeAtkMaxComboCount;

    // 근접 무기 콤보 지속 시간
    public float meleeAtkComboDurationTime;

    // 근접 무기 콤보 판별 시간
    float comboInitTime = 0;
    // 근접 무기 콤보 카운트 변화 감지 변수
    bool isComboTrans = false;
    // 근접 무기 콤보가 적용 되었는지 판별하는 변수
    bool isAppliedCombo = false;

    // 근접 무기 콤보가 초기화 되었음을 확인하는 변수
    bool isInitializedCombo = false;


    // 각종 스택 관련

    // 솔선 스택 적용 판별 변수
    public bool isLeadStackOn = false;
    // 솔선 스택
    public int leadStackCount = 0;


    /* 미사용 더미 변수들 */

    // 충돌 마스크.
    // public LayerMask collisionMask;

    // 휘두름.
    // bool swingHit = false;

    /*
    // 근접 무기에 대한 제어 변수들

    // 횡으로 휘두르는 경우

    // 휘두르는 속도 (공격 속도)
    public float swingVelocity;

    // 휘두르는 각 (공격 범위 각)
    public float swingAngle;

    // 휘두루는 길이 (공격 길이)
    float swingLength;


    // 종으로 내려치는 경우

    // 휘두르는 속도 (공격 속도)
    public float chopVelocity;

    // 휘두르는 각 (공격 범위 각)
    public float chopAngle;

    // 휘두루는 길이 (공격 길이)
    float chopLength;


    // 찌르기를 하는 경우

    // 찌르는 속도 (공격 속도)
    public float pierceVelocity;

    // 찔러서 이동할 총 거리 (공격 최대 거리)
    public float pierceDistance;

    // 찌르는 길이 (공격 길이)
    float pierceLength;


    // 360도 회전하는 공격을 하는 경우

    // 360도 회전하는 속도 (공격 속도)
    public float rotAttackVelocity;


    // 움직인 이동 거리
    float moveDistance;
    */



    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    void Update()
    {
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


        // 휘두른 히트카운트 초기화.
        if (HitCount != 0)
        {
            StartCoroutine(RecoverHitCount());
        }

        // 피격후 색깔 변경
        if (skinColorChange == true)
        {
            StartCoroutine(ChangeSkin());
        }

        // 색 변경 후 복구
        if(skinRecove == true)
        {
            StartCoroutine(ReCoverSkin());
        }

        // 콤보 계속해서 연계되는지 확인.
        if(isAppliedCombo == true)
        {
            CheckComboTime();
            if(transform.tag == "Player_Melee")
            {
                print("=== COMBO ===" + meleeAtkComboCount);
                //print("!!! Damage !!!" + damage);
            }   
        }

    }


    /* 히트 카운트 메소드들 */

    // HitCount를 증가 시키는 메소드
    public void IncHitCount()
    {
        // 히트 카운트 증가
        HitCount++;

        // 콤보 카운트도 증가
        meleeAtkComboCount++;

        // 솔선 스택 추가
        if(isLeadStackOn == true)
        {
            leadStackCount++;
            //print("== Lead Stack == " + leadStackCount);
        }

        // 최대 콤보에는 데미지 강화
        if(meleeAtkComboCount >= meleeAtkMaxComboCount)
        {
            meleeAtkComboCount = meleeAtkMaxComboCount;
            damage = initDamage + lastComboDamage;
        }
        else
        {
            damage = initDamage;
        }

        isComboTrans = true;
        isAppliedCombo = true;

        //print(isAppliedCombo);
    }


    // 특수하게 돌진에 대한 처리를 하는 메소드
    public void Assassin_RushAttack()
    {
        // 돌진 상태임을 알림.
        isRushOn = true;

        // 히트 카운트 증가
        HitCount++;

        // 데미지 변경
        damage = damage * 1.75f;

        print("rush damage" + damage);
        damage = (int)damage;

    }

    // 돌진 상태가 끝나서 데미지를 원래대로 돌림.
    public void Assassin_RushAttack_Off()
    {
        isRushOn = false;

        damage = initDamage;
    }

    // 데미지를 증가시키는 메소드
    public void AddDamage(float _damage)
    {
        initDamage += _damage;
    }

    // 데미지를 감소시키는 메소드
    public void SubtractDamage(float _damage)
    {
        initDamage -= _damage;
    }

    // 데미지를 증가시키지만, 일정하게 유지할 필요가 있을 경우의 메소드
    public void AddDamage_Lasting(float _damage)
    {
        // tmpinitDamage = initDamage;
        // initDamage = tmpinitDamage + _damage;
    }

    // 허공에 휘두를 경우, HitCount 초기화
    IEnumerator RecoverHitCount()
    {
        // 근접 무기 사용 쿨타임 만큼 대기 후 초기화
        yield return new WaitForSeconds(1);
        HitCount = 0;
    }


    /* 콤보 관련 메소드들 */

    // 현재 콤보를 얻는 메소드
    public int getCurrentCombo()
    {
        return meleeAtkComboCount;
    }

    // 콤보 타임을 체크하는 메소드
    void CheckComboTime()
    {
        if(isComboTrans == true && meleeAtkComboCount >= 0)
        {
            comboInitTime = Time.time;
            comboInitTime += meleeAtkComboDurationTime;
            isComboTrans = false;
        }

        // 설정시간보다 더 많은 시간이 흘렀을 경우, 
        // 콤보를 초기화
        if (isComboTrans == false && comboInitTime < Time.time)
        {
            meleeAtkComboCount = 0;
            damage = initDamage;

            // 콤보 초기화 확인.
            isInitializedCombo = true;
            isAppliedCombo = false;
        }
    }

    // 현재 콤보가 초기화 되었는지 확인하는 메소드
    public bool getInitializedCombo()
    {
        return isInitializedCombo;
    }

    // 현재 콤보가 초기화 되었음을 체크하고, 다시 false로 바꾸는 메소드
    public void setInitializedCombo()
    {
        isInitializedCombo = false;
    }


    /* 특성 관련 처리 */

    // 생존 주의 효과
    public void AddAbsorptionDamage_Survivalism()
    {
        isAbsorptionDamage = true;
    }

    // 넉백 효과
    public void AddKnockBack()
    {
        isKnockBack = true;
    }

    // 넉백 효과 해제
    public void SubtractKnockBack()
    {
        isKnockBack = false;
    }

    // 솔선 효과 추가
    public void AddLeadAbility()
    {
        isLeadStackOn = true;
    }

    // 솔선 스택을 반환
    public int getLeadStack()
    {
        return leadStackCount;
    }


    /* 충돌 후 처리 */

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


    /* 충돌 판정 */

    // Trigger 방식.

    // 매 충돌시 범용적으로 판정하는 메소드
    private void OnTriggerEnter(Collider other)
    {
        //print("=== OnTriggerEnter");
        //print(HitCount);

        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player" || other.tag == "Transporter" || other.tag == "Boss1"))
        {
            return;
        }

        if(meleeAtkComboCount > meleeAtkMaxComboCount)
        {
            meleeAtkComboCount = 0;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        // 피격 대상이 'Enemy'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Enemy" && HitCount == 1 && this.gameObject.tag == "Player_Melee")
        {
            Debug.Log("Enemy Hit!");
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

            // rush 상태의 공격인지
            if(isRushOn == true)
            {
                print("==Melee Rush Attack ==");
            }

            // 넉백 상태의 공격인지
            if(isKnockBack == true)
            {
                Enemy enemy = other.GetComponent<Enemy>();

                isKnockBack = false;
                print("== Melee KnockBack ==");
            }

            OriginColor = EnemyColor;
            
            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Boss1'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Boss1" && HitCount == 1 && this.gameObject.tag == "Player_Melee")
        {
            Debug.Log("Boss Hit!");
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

            // rush 상태의 공격인지
            if (isRushOn == true)
            {
                print("==Melee Rush Attack ==");
            }

            print("== Boss is Damaged == " + damage);
            print("== Boss Healt Point == ");
            other.GetComponent<Boss1>().PrintHealthPoint();

            //OriginColor = EnemyColor;

            HitCount--;

            // 피격으로 색깔 변경.
            //skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        // 참고) 적이 공격.
        if (other.tag == "Player" && HitCount == 1 && this.gameObject.tag == "Enemy_Melee")
        {  
            Debug.Log("Player Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = PlayerColor;

            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 적 근접무기로 Transporter를 공격할 경우
        // 참고) 적이 공격.
        if (other.tag == "Transporter" && HitCount == 1 && this.gameObject.tag == "Enemy_Melee")
        {
            Transporter transporter = other.GetComponent<Transporter>();

            Debug.Log("Transporter Hit!");
            print("Transporter Health : ");

            damageableObject.TakeDamage(damage);
            transporter.PrintHealthPoint();

            // 주기 안에 들수 있는지 판별.
            if (transporter.cycleSlow == true)
            {
                transporter.OnceCycleSlow = true;

                // 속도 감속은 적용하고,
                // 속도 가속은 막는다.
                transporter.isFast = false;
                transporter.isSlow = true;

                transporter.AttackedSlow();
            }

            //OriginColor = PlayerColor;

            HitCount--;

            // 피격으로 색깔 변경.
            // skinColorChange = true;
        }

    }


    // Collision 방식

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        print("===OnCollision");

        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player" || other.tag == "Transporter"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        // 피격 대상이 'Enemy'일 경우
        if (other.tag == "Enemy" && HitCount == 1)
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;

            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        if (other.tag == "Player" && HitCount == 1)
        {
            Debug.Log("Player Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = PlayerColor;

            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 적 근접무기로 Transporter를 공격할 경우
        if (other.tag == "Transporter" && HitCount == 1)
        {
            Transporter transporter = other.GetComponent<Transporter>();

            Debug.Log("Transporter Hit!");
            print("Transporter Health : ");

            damageableObject.TakeDamage(damage);
            transporter.PrintHealth();

            // 주기 안에 들수 있는지 판별.
            if (transporter.cycleSlow == true)
            {
                transporter.OnceCycleSlow = true;

                // 속도 감속은 적용하고,
                // 속도 가속은 막는다.
                transporter.isFast = false;
                transporter.isSlow = true;

                transporter.AttackedSlow();
            }

            //OriginColor = PlayerColor;

            HitCount--;

            // 피격으로 색깔 변경.
            // skinColorChange = true;
        }

    }
    */




    /* 미사용 더미 코드 */

    // 충돌 판정 메소드 (현재 미사용)
    void CheckCollision(float moveDistance)
    {
        // swingHit = true;


        //Vector3 WeaponRowRotate = new Vector3(transform.position.x, -swingAngle * swingVelocity * Time.deltaTime, transform.position.z);
        // Ray ray = new Ray(transform.position, Vector3.right);
        // RaycastHit hit;

        /*
        // 무기가 물체에 충돌하면,
        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
        */
    }

    // 근접 무기 운동 관련 메소드
    // 2018.08.18 부로 애니메이션 모션을 사용하므로 미사용.

    /*

    // 충돌 후 처리 메소드 (현재 미사용)
    void OnHitObject(RaycastHit hit)
    {
        // 충돌한 오브젝트를 불러옴
        Idamageable damageableObject = hit.collider.GetComponent<Idamageable> ();

        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        print(hit.collider.gameObject.name);
    }


    // 휘두루는 길이(반지름)을 조절하는 메소드
    public void SetLength(float _swingLength)
    {
        swingLength = _swingLength;
    }


    // 횡으로 휘둘렀을 때

    // 휘두루는 속도 조절하는 메소드
    public void SetSwingSpeed(float _swingVelocity)
    {
        swingVelocity = _swingVelocity;
    }

    // 휘두루는 각을 조절하는 메소드
    public void SetSwingAngle(float _swingAngle)
    {
        swingAngle = _swingAngle;
    }


    // 종으로 내려쳤을 때

    // 내려치는 속도 조절하는 메소드
    public void SetChopSpeed(float _chopVelocity)
    {
        chopVelocity = _chopVelocity;
    }

    // 내려치는 각을 조절하는 메소드
    public void SetChopAngle(float _chopAngle)
    {
        chopAngle = _chopAngle;
    }


    // 찔렀을 때

    // 찌르는 속도 조절하는 메소드
    public void SetPierceSpeed(float _pierceVelocity)
    {
        pierceVelocity = _pierceVelocity;
    }

    public void SetPierceDst(float _pierceDistance)
    {
        pierceDistance = _pierceDistance;
    }


    // 360도 회전공격을 할 때

    public void SetRotAttackSpeed(float _rotAttackVelocity)
    {
        rotAttackVelocity = _rotAttackVelocity;
    }


    // 무기 휘두름

    // 휘둘렀을 때를 제어하는 메소드
    public void Swing()
    {
        // 반지름, 속도, 각 설정
        SetLength(transform.lossyScale.z);
        SetSwingSpeed(swingVelocity);
        SetSwingAngle(swingAngle);
        moveDistance = swingAngle * swingVelocity * Time.deltaTime;

        // 특정 각 까지 움직일 때까지 휘두름.
        StartCoroutine(SwingUntilAngle(swingVelocity, swingAngle, moveDistance));
    }

    // 특정 각 까지 휘둘러서 움직일때까지 제어하는 메소드
    IEnumerator SwingUntilAngle(float velocity, float angle, float distance)
    {
        SwingHitCount++;

        float nowAngle = 0;

        // 목표 각까지 움직임.
        while (nowAngle < angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 0); ;
            yield return null;
        }

        // 원래 점으로 회귀.
        nowAngle = 0;

        while (nowAngle < angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 1); ;
            yield return null;
        }

    }


    // 무기 내리침

    // 내리쳤을 때를 제어하는 메소드
    public void Chop()
    {
        // 반지름, 속도, 각 설정
        SetLength(transform.lossyScale.z);
        SetChopSpeed(chopVelocity);
        SetChopAngle(chopAngle);
        moveDistance = chopAngle * chopVelocity * Time.deltaTime;

        // 특정 각 까지 움직일 때까지 휘두름.
        StartCoroutine(ChopUntilAngle(chopVelocity, chopAngle, moveDistance));
    }


    // 특정 각 까지 내리쳐서 움직일때까지 제어하는 메소드
    IEnumerator ChopUntilAngle(float velocity, float angle, float distance)
    {
        SwingHitCount++;

        float nowAngle = 0;

        // 목표 각까지 움직임.
        while (nowAngle <= angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 2);
            yield return null;
        }

        // 원래 점으로 회귀.
        nowAngle = 0;

        while (nowAngle <= angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 3);
            yield return null;
        }

        nowAngle = 0;

        while (nowAngle <= angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 3);
            yield return null;
        }

        nowAngle = 0;

        while (nowAngle <= angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 2);
            yield return null;
        }

    }


    // 무기로 찌름.

    public void Pierce()
    {
        // 길이, 속도, 움직일 거리 설정
        SetLength(transform.lossyScale.z);
        SetPierceSpeed(pierceVelocity);
        SetPierceDst(pierceDistance);


        StartCoroutine(PierceUntilDst(pierceVelocity, pierceDistance));

    }

    IEnumerator PierceUntilDst(float velocity, float distance)
    {
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = Vector3.forward * distance * 0.1f;

        SwingHitCount++;

        // 얼마나 갔는지.
        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.deltaTime * velocity;

            //찔렀다가 다시 원점으로 회귀하기 위하여 대칭함수 이용. (보간함수)
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 2;
            // Vector3의 Lerp함수
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;

        }

    }


    // 무기로 360도 회전공격하는 메소드

    public void RotAttack()
    {
        // 길이, 속도, 움직일 거리 설정
        SetLength(transform.lossyScale.z);
        SetRotAttackSpeed(rotAttackVelocity);
        moveDistance = 360 * rotAttackVelocity * Time.deltaTime;

        StartCoroutine(RotAttackUntilAngle(360, rotAttackVelocity, moveDistance));

    }

    // 360도 까지 회전공격을 제어하는 메소드
    IEnumerator RotAttackUntilAngle(float angle, float velocity, float distance)
    {
        SwingHitCount++;

        float nowAngle = 0;

        Vector3 axisVect = new Vector3(PlayerTransform.position.x, 1, PlayerTransform.position.z);

        // 목표 각까지 움직임.
        while (nowAngle <= angle)
        {
            nowAngle += MoveStick(velocity, 1, distance, 4);
            yield return null;
        }

        Player.isRot = false;
    }


    // Stick 움직이는 에니메이트 총괄 메소드.
    // Pierce()는 예외. (자체메소드의 Coroutine 에서 처리)

    // 무기를 움직이는 메소드
    float MoveStick(float swingVelocity, float swingAngle, float moveDistance, int SwingFlag)
    {

        // 각과 관련된.
        // float AngleVelocity = swingVelocity / swingLength;

        // 횡으로 휘두르기
        // SwingFlag가 0 => 왼쪽   휘두름
        // SwingFlag가 1 => 오른쪽 휘두름
        // 종으로 휘두르기
        // SwingFlag가 2 => 위로 휘두름
        // SwingFlag가 3 => 아래로 휘두름
        // 360도 돌면서 휘두르기
        // SwingFlag가 4

        if (SwingFlag == 0)
        {
            transform.Rotate(-Vector3.up * 1 * swingVelocity);
        }
        else if (SwingFlag == 1)
        {
            transform.Rotate(Vector3.up * 1 * swingVelocity);
        }
        else if (SwingFlag == 2)
        {
            transform.Rotate(-Vector3.right * 1 * swingVelocity);
        }
        else if (SwingFlag == 3)
        {
            transform.Rotate(Vector3.right * 1 * swingVelocity);
        }
        else if (SwingFlag == 4)
        {

            //print(axisVect);

            PlayerTransform.rotation *= Quaternion.AngleAxis(1 * swingVelocity, Vector3.up);
            print(PlayerTransform.up);

            //PlayerTransform.Rotate(0, 360 * Time.deltaTime, 0, Space.Self);
            //print(PlayerTransform.position);
        }

        return 1 * swingVelocity;

        // 횡으로 휘두르기


        // 손 회전
        //WeaponHoldTransform.RotateAround(WeaponHoldTransform.position, Vector3.up , -60f);
        //WeaponHold.Rotate(HandPosRotate); 
    }
    */

}
