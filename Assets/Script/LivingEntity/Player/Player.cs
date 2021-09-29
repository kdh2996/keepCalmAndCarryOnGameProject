using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

// 2018.08.18

[RequireComponent(typeof(PlayerController))]
// PlyaerController 가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(MeleeController))]
// MeleeController 가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(RangedWeaponController))]
// GunController가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(RangedWeapon))]
// Projectile이 객체에 무조건 따라 붙게 된다
[RequireComponent(typeof(BehaviourResource))]
// BehaviourResource 가 객체에 무조건 따라 붙게 된다.

[RequireComponent(typeof(Animator))]
// Animator가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(Animation))]
// Animator가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(AnimatorClipInfo))]
// Animator가 객체에 무조건 따라 붙게 된다.

// UnityStandardAsset 에셋 컴포넌트
[RequireComponent(typeof(ThirdPersonCharacter))]


public class Player : LivingEntity {

    /* 상태 관련 변수 */

    // 움직이지 않는 시간을 판별하는 시간 변수
    float nextNotMovingtime = 0;
    // 움직이지 않았는지 판별하는 변수
    bool isMoving = false;
    // 심호흡 스킬의 움직이지 않고 있을 안전시각
    public float deepbreath_notMovingSafeTime = 2000;

    // 데미지 받지 않았는지 판별하는 변수
    bool isAttacked = false;
    // 위치선점 데미지 받지 않고 있을 시각
    public float notAttackedTime_PositionPreemption = 0;
    // 데미지 받았는지 판별하기 위하여 존재하는 임시 체력 저장 변수
    public float tmpHealth;

    // 위치선점 준비 완료 판별 변수
    bool PositionPreemption_Ready = false;
    // 위치선점 스킬 상태인지 판별 변수
    bool isStateOnPositionPreemption = false;
    // 위치선점 스킬 쿨타임 판별 변수
    bool isPositionPreemptionCool = true;

    // 돌진 스킬 쿨타임 판별 변수
    bool isRushCool = true;

    // 방패 선봉대 쿨타임 판별 변수
    bool isDefenseVanguardCool = true;

    // 넉백 쿨타임 판별 변수
    bool isKnockBackCool = true;

    // 깃발 쿨타임 판별 변수
    bool isFlagCool = true;

    // 주문 불꽃 판별 변수
    bool isSpellBlazeCool = true;


    // 현재 스피드 배수
    public float nowMultiplySpeed = 1f;

    // 현재 구르기 준비 완료 확인 변수
    public bool isRollReady = true;
    // 구르기 캐스팅 시간
    public float PlayerRollCasting = 1000f;
    // 구르기 쿨 타임
    public float PlayerRollCool = 5000f;
    // 현재 구르기 거리
    // public float RollDst = 7f;


    /* 데미지 관련 변수 */

    // 플레이어가 지니고 있는 추가 데미지
    public float additionalDamage = 0;
    // 생존주의(피해 후 체력회복) 체크 변수
    public bool Survivalism_On = false;


    Camera viewCamera;
    PlayerController controller;

    [HideInInspector]
    public MeleeController meleeContoller;

    RangedWeaponController rangedController;
    BehaviourResource behaviourResource;
    SummonedController summonedController;

    Animator animator;
    Animation myanimation;
    AnimatorClipInfo[] animatorClip;

    ThirdPersonUserControl userControl;
    ThirdPersonCharacter player_ThridPersoncharacter;

    // 필드 공격 관련 오브젝트
    public FieldColliderBox fieldColliderBox;

    /* 모바일 조작 제어 변수들 */

    // 모바일 용 임을 판별
    public bool isMobileInput = false;


    // 모바일 용 특수 x 이동 값.
    [HideInInspector]
    public float mobile_h;
    // 모바일 용 특수 y 이동 값.
    [HideInInspector]
    public float mobile_v;

    [HideInInspector]
    public bool isPressedMeleeAtkButton = false;
    [HideInInspector]
    public bool isPressedReloadButton = false;
    [HideInInspector]
    public bool isPressedRangedAtkButton = false;
    [HideInInspector]
    public bool isPressedRollButton = false;
    [HideInInspector]
    public bool isPressedSpellButton = false;
    [HideInInspector]
    public bool isPressedShieldButton = false;
    [HideInInspector]
    public bool isPressedTornadoBlazeButton = false;

    [HideInInspector]
    public bool isPressedKnockBackButton = false;
    [HideInInspector]
    public bool isPressedTauntButton = false;
    [HideInInspector]
    public bool isPressedRoarButton = false;
    [HideInInspector]
    public bool isPressedHeavySmashButton = false;

    [HideInInspector]
    public bool isPressedRushButton = false;

    /* 플레이어 상태 */

    // 보는 속도 
    public float viewSpeed = 5;
    // 자동 시점 에임 (적 향해서)
    public bool lookAimToEnemy;


    /* 적 타겟 상테 */

    // 적 타겟 지정.
    Transform targetEnemy;
    // 적 타겟까지의 거리.
    float dstToEenmy;
    // 시점 변경 임계거리.
    float allowLookAimDst = 6;


    /* 힐 관련 변수 */

    // 힐 관련 스킬이 없는 관계로 현재 비활성화
    /*
    // 스킬 힐 수치.
    public float SkillhealValue;
    // 스킬 힐 속도.
    public float SkillHealSpeed;
    // 스킬 힐 시전 시간.
    public float SkillHealTime;
    */


    /* 근접 무기 관련 변수 */

    // 회전 판별 변수
    // 현재 사용하지 않음.
    // public static bool isRot = false;

    // 근접 무기 사용시 이동 불가 변수
    bool isMeleeAttack = false;
    // 근접 무기 사용 중인지 판별하는 변수
    public bool isSwing;

    // 플레이어 한 손 근접 무기 휘두름 사용 시전 타임
    public float PlayerOneMeleeSwingCasting;
    // 플레이어 한 손 근접 무기 휘두름 쿨타임
    public float PlayerOneMeleeSwingCool;

    // 근접무기가 콤보 유지시간을 지나고 초기화 됐는지 확인
    bool checkMeleeIntializedCombo;


    /* 원거리 무기 관련 변수 */

    // 원거리 무기 사용 중인지 판별하는 변수
    public bool isShot;

    // 쏘기 모션까지 시간
    float shotMotionTime = 2000;

    // 볼트 스킬의 시야
    public float boltSkillEyeSight = 10f;
    // 볼트 스킬에 추가적으로 심호흡에 적용되는지
    bool DeepBreath_On = false;
    // 볼트 스킬에 추가적으로 약점 관통이 적용되는지
    bool PenetratingWeakness_On = false;

    // 볼트 스킬의 추가데미지 적용 (심호흡)
    public bool isdeepBreath_AdditionalDamage = false;
    // 볼트 스킬의 추가사거리 적용 (천리안)
    public bool isPerfectSight_AdditionalDst = false;

    // 플레이어 석궁 무기 사용 시전 타임
    public float PlayerCrossBowShotCasting;
    // 플레이어 석궁 무기 휘두름 쿨타임
    public float PlayerCrossBowCool;

    // 키가 눌러졌는지 확인하는 변수
    bool keydownForShot = false;
    // 한 번 쏘고나서 원래 회전값으로 복귀하는 변수
    bool shotRotationSet = false;

    // 조준 시간
    float aimTime;

    // 장전 시 슬로우 비율.
    float reloadSlowRate = 30;

    float anitime;


    /* 특성 관련 변수 */

    // Archer 인가
    [HideInInspector]
    public bool isArcher = false;
    // Warrior 인가
    [HideInInspector]
    public bool isWarrior = false;
    // Mage 인가
    [HideInInspector]
    public bool isMage = false;


    // 캐릭터 변환 관련 특성 변수

    // 위치 선점 On 체크
    bool PositionPreemption_On;

    // 위압 On 체크
    [HideInInspector]
    public bool coercion_On = false;

    // 위압 스택에 변화가 있었는지 체크
    [HideInInspector]
    public bool isCoercionStackTrans = false;

    // 위압 스택 총 스택 유지 시간
    float coercion_Time = 0;

    // 분노 On 체크
    [HideInInspector]
    public bool rage_On = false;

    // 방패 선봉대 On 체크
    [HideInInspector]
    public bool DefenseVanguard_On = false;
    // 방패 선봉대 사이클
    bool isDefenseVanguard_Cycle = false;

    // 방패 강화 On 체크
    [HideInInspector]
    public bool DefenseReinforcement_On = false;

    // 솔선 On 체크
    [HideInInspector]
    public bool Lead_On = false;

    // Archer 특성
    public PlayerArcherController playerArcherController = new PlayerArcherController();

    // Warrior 특성
    public PlayerWarriorController playerWarriorController = new PlayerWarriorController();


    // Mage 특성
    public PlayerMageController playerMageController = new PlayerMageController();



    protected override void Start()
    {
        // 상위 클래스 LivingEntity Start() 호출.
        base.Start();


        /* Component 지정. */
        controller = GetComponent<PlayerController>();
        behaviourResource = GetComponent<BehaviourResource>();

        meleeContoller = GetComponent<MeleeController>();
        rangedController = GetComponent<RangedWeaponController>();
        summonedController = GetComponent<SummonedController>();

        animator = GetComponent<Animator>();
        myanimation = GetComponent<Animation>();

        player_ThridPersoncharacter = GetComponent<ThirdPersonCharacter>();
        userControl = GetComponent<ThirdPersonUserControl>();

        animatorClip = this.animator.GetCurrentAnimatorClipInfo(0);

        // 애니메이터 클립 시간 지정
        // anitime = animatorClip[0].clip.name;
        // print(animatorClip[0].clip.name);

        // 메인 카메라 할당
        viewCamera = Camera.main;


        /* 무기 설정 */
        // 항상 근거리 무기 사용 설정.
        meleeContoller.UseMelee = true;
        // 항상 원거리 무기 사용 설정.
        rangedController.UseRanged = true;

        // 초기 원거리 무기는 석궁으로.
        // rangedController.equippedRangedWeapon.ConfirmCrossBow();

        isSwing = true;
        isShot = true;

        // 속도에 대한 기본 설정.
        // isSpeedUp = true;
        // isSpeedDown = false;


        /* 특성 페이지 반영 설정 */

        InGameTalentsDB.InGameTalents_SingleTon();

        Invoke("SetTalentsScenseDB", 2.0f);

        //SetTalentsScenseDB();
    }

    // 외부에서 메소드 호출을 위해 새로 만듦.
    public void CallSetTalentsScenseDB()
    {
        InGameTalentsDB.InGameTalents_SingleTon();

        Invoke("SetTalentsScenseDB", 2.0f);
    }

    // 외부에서 아처로 변환함에 따라 호출되는 메소드
    public void CallArcherTranfer_Update()
    {
        // 위치 선점 스킬이 On 되었는지 확인
        if (PositionPreemption_On == true)
        {
            // 위치 선점에 대한 처리
            Process_SkillPositionPreemption();
        }

    }

    // 외부에서 스와핑 이후, 솔선 스택에 따른 처리를 위해 호출되는 메소드
    public void CallLeadStackTranfer_Update()
    {
        // 솔선 스킬이 On 되었는지 확인
        if (Lead_On == true)
        {
            // 솔선 스택에 대한 처리
            Process_SkillLead();
        }

    }


