using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;


[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(MeleeController))]
// StickController 가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(RangedWeaponController))]
// GunController가 객체에 무조건 따라 붙게 된다.
[RequireComponent(typeof(RangedWeapon))]
// Projectile이 객체에 무조건 따라 붙게 된다
[RequireComponent(typeof(Animator))]
// Animator 객체에 무조건 따라 붙게 된다
[RequireComponent(typeof(ThirdPersonCharacter))]
// ThirdPersonCharacter 객체에 무조건 따라 붙게 된다

public class Enemy : LivingEntity {

    /* 상태 관련 변수 */

    public enum State {Idle, Chasing, MeleeAttacking, ProjectAttacking, Taunt, KnockBack}
    State currentState;

    // 도발 상태
    bool isTaunt = false;

    // 폭렬마법 피격 상태인지
    [HideInInspector]
    public bool isAppliedExplosionSpell = false;


    /* 객체 관리 */

    // Animator와 character
    Animator animator;
    ThirdPersonCharacter character;

    // 길찾기 관리 NavMeshAgent
    NavMeshAgent pathFinder;
    Transform target;
    Transform player;
    Transform transporter;

    // 근접 무기 관련 controller
    MeleeController meleeContoller;
    // 원거리 무기 관련 controller
    RangedWeaponController gunController;


    // Transporter가 존재하는지 판별하는 변수.
    bool existTransporter = false;


    /* 거리 계산 변수 */

    // 적과 멀어지는 한계 거리. (target 지정에 사용)
    float targetDstThreshold = 8.0f;
    // 적과 멀어지는 한계 거리의 제곱값. (값 비교를 위해 지정)
    float powTargetDstTreshHold;

    // target 사이 거리를 계산하는 변수.
    float SqrDstToTarget;
    // 적과 플레이어 사이 거리를 계산하는 변수.
    float SqrDstToPlayer;
    // 적과 운송체 사이 거리를 계산하는 변수.
    float SqrDstToTransporter;

    // 특정 지점 도달하기 전에 미리 멈추도록 빼는 변수
    float StopDstToDest;

    // 적과 플레이어 사이 벡터.
    Vector3 VectorPlayerToEnemy;


    /* 이동 관련 변수 */

    // 이동속도
    public float moveSpeed = 5;
    // 추적 허용 변수
    public bool Chasing = false;


    /* 패턴 이동 관련 변수 */

    /*
    // 지점 이동 패턴 허용 변수
    public bool PointPatternMoving = false;
    // 순간이동 패턴 허용 변수
    public bool WarpPatternMoving = false;

    // 패턴 이동을 하도록 좌표를 저장하는 배열.
    public Coord[] movingPointArr;
    // 패턴 갱신 간격
    public float patternRefreshRate = 3;
    // 좌표의 끝을 향해 가는지 판별하는 변수
    bool forWardPoint = true;
    // 좌표의 처음을 향해 가는지 판별하는 변수
    bool backWardPoint = false;
    */


    /* 워프 이동 관련 변수 */

    /*
    // 상황별 순간이동 허용 변수
    public bool WarpBySituation = false;

    // 워프 가능한지 판별하는 변수.
    bool isWarpAvailable = true;
    // 워프 사용 쿨타임.
    public float WarpCoolTime = 5;
    // 워프 허용 거리.
    public float warpAvailableDst = 4.0f;
    */

    /* 수학 이동 관련 변수 */
    /*
    // 수학적 함수 이동 허용 변수
    public bool MathFuncMoving = false;

    // 수학 자취이동할 함수 체크
    public MathFuctionTrace mathFunctionChecker;
    // 수학적 이동을 위해 입력받는 x,y 좌표
    public Coord mathTraceCoord;

    // 수학 이동할 함수 갱신 주기
    // Update 메소드 사용으로 현재는 미사용.
    // public float mathTraceRefreshRate = 1;

    // 원 이동할 때 속도.
    public float circleMoveSpeed;
    */


    /* 공격 모드 판별 변수 */

    // 적 원거리 공격 사용 판별 변수
    public bool enemyUseMelee = false;
    // 적 원거리 공격 사용 판별 변수
    public bool enemyUseRanged = false;
    // 거리에 따라서, 근거리 원거리 전환 제어 변수
    public bool flexDstAttack = false;


    /* 무기 변환 모드 관련 변수 */

    // 유동 공격 적용을 위한 거리 계산 변수.
    float FlexSqrDstToPlayer;
    // 유동 공격 적용시 근접 공격 범위
    public float MeleeAtkDst = 5;

