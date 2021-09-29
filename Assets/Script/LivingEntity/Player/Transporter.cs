using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeleeWeapon))]
[RequireComponent(typeof(Projectile))]

public class Transporter : LivingEntity {

    /* 상태 관련 변수 */

    public enum State {Idle, Moving, Attacked}
    State currentState;

    /* 객체 관리 */

    // 길찾기 관리 NavMeshAgent
    NavMeshAgent pathFinder;
    // 근접 무기
    MeleeWeapon EnemyStick;
    // 원거리 무기
    Projectile EnemyProjectile;

    // 충돌에 영향을 받는 오브젝트를 생성
    Rigidbody myRigidbody;


    /* 이동 관련 변수 */

    // 처음 속도
    public float startSpeed = 5;
    // 이동속도
    public float moveSpeed = 5;
    // 이동 갱신 주기
    float MoveRefreshRate = 1;
    // 도착했는지 판별하는 변수
    bool isDestination = false;

    // 패턴 이동을 하도록 좌표를 저장하는 배열.
    public Coord[] movingPointArr;


    /* 피격 및 공격 상태 관련 변수 */

    // 특정시간 동안 체력 판정.
    float tmpHealth = 0;

    // 데미지
    float damage;
    // 데미지 받고 있는지
    bool isDamaged = false;

    // 데미지 판정 사이클이 얼마나 돌았는지.
    int cycleDamagedCount = 0;
    // 데미지 판정 사이클 변수
    bool cycleDamaged = false;

    // 데미지를 받지 않고 있을 안정시간
    public float SafeTime = 5000;
    // 다음 데미지 받기까지 걸리는 시간
    float nextAttackedTime;

    // 피격 되어 속도가 느려지는 정도
    public float attackedSlowSpeed = 0.5f;
    // 피격된 후, 다시 속도가 빨라지는 정도
    public float attackeUpSpeed = 0.5f;
    // 몇 초당 속도가 느려질지.
    public float attackedSlowRate = 1;

    // 느려지는 사이클.
    public bool cycleSlow = true;
    // 느려지는 사이클 한 주기.
    public bool OnceCycleSlow = true;

    // 빨라지는 사이클.
    public bool cycleFast = true;

    // 속도 증가 판별 변수.
    public bool isFast = false;
    // 속도 감소 판별 변수.
    public bool isSlow = false;



    protected override void Start ()
    {
        // 상위 부모 계층 Start() 호출
        base.Start();

        // 컴포넌트 획득
        myRigidbody = GetComponent<Rigidbody>();
        pathFinder = GetComponent<NavMeshAgent>();
        EnemyStick = GetComponent<MeleeWeapon>();
        EnemyProjectile = GetComponent<Projectile>();

        // 초기 상태 지정
        currentState = State.Moving;

        cycleSlow = true;
        cycleFast = true;

        OnceCycleSlow = true;

        // 이동 루틴
        StartCoroutine(MoveToDest());
	}


    /* 움직임 관련 메소드 */