    void Update()
    {

        /* 카메라 설정 */
        Vector3 cameraMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        viewCamera.transform.position = new Vector3(transform.position.x - 8, transform.position.y + 8, transform.position.z - 8);


        /* 움직이는 입력 관련 처리 */
        // 현재 사용하지 않아서 비활성 화

        // Stadandard Asset 사용으로 미사용.
        /*
        // 사용자 입력
        if (!isMeleeAttack && !isRot)
        {
            // 속도 관련 적용사항 처리.
            ProcessVelocity();

            // 현재 GetAxisRaw를 사용하여 스무딩 적용 안함.
            Vector3 moveinput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            // 입력 값의 방향 단위 벡터에다 속도 곱하기.
            Vector3 moveVelocity = moveinput.normalized * moveSpeed;

            // 방향키 대로 움직이게 조절.
            controller.Move(moveVelocity);

            // 바라보는 시점 조절.
            if (lookAimToEnemy == false)
            {
                // 방향키 보는 방향으로 시점 조절.
                // 아무것도 입력 받지 않았을 때는, 바라보는 시점을 조절하지 않는다.
                if (moveinput != Vector3.zero)
                {
                    //print(moveinput);
                    controller.LookAt(moveinput.normalized * viewSpeed);
                }
            }
            else if (lookAimToEnemy == true && dstToEenmy > Mathf.Pow(allowLookAimDst,2))
            {                
                // 방향키 보는 방향으로 시점 조절.
                if (moveinput != Vector3.zero)
                {
                    //print(moveinput);
                    controller.LookAt(moveinput.normalized * viewSpeed);
                }
            }
            else if (lookAimToEnemy == true && dstToEenmy <= Mathf.Pow(allowLookAimDst, 2))
            {
                // 위의 설정대로 적을 향해서 보는 방향으로 시점 조절.
                controller.LookAtToEnemy(targetEnemy);       
            }
        }
        */


        /* 카메라 관련 처리 */

        // 마우스에 위치에 따라 보이는 방법은 현재 폐기.
        /*
        if (isSwing)
        {
            // 카메라에서 마우스 위치를 통과하는 레이를 반환.
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            // ray가 바닥에 교차했을 경우
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                // 카메라 ray와 바닥이 교차한 지점 좌표 반환
                Vector3 point = ray.GetPoint(rayDistance);
                //Debug.DrawLine(ray.origin, point, Color.red);

                controller.LookAt(point);
            }

        }
        */

        /* 행동과 상태에 관련된 처리 */

        // Archer

        // Scout특성 심호흡 관련 처리
        if (DeepBreath_On == true)
        {
            // 움직였는지 판별하는 메소드
            DistinctNotMoving();
        }

        // Scout 특성 위치 선점 관련 처리
        if(PositionPreemption_On == true && PositionPreemption_Ready == true)
        {
            // 데미지 받았는지 판별하는 메소드
            DistinctNotAttacked_PositionPreemption();
        }

        if (PositionPreemption_On == true && isPositionPreemptionCool == false)
        {
            //print(isPositionPreemptionCool);
        }


        // Mage

        // SwordMage 에너지 방벽 관련 처리
        if(playerMageController.EnergyBarrier_On == true && isMage == true)
        {
            isEnergyBarrierOn = true;
        }
        else
        {
            isEnergyBarrierOn = false;
        }


        // 구르기 상태시, 데미지 면역 상태
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Roll-Forward"))
        {
            isDamageimmunity = true;
            print("== DAMAGE IMMUNITY (Roll State)s ==");
        }
        else
        {
            isDamageimmunity = false;
        }


        /* 행동력 관련 처리 */
        // 특정 행동을 하지 않을 경우 회복 실행.
        if (behaviourResource.isBehaving == false && behaviourResource.IsResourceCycle == false)
        {
            behaviourResource.SetRecoverOverTime(5f, behaviourResource.resourceRecoveryRate, behaviourResource.resourceRecoveryVal);
        }

        // 영혼에너지 적용
        if (playerMageController.SpritEnergy_On == true && isMage == true)
        {
            behaviourResource.isSpiritEnergyApplied = true;
        }
        else
        {
            behaviourResource.isSpiritEnergyApplied = false;
        }


        /* 데미지 관련 처리 */

        // Acher

        // 생존 주의 관련 처리
        if (Survivalism_On == true)
        {
            // 원거리 무기 생존주의 효과 적용.
            rangedController.AddAbsorptionDamage_Survivalism();

            // 근거리 무기 생존주의 효과 적용.
            meleeContoller.AddAbsorptionDamage_Survivalism();
        }

        // Warrior

        // 위압 관련 처리
        if (coercion_On == true)
        {
            // 스택 유지시간 적용
            CheckCoercionStack_InitTimer();
        }

        /* 무기 입력 관련 처리 */

        // 모든 처리에서 행동력에 영향을 받음.

        // 'K' 버튼 클릭시 근거리 무기 사용.
        // 반드시 Grounded 상태가 되어있을 경우 공격.
        if (behaviourResource.behaviourPoint > behaviourResource.MeleeActionCost || (behaviourResource.spiritEnergy > behaviourResource.MeleeActionCost && playerMageController.SpritEnergy_On == true))
        {
            if ((Input.GetKeyDown(KeyCode.K) || isPressedMeleeAtkButton == true) && isSwing == true)
            {
                // 버튼 클릭 후에, 근거리 무기 처리.
                ProcessKeyDown_Melee();

                // 행동력 처리
                behaviourResource.UseBehaviourResource(behaviourResource.MeleeActionCost);
                print("=== BehaviourResource " + behaviourResource.behaviourPoint);

                // 솔선 효과가 있으면, 그에 따른 효과를 추가한다.
                if (Lead_On == true)
                {
                    meleeContoller.AddLeadAbility();
                    playerWarriorController.Lead_StackCount++;
                    print("== Lead Stack : " + playerWarriorController.Lead_StackCount);
                }

                // 히트 카운트 증가시켜 피격 판정.
                meleeContoller.IncHitCount();
            }
        }


        // 'O'버튼 클릭시 원거리 무기 사용.
        // 자세를 취하고 원거리 무기로 투사체를 쏨.
        // 반드시 Grounded 상태가 되어있을 경우 공격.
        if ((Input.GetKeyDown(KeyCode.O) || isPressedRangedAtkButton == true) && isShot == true && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // 버튼 클릭 후에, 원거리 무기 처리.
            ProcessKeyDown_Ranged();

            // 행동력 처리
            behaviourResource.UseBehaviourResource(behaviourResource.RangedActionCost);
            print("=== BehaviourResource " + behaviourResource.behaviourPoint);
        }


        // 'J'버튼 클릭시 방패 사용.
        if ((Input.GetKeyDown(KeyCode.J) || isPressedShieldButton == true) && isSwing == true && playerWarriorController.isDefense_On == true)
        {
            // 버튼 클릭 후에, 방패 처리.
            ProcessKeyDown_Shield();
        }


        // 'L'버튼 클릭시 육중한 강타 사용.
        if (playerWarriorController.HeavySmash_On == true)
        {
            if ((Input.GetKeyDown(KeyCode.L) || isPressedHeavySmashButton == true) && playerWarriorController.HeavySamsh_SkillCount > 0)
            {
                // 버튼 클릭 후에, 육중한 강타 처리.
                ProcessKeyDown_HeavySmash();
            }
        }


        // 'P'버튼 클릭시 주문불꽃을 사용함.
        // 원하는 위치에 불꽃을 소환.
        if(behaviourResource.behaviourPoint > behaviourResource.SpellCaster_SpellBlazeCost || (behaviourResource.spiritEnergy > behaviourResource.SpellCaster_SpellBlazeCost && playerMageController.SpritEnergy_On == true))
        {
            if ((Input.GetKeyDown(KeyCode.P) || isPressedSpellButton == true) && isSwing == true && playerMageController.SpellBlaze_On == true && playerMageController.isSpellBlazeCasting == false && isSpellBlazeCool == true)
            {
                isSpellBlazeCool = false;
                StartCoroutine(SkillCool_SpellBlaze(playerMageController.SpellBlaze_CoolTime));

                // 시전 딜레이 적용
                // StartCoroutine(SpellBlazePreDelay(playerMageController.spellBlazeCastingPreDelayTime));

                // 버튼 클릭 후에, 주문 불꽃 처리.
                ProcessKeyDown_SpellBlaze();

                // 행동력 처리
                behaviourResource.UseBehaviourResource(behaviourResource.SpellCaster_SpellBlazeCost);
                print("=== BehaviourResource " + behaviourResource.behaviourPoint);
            }
        }


        // 'F'버튼 클릭시 
        // 구르기를 함.
        // 특성 찍으면, 순간이동을 사용함.
        // 짧은 거리 만큼 앞쪽으로 순간이동.
        if ((Input.GetKeyDown(KeyCode.F) || isPressedRollButton == true) && isRollReady == true)
        {
            isRollReady = false;
            // 시전 시간 적용
            StartCoroutine(RollCasting(PlayerRollCasting));

            // 버튼 클릭 후에, 순간 이동 처리.
            if (playerMageController.Blink_On == true)
            {
                ProcessKeyDown_Blink();
            }
            else
            {
                // 버튼 클릭 후에, 앞 구르기 처리.
                ProcessKeyDown_Roll();
            }

        }


        // 'T'버튼 클릭시 회오리 불꽃을 사용함.
        // UI타일을 만들고, 이동하는 방식으로 사용.
        if ((Input.GetKeyDown(KeyCode.T) || isPressedTornadoBlazeButton == true) && playerMageController.TornadoBlaze_On == true)
        {
            // 버튼 클릭 후에, 회오리 불꽃 처리.
            ProcessKeyDown_TornadoBlaze();
        }


        // 돌진 버튼 클릭시 돌진을 사용함.
        // 깃발 버튼으로도 활용.
        if (behaviourResource.behaviourPoint > behaviourResource.Assassin_RushAtkCost)
        {
            if (isPressedRushButton == true && (playerArcherController.Rush_On == true || playerWarriorController.Flag_On == true) && isRushCool == true)
            {
                isRushCool = false;

                // 돌진 스킬 쿨 타임 적용
                StartCoroutine(SkillCool_Rush(playerArcherController.rushCoolTime));

                // 버튼 클릭 후에, 돌진 처리.
                if (playerArcherController.Rush_On == true)
                {
                    ProcessKeyDown_Rush();
                }

                // 버튼 클릭 후에, 깃발 처리.
                if (playerWarriorController.Flag_On == true)
                {
                    ProcessKeyDown_Flag();

                }

                // 행동력 처리
                behaviourResource.UseBehaviourResource(behaviourResource.Assassin_RushAtkCost);
                print("=== BehaviourResource " + behaviourResource.behaviourPoint);

            }
        }



        /* 근거리 무기 관련 후 처리 */

        PostProcess_Melee();


        /* 방패 관련 후 처리 */

        PostProcess_Shield();


        /* 원거리 무기 관련 후 처리 */

        PostProcess_Ranged();


        /* 마법 공격 후 처리 */

        PostProcess_Spell();


        /* 테스트 키 */

        // 'Q' TEST 적용 키.
        if (Input.GetKeyUp(KeyCode.Q))
        {
            // Test키 눌렀을 때 관련 처리
            ProcessKeyDown_TestKey();
        }


        /* 회복 관련 처리 */
        RegenerateHealth();
        //ProcessHeal();

        // 스킬 회복 적용. (스킬 자원에 대한)
        if (playerWarriorController.isSmashRecvoer_On == true && playerWarriorController.heavySmashOneCycle == true)
        {
            playerWarriorController.heavySmashOneCycle = false;
            playerWarriorController.isSmashRecvoer_On = false;

            StartCoroutine(RecoverHeavySmash(playerWarriorController.HeavySmash_RecoverTime));
        }

    }

    /*
    public override bool TakeDamage(float damage)
    {
        // 데미지 면역 상태 일 경우, 바로 return
        if (isDamageimmunity == true)
        {
            return false;
        }

        // 에너지 방벽 상태 일 경우,
        if (isEnergyBarrierOn == true)
        {
            behaviourResource.spiritEnergy -= damage * 0.6f;
            behaviourResource.spiritEnergy = (int)behaviourResource.spiritEnergy;

            print("** SpiritEnergy : " + behaviourResource.spiritEnergy + " **");

            if (behaviourResource.spiritEnergy > 0)
            {
                print("== EnergyBarrier prevents Damage ==");
                return false;
            }

        }

        base.TakeDamage(damage);

    }
    */



    // 회복 관련 처리하는 메소드
    // 2018.10.09 회복 스킬이 현재는 없으므로 비활성화

    /*
    public void ProcessHeal()
    {
        //  'H'버튼 클릭시 즉시 체력 회복.
        // 체력 회복.
        if (Input.GetKeyDown(KeyCode.H))
        {
            this.Heal(SkillhealValue);
            print(this.healthPoint);
        }

        // 'G버튼 클릭시 시간에 걸쳐서 체력 회복.
        // 체력 회복.
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetHealOverTime(SkillHealTime, SkillHealSpeed, SkillhealValue);
        }
    }
    */


    // 2018.10.05 부로 현재 예전의 속도 설정 메소드는 미사용하므로 비활성화함.

    // 속도 관련 설정을 처리하는 메소드
    public void ProcessVelocity()
    {
        // 'R'키를 누르면 속도 리셋.
        // 이 메소드는 미사용이므로 비활성화.

        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            resetSpeed = true;
        }
        */

        // 'T'키를 누르면 일정한 가속도 적용해서 속도 변경.
        // 이 메소드는 미사용이므로 비활서화.

        /*
        if (Input.GetKeyDown(KeyCode.T))
        {
            applyConstantAccer = true;
        }
        */

        // 'Y'키를 누르면 변하는 가속도 적용해서 속도 변경.