    // 첫번째 무기 변환인지 판별하는 변수
    bool InitialTransforming = true;
    // 근접 무기 공격 변환 체크 플래그 변수
    int MeleeTransforming = 1;
    // 원거리 무기 공격 변환 체크플래그 변수
    int GunTransforming = 1;


    /* 근접 공격 관련 변수 */

    // 근접 무기 사용 중인지 판별하는 변수
    public bool isSwing = true;
    // 적 근접 무기 한계 거리
    float meleeDstThreshold = 2.0f;

    // 다음 공격 시각.
    float nextAttackTime;

    // 다음 공격 까지 시간.
    public float timeBetweensMelee = 1000;

    // 적 한 손 근접 무기 휘두름 사용 시전 타임
    public float EnemyOneMeleeSwingCasitng;
    // 적 한 손 근접 무기 휘두름 쿨타임
    public float EnemyOneMeleeSwingCool;


    /* 원거리 무기 공격 관련 변수 */

    // 원거리 무기로 투사체가 날라갔는지 판별하는 변수
    public bool isShooting = true;
    // 원거리 무기로 투사체를 한 번 쏜 횟수.
    int oneShotCount = 0;
    // 타임 초기화 판단 변수.
    int shotTimeDefault = 0;

    // 다음 공격 시각.
    float nextShootTime;
    // 다음 공격 까지 시간.
    public float timeBetweensShoot = 1000;
    // 적 원거리 무기 사용 쿨타임
    public float shotCoolTime;


    /* 중첩 스택 관련 변수 */
    public int TornadoBlaze_OverlapCount = 0;



    protected override void Start ()
    {
        // 상위 클래스 LivingEntity Start() 호출
        base.Start();

        // 기본은 추적상태.
        currentState = State.Chasing;

        /* 컴포넌트 획득 */

        // 애니메이터 지정.
        animator = GetComponent<Animator>();
        // 캐릭터 지정.
        character = GetComponent<ThirdPersonCharacter>();

        // 컨트롤러 지정.
        meleeContoller = GetComponent<MeleeController>();
        gunController = GetComponent<RangedWeaponController>();
        pathFinder = GetComponent<NavMeshAgent>();


        /* 오브젝트 찾기 */

        // Player 객체 찾기.
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Transporter객체 찾기.
        if (GameObject.FindGameObjectWithTag("Transporter") != null)
        {
            transporter = GameObject.FindGameObjectWithTag("Transporter").transform;
        }
        
        // 운송체가 있으면 판별변수도 true로 만들어준다.
        if (transporter != null)
        {
            existTransporter = true;
        }


        /* 타겟 설정 및 거리 계산 */

        // target으로 Transporter객체 지정.
        target = transporter;

        pathFinder.updateRotation = false;
        pathFinder.updatePosition = true;

        // 빈번한 사용으로, 아예 tartget 거리 한계를 제곱한 것을 따로 지정.
        powTargetDstTreshHold = Mathf.Pow(targetDstThreshold, 2);


        /* 공격 설정 */

        // 공격방식 지정.
        if (enemyUseMelee == true)
        {
            meleeContoller.UseMelee = true;
        }
        if (enemyUseRanged == true)
        {
            gunController.UseRanged = true;
        }

        isSwing = true;


        /* 이동 설정 */

        // 이동방식 지정.
        if (Chasing == true)
        {
            // 추적 메소드 호출
            StartCoroutine(UpdatePath());
        }

        /*
        if (PointPatternMoving == true)
        {
            // 지점 간 패턴 이동 메소드 호출
            StartCoroutine(MovePattern());
        }

        if (WarpPatternMoving == true)
        {
            // 순간이동 패턴 이동 메소드 호출
            StartCoroutine(WarpPattern());
        }

        // Coroutine 대신, Update로 처리.
        if (MathFuncMoving == true)
        {
            // 수학적 이동 메소드 호출
            //StartCoroutine(MathPattern());
        }
        */

        // 이동 관련 요소 초기화
        // isSpeedUp = true;
        // isSpeedDown = false;
    }