    // 지점간 패턴 이동 경로 업데이트 해주는 메소드
    IEnumerator MoveToDest()
    {
        int i = 0;

        // 갱신 간격 만큼 루프 반복
        while (isDestination == false)
        {
            if (currentState == State.Moving)
            {
                    Vector3 targetPosition = new Vector3((float)movingPointArr[i].x, 1, (float)movingPointArr[i].y);

                    // 객체가 죽어서 파괴되기전 까지만, 추적.
                    if (!dead)
                    {
                        pathFinder.SetDestination(targetPosition);

                        // 매번 NavmeshAgent의 속도도 변경해준다.
                        pathFinder.speed = moveSpeed;
                  
                    }

                    //print(targetPosition);
                    //print(transform.position);
                    //print(i);

                    //print(transform.position.x);
                    //print(transform.position.z);    

                    // 목표지점에 도달했으면, 다음 목표지점으로 지정.
                    if ((transform.position.x == targetPosition.x) && (transform.position.z == targetPosition.z))
                    {
                        i++;
                    }

                    // 최종 목적지 도착.
                    if (i == movingPointArr.Length)
                    {
                        isDestination = true;
                    }
                    
                    // 일정시간 동안 데미지를 받지 않으면, 다시 속도를 올린다.
                    if (moveSpeed < startSpeed)
                    {
                        // 데미지를 받았는지 판정한다.
                        NotAttackedAnything();
                    }

            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    // 데미지를 일정 시간 동안 받지 않는지 판단하는 메소드
    public void NotAttackedAnything()
    {

        if (Time.time >= nextAttackedTime)
        {
            //print("!!!");

            // 데미지를 받지 않았으면 속도를 다시 올림.
            if (isDamaged == false && cycleDamagedCount > 0)
            {
                //print("YES! NO DAMAGE!");

                isFast = true;

                // 가속이 중첩되지 않도록, 한 주기씩 Routine 한다.
                if (cycleFast == true)
                {
                    //print("in" + cycleFast);
                    StartCoroutine(UpSpeed());
                }

            }

            // 다음 데미지 판정 안 받을 때 까지 버틸 시간 설정.
            nextAttackedTime = Time.time + SafeTime / 1000;

            isDamaged = false;
            tmpHealth = healthPoint;

            cycleDamagedCount++;

        }
            
        if (Time.time < nextAttackedTime)
        {

            /*
            print("NEXT" + nextAttackedTime);
            print("TIME" + Time.time);

            print(isDamaged);

            print("???");
            */

            // 체력이 줄어들었다면 (데미지등을 받아서)
            if (healthPoint < tmpHealth)
            {
                isDamaged = true;

                // 속도 회복 시스템 끔.
                isFast = false;
                cycleFast = true;
            }
        }

    }



    /*
    // 지점간 패턴 이동 경로 업데이트 해주는 메소드
    IEnumerator MovePattern()
    {
        int i = 0;

        // 움직일 벡터
        Vector3 moveVector;

        // 목표로 하는 벡터
        Vector3 destVector = new Vector3(movingPointArr[0].x, 0, movingPointArr[0].y);



        // 갱신 간격 만큼 루프 반복
        if (currentState == State.Moving)
        {
            while (isDestination == false)
            {

                moveVector = (destVector - transform.position).normalized;

                Vector3 moveVelocity = moveVector;

                myRigidbody.MovePosition(moveVelocity);

                print(moveVelocity);
                print(destVector);
                print(transform.position);
                print(i);

                if (moveVector == Vector3.zero)
                {
                    destVector = new Vector3(movingPointArr[i].x, 0, movingPointArr[i].y);
                    i++;
                }

                if (i == movingPointArr.Length)
                {
                    isDestination = true;
                };

                yield return new WaitForSeconds(MoveRefreshRate / (moveSpeed / startSpeed));
            }
        }
        
    
    }
    */


    /* 피격관련 메소드 */
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerStick" || other.tag == "EnemyBullet")
        {
            Idamageable damageableObject = other.GetComponent<Idamageable>();
            Stick damageStick = other.GetComponent<Stick>();
            //SkinMaterial = other.GetComponent<Renderer>().material;

            // 근거리 무기 일 경우 데미지 지정.
            if (other.tag == "PlayerStick")
            {
                print("Transporter Hit by stick");
                
            }

            // 원거리 무기 일 경우 데미지 지정.
            if (other.tag == "EnemyBullet")
            {
                Debug.Log("Transporter Hit!");
                damage = EnemyProjectile.damage;
                damageableObject.TakeDamage(damage);
            }
        }
    }
    */


    // 공격 받았을 때 이동속도를 감속하는 메소드
    public void AttackedSlow()
    {
        StartCoroutine(SlowSpeed());
    }

    // 공격 받았을 때 이동속도를 다시 가속하는 메소드
    public void AttackSpeedRecovery()
    {
        StartCoroutine(UpSpeed());
    }

    // 속도 느려지는 Coroutine
    IEnumerator SlowSpeed()
    {
        while (moveSpeed > 0 && OnceCycleSlow == true && isSlow == true)
        {
            moveSpeed -= attackedSlowSpeed;

            cycleSlow = false;

            // 설정 속도가 0보다 더 느려지는 것을 방지.
            // 만약 속도가 0이면 Coroutine 한 주기를 종료한다.
            if (moveSpeed < 1)
            {

                moveSpeed = 0;

                cycleSlow = true;
                OnceCycleSlow = false;

                isSlow = false;
            }

            print("Slow Speed" + moveSpeed);

            yield return new WaitForSeconds(attackedSlowRate);
        }
        
    }

    // 속도가 빨라지는 Coroutine
    IEnumerator UpSpeed()
    {
        while (moveSpeed >= 0 && isFast == true)
        {
            moveSpeed += attackeUpSpeed;

            cycleFast = false;

            // 초기 설정 속도보다 더 빨라지는 것을 방지.
            if (moveSpeed >= startSpeed)
            {
                moveSpeed = startSpeed;

                isFast = false;
                cycleFast = true;
            }

            print("Up Speed" + moveSpeed);

            yield return new WaitForSeconds(attackedSlowRate);
        }

    }
}