        /*
        if (Input.GetKeyDown(KeyCode.Y))
        {
            applyChangeAccer = true;
        }

        // 'U키를 누르면 즉각 속도 변경.
        if (Input.GetKeyDown(KeyCode.U))
        {
            applySuddenChangeSpeed = true;
        }

        ControlSpeed();
        */
    }


    /* 특정 키를 누른 것에 대한 처리 */

    // 근거리 무기 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_Melee()
    {
        isSwing = false;
        behaviourResource.isBehaving = true;

        // 시전 타임 적용.
        // 쿨 타임은 시전 타임이 끝나면 자동으로 연결.
        StartCoroutine(SwingCasting(PlayerOneMeleeSwingCasting));

        // 공격 애니메이션 호출.
        // 콤보별 모션 호출

        // 콤보별로 모션 유지가 되지 않고 초기화됐는지 체크하도록 함.
        checkMeleeIntializedCombo = true;

        print("=== MeleeCombo " + meleeContoller.getCurrentWeaponCombo());

        if (playerWarriorController.HammerSecurities_On == true)
        {

            animator.SetTrigger("Hammer_Attack_Act");

            // Combo 1.
            if (meleeContoller.getCurrentWeaponCombo() == 0)
            {
                print("combo 1");
                animator.SetBool("Hammer_Attack_1", true);
            }

            // Combo 2.
            if (meleeContoller.getCurrentWeaponCombo() == 1)
            {
                animator.SetBool("Hammer_Attack_2", true);
            }

            // Combo 3.
            if (meleeContoller.getCurrentWeaponCombo() == 2)
            {
                animator.SetBool("Hammer_Attack_3", true);
            }
        }
        else
        {
            // animator.SetFloat("Archer_Knife_Attack_Speed", 2f);

            // Warrior Animation 적용.
            if (isWarrior == true)
            {
                // Combo 1.
                if (meleeContoller.getCurrentWeaponCombo() == 0)
                {
                    animator.SetBool("Warrior_Knife_Attack_1", true);
                }

                // Combo 2.
                if (meleeContoller.getCurrentWeaponCombo() == 1)
                {
                    animator.SetBool("Warrior_Knife_Attack_2", true);
                }

                // Combo 3.
                if (meleeContoller.getCurrentWeaponCombo() == 2)
                {
                    animator.SetBool("Warrior_Knife_Attack_3", true);
                }
            }
            else if (isMage == true) // Mage Animation 적용
            {
                // Combo 1.
                if (meleeContoller.getCurrentWeaponCombo() == 0)
                {
                    animator.SetBool("Mage_Knife_Attack_1", true);
                }

                // Combo 2.
                if (meleeContoller.getCurrentWeaponCombo() == 1)
                {
                    animator.SetBool("Mage_Knife_Attack_2", true);
                }

            }
            else // 기본 Archer Animation 적용
            {
                // Combo 1.
                if (meleeContoller.getCurrentWeaponCombo() == 0)
                {
                    print("combo 1");

                    animator.SetTrigger("Archer_Act");
                    animator.SetBool("Archer_Knife_Attack_1", true);
                }
                // Combo 2.
                if (meleeContoller.getCurrentWeaponCombo() == 1)
                {
                    animator.SetBool("Archer_Knife_Attack_2", true);
                }

                // Combo 3.
                if (meleeContoller.getCurrentWeaponCombo() == 2)
                {
                    animator.SetBool("Archer_Knife_Attack_3", true);
                }

                // Combo 4.
                if (meleeContoller.getCurrentWeaponCombo() == 3)
                {
                    animator.SetBool("Archer_Knife_Attack_4", true);
                }

                // Combo 5.
                if (meleeContoller.getCurrentWeaponCombo() >= 4)
                {
                    animator.SetBool("CrossBow_Knife_Attack_5", true);
                }
            }

        }

    }


    // 원거리 무기 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_Ranged()
    {
        isShot = false;
        behaviourResource.isBehaving = true;


        // 자동 에임 시스템 적용.
        if (playerMageController.isAutoAimSystem_On == true)
        {
            print("AUTO ON");

            // targetEnemy로 지정할 Enemy객체 찾기.
            targetEnemy = GameObject.FindGameObjectWithTag("Enemy").transform;
            // Enemy객체와 Player객체까지의 거리.
            // 참고로 제곱값임.
            dstToEenmy = (targetEnemy.position - transform.position).sqrMagnitude;

            this.transform.LookAt(targetEnemy);
            rangedController.equippedRangedWeapon.transform.LookAt(targetEnemy);
        }


        // 일반 석궁인지 소드오프인지에 따라 다르게 처리.

        // 석궁을 들어 공격 자세를 취한다.
        if (playerArcherController.SwordOffBowGun_On == true)
        {
            animator.SetBool("CrossBow_2SwordOff_ToGround", false);
            animator.SetBool("CrossBow_2SwordOff_Shot", false);

            // 시전 타임 적용.
            // // 쿨 타임은 시전 타임이 끝나면 자동으로 연결.
            StartCoroutine(ShotCasting(PlayerCrossBowShotCasting));

            animator.SetBool("CrossBow_2SwordOff_Atk", true);

            // 공격 애니메이션 호출 뒤에, 투사체 날림
            StartCoroutine(ReadyToShot(shotMotionTime));

        }
        else
        {
            animator.SetBool("CrossBowToGround", false);
            animator.SetBool("CrossBow_Shot", false);

            // 시전 타임 적용.
            // 쿨 타임은 시전 타임이 끝나면 자동으로 연결.
            StartCoroutine(ShotCasting(PlayerCrossBowShotCasting));

            animator.SetBool("CrossBowAtk", true);

            // 공격 애니메이션 호출 뒤에, 투사체 날림
            StartCoroutine(ReadyToShot(shotMotionTime));
        }
    }


    // 구르기 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_Roll()
    {
        animator.SetTrigger("RollForward");

        /*
        Vector3 RollPos = this.transform.position + this.transform.forward.normalized * RollDst;
        // 구르기 도약 적용.
        this.transform.position = RollPos;
        */

    }


    // 방패 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_Shield()
    {
        animator.SetBool("ShieldToGround", false);

        // 방어 애니메이션 재생
        animator.SetTrigger("Shield");

        // 방어력 적용
        // StartCoroutine(DefenseCasting(playerWarriorController.PlayerShieldTime));

        // 방어와 연계되는 스킬 시전 가능하도록.
        playerWarriorController.keydownForShieldSkill = true;
    }


    // 스킬 '육중한 강타' 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_HeavySmash()
    {
        playerWarriorController.isSmashRecvoer_On = true;

        // 스킬 갯수 하나 소모.
        playerWarriorController.HeavySamsh_SkillCount--;

        // 강타 모션 재생.
        animator.SetTrigger("Hammer_HeavySmash_Trigger");

        // 플레이어가 바라보는 방향으로 WID=2 RANGE=3 만큼 
        FieldColliderBox HeavySmashHitBox = Instantiate(fieldColliderBox, transform.position + transform.forward * 2, transform.rotation) as FieldColliderBox;
        HeavySmashHitBox.transform.localScale = new Vector3(2, 10, 3);

        // 일직선으로 공격력의 80 % 만큼 광역 물리피해
        HeavySmashHitBox.setDamage((int)meleeContoller.equippedMeleeWeapon.damage * 0.8f);
        // 필드 라이프 타임 설정.
        HeavySmashHitBox.setLifeTime(1000f);

        print("=== HeavySmash Damage " + HeavySmashHitBox.damage);

    }

    // 스킬 '주문 불꽃' 관련 키를 눌렀을 떄 처리.
    public void ProcessKeyDown_SpellBlaze()
    {
        // 캐스팅 판별 변수 On
        playerMageController.isSpellBlazeCasting = true;

        // 캐스팅 사전 캐스팅 시간 적용
        // StartCoroutine(SpellBlazePreDelay(playerMageController.spellBlazeCastingPreDelayTime));

        // 모션 재생
        animator.SetBool("SpellAtk", true);

        // SpellBlaze 시전 UI 오브젝트 생성.
        playerMageController.SpellBlazeTile = Instantiate(playerMageController.SpellBlaze_UI, transform.position + transform.forward * 2, Quaternion.Euler(Vector3.right * 90)) as GameUIObejct;
        playerMageController.SpellBlazeTile.transform.localScale = new Vector3(4, 4, 1);

        playerMageController.isSpellBlazeTileUI_Ready = true;


        // Blaze 생성 준비
        playerMageController.keyDownForSpellBlaze = true;
    }

    // 스킬 '순간 이동' 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_Blink()
    {
        Vector3 BlinkPos = this.transform.position + this.transform.forward.normalized * playerMageController.BlinkDst;

        // 순간이동 적용.
        this.transform.position = BlinkPos;
    }

    // 스킬 '회오리 불꽃' 관련 키를 눌렀을 때 처리.
    public void ProcessKeyDown_TornadoBlaze()
    {
        // 모션 재생
        animator.SetBool("SpellTornadoBlaze_Trigger", true);

        // TornadoBlaze UI 오브젝트 생성.
        playerMageController.TornadoBlazeTile = Instantiate(playerMageController.TornadoBlazeTile_UI, transform.position + transform.forward * 2, Quaternion.Euler(Vector3.right * 90)) as GameUIObejct;
        playerMageController.TornadoBlazeTile.transform.localScale = new Vector3(3, 3, 1);

        // 토네이도 필드 공격 박스 생성.
        FieldColliderBox TornadoBlazeColliderBox = Instantiate(fieldColliderBox, playerMageController.TornadoBlazeTile.transform.position, playerMageController.TornadoBlazeTile.transform.rotation) as FieldColliderBox;
        TornadoBlazeColliderBox.transform.localScale = new Vector3(3, 3, 10);

        // UI를 부모로 설정.
        TornadoBlazeColliderBox.transform.parent = playerMageController.TornadoBlazeTile.transform;

        // 토네이도 필드 공격 박스 데미지 설정.
        TornadoBlazeColliderBox.setDamage(playerMageController.TornadoBlazeDamage);
        // 토네이도 필드 공격 박스 추가 데미지 설정.
        TornadoBlazeColliderBox.setAdditionalDamage(playerMageController.TornadoBlazeAdditionalDamage);

        // 토네이도 필드 공격 박스 라이프 타임 설정.
        TornadoBlazeColliderBox.setLifeTime(playerMageController.TornadoBlazeCastingMsTime);
        // 토네이도 필드 공격 박스 필드 공격 DOT 적용.
        TornadoBlazeColliderBox.setDamageOverTime(playerMageController.TornadoBlazeDamagePerMs);
        // 토네이도 필드 공격 끌어당김 적용.
        TornadoBlazeColliderBox.setTornadoDraw();


        // 토네이도 방향 설정
        playerMageController.TornadoDirection = this.transform.forward;

        // Tornado 이동과 효과 적용 준비 완료
        playerMageController.TornadoMoveReady = true;

    }

    // 스킬 '돌진' 관련 키를 눌렀을 때 처리
    public void ProcessKeyDown_Rush()
    {
        animator.SetTrigger("Assassin_Rush");

        // 앞으로 밀려나가는 것 처리
        StartCoroutine(RushRoutine(playerArcherController.totRushdst, playerArcherController.rushSpeed, playerArcherController.rushTime));

        // 공격에 대한 처리
        meleeContoller.Assassin_RushAttack();

        // rushAttack 후 상승된 데미지 초기화
        StartCoroutine(RushInitialization(playerArcherController.rushTime));
    }

    // 스킬 '깃발' 관련 키를 눌렀을 때 처리
    public void ProcessKeyDown_Flag()
    {
        // 깃발 오브젝트 생성
        Flag flagGameObject = Instantiate(playerWarriorController.flagPrefab, transform.position, Quaternion.identity) as Flag;

        // 깃발 유지시간 설정
        flagGameObject.setLifeTime(playerWarriorController.flag_Lifetime);
        // 깃발 효과 적용 거리 설정
        flagGameObject.setAdditionalDamageRangeDst(playerWarriorController.flag_AppliedDst);
        // 깃발 효과 적용 후, 추가 데미지 설정
        flagGameObject.setAdditionalDamage(playerWarriorController.flag_AdditionalDamage);

    }

    // '테스트 키' 관련 키를 눌렀을 때 처리
    public void ProcessKeyDown_TestKey()
    {
        if (playerArcherController.isBoltOn == true)
        {
            print("==SKILL Bolt on==");
            //ArcherBaseActiveSkill_Bolt_On();
        }

        if (playerArcherController.BulkBoltMagazie_On == true)
        {
            print("==SKILL BulkBoltMagazie==");
            EngineerCoreTalentsSkill_BulkBoltMagazie_On();
        }

        if (playerArcherController.SwordOffBowGun_On == true)
        {
            print("==SKILL SwordOffBowGun==");
            EngineerExpertTalentsSkill_SwordOffBowGun_On();

        }

        if (playerArcherController.ReloadLever_On == true)
        {
            print("==SKILL ReloadLever==");
            EngineerStyleTalentsSkill_ReloadLever_On();
        }

        if (playerWarriorController.isDefense_On == true)
        {
            print("==SKILL Defense");
            WarriorBaseActiveSkill_Defense();
        }

        if (playerWarriorController.KnockBack_On == true)
        {
            print("==SKILL Knock Back");
            GuardianStyleTalentsSkill_KnockBack();
        }

        if (playerWarriorController.HammerSecurities_On == true)
        {
            print("==SKILL HammerSecurities");
            GuardianExpertTalentsSkill_HammerSecurities();
        }

        if (playerWarriorController.HeavySmash_On == true)
        {
            print("==SKILL Heavy Smash");
            GuardianExpertTalentsSkill_HeavySmash();
        }

        if (playerMageController.MageBasePassive_On == true)
        {
            print("==Mage Base Passive SKILL");
            MageBasePassiveSkill();
        }

        /*
        if (playerMageController.isSpellBlaze_On == true)
        {
            print("==SKILL Spell Blaze");
            MageBaseActiveSkill_SpellBlaze();
        }
        */

        if (playerMageController.RuneCasting_On == true)
        {
            print("==SKILL Rune Casting");
            SpellCasterCoreTalentsSkill_RuneCasiting();
        }

        if (playerMageController.Blink_On == true)
        {
            print("==SKILL Blink");
            SpellCasterCoreTalentsSkill_Blink();
        }

        if (playerMageController.CoreMedium_On == true)
        {
            print("==SKILL CoreMedium");
            SpellCasterExpertTalentsSkill_CoreMedium();
        }

        if (playerMageController.TornadoBlaze_On == true)
        {
            print("==SKILL TornadoBlaze");
            SpellCasterExpertTalentsSkill_TornadoBlaze();
        }

        if (playerArcherController.PerfectEyeSight_On == true)
        {
            print("==SKILL PerfectEyeSight");
            ScoutStyleTalentsSkill_PerfectEyeSight();
        }

        if (playerArcherController.DeepBreath_On == true)
        {
            print("==SKILL DeepBreath");
            ScoutStyleTalentsSkill_DeepBreath();
        }

        if (playerArcherController.PositionPreemption_On == true)
        {
            print("==SKILL PositionPreemption");
            ScoutStyleTalentsSkill_PositionPreemption_On();
        }

        if (playerArcherController.PenetratingWeakness_On == true)
        {
            print("==SKILL PenetratingWeakness");
            ScoutStyleTalentsSkill_PenetratingWeakness_On();
        }

        if (playerArcherController.Survivalism_On == true)
        {
            print("==SKILL Survivalism");
            AssassinStyleTalents_Survivalism_On();
        }

        if (playerArcherController.Rush_On == true)
        {
            print("==SKILL Rush");
            AssassinStyleTalents_Rush_On();
        }

        if (playerWarriorController.Coercion_On == true)
        {
            print("==SKILL Coercion");
            BerserkerStyleTalents_Coercion();
        }

        if (playerWarriorController.Rage_On == true)
        {
            print("==SKILL Rage");
            BerserkerStyleTalents_Rage();
        }

        if (playerWarriorController.DefenseVanguard_On == true)
        {
            print("==SKILL DefenseVanguard");
            GuardianStyleTalentsSkill_DefenseVanguard();
        }

        if (playerWarriorController.DefenseReinforcement_On == true)
        {
            print("==SKILL DefenseReinforcement");
            GuardianStyleTalentsSkill_DefenseReinforcement();
        }

        if (playerWarriorController.Lead_On == true)
        {
            print("==SKILL Lead");
            WarlordStyleTalents_Lead();
        }

        if (playerWarriorController.Flag_On == true)
        {
            print("==SKILL Flag");
            WarlordStyleTalents_Flag();
        }

        if (playerMageController.SpritEnergy_On == true)
        {
            print("==SKILL SpiritEnergy");
            SpellCasterStyleTalents_SpiritEnergy();
        }

        if (playerMageController.SpellBlaze_On == true)
        {
            print("==SKILL SpellBlaze");
            SpellCasterStyleTalents_SpellBlaze();
        }

        if (playerMageController.ExplosionBlaze_On == true)
        {
            print("==SKILL ExplosionBlaze");
            SpellCasterStyleTalents_ExplosionBlaze();
        }

        if(playerMageController.SpellAcceleration_On == true)
        {
            print("==SKILL SpellAcceleration");
            SpellCasterStyleTalents_SpellAcceleration();
        }

        

    }


    /* 버튼 클릭 후 처리 */

    // 근거리 무기 버튼 클릭 후 처리
    public void PostProcess_Melee()
    {
        // 근거리 콤보 관련 처리

        // 콤보가 유지시간을 지나서 초기화 됐는지 확인.
        if (checkMeleeIntializedCombo == true)
        {
            if (meleeContoller.equippedMeleeWeapon.getInitializedCombo() == true)
            {
                print("ALL RESET!");

                // 모든 모션 조건을 초기화하고, 다시 회귀하는 모션 적용.
                animator.SetBool("Archer_Knife_Attack_1", false);
                animator.SetBool("Archer_Knife_Attack_2", false);
                animator.SetBool("Archer_Knife_Attack_3", false);
                animator.SetBool("Archer_Knife_Attack_4", false);
                animator.SetBool("Archer_Knife_Attack_5", false);

                animator.SetTrigger("Archer_Knife_Attack_End");

                if (isWarrior == true)
                {
                    animator.SetBool("Warrior_Knife_Attack_1", false);
                    animator.SetBool("Warrior_Knife_Attack_2", false);
                    animator.SetBool("Warrior_Knife_Attack_3", false);

                    animator.SetTrigger("Warrior_Knife_Attack_End");
                }

                if (isMage == true)
                {
                    animator.SetBool("Mage_Knife_Attack_1", false);
                    animator.SetBool("Mage_Knife_Attack_2", false);

                    animator.SetTrigger("Mage_Knife_Attack_End");
                }

                // 망치 모션 역시 초기화하고, 다시 회귀.
                if (playerWarriorController.HammerSecurities_On == true)
                {
                    animator.SetBool("Hammer_Attack_1", false);
                    animator.SetBool("Hammer_Attack_2", false);
                    animator.SetBool("Hammer_Attack_3", false);

                    animator.SetTrigger("Hammer_Attack_End");
                }

                // 콤보 유지가능하도록 다시 설정.
                checkMeleeIntializedCombo = false;
                meleeContoller.equippedMeleeWeapon.setInitializedCombo();
            }
        }

    }


    // 방패 버튼 클릭 후 처리
    public void PostProcess_Shield()
    {
        if (playerWarriorController.keydownForShieldSkill)
        {
            // 키를 계속 누르고 있으면,
            if ((Input.GetKey(KeyCode.J) || isPressedShieldButton == true))
            {

                // 속도 50% 감소
                player_ThridPersoncharacter.MultiplySpeed(0.5f);


                // 방패 선봉대 효과 추가
                if (DefenseVanguard_On == true && isDefenseVanguard_Cycle == true && isDefenseVanguardCool == true)
                {
                    isDefenseVanguard_Cycle = false;
                    StartCoroutine(DefenseTempChange_Inc(playerWarriorController.DefenseVanguard_DurationTime, playerWarriorController.DefenseVanguard_DefenseVal));

                    isDefenseVanguardCool = false;
                    StartCoroutine(SkillCool_DefenseVanguard(playerWarriorController.DefenseVanguard_CoolTime));
                }
                else if (DefenseVanguard_On == false)
                {
                    // 방어력 적용
                    this.RateDefensePercent = 50;
                }


                // 방패 강화 효과
                if (DefenseReinforcement_On == true)
                {
                    // 고정 피해 감소량 적용
                    FixedDamageDecrement = 25;
                }


                // 'KnockBack'스킬 사용시
                if (playerWarriorController.KnockBack_On == true)
                {
                    // 'K'버튼 클릭시 'KnockBack'스킬 사용.
                    if (Input.GetKeyDown(KeyCode.K) || isPressedMeleeAtkButton == true && isKnockBackCool == true)
                    {
                        isKnockBackCool = false;
                        // 넉백 쿨타임 적용
                        StartCoroutine(SkillCool_KnockBack(playerWarriorController.KnockBack_CoolTime));

                        // 넉백 애니메이션 재생.
                        animator.SetTrigger("Shield_KnockBack_Trigger");

                        // 넉백 피격에 대한 처리.
                        meleeContoller.KnockBackEnemy();

                        //playerWarriorController.keydownForShieldSkill = false;
                    }


                    /*
                    if (behaviourResource.behaviourPoint > behaviourResource.MeleeActionCost)
                    {
                        // 'K' 버튼 클릭시 근거리 무기 사용.
                        // 반드시 Grounded 상태가 되어있을 경우 공격.
                        if ((Input.GetKeyDown(KeyCode.K) || isPressedMeleeAtkButton == true) && isSwing == true)
                        {
                            meleeContoller.AddKnockBack();

                            // 버튼 클릭 후에, 근거리 무기 처리.
                            ProcessKeyDown_Melee();

                            // 행동력 처리
                            behaviourResource.UseBehaviourResource(behaviourResource.MeleeActionCost);
                            print("=== BehaviourResource " + behaviourResource.behaviourPoint);

                            // 히트 카운트 증가시켜 피격 판정.
                            meleeContoller.IncHitCount();
                        }
                    }
                    */

                }

                // 'Taunt' 스킬 사용시
                if (playerWarriorController.Taunt_On == true)
                {
                    // 'B'버튼 클릭시 'Taunt'스킬 사용.
                    if (Input.GetKeyDown(KeyCode.B) || isPressedTauntButton == true)
                    {
                        // 도발 모션 재생.
                        animator.SetTrigger("Shield_Taunt_Trigger");

                        // 도발 효과 적용.
                        meleeContoller.TauntEnemy(playerWarriorController.TauntDst);

                        playerWarriorController.keydownForShieldSkill = false;
                    }
                }

                // 'Roar' 스킬 사용시
                if (playerWarriorController.Roar_On == true)
                {
                    // 'M'버튼 클릭시 'Roar'스킬 사용.
                    if (Input.GetKeyDown(KeyCode.M) || isPressedRoarButton == true)
                    {
                        // 포효 모션 재생.
                        animator.SetTrigger("Shield_Roar_Trigger");

                        // 적을 찾고 넉백을 하는 메소드 호출.
                        meleeContoller.Roar(playerWarriorController.RoarDst);

                        playerWarriorController.keydownForShieldSkill = false;
                    }
                }
            }

            // 키 올림을 감지.
            if ((Input.GetKeyUp(KeyCode.J) || isPressedShieldButton == false))
            {
                // 그냥 Ground 모션으로 돌아감.
                animator.SetBool("ShieldToGround", true);
                playerWarriorController.keydownForShieldSkill = false;

                // 방어 효과 초기화
                this.RateDefensePercent = this.initRateDefensePercent;
                // 속도 초기화
                player_ThridPersoncharacter.SetSpeedMultiplier(nowMultiplySpeed, nowMultiplySpeed);

                if (DefenseReinforcement_On == true)
                {
                    FixedDamageDecrement = 0;
                }

                // 넉백 효과 제거
                if (playerWarriorController.KnockBack_On == true)
                {
                    //meleeContoller.SubtractKnockBack();
                }
            }
        }

    }


    // 원거리 무기 버튼 클릭 후 처리
    public void PostProcess_Ranged()
    {
        // 'O' 키를 눌렀다 땔떼까지 쏘기 대기.
        if (keydownForShot == true)
        {

            // 조준 하는 시간이 2초 이상일 경우,
            if (Time.time - aimTime >= 2)
            {
                if (Time.time - aimTime >= 2.4)
                {
                    // 카메라 조정
                    viewCamera.transform.position = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y + boltSkillEyeSight, viewCamera.transform.position.z);
                }

                print(Time.time - aimTime);

                // 사거리 1씩
                // 데미지 1%씩 증가하도록 설정.
                rangedController.SetMultiplyPercentDamage();
                rangedController.SetAddRangedDst();
            }

            if (Input.GetKeyUp(KeyCode.O) || isPressedRangedAtkButton == false)
            {
                // 천리안
                if (isPerfectSight_AdditionalDst == true)
                {
                    rangedController.ChangeRangedDstToPercent_Inc(25f);
                }

                // 심호흡 추가 데미지 적용
                if (DeepBreath_On == true)
                {
                    if (isdeepBreath_AdditionalDamage == true)
                    {
                        isdeepBreath_AdditionalDamage = false;

                        // 데미지 증가
                        rangedController.ChangeDamageToPercent_Inc(25f);
                    }
                }

                if (playerArcherController.SwordOffBowGun_On == true)
                {
                    // 먼저 발사 준비가 됐는지 확인
                    if (rangedController.ShootReadyCompletedOrNot() == true)
                    {
                        rangedController.Shoot();
                        rangedController.SetInitDamage();
                        rangedController.SetInitRangedDst();

                        keydownForShot = false;

                        shotRotationSet = true;

                        animator.SetBool("CrossBow_2SwordOff_Shot", true);
                        animator.SetBool("CrossBow_2Swordoff_ToGround", true);

                        animator.SetBool("CrossBow_2SwordOff_Atk", false);
                    }
                    else
                    {
                        keydownForShot = false;

                        shotRotationSet = true;

                        animator.SetBool("CrossBow_2SwordOff_Shot", true);
                        animator.SetBool("CrossBow_2Swordoff_ToGround", true);

                        animator.SetBool("CrossBow_2SwordOff_Atk", false);

                        print("==PLYAER SHOT IS NOT READY==");

                    }
                }
                else
                {
                    // 먼저 발사 준비가 됐는지 확인
                    if (rangedController.ShootReadyCompletedOrNot() == true)
                    {
                        rangedController.Shoot();
                        rangedController.SetInitDamage();
                        rangedController.SetInitRangedDst();

                        keydownForShot = false;

                        shotRotationSet = true;

                        animator.SetBool("CrossBow_Shot", true);
                        animator.SetBool("CrossBowToGround", true);

                        animator.SetBool("CrossBowAtk", false);
                    }
                    else
                    {
                        keydownForShot = false;

                        shotRotationSet = true;

                        animator.SetBool("CrossBow_Shot", true);
                        animator.SetBool("CrossBowToGround", true);

                        animator.SetBool("CrossBowAtk", false);

                        print("==PLYAER SHOT IS NOT READY==");
                    }
                }
            }
        }

        // 'R'키를 누르면 '장전'
        if (Input.GetKeyUp(KeyCode.R) || isPressedReloadButton == true)
        {
            if (playerArcherController.SwordOffBowGun_On == true)
            {
                // 장전 애니메이션 호출
                animator.SetTrigger("Overlayer_2SwordOff_Trigger");
            }
            else
            {
                // 장전 애니메이션 호출
                animator.SetTrigger("Overlayer_crossbow_lever_Trigger");
            }

            // 장전 수행
            rangedController.RestoreProjectileCount();

            // 장전시 2.5초동안 속도를 감소시킨다.
            StartCoroutine(ReloadSlow(2000, reloadSlowRate));
        }

        // 원거리 무기 원래 회전값으로 회귀
        if (shotRotationSet == true && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            //print("OKAY?");
            shotRotationSet = false;
        }

    }


    // 마법 공격 버튼 클릭 후 처리
    public void PostProcess_Spell()
    {
        // UI 처리.
        if (playerMageController.keyDownForSpellBlaze)
        {
            // 키를 계속 누르고 있으면,
            if (Input.GetKey(KeyCode.P) || isPressedSpellButton == true)
            {
                this.transform.LookAt(playerMageController.SpellBlazeTile.transform);

                // UI 이동 처리
                if (playerMageController.SpellBlazeTile != null && playerMageController.isSpellBlazeTileUI_Ready == true)
                {
                    // 현재 GetAxisRaw를 사용하여 스무딩 적용 안함.
                    Vector3 moveinput;

                    // 모바일 입력에 대한 처리 or
                    // PC 플랫폼 일반 처리
                    if (isMobileInput == true)
                    {
                        moveinput = new Vector3(mobile_h, 0, mobile_v);
                    }
                    else
                    {
                        moveinput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    }

                    // 입력 값의 방향 단위 벡터에다 속도 곱하기.
                    Vector3 moveVelocity = moveinput.normalized * playerMageController.SpellBlazeUI_moveSpeed;

                    // 방향키 대로 움직이게 조절.
                    playerMageController.SpellBlazeTile.Move(moveVelocity);

                    // SpellBlazeTile.LookAt(moveinput.normalized * viewSpeed);
                }
            }
        }

        // Spell Blaze 공격.
        if ((Input.GetKeyUp(KeyCode.P) || isPressedSpellButton == false) && playerMageController.isSpellBlazeTileUI_Ready == true && playerMageController.isSpellBlazeCasting == true)
        {
            playerMageController.keyDownForSpellBlaze = false;

            // 캐스팅 
            playerMageController.isSpellBlazeCasting = false;

            // 모션 재생
            animator.SetTrigger("SpellBlaze_Trigger");
            animator.SetBool("SpellAtk", false);

            // 캐스팅 딜레이 설정 후 공격.
            StartCoroutine(SpellBlazePreDelay(playerMageController.spellBlazeCastingPreDelayTime));

        }

        // TornadoBlaze 공격
        if (playerMageController.TornadoMoveReady == true)
        {
            playerMageController.TornadoMoveReady = false;
            // 주어진 시간 만큼 Routine 하여 이동.
            StartCoroutine(TornadoBlazeMoveRoutine(playerMageController.TornadoBlaze_MoveTime));
        }

    }

    // 움직이지 않는지 판별하는 메소드
    public void DistinctNotMoving()
    {
        if (Time.time >= nextNotMovingtime)
        {
            if (isMoving == false)
            {
                isdeepBreath_AdditionalDamage = true;
            }
            else
            {
                isdeepBreath_AdditionalDamage = false;
            }

            // 다음 데미지 판정 안 받을 때 까지 버틸 시간 설정.
            nextNotMovingtime = Time.time + deepbreath_notMovingSafeTime / 1000;
        }

        if (Time.time < nextNotMovingtime)
        {
            // 움직였을 경우,
            if (!(userControl.mobileInput_v == 0f && userControl.mobileInput_h == 0f))
            {
                isMoving = true;
            }
        }
    }

    // 데미지 받지 않는지 판별하는 메소드
    // 한 번만 판별
    // 위치 선점에 한정되는 메소드
    public void DistinctNotAttacked_PositionPreemption()
    {
        if (Time.time >= notAttackedTime_PositionPreemption)
        {
            // 지속 시간이 끝났으므로 종료.
            DistinctNotAttacked_PositionPreemption_Initialization();
            isAttacked = false;

            PositionPreemption_Ready = false;
            isStateOnPositionPreemption = false;
            return;
        }

        if (Time.time < notAttackedTime_PositionPreemption)
        {
            if (tmpHealth < healthPoint)
            {
                // 데미지를 받으면 곧바로 값을 초기화하고 종료.
                DistinctNotAttacked_PositionPreemption_Initialization();
                isAttacked = true;

                PositionPreemption_Ready = false;
                isStateOnPositionPreemption = false;

                return;
            }
        }
    }

    // 위치 선점 관련 설정
    public void DistinctNotAttacked_PositionPreemption_Setting()
    {

        // 2초동안 데미지 받지 않는지 판별함.
        // 쿨타임 체크 후 Routine
        if (isPositionPreemptionCool == true)
        {
            // 속도 100% 증가
            player_ThridPersoncharacter.MultiplySpeed(2.0f);
            // 유닛 통과 무시 (바닥은 제외)
            this.gameObject.layer = 1;

            // 초기 시간 설정 
            // (얼마나 데미지를 안 받고 있어야 하는 지에 대한 시간)
            notAttackedTime_PositionPreemption = Time.time + playerArcherController.notAttackedTime_PositionPreemption / 1000;

            print(notAttackedTime_PositionPreemption);
            print(Time.time);

            tmpHealth = healthPoint;

            PositionPreemption_Ready = true;

            isPositionPreemptionCool = false;
            StartCoroutine(SkillCool_PositionPreemption(playerArcherController.PositionPreemptionCoolTime));
        }
       
    }

    // 위치 선점 관련 초기화
    public void DistinctNotAttacked_PositionPreemption_Initialization()
    {
        // 현재 속도 배수로 초기화
        player_ThridPersoncharacter.SetSpeedMultiplier(nowMultiplySpeed, nowMultiplySpeed);
        // 유닛 통과 적용
        this.gameObject.layer = 0;

        // 위치선점에 관련된 모든 값을 초기화
        isAttacked = false;

        notAttackedTime_PositionPreemption = 0;
        tmpHealth = 0;

    }


    /* 특성 관련 제어 메소드 */

    // Archer - Base

    // 기본 패시브 스킬
    // [경장갑]
    public void ArcherBasePassive()
    {
        print("== Archer Base Passive On ==");

        // 이동속도 5% 증가
        player_ThridPersoncharacter.MultiplySpeed(1.05f);

        nowMultiplySpeed = 1.05f;

        // 일반공격에 의한 피해량 5% 무시
        RateDefensePercent = 5f;
    }

    // 기본 액티브 스킬
    // [볼트]

    // 기본 액티브 스킬
    // [구르기]
    // Archer 기본 액티브 스킬 Roll (구르기)
    public void ArcherBaseActiveSkill_Roll()
    {
        // 구르기 활성화
        playerArcherController.isArcherRoll = true;
    }


    // Archer - Scout

    // 스카웃 스타일 특성
    // [천리안]
    // [볼트] 조준시 시야가 (25%/50%) 확장됩니다. 
    // 더불어 볼트의 사거리도(25%/50%) 증가합니다.
    public void ScoutStyleTalentsSkill_PerfectEyeSight()
    {
        playerArcherController.PerfectEyeSight_On = true;

        // 시야가 (25%/50%) 확장
        boltSkillEyeSight *= 1.25f;

        // 볼트의 사거리가 (25%/50%) 증가
        // 최종 사거리에 대해서 증가 시키는 것으로 변경.
        isPerfectSight_AdditionalDst = true;

        // rangedController.ChangeInitRangedDst_incPercent(25f);
    }

    // [약점 관통]
    // [볼트]스킬로 인한 피해를 줄때 상대방의 방어력의 
    // (30%/45%/60%)를 감소시킨뒤 연산한다.
    public void ScoutStyleTalentsSkill_PenetratingWeakness_On()
    {
        // 약점 관통 추가
        playerArcherController.PenetratingWeakness_On = true;

        // PenetratingWeakness_On = true;

        // 약점 관통 효과 적용
        rangedController.AddPenetratingWeakness_BoltProjectile();
    }

    // [심호흡]
    // [볼트] 발사 이전 2초간 움직이지 않았다면 
    // [볼트] 적중시 기존 피해의(25%/33%/40%)만큼 추가 물리피해를 줍니다.
    public void ScoutStyleTalentsSkill_DeepBreath()
    {
        // 심호흡 추가
        playerArcherController.DeepBreath_On = true;

        DeepBreath_On = true;
    }

    // [위치선점]
    // [빙의] 직후 2초 동안 이동속도가 100% 증가하고 오브젝트를 통과할 수 있습니다. 
    // 단 이때 이동외의 행동은 할수없으며 피해를 받으면 상태가 취소됩니다.
    public void ScoutStyleTalentsSkill_PositionPreemption_On()
    {
        playerArcherController.PositionPreemption_On = true;

        PositionPreemption_On = true;
    }

    // 위치 선점 스킬에 대한 처리 
    // Archer 변환시 호출되는 메소드
    public void Process_SkillPositionPreemption()
    {
        DistinctNotAttacked_PositionPreemption_Setting();
    }


    // Archer - Assassin

    // 어쌔신 스타일 특성
    // [신속]
    // 이동속도 (15%/30%/30%)가 증가합니다. 
    // 최대레벨 달성시[구르기]를 연속해서 한번 더 사용 가능합니다. - 2티어
    public void AssassinStyleTalents_Swiftness_On()
    {
        // 이동속도 증가
        player_ThridPersoncharacter.MultiplySpeed(1.15f);
    }

    // [생존주의]
    // 적에게 피해를 주면 해당 피해의 (5%/8%/11%)만큼 체력을 회복함.
    public void AssassinStyleTalents_Survivalism_On()
    {
        // 적에게 피해를 주면 해당 피해만큼 체력 회복 시스템 적용.
        Survivalism_On = true;
    }

    // 생존 주의 데미지 회복
    public void Process_Survivalism_AbsorptionDamage(float damage)
    {
        // 데미지 회복 지수
        float absorptionDamage = damage * 0.05f;

        // 체력 회복 적용
        healthPoint += absorptionDamage;

        healthPoint = (int)healthPoint;

        if (healthPoint >= totHealthPoint)
        {
            healthPoint = totHealthPoint;
        }

        print("== Player Absorb Damage : " + absorptionDamage);
        print("== Player Health Point : " + healthPoint);
    }

    // [돌진]
    // 진행방향으로 빠르게 접근하여 적에게 
    // 공격력의(75%/125%/175%)만큼의 물리피해를 줍니다 - 2티어
    public void AssassinStyleTalents_Rush_On()
    {

    }


    // Archer - Engineer

    // 엔지니어 스타일 특성
    // [장전 지렛대]
    // Engineer 스타일 액티브 스킬
    public void EngineerStyleTalentsSkill_ReloadLever_On()
    {
        // 장전시에
        // 이동속도 감소(20 %/ 10 %)가 줄어듦
        reloadSlowRate = 20;

        // 장전속도도(0.7초 / 1.0초) 감소
        animator.SetFloat("Overlayer_CrossBow_lever_Speed", 1.7f);

    }

    // 엔지니어 핵심 특성
    // [대용량 볼트 탄창]
    // Enginner 핵심 액티브 스킬
    public void EngineerCoreTalentsSkill_BulkBoltMagazie_On()
    {
        // 장전 가능한 볼트 최대치가 6개로 대폭상승
        rangedController.InctotProjectileCount();

        // [볼트] 스킬 사용시에, 행동력이 25% 감소.
        behaviourResource.DecByPercent_RangedWeaponBehaviour(25);
    }

    // 엔지니어 전문가 특성
    // [소드 오프 보우건]
    // Engineer 전문가 액티브 스킬
    public void EngineerExpertTalentsSkill_SwordOffBowGun_On()
    {
        // 석궁을 한손으로 쏠 수 있도록 개량. (모델링 / 애니메이션 변경)
        // 무기 스위칭 시간이 없음

        // 오른쪽에 있는 무기를 해제.
        if (meleeContoller.equippedMeleeWeapon != null)
        {
            Destroy(meleeContoller.equippedMeleeWeapon.gameObject);
        }

        // 소드 오프건 장착.
        rangedController.EquiSwordOffGun();

        // 볼트 공격력과 사거리가 40% 줄어듦 
        rangedController.ChangeInitDamage_decPercent(40f);
        rangedController.ChangeInitRangedDst_decPercent(40f);

        // 행동력소모가 50% 줄어듦
        behaviourResource.DecByPercent_RangedWeaponBehaviour(50);

        // 장전가능한 볼트 최대치가 2배가 됨.
        rangedController.SquareProjectileCount();

    }



    // Warrior - Base

    // 기본 패시브 스킬
    // [중장갑]
    public void WarriorBasePassive()
    {
        print("== Warrior Base Passive On ==");

        isWarrior = true;

        // Warrior 전용 근접무기로 변경 

        // 왼쪽에 있는 근접 무기를 해제.
        if (meleeContoller.equippedMeleeWeapon != null)
        {
            Destroy(meleeContoller.equippedMeleeWeapon.gameObject);
        }

        // Warrior 전용 근접무기 장착 (단검)
        meleeContoller.EquipWarriorMeleeWeapon_Knife();

        //일반공격에 의한 피해량 10% 무시
        this.initRateDefensePercent = 10f;
        this.RateDefensePercent = this.initRateDefensePercent;

        //이동속도 감소 5%
        player_ThridPersoncharacter.MultiplySpeed(0.95f);
    }

    // 기본 액티브 스킬
    // [방어 Defense]
    // 스킬 - 방패를 사용하는 동안 받는 물리 피해의 70% 가량을 무시함. 
    // 지속시간동안 이동불가.
    public void WarriorBaseActiveSkill_Defense()
    {
        print("== Warrior Shield On ==");
        // 왼쪽에 있는 무기를 해제.
        if (rangedController.equippedRangedWeapon != null)
        {
            //print("??");
            Destroy(rangedController.equippedRangedWeapon.gameObject);
        }

        // 방패사용 허용.
        playerWarriorController.isDefense_On = true;

        // 방패를 장착.
        meleeContoller.EquipShield();

    }

    // 기본 액티브 스킬
    // [구르기 Tumble] 
    // 스킬 - 진행방향으로 단숨에 도약.
    // (500ms 만에 3만큼 이동, 선딜레이 x, 후딜레이 3000ms)
    public void WarriorBaseActiveSkill_Tumble()
    {
        // 구르기 활성화
    }


    // Warrior - Berserker

    // 버서커 스타일 특성
    // [위압]
    // 워리어로 적을 처치하면 마다 [위압] 스택이 쌓임. 
    // 1스텍 당(3/5/7)만큼 상대 물리방어력을 고정감소.최대 중첩 5. 
    // 추가 중첩이 5초동안 발생하지 않으면 스텍이 0이 됨. -2티어
    public void BerserkerStyleTalents_Coercion()
    {
        coercion_On = true;
    }

    // 위압 스택 쌓는 메소드
    public void Coercion_StackCount_Inc()
    {
        playerWarriorController.Coercion_StackCount++;
        isCoercionStackTrans = true;

        // 최대 스택 도달 시 최대값 초과 방지
        if (playerWarriorController.Coercion_StackCount >= playerWarriorController.Coercion_StackCount_Max)
        {
            playerWarriorController.Coercion_StackCount = playerWarriorController.Coercion_StackCount_Max;
            print("== Coercion MAX ==");
        }

        print("** Coercion Stack Count : " + playerWarriorController.Coercion_StackCount + " **");
    }

    // 위압 스택 초기화 타이머
    void CheckCoercionStack_InitTimer()
    {
        if (isCoercionStackTrans == true)
        {
            coercion_Time = Time.time + playerWarriorController.Coercion_Stack_DurationTime / 1000;

            isCoercionStackTrans = false;

            print("== Coercion Stack Applied ==");
            print("== Coercion Duration Time :  " + playerWarriorController.Coercion_Stack_DurationTime / 1000);
        }

        if (isCoercionStackTrans == false && Time.time > coercion_Time)
        {
            playerWarriorController.Coercion_StackCount = 0;

            //print("== Coercion Stack All Reset ==");
        }

    }


    // 버서커 스타일 특성
    // [분노]
    // 적을 3마리 처치할때마다 [분노] 스텍이 1쌓임. 
    // [분노] 스텍 1당 공격력이(3/4/5) 고정증가.
    public void BerserkerStyleTalents_Rage()
    {
        rage_On = true;
    }

    public void Rage_StackCount_Inc()
    {
        playerWarriorController.Rage_StackCount++;

        // 최대 스택 도달 시 최대값 초과 방지
        if (playerWarriorController.Rage_StackCount >= playerWarriorController.Rage_StackCount_Max)
        {
            playerWarriorController.Rage_StackCount = playerWarriorController.Rage_StackCount_Max;
            print("== Rage MAX ==");
        }

        print("** Rage Stack Count : " + playerWarriorController.Rage_StackCount + " **");

        meleeContoller.AddDamage(playerWarriorController.Rage_AdditionalDamage);
        additionalDamage += playerWarriorController.Rage_AdditionalDamage;

    }


    // Warrior - Guardian

    // 가디언 스타일 특성
    // [방패선봉대]
    // [방어]로 받는 피해를 감소시키면 
    // 1초간 물리방어력과 마법저항력이 각각 15/30/45 고정 증가함. 중첩불가. -1티어
    public void GuardianStyleTalentsSkill_DefenseVanguard()
    {
        DefenseVanguard_On = true;
        isDefenseVanguard_Cycle = true;
    }

    // 가디언 스타일 특성
    // [방패강화]
    // [방어] 사용시 피해감소 마지막 연산에서 25/50/75만큼의 피해를 빼고 받음. 
    // 단, 체력회복의 수단이 될순없음.
    public void GuardianStyleTalentsSkill_DefenseReinforcement()
    {
        DefenseReinforcement_On = true;
    }

    // 가디언 스타일 특성
    // [넉백] [Knock-back] 
    // : [방어]스킬이 발동중일때만 사용가능 
    public void GuardianStyleTalentsSkill_KnockBack()
    {
        // 방패로 적을(3/4/5)만큼 뒤로 밀쳐냄.
        // 공격력의(30%/40%/50%)만큼 마법피해를 입힘.

        playerWarriorController.KnockBack_On = true;

    }

    // 가디언 핵심 특성
    // [도발 Taunt] 
    public void GuardianCoreTalentsSkill_Taunt()
    {
        // 방패를 두드림.
        // RAD=8 주변의 적이 자신을 공격하게 함.
        playerWarriorController.Taunt_On = true;
    }

    // 가디언 핵심 특성
    // [포효] [Roar] 
    // : [방어] 스킬을 사용중일때만 사용가능한 연계기. 
    public void GuardianCoreTalentsSkill_Roar()
    {
        // 자신을 기점으로 RAD=2안에 있는 적들 지정
        // 각각 플레이어를 기점으로 RAD=5까지 넉백됨
        playerWarriorController.Roar_On = true;
    }

    // 가디언 전문가 특성
    // [고유: 망치 경호대] [Hammer Securities] 
    public void GuardianExpertTalentsSkill_HammerSecurities()
    {
        // 오른쪽에 있는 무기를 해제.
        if (meleeContoller.equippedMeleeWeapon != null)
        {
            Destroy(meleeContoller.equippedMeleeWeapon.gameObject);
        }

        // 일반무기가 망치로 변경.
        meleeContoller.EquipHammer();

        // 기본공격 마지막 콤보에 맞은 적은 넉백되도록 설정.

    }

    // 가디언 전문가 특성
    // [육중한 강타] 
    public void GuardianExpertTalentsSkill_HeavySmash()
    {
        // 플레이어가 바라보는 방향으로 WID=2 RANGE=3 만큼 일직선으로 공격력의 80%만큼 광역 물리피해
        // 4초에 1개씩 충전됨 최대 장전수 5개

        playerWarriorController.HeavySamsh_SkillCount = playerWarriorController.HeavySmash_MaxSkillCount;

        playerWarriorController.HeavySmash_On = true;
    }

    // 육중한 강타 스킬 카운트 회복 메소드
    public void IncHeavySmashSkillCount()
    {
        playerWarriorController.HeavySamsh_SkillCount++;

        if (playerWarriorController.HeavySamsh_SkillCount > playerWarriorController.HeavySmash_MaxSkillCount)
        {
            playerWarriorController.HeavySamsh_SkillCount = playerWarriorController.HeavySmash_MaxSkillCount;
        }

    }


    // Warrior - Warlord

    // 워로드 스타일 특성
    // [솔선]
    // 기본공격시 [솔선]스텍 1증가. 
    // [빙의] 사용시[솔선] 스택 1당 행동력이(5/10/15)씩 회복함.
    public void WarlordStyleTalents_Lead()
    {
        Lead_On = true;
    }

    // 솔선에 대한 처리
    public void Process_SkillLead()
    {
        // 총 회복량
        float totRecoverVal = playerWarriorController.Lead_StackCount * playerWarriorController.Lead_BehaviourRecoveryVal;

        // 스택에 따라 행동력 회복
        behaviourResource.RecoverBehaviourResource(totRecoverVal);

        print("== Your BehaviourResource is Recovered (By Lead Stack) ==");
        print("** total RecoveryMount : " + totRecoverVal);

        // 스택 값 초기화
        playerWarriorController.Lead_StackCount = 0;
    }

    // 워로드 스타일 특성
    // [깃발]
    // 워로드의 위치에 설치. 
    // 전장에 8초간 유지되면서 RAD=5 영역에 있는
    // 우호적 오브젝트의 기본공격력을 10/20/30 고정 증가.
    public void WarlordStyleTalents_Flag()
    {

    }


    // Mage-Base

    // 기본 패시브
    // [천갑]
    public void MageBasePassiveSkill()
    {
        print("== Mage Base Passive On ==");

        isMage = true;

        // Mage 전용 근접무기로 변경 

        // 왼쪽에 있는 근접 무기를 해제.
        if (meleeContoller.equippedMeleeWeapon != null)
        {
            Destroy(meleeContoller.equippedMeleeWeapon.gameObject);
        }
        // 오른쪽에 있는 근접 무기를 해제.
        if (meleeContoller.equippedShield != null)
        {
            Destroy(meleeContoller.equippedShield.gameObject);
        }
        // 왼쪽에 있는 원거리 무기를 해제.
        if (rangedController.equippedRangedWeapon != null)
        {
            Destroy(rangedController.equippedRangedWeapon.gameObject);
        }

        // Mage 전용 근접무기 장착 (양손검)
        meleeContoller.EquipMageMeleeWeapon_TwoHandsSword();

        // 이동속도 10% 증가
        player_ThridPersoncharacter.MultiplySpeed(1.1f);
    }

    // 기본 액티브
    // [주문 불꽃 Spell blaze] 
    public void MageBaseActiveSkill_SpellBlaze()
    {

        // 잘못된 방식으로 호출 됐을 경우
        print("== Empty Skill is called == ");
        print("== 'MageBaseActiveSkill_SpellBlaze() ==");
        print("== Sine 2018.11.29 ==");

        // 왼쪽에 있는 무기를 해제.
        /*
        if (rangedController.equippedRangedWeapon != null)
        {
            Destroy(rangedController.equippedRangedWeapon.gameObject);
        }
        */

        /*
        // 오른쪽에 있는 무기를 해제.
        if (meleeContoller.equippedMeleeWeapon != null)
        {
            Destroy(meleeContoller.equippedMeleeWeapon.gameObject);
        }
        */

        // 주문 불꽃 사용을 허용.
        // playerMageController.isSpellBlaze_On = true;

    }

    // 기본 액티브
    // [구르기 Tumble]
    // 진행방향으로 단숨에 도약합니다. 
    // (500ms 만에 1초만에 뛰어서 갈수있는 거리만큼 이동, 선딜레이 x, 후딜레이 3000ms)
    public void MageBaseActiveSkill_Tumble()
    {

    }


    // Mage - SpellCaster

    // 스펠케스터 스타일 특성
    // [영혼 에너지]
    // 행동력보다 우선적으로 소모되는 자원인 
    // 영혼에너지가(100/200/300) 추가됩니다.
    public void SpellCasterStyleTalents_SpiritEnergy()
    {
        playerMageController.SpritEnergy_On = true;
        behaviourResource.InitSpiritEnergy();
    }

    // 스펠케스터 스타일 특성
    // [주문 불꽃]
    public void SpellCasterStyleTalents_SpellBlaze()
    {
        playerMageController.SpellBlaze_On = true;
    }

    // 스펠케스터 스타일 특성
    // [폭렬 불꽃]
    public void SpellCasterStyleTalents_ExplosionBlaze()
    {
        playerMageController.ExplosionBlaze_On = true;
    }

    // 스펠케스터 스타일 특성
    // [주문 가속]
    public void SpellCasterStyleTalents_SpellAcceleration()
    {
        // 시전 딜레이 1/3 축소
        playerMageController.spellBlazeCastingPreDelayTime = playerMageController.spellBlazeCastingPreDelayTime * (2 / 3f);
    }

    // 스펠케스터 핵심 특성
    // [룬캐스팅] [Rune Casting] 
    public void SpellCasterCoreTalentsSkill_RuneCasiting()
    {
        playerMageController.RuneCasting_On = true;

        // 룬 캐스팅용 빈 게임 오브젝트 장착.
        rangedController.EquipRuneCasting();

        // 자동 에임 시스템 허용.
        playerMageController.isAutoAimSystem_On = true;
    }

    // 스펠캐스터 핵심 특성
    // [순간이동] [Blink]
    public void SpellCasterCoreTalentsSkill_Blink()
    {
        playerMageController.Blink_On = true;
    }

    // 스펠케스터 전문가 특성
    // [고유: 핵심매개체] 
    public void SpellCasterExpertTalentsSkill_CoreMedium()
    {
        playerMageController.CoreMedium_On = true;

        // 구체 생성.
        summonedController.CreateSummonedObject();

        // 구체의 힐 적용.
        summonedController.currentSummonedObject.HealToAlliance();
    }

    // 스펠케스터 전문가 특성
    // [회오리불꽃] [Tornado Blaze] 
    public void SpellCasterExpertTalentsSkill_TornadoBlaze()
    {
        playerMageController.TornadoBlaze_On = true;
    }


    // Mage - SwordMage

    // 소드메이지 스타일 특성
    // [영혼에너지]
    public void SwordMageStyleTalents_SpiritEnergy()
    {
        playerMageController.SpritEnergy_On = true;
        behaviourResource.InitSpiritEnergy();
    }

    // 소드메이지 스타일 특성
    // [에너지 방벽]
    public void SwordMageStyleTalents_EnergyBarrier()
    {

    }


    // 특성 씬에서 반영된 것 inGameDB를 이용해 반영하는 메소드
    public void SetTalentsScenseDB()
    {
        // Archer
        
        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curArcherPlayer.archerTalents_DB.ArcherBaseTalents_On == true)
        {
            ArcherBasePassive();
            playerArcherController.isBoltOn = true;
        }

        // Archer-Scout
        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curArcherPlayer.scoutStyleTalents_DB.ScoutStyleTalents_PerfectEyeSight_On == true)
        {
            print("==Scout : SKILL PerfectEyeSight==");
            ScoutStyleTalentsSkill_PerfectEyeSight();
        }

        // Archer-Engineer
        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curArcherPlayer.engineerStyleTalents_DB.EngineerStyleTalents_ReloadLever_On == true)
        {
            print("==SKILL ReloadLever==");

            EngineerStyleTalentsSkill_ReloadLever_On();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curArcherPlayer.engineerCoreTalents_DB.EngineerCoreTalents_BulkBoltMagazie_On == true)
        {
            print("==SKILL BulkBoltMagazie==");

            EngineerCoreTalentsSkill_BulkBoltMagazie_On();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curArcherPlayer.engineerExpertTalents_DB.EngineerExpertTalents_SwordOffBowGun_On == true)
        {
            print("==SKILL SwordOffBowGun==");

            EngineerExpertTalentsSkill_SwordOffBowGun_On();
        }

        // Warrior
        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.warriorTalents_DB.WarriorBaseTalents_On == true)
        {
            WarriorBasePassive();
            WarriorBaseActiveSkill_Defense();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.guardianStyleTalents_DB.GuardianStyleTalents_KnockBack_On == true)
        {
            print("==SKILL Knock Back==");

            GuardianStyleTalentsSkill_KnockBack();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.guardianCoreTalents_DB.GuardianCoreTalents_Taunt_On == true)
        {
            print("==SKILL Taunt On==");

            playerWarriorController.Taunt_On = true;
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.guardianCoreTalents_DB.GuardianCoreTalents_Roar_On == true)
        {
            print("==SKILL Roar On==");

            playerWarriorController.Roar_On = true;
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.guardianExpertTalents_DB.GuardianExpertTalents_HammerSecurities_On == true)
        {
            print("==SKILL HammerSecurities");

            GuardianExpertTalentsSkill_HammerSecurities();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curWarriorPlayer.guardianExpertTalents_DB.GuardianExpertTalents_HeavySmash_On == true)
        {
            print("==SKILL Heavy Smash");

            GuardianExpertTalentsSkill_HeavySmash();
        }

        // Mage

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curMagePlayer.mageTalents_DB.MageBaseTalents_On == true)
        {
            MageBasePassiveSkill();

            // Mage 기본 특성으로 비활성화 
            // (2018.11.29)
            //MageBaseActiveSkill_SpellBlaze();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curMagePlayer.spellCasterCoreTalents_DB.SpellCasterCoreTalents_RuneCasiting_On == true)
        {
            print("==SKILL Rune Casting");

            SpellCasterCoreTalentsSkill_RuneCasiting();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curMagePlayer.spellCasterCoreTalents_DB.SpellCasterCoreTalents_Blink_On == true)
        {
            print("==SKILL Blink");

            SpellCasterCoreTalentsSkill_Blink();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curMagePlayer.spellCasterExpertTalents_DB.SpellCasterExpertTalents_CoreMedium_On == true)
        {
            print("==SKILL CoreMedium");

            SpellCasterExpertTalentsSkill_CoreMedium();
        }

        if (InGameTalentsDB.SingleTon_Talents.curPlayer.curMagePlayer.spellCasterExpertTalents_DB.SpellCasterExpertTalents_TornadoBlaze_On == true)
        {
            print("==SKILL TornadoBlaze");

            SpellCasterExpertTalentsSkill_TornadoBlaze();
        }

    }


    /* 각종 Routine 처리 */

    // 플레이어의 구르기 캐스팅 Rountine
    IEnumerator RollCasting(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        // 시전 후 쿨타임 적용.
        StartCoroutine(RollCool(PlayerRollCool));
    }

    // 플레이어의 휘두름 무기 쿨 Routine
    IEnumerator RollCool(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isRollReady = true;
    }

    // 플레이어의 휘두름 무기 캐스팅 Routine
    IEnumerator SwingCasting(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing/1000);

        // 시전 후 쿨타임 적용.
        StartCoroutine(SwingCool(PlayerOneMeleeSwingCool));

    }

    // 플레이어의 휘두름 무기 쿨 Routine
    IEnumerator SwingCool(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isSwing = true;
        behaviourResource.isBehaving = false;
        isMeleeAttack = false;
    }

    // 플레이어의 원거리 무기 캐스팅 Routine
    IEnumerator ShotCasting(float msBetweenShot)
    {
        yield return new WaitForSeconds(msBetweenShot / 1000);
        // 시전 후 쿨타임 적용.
        StartCoroutine(ShotCool(PlayerCrossBowCool));
    }

    // 플레이어의 휘두름 무기 쿨 Routine
    IEnumerator ShotCool(float msBetweenShot)
    {
        yield return new WaitForSeconds(msBetweenShot / 1000);

        isShot = true;
        behaviourResource.isBehaving = false;
    }

    // 플레이어의 쏘기 모션 후에 무기를 쏘는 Routine
    IEnumerator ReadyToShot(float msBetweenShot)
    {
        aimTime = Time.time;

        yield return new WaitForSeconds(msBetweenShot / 1000);
        keydownForShot = true;
    }

    // 플레이어 장전시 느려지는 Routine
    IEnumerator ReloadSlow(float msBetweenReload, float slowRate)
    {
        // 임시로 현 속도 저장.
        float tmpMoveSpeed = player_ThridPersoncharacter.getMoveSpeedMultiplier();
        float tmpAnimeSpeed = player_ThridPersoncharacter.getAnimeSpeedMultiplier();

        // 슬로우 적용.
        player_ThridPersoncharacter.MultiplySpeed((1-slowRate/100));

        yield return new WaitForSeconds(msBetweenReload / 1000);

        // 원래 속도로 회귀
        player_ThridPersoncharacter.SetSpeedMultiplier(tmpMoveSpeed * 1.05f, tmpAnimeSpeed * 1.05f);
    }

    // 플레이어의 방패 캐스팅 Routine
    IEnumerator DefenseCasting(float msBetweenSwing)
    {
        // 임시값에다가 현재 비율방어력 저장.
        float tmpRateDefensePercent = RateDefensePercent;

        RateDefensePercent = 70;

        yield return new WaitForSeconds(msBetweenSwing / 1000);

        RateDefensePercent = tmpRateDefensePercent;

        // 시전 후 쿨타임 적용 하지는 않음.
        // StartCoroutine(ShotCool(PlayerCrossBowCool));
    }

    // 플레이어의 방어를 일시적으로 증가하는 Routine
    IEnumerator DefenseTempChange_Inc(float msTime, float defenseVal)
    {
        float tmpRateDefensePercent = RateDefensePercent;

        // 일시적으로 현재 비율방어력을 상승
        this.RateDefensePercent = 50 + defenseVal;
        print("???");

        yield return new WaitForSeconds(msTime / 1000);

        // 다시 원상 복귀
        this.RateDefensePercent -= defenseVal;

        if(RateDefensePercent <= 0)
        {
            RateDefensePercent = initRateDefensePercent;
        }

        isDefenseVanguard_Cycle = true;

    }


    // 플레이어의 스킬 쿨 Rountine
    IEnumerator SkillCool(bool skillFlag, float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        skillFlag = true;
        behaviourResource.isBehaving = false;
    }

    // 플레이어의 위치 선점 스킬 쿨 Rountine
    IEnumerator SkillCool_PositionPreemption(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isPositionPreemptionCool = true;
    }

    // 플레이어의 돌진 스킬 쿨 Routine
    IEnumerator SkillCool_Rush(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isRushCool = true;
    }

    // 플레이어의 방패선봉대 스킬 쿨 Routine
    IEnumerator SkillCool_DefenseVanguard(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isDefenseVanguardCool = true;
    }

    // 플레이어의 넉백 스킬 쿨 Routine
    IEnumerator SkillCool_KnockBack(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isKnockBackCool = true;
    }

    // 플레이어의 깃발 스킬 쿨 Routine
    IEnumerator SkillCool_Flag(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isFlagCool = true;
    }

    // 플레이어의 주문 불꽃 스킬 쿨 Routine
    IEnumerator SkillCool_SpellBlaze(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        isSpellBlazeCool = true;
    }


    // Heavy Smash 스킬 회복 Routine
    IEnumerator RecoverHeavySmash(float msTime)
    {
        while (playerWarriorController.HeavySamsh_SkillCount < playerWarriorController.HeavySmash_MaxSkillCount)
        {
            yield return new WaitForSeconds(msTime/1000);

            IncHeavySmashSkillCount();
            // print(this.HeavySamsh_SkillCount);
        }

        if(playerWarriorController.HeavySamsh_SkillCount >= playerWarriorController.HeavySmash_MaxSkillCount)
        {
            // 사이클이 끝남.
            playerWarriorController.heavySmashOneCycle = true;
        }
    }

    // TornadoBlaze 스킬 이동 Routine
    IEnumerator TornadoBlazeMoveRoutine(float moveTime)
    {
        // 초기 시간.
        float startTime = Time.time;

        while(Time.time < startTime + moveTime)
        {
            // 토네이도 방향 단위 벡터에다 속도 곱하기.
            Vector3 tornadoVelocity = playerMageController.TornadoDirection.normalized * playerMageController.TornadoBlaze_moveSpeed;
            // 토네이도 방향대로 대로 움직이게 조절.
            playerMageController.TornadoBlazeTile.Move(tornadoVelocity);

            yield return null;
        }

        // 이동을 멈추고 라이프 타임 시간 만큼 지속
        playerMageController.TornadoBlazeTile.Move(Vector3.zero);
        playerMageController.TornadoBlazeTile.SetLifeTime(playerMageController.TornadoBlazeLifeTime);
    }

    IEnumerator RushRoutine(float totRushDst, float rushSpeed, float castingTime)
    {
        // 위치 정보
        Vector3 firstPos = this.transform.position;
        Vector3 nowPos = this.transform.position;
        float rushDst = 0;

        // 시간 정보
        float firstTime = Time.time;
        float limitTime = firstTime + castingTime;

        rushDst = (firstPos - nowPos).sqrMagnitude;

        while( (rushDst < totRushDst) && (Time.time < limitTime))
        {
            this.transform.Translate(Vector3.forward * rushSpeed * Time.deltaTime);

            nowPos = this.transform.position;

            rushDst = (firstPos - nowPos).sqrMagnitude;

            yield return null;
        }

    }

    IEnumerator RushInitialization(float initTime)
    {
        yield return new WaitForSeconds(initTime);

        meleeContoller.Assassin_RushAttack_Off();
    }


    // 주문 불꽃 사전 딜레이 시간
    IEnumerator SpellBlazePreDelay(float mstime)
    {
        Vector3 UIPosition = playerMageController.SpellBlazeTile.transform.position;
        Quaternion UIRotation = playerMageController.SpellBlazeTile.transform.rotation;

        //print("?");
        print("==Spell Blaze cast delay ==");
        print("** Delay Time : " + (mstime / 1000) + "s **");

        yield return new WaitForSeconds(mstime / 1000);

        print("==Spell Blaze cast ==");

        //print("!");

        // 필드 공격 생성
        FieldColliderBox spellBlazeHitBox = Instantiate(fieldColliderBox, UIPosition, UIRotation) as FieldColliderBox;
        spellBlazeHitBox.transform.localScale = new Vector3(4, 4, 8);

        // 필드 공격의 데미지 설정.
        spellBlazeHitBox.setDamage(playerMageController.spellBlazeDamage);
        // 필드 공격 라이프 타임 설정. (죽을 때는 데미지 적용이 안 되므로 spellBlazeApplyPerSec를 더해서 라이프 타임 설정.)
        spellBlazeHitBox.setLifeTime(playerMageController.spellBlazeCastingMsTime);
        // 필드 공격 DOT 적용.
        spellBlazeHitBox.setDamageOverTime(playerMageController.spellBlazeApplyPerSec);

        // 폭렬 불꽃 적용중이면, 특수 효과를 부여.
        if (playerMageController.ExplosionBlaze_On == true)
        {
            // 폭렬 불꽃 설정 (캐스팅 시간, 추가데미지, 추가데미지 주기)
            spellBlazeHitBox.setExplosionBlazeApplied(playerMageController.ExplosionBlaze_CastingTime, playerMageController.ExplosionBlaze_AdditionalDamage, playerMageController.ExplosionBlaze_AdditionalDamagePeriod);
            // 폭렬 불꽃 슬로우 설정 (슬로우 비율)
            spellBlazeHitBox.setSlowSpeed(playerMageController.ExplosionBlaze_SlowRate);
        }

        
        // UI 타일 삭제.
        Destroy(playerMageController.SpellBlazeTile.gameObject);


        //playerMageController.isSpellBlazeTileUI_Ready = false;
        //playerMageController.isOverCastingDelay = false;

        // 핵심 매개체 스킬이 On 이면, 공격 수행.
        if (playerMageController.CoreMedium_On == true)
        {
            summonedController.Shoot();
        }

        
    }


    [System.Serializable]
    public class PlayerArcherController
    {
        // Archer 특성

        // Archer 기본 액티브 Bolt
        public bool isBoltOn = false;
        // Archer 기본 액티브 Roll
        public bool isArcherRoll = false;

        // Archer - Scout 특성
        public bool PerfectEyeSight_On = false;

        public bool PenetratingWeakness_On = false;

        public bool DeepBreath_On = false;

        public bool PositionPreemption_On = false;

        // 위치선점 데미지 받지 않을 시각
        public float notAttackedTime_PositionPreemption = 2000f;
        // 위치선점 쿨 확인 변수
        public bool isPositionPreemptionCool = true;
        // 위치선점 쿨 타임
        public float PositionPreemptionCoolTime = 15000f;


        // Archer - Assassin 특성
        public bool Survivalism_On = false;

        public bool Rush_On = false;
        // 돌진 쿨타임
        public float rushCoolTime = 7000f;

        // 돌진해서 이동되는 거리
        public float totRushdst = 9f;
        // 돌진 스피드
        public float rushSpeed = 4f;
        // 돌진 시간
        public float rushTime = 2f;


        // Archer - Engineer 특성
        public bool BulkBoltMagazie_On;

        public bool SwordOffBowGun_On;

        public bool ReloadLever_On;

        
    }

    [System.Serializable]
    public class PlayerWarriorController
    {
        // 오브젝트 관련
        public Flag flagPrefab;
        public Flag flagGameObject;

        // Warrior 기본 액티브 defense
        public bool isDefense_On = false;
        // Warrior 기본 액티브 Roll
        public bool isWarriorRoll = false;

        // 방패 키가 눌러졌는지 확인하는 변수
        public bool keydownForShieldSkill = false;
        // 플레이어 방패 사용 시전 타임
        public float PlayerShieldTime;


        // Warrior - Berserker 특성

        public bool Coercion_On = false;

        // 위압 스택 쌓는 변수
        public int Coercion_StackCount = 0;
        // 위압 스택 최대 스택
        public int Coercion_StackCount_Max = 5;
        // 위압 스택 유지 시간
        public float Coercion_Stack_DurationTime = 5000f;

        public bool Rage_On = false;

        // 분노 스택 쌓는 변수
        public int Rage_StackCount = 0;
        // 분노 스택 최대 스택
        public int Rage_StackCount_Max = 5;
        // 분노 스택 추가 데미지
        public float Rage_AdditionalDamage = 3f;


        // Warrior - Guardian 특성

        public bool DefenseVanguard_On = false;
        // 방패 선봉대 쿨타임
        public float DefenseVanguard_CoolTime = 3000f;

        // 방패 선봉대 유지시간
        public float DefenseVanguard_DurationTime = 1000f;
        // 방패 선봉대 방어 값
        public float DefenseVanguard_DefenseVal = 15f;


        public bool DefenseReinforcement_On = false;


        // Guardian 스킬 KnocBack 허용 변수
        public bool KnockBack_On = false;
        // KnockBack 쿨타임
        public float KnockBack_CoolTime = 6000f;

        // Guardian 스킬 Taunt 허용 변수
        public bool Taunt_On = false;
        // Taunt 스킬의 적용 거리
        public float TauntDst = 8f;

        // Guardian 스킬 Roar 허용 변수
        public bool Roar_On = false;
        // Roar 스킬의 적용 거리
        public float RoarDst = 2f;

        // Guardian 스킬 HammerSecurities 허용 변수
        public bool HammerSecurities_On = false;

        // Guardian 스킬 HeavySmash 허용 변수
        public bool HeavySmash_On = false;
        // 현재 스킬 갯수
        public int HeavySamsh_SkillCount;
        // 최대 스킬 갯수
        public int HeavySmash_MaxSkillCount;

        // 스킬 갯수 회복 시간
        public float HeavySmash_RecoverTime;
        // 스킬 갯수 회복 판별 변수
        public bool isSmashRecvoer_On = true;
        // 스킬 사이클 판별 변수
        public bool heavySmashOneCycle = true;


        // Warrior - Warlord 특성

        public bool Lead_On = false;
        // 솔선 스택
        public int Lead_StackCount = 0;
        // 솔선 스택 행동력 회복 수치
        public float Lead_BehaviourRecoveryVal = 5;

        public bool Flag_On = false;
        // 깃발 쿨타임
        public float flag_CoolTime = 15000f;

        // 깃발 유지시간
        public float flag_Lifetime = 8000f;
        // 깃발 효과 거리
        public float flag_AppliedDst = 5f;
        // 깃발 추가 데미지
        public float flag_AdditionalDamage = 10f;

    }

    [System.Serializable]
    public class PlayerMageController
    {
        // UI 관련 오브젝트
        public GameUIObejct SpellBlaze_UI;
        public GameUIObejct TornadoBlazeTile_UI;
        public GameUIObejct SpellBlazeTile;
        public GameUIObejct TornadoBlazeTile;

        // Mage 기본 패시브 천갑
        public bool MageBasePassive_On = false;

        // Mage의 기본 데미지
        public float MageDefaultDamage = 100f;


        // Mage - SpellCaster 특성

        public bool SpritEnergy_On = false;


        public bool SpellBlaze_On = false;
        // 주문 불꽃 쿨타임
        public float SpellBlaze_CoolTime = 8000f;

        // Mage 기본 액티브에서 SpellCaster 스타일 특성으로 이동 (2018.11.29)
        // Mage 기본 액티브 주문 불꽃
        // public bool isSpellBlaze_On = false;

        // 주문 불꽃 UI 이동 속도.
        public float SpellBlazeUI_moveSpeed = 1f;
        // 주문 불꽃 UI 이동 준비 완료 판별 변수.
        public bool isSpellBlazeTileUI_Ready = false;

        // 주문 불꽃 시전 딜레이 완료 판별 변수
        // public bool isOverCastingDelay = false;

        // 주문 불꽃 캐스팅 판별
        public bool isSpellBlazeCasting = false;
        // 주문 불꽃 몇 초당 캐스팅 할 것인지 값.
        public float spellBlazeApplyPerSec = 1000f;

        // 주문 불꽃 시전 딜레이 시간
        public float spellBlazeCastingPreDelayTime = 500f;
        // 주문 불꽃 캐스팅 시간 (실제 죽는 시간에는 데미지 포함안 되므로 1초 더함.)
        public float spellBlazeCastingMsTime = 3000f;
        // 주문 불꽃의 데미지.
        public float spellBlazeDamage = 35f;

        // 주문 불꽃 키가 눌렸는지 확인하는 변수
        public bool keyDownForSpellBlaze = false;


        public bool ExplosionBlaze_On = false;
        // 폭렬 불꽃 이동감속 수치
        public float ExplosionBlaze_SlowRate = 40f;
        // 폭렬 불꽃 시전 시간
        public float ExplosionBlaze_CastingTime = 4000f;
        // 폭렬 불꽃 추가 데미지
        public float ExplosionBlaze_AdditionalDamage = 5f;
        // 폭렬 불꽃 추가 데미지 주기
        public float ExplosionBlaze_AdditionalDamagePeriod = 100f;


        public bool SpellAcceleration_On = false;


        // SpellCaster 스킬 Rune Casting 허용 변수
        public bool RuneCasting_On = false;
        // 자동 에임 시스템 판별 변수
        public bool isAutoAimSystem_On = false;

        // SpellCaster 스킬 Blink 허용 변수
        public bool Blink_On = false;
        // Blink해서 이동할 거리
        public float BlinkDst = 5f;

        // SpellCaster 스킬 CoreMedium 허용 변수
        public bool CoreMedium_On = false;

        // SpellCaster 스킬 TornadoBlaze 허용 변수
        public bool TornadoBlaze_On = false;
        // TornadoBlaze 이동 준비 완료 확인 변수
        public bool TornadoMoveReady = false;

        // TornadoBlaze 이동 속도
        public float TornadoBlaze_moveSpeed = 1f;
        // TornadoBlaze 방향
        public Vector3 TornadoDirection;
        // TornadoBlaze 이동 시간
        public float TornadoBlaze_MoveTime = 2f;
        // TornadoBlaze 라이프 타임
        public float TornadoBlazeLifeTime = 4f;

        // TornadoBlaze 데미지
        public float TornadoBlazeDamage = 5f;
        // TorandoBlaze 추가 데미지
        public float TornadoBlazeAdditionalDamage = 10f;

        // TornadoBlaze 시전 시간
        public float TornadoBlazeCastingMsTime = 6000f;
        // TornadoBlaze 몇 Ms 당 데미지를 적용할 건지
        public float TornadoBlazeDamagePerMs = 300f;


        // Mage - SwordMage 특성

        public bool EnergyBarrier_On = false;

    }

}