    void Update()
    {
        /* 플레이어의 상태를 파악 */

        // 위압 스택에 따른 상태
        if(player.GetComponent<Player>().coercion_On == true)
        {
            this.RateDefensePercent = this.initRateDefensePercent - player.GetComponent<Player>().playerWarriorController.Coercion_StackCount * 3;
        }

        /* 적 - 다른 오브젝트들 거리 계산 */
        // 플레이어와 Transporter 거리 계속해서 계산.
        SqrDstToPlayer = (player.position - transform.position).sqrMagnitude;
        // 플레이어와 적과의 거리 벡터.
        VectorPlayerToEnemy = player.position - transform.position;

        // 운송체 존재하면, 거리 계산
        if (existTransporter == true)
        {
            SqrDstToTransporter = (transporter.position - transform.position).sqrMagnitude;
            //print(SqrDstToTransporter);
        }

        // 플레이어 - 운송체 와의 거리에 따른 타겟 지정.
        // 도발 상태가 아닐때 타겟 지정.
        if (existTransporter == true && isTaunt == false)
        {
            // 플레이어, 운송체 모두 특정거리 안에 있을 경우
            if (SqrDstToTransporter < powTargetDstTreshHold && SqrDstToPlayer < powTargetDstTreshHold)
            {
                target = player;
            }
            else if (SqrDstToTransporter > powTargetDstTreshHold && FlexSqrDstToPlayer < powTargetDstTreshHold)
            {
                target = player;
            }
            else
            {
                target = transporter;
            }
        }

        if(existTransporter == false)
        {
            target = player;
        }


        /* 적 이동 관련 처리 */

        // 2018.10.05 부로 현재는 사용하지 않음.

        // 적 이동 관련 속도를 처리함
        /*
        if (applySuddenChangeSpeed == true || applyConstantAccer == true || applyDeceleration == true || applyChangeAccer == true || resetSpeed == true)
        {
            ControlSpeed();
        }
        */

        // 지점간 패턴 이동이 적용될 때, 타겟을 향하도록함.
        /*
        if (PointPatternMoving == true)
        {
            if (target != null)
            {
                // 목표물 향해 봄.
                transform.LookAt(target.position);
            }
        }

        // 패턴별 워프가 적용될 때, 타겟을 향하도록 함.
        if (WarpPatternMoving == true)
        {
            if (target != null)
            {
                // 목표물 향해 봄.
                transform.LookAt(target.position);
            }
        }

        // 상황별 워프가 적용될 때, 타겟을 향하도록함.
        if (WarpBySituation == true)
        {
            if (target != null)
            {
                // 목표물 향해 봄.
                transform.LookAt(target.position);
            }
        }

        // 상황에 따라 워프.
        if (target != null)
        {
            if (WarpBySituation == true)
            {
                // 플레이어 객체와의 거리
                FlexSqrDstToPlayer = (target.position - transform.position).sqrMagnitude;

                if ( (FlexSqrDstToPlayer > Mathf.Pow(warpAvailableDst, 2)) && ( isWarpAvailable == true))
                {
                    isWarpAvailable = false;
                    StartCoroutine(WarpSkill());
                }
            }
        }

        // 수학적 이동.
        if (target != null)
        {
            if (MathFuncMoving == true)
            {
                if (mathFunctionChecker.CircleTrace == true)
                {
                    transform.RotateAround(Vector3.zero, Vector3.up, circleMoveSpeed * Time.deltaTime);
                }
            }
        }
        */

        // 근접 공격 시전 시간이 되면.
        if (target != null)
        {
            // 근접 무기 사용시.
            if (enemyUseMelee == true)
            {
                AttackWithMeleeWeapon();
            }
        }

        // 원거리 무기 공격 시전 시간이 되면.
        if (target != null)
        {
            // 원거리 공격 사용시,
            if (enemyUseRanged == true)
            {
                AttackWithRangedWeapon();
            }
        }

        // 근거리 - 원거리 자동변환 공격.
        if (target != null)
        {
            // 해당 변수가 참이면,
            if (flexDstAttack == true)
            {
                // 플레이어-적 위치 관련 변수
                FlexSqrDstToPlayer = (target.position - transform.position).sqrMagnitude;

                if (FlexSqrDstToPlayer < Mathf.Pow(MeleeAtkDst, 2) )
                {
                    // 근거리 공격 범위 안에 들면, 근거리로 전환
                    if (MeleeTransforming == 1)
                    {
                        // 전환 첫번째일시, 무기 교체하는 부분.
                        //print("MeleeAttack");
                        gunController.DestroyGun();
                        meleeContoller.EquiTransStick();

                        if (InitialTransforming == true)
                        {
                            // 처음하는 변환일 경우
                            MeleeTransforming--;
                            InitialTransforming = false;
                        }
                        else
                        {
                            // 그 외의 경우는, 각각 플래그 값을 0과 1로 전환하면서 상태를 바꾼다.
                            MeleeTransforming--;
                            GunTransforming++;
                        }
                    }

                    AttackWithMeleeWeapon();

                    // 원거리 무기로 전환 될 때를 대비하여 원거리 관련 모든 값을 초기화.
                    isShooting = true;
                    oneShotCount = 0;
                    shotTimeDefault = 0;
                    nextShootTime = 0;
                }
                else if (FlexSqrDstToPlayer >= Mathf.Pow(MeleeAtkDst, 2))
                {
                    // 원거리 범위 안에 들면 원거리로 전환.
                    if (GunTransforming == 1)
                    {
                        //print("GunAttack");
                        meleeContoller.DestroyStick();
                        gunController.EquiTransGun();

                        if (InitialTransforming == true)
                        {
                            GunTransforming--;
                            InitialTransforming = false;
                        }
                        else
                        {
                            MeleeTransforming++;
                            GunTransforming--;
                        }
                    }

                    AttackWithRangedWeapon();

                    // 근거리 무기로 전환 될 때를 대비하여 근거리 관련 모든 값을 초기화.
                    isSwing = true;
                    nextAttackTime = 0;
                }
            }

        }

    }


    /* 중첩 상태를 확인하는 메소드들  */
    // TornadoBlaze 중첩을 초기화하는 메소드
    public void InitCount_TornadoBlaze()
    {
        TornadoBlaze_OverlapCount = 0;
    }

    // TornadoBlaze 중첩 확인 메소드
    public int getCount_TornadoBlaze()
    {
        return TornadoBlaze_OverlapCount;
    }

    // TornadoBlaze 중첩상태 증가시키는 메소드
    public void IncCount_TornadoBlaze()
    {
        TornadoBlaze_OverlapCount++;
    }


    /* 이동 관련 메소드 */
    // 정지 상태로 만드는 메소드
    public void setStopMove()
    {
        character.Move(Vector3.zero, false, false);
    }

    // 적과 플레이어 사이 거리를 구하는 메소드
    public float getDstToPlayer()
    {
        return SqrDstToPlayer;
    }

    // 적과 플레이어 사이 벡터를 구하는 메소드
    public Vector3 getVectorToPlayer()
    {
        return VectorPlayerToEnemy;
    }

    // 타겟을 플레이어로 바꾸는 메소드
    public void setTragetToPlayer()
    {
        if(player != null)
        {
            target = player;
        }
    }


    /* 상태 지정 관련 메소드 */

    // 현재 상태를 불러오는 메소드
    public State getCurrentState()
    {
        return currentState;
    }

    // 상태를 도발로 바꾸는 메소드
    public void setTaunt(bool orNot)
    {
        isTaunt = orNot;
    }

    // 상태를 넉백상태로 바꾸는 메소드
    public void setStateToKnockBack()
    {
        currentState = State.KnockBack;
    }

    // 상태를 추적상태로 바꾸는 메소드
    public void  setStateToChasing()
    {
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }


    /* 공격 관련 제어 */

    // 근거리 공격 명령 메소드
    public void AttackWithMeleeWeapon()
    {
        if(currentState != State.KnockBack)
        {
            if (Time.time > nextAttackTime)
            {

                // 공격 target 지정
                if (SqrDstToPlayer < Mathf.Pow(meleeDstThreshold, 2))
                {
                    target = player;
                }
                if (SqrDstToTransporter < Mathf.Pow(meleeDstThreshold, 5))
                {
                    if (existTransporter == true && isTaunt == false)
                    {
                        target = transporter;
                    }
                }

                // target-적 위치 관련 변수
                SqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                //print(isSwing);
                //print(SqrDstToTarget);
                //print(SqrDstToTarget < Mathf.Pow(meleeDstThreshold, 2));

                // 운송체가 공격 타겟이면,
                if (target == transporter)
                {
                    if ((SqrDstToTarget < Mathf.Pow(meleeDstThreshold, 3)) && isSwing == true && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                    {
                        isSwing = false;

                        nextAttackTime = Time.time + timeBetweensMelee / 1000;

                        // 추적 기능 끄기.
                        pathFinder.enabled = false;
                        currentState = State.MeleeAttacking;

                        // 근접 무기 시전 타임 적용.
                        // 쿨 타임은 시전 타임이 끝나면 자동으로 연결.
                        StartCoroutine(SwingCasting(EnemyOneMeleeSwingCasitng));

                        // 근접 무기를 휘두른다.
                        animator.SetTrigger("Attack");
                        // HitCount를 증가시켜 피격 판정한다.
                        meleeContoller.IncHitCount();

                        // 추적 기능 켜기.
                        pathFinder.enabled = true;
                        currentState = State.Chasing;

                    }

                }
                else
                {
                    // 공격 거리 내에 들었는지 판별
                    if ((SqrDstToTarget < Mathf.Pow(meleeDstThreshold, 2)) && isSwing == true && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
                    {
                        isSwing = false;

                        nextAttackTime = Time.time + timeBetweensMelee / 1000;

                        // 추적 기능 끄기.
                        pathFinder.enabled = false;
                        currentState = State.MeleeAttacking;


                        // 근접 무기 시전 타임 적용.
                        // 쿨 타임은 시전 타임이 끝나면 자동으로 연결.
                        StartCoroutine(SwingCasting(EnemyOneMeleeSwingCasitng));

                        // 근접 무기를 휘두른다.
                        animator.SetTrigger("Attack");
                        // HitCount를 증가시켜 피격 판정한다.
                        meleeContoller.IncHitCount();

                        // 추적 기능 켜기.
                        pathFinder.enabled = true;
                        currentState = State.Chasing;

                    }
                }

            }
        }
    }

    // 원거리 공격 명력 메소드
    public void AttackWithRangedWeapon()
    {
        // 초기 시간
        if (nextShootTime == 0)
        {
            nextShootTime = Time.time + timeBetweensShoot / 1000;
        }

        // 공격 시전 시간
        if (Time.time < nextShootTime && isShooting == true && oneShotCount == 0)
        {
            oneShotCount++;
            isShooting = false;

            // 원거리 무기 공격을 한다.
            StartCoroutine(ShotTime());
        }

        // 쿨타임 및 다음 공격 시전까지 시간
        if (Time.time >= nextShootTime && shotTimeDefault == 0)
        {
            // 오직 초기화를 한 번 적용하기 위하여.
            shotTimeDefault++;
            // 공격 후 캐스팅 적용.
            StartCoroutine(ShootCasting(shotCoolTime));
        }
    }


    // 적의 원거리 무기 발사.
    IEnumerator ShotTime()
    {
        // 한 발 총알을 쏘고 났을 때의 시간.
        float OneShotTime = gunController.equippedRangedWeapon.msBetweenShot;
        gunController.Shoot();
        //print(OneShotTime);

        yield return new WaitForSeconds(OneShotTime / 1000);

        isShooting = true;
        oneShotCount = 0;
    }


    // 플레이어 추적시 경로 업데이트 해주는 메소드
    IEnumerator UpdatePath()
    {
        // 추적 갱신 간격
        // float RefreshRate = 1;

        // 갱신 간격 만큼 루프 반복
        while (target != null)
        {
            if(currentState == State.Chasing)
            {
                // 목적지 도착 전에, 멈출 거리를 미리 설정
                StopDstToDest = meleeDstThreshold - 0.1f;

                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * StopDstToDest;

                //Vector3 targetPosition = target.position;

                // 객체가 죽어서 파괴되기전 까지만, 추적.
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);

                    // 자연스럽게 움직이기 위해, character를 움직임.
                    if (pathFinder.remainingDistance > pathFinder.stoppingDistance)
                    { 
                        character.Move(pathFinder.desiredVelocity, false, false);
                    }
                    else
                    { 
                        character.Move(Vector3.zero, false, false);
                    }
                }
            }

            if(currentState == State.KnockBack)
            {
                pathFinder.enabled = false;
                character.Move(Vector3.zero, false, false);
            }

            // 2018.08.19 움직임의 자연스러움을 위해, 갱신 주기형 WaitsForSeconds를 없앰.
            // 현재 Time.deltaTime, 즉 한 프레임 속도로 추적을 갱신함. (사실상 Update() 메소드 처리와 동일)
            //yield return new WaitForSeconds(RefreshRate / (moveSpeed/5));

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    // 지점간 패턴 이동 경로 업데이트 해주는 메소드
    /*
    IEnumerator MovePattern()
    {
        int i = 0;

        // 갱신 간격 만큼 루프 반복
        while (target != null)
        {
            if (currentState == State.Chasing)
            {
                if (forWardPoint == true)
                {
                    print("Forward" + forWardPoint);
                    print("Backward" + backWardPoint);
                    Vector3 targetPosition = new Vector3((float)movingPointArr[i].x, 0, (float)movingPointArr[i].y);

                    // 객체가 죽어서 파괴되기전 까지만, 추적.
                    if (!dead)
                    {
                        pathFinder.SetDestination(targetPosition);
                    }

                    i++;

                    if (i == movingPointArr.Length)
                    {
                        i = movingPointArr.Length - 1;
                        forWardPoint = false;
                        backWardPoint = true;
                    }
                }
                else if (backWardPoint == true)
                {
                    print("Forward" + forWardPoint);
                    print("Backward" + backWardPoint);
                    Vector3 targetPosition = new Vector3((float)movingPointArr[i].x, 0, (float)movingPointArr[i].y);


                    // 객체가 죽어서 파괴되기전 까지만, 추적.
                    if (!dead)
                    {
                        pathFinder.SetDestination(targetPosition);
                    }

                    i--;

                    if (i == 0)
                    {
                        forWardPoint = true;
                        backWardPoint = false;
                    }

                }
            }
            yield return new WaitForSeconds(patternRefreshRate);
        }
    }

    // 지점간 워프 패턴 이동 경로 업데이트 해주는 메소드
    IEnumerator WarpPattern()
    {
        int i = 0;

        // 갱신 간격 만큼 루프 반복
        while (target != null)
        {
            if (currentState == State.Chasing)
            {
                if (forWardPoint == true)
                {
                    print("Forward" + forWardPoint);
                    print("Backward" + backWardPoint);

                    // 객체가 죽어서 파괴되기전 까지만, 추적.
                    if (!dead)
                    {
                        transform.position = new Vector3(movingPointArr[i].x, 0, movingPointArr[i].y);
                    }

                    i++;

                    if (i == movingPointArr.Length)
                    {
                        i = movingPointArr.Length - 1;
                        forWardPoint = false;
                        backWardPoint = true;
                    }
                }
                else if (backWardPoint == true)
                {
                    print("Forward" + forWardPoint);
                    print("Backward" + backWardPoint);

                    // 객체가 죽어서 파괴되기전 까지만, 추적.
                    if (!dead)
                    {
                        transform.position = new Vector3(movingPointArr[i].x, 0, movingPointArr[i].y);
                    }

                    i--;

                    if (i == 0)
                    {
                        forWardPoint = true;
                        backWardPoint = false;
                    }

                }
            }
            yield return new WaitForSeconds(patternRefreshRate);
        }
    }

    // 상황별 워프 이동
    IEnumerator WarpSkill()
    {
        // 객체가 죽어서 파괴되기전 까지만, 워프.
        if (!dead)
        {
            // 곧바로 목표물 쪽으로 이동.
            transform.position = new Vector3(target.position.x, 0, target.position.z + 1);
        }

        yield return new WaitForSeconds(WarpCoolTime);
        isWarpAvailable = true;
    }

    // 수학적 함수 자취 이동.
    // 현재 Update 메소드 처리로 인해 미 사용.
    IEnumerator MathPattern()
    {
        circleMoveSpeed = moveSpeed;

        while (target != null)
        {
            if (mathFunctionChecker.CircleTrace == true)
            {
                print("rotate");
                transform.RotateAround(Vector3.zero, Vector3.up, circleMoveSpeed * Time.deltaTime);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    */


    // 적의 한 손 근접 무기 휘두름 캐스팅 Routine
    IEnumerator SwingCasting(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);

        // 시전 후 쿨타임 적용.
        StartCoroutine(SwingCool(EnemyOneMeleeSwingCool));

    }

    // 적의 한 손 근접 무기 쿨타임 Routine
    IEnumerator SwingCool(float msBetweenSwing)
    {
        yield return new WaitForSeconds(msBetweenSwing / 1000);
        isSwing = true;
    }

    // 적의 원거리 무기 캐스팅 속도
    IEnumerator ShootCasting(float msBetweenShoot)
    {
        //print(msBetweenShoot);
        yield return new WaitForSeconds(msBetweenShoot / 1000);

        // 다음 시전 시간까지 변수 초기화
        shotTimeDefault = 0;
        nextShootTime = Time.time + timeBetweensShoot / 1000;
    }

    // 여러 수학적 함수 자취 판별을 위한 구조체(Struct)
    [System.Serializable]
    public struct MathFuctionTrace
    {
        public bool CircleTrace;

        public MathFuctionTrace(bool _CircleTrace)
        {
            CircleTrace = _CircleTrace;
        }
    }
}
