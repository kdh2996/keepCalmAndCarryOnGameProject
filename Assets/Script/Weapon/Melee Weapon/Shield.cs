using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Enemy[]))]

public class Shield : MonoBehaviour {

    // 방패 스킬 대상
    GameObject[] enemies_GameObject;
    Enemy[] enemies;

    Transform player;


    // 데미지
    float damage = 3;


    /* 피격 변수 */

    // 방패를 휘둘러서 때린 횟수 카운트.
    public int HitCount = 0;


    /* 피격 표시 변수 */

    // 피격 대상 관련 정보.
    Material SkinMaterial;
    Color OriginColor;

    // 적 색깔 저장.
    Color EnemyColor;
    // 플레이어 색깔 저장.
    Color PlayerColor;

    // 피격 후 겉 색깔 변화
    bool skinColorChange = false;
    // 피격 후 겉 색깔 회복
    bool skinRecove = false;


    /* 넉백 */
    // 넉백 거리
    public float knockbackDst;
    // 넉백 스피드
    public float knockBackspeed;

    // 처음 위치와 넉백 후 위치와의 거리.
    float sqrDstToStartKnockBack;


    /* 도발 */
    // 도발 시간
    public float msTauntTime;



    void Start()
    {
        enemies = GetComponent<Enemy[]>();
    }

    void Update()
    {

        // 휘두른 히트카운트 초기화.
        if (HitCount != 0)
        {
            StartCoroutine(RecoverHitCount());
        }

    }


    /* 데미지 관련 기본 처리 */

    // 데미지를 설정하는 메소드
    public void setDamage(float _damage)
    {
        damage = _damage;
    }

    // HitCount를 증가 시키는 메소드
    public void IncHitCount()
    {
        // 히트 카운트 증가
        HitCount++;

        print("HIT COUNT" + HitCount);
    }


    /* 넉백 효과 */

    // 넉백 효과를 발생 시키는 메소드
    public void KnockbackCollider(Rigidbody rigidbody, float dst)
    {
        StartCoroutine(KnockBackTimer(rigidbody, dst));
    }


    /* 도발 효과 */

    public void Taunt(float Dst)
    {
        int i = 0;

        print("== TAUNT ==");

        // 적들에 대해 찾음.
        enemies_GameObject = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemies_GameObject == null)
        {
            print("== ENEMIES ARE NULL ==");
        }
        else
        {
            print("== ENEMIES COUNT : " + enemies_GameObject.Length);
        }

        // 모든 적들에 대해서 타겟 지정.
        foreach (GameObject enemy_GameObject in enemies_GameObject)
        {
            Enemy eachEnemy = enemy_GameObject.GetComponent<Enemy>();

            // 특정거리 안에 있는 적은 도발하도록 함.
            if(eachEnemy.getDstToPlayer() < Mathf.Pow(Dst, 2f))
            {
                StartCoroutine(TauntTimer(eachEnemy,msTauntTime));
                print("== TAUNT APPLIED==");
            }
                
        }

        // 모든 적들에 대해서 타겟 지정.
        /*
        while (i < enemies_GameObject.Length)
        {
            enemies[i] = enemies_GameObject[i].GetComponent<Enemy>();

            // 특정거리 안에 있는 적은 도발하도록 한다.
            if (enemies[i].getDstToPlayer() < Mathf.Pow(Dst, 2f))
            {
                enemies[i].SetTragetToPlayer();
            }

            i++;
        }
        */
    }

    /* 포효 효과 */

    // 포효 효과를 발생 시키는 메소드
    public void RoarCollider(Enemy enemy, Rigidbody rigidbody, float dst)
    {
        StartCoroutine(RoarTimer(enemy, rigidbody, dst));
    }

    public void Roar(float dst)
    {
        // 적들에 대해 찾음.
        enemies_GameObject = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies_GameObject == null)
        {
            print("== ENEMIES ARE NULL ==");
        }
        else
        {
            print("== ENEMIES COUNT : " + enemies_GameObject.Length);
        }

        // 모든 적들에 대해서 타겟 지정.
        foreach (GameObject enemy_GameObject in enemies_GameObject)
        {
            Enemy eachEnemy = enemy_GameObject.GetComponent<Enemy>();
            Rigidbody enemyRigidBody = eachEnemy.GetComponent<Rigidbody>();

            // 특정 거리 안에 있는 적에 대해서 '포효'효과를 적용.
            if (eachEnemy.getDstToPlayer() < Mathf.Pow(dst, 2f))
            {
                print("== Roar APPLIED==");
                RoarCollider(eachEnemy, enemyRigidBody, 5f);
            }
        }
    }



    /* 충돌 판정 */

    // Trigger 방식. 
    // - 이미 충돌체가 콜라이더 박스 안에 있을 경우
    private void OnTriggerStay(Collider other)
    {
        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player" || other.tag == "Transporter"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        Rigidbody rigidbodyObject = other.GetComponent<Rigidbody>();

        // 피격 대상이 'Enemy'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Enemy" && HitCount == 1 && this.gameObject.tag == "Player_Shield")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;

            // 넉백 적용.
            KnockbackCollider(rigidbodyObject, knockbackDst);
            // rigidbodyObject.MovePosition(Vector3.back * 3);

            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

    }

    // 매 충돌시 범용적으로 판정하는 메소드
    private void OnTriggerEnter(Collider other)
    {
        //print("=== OnTriggerEnter");
        //print(HitCount);

        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player" || other.tag == "Transporter"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        Rigidbody rigidbodyObject = other.GetComponent<Rigidbody>();


        // 피격 대상이 'Enemy'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Enemy" && HitCount == 1 && this.gameObject.tag == "Player_Shield")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;

            // 넉백 적용.
            KnockbackCollider(rigidbodyObject,knockbackDst);
            // rigidbodyObject.MovePosition(Vector3.back * 3);
            
            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        // 참고) 적이 공격.
        if (other.tag == "Player" && HitCount == 1 && this.gameObject.tag == "Enemy_Shield")
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
        if (other.tag == "Transporter" && HitCount == 1 && this.gameObject.tag == "Enemy_Shield")
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


    // 허공에 휘두를 경우, HitCount 초기화
    IEnumerator RecoverHitCount()
    {
        // 근접 무기 사용 쿨타임 만큼 대기 후 초기화
        yield return new WaitForSeconds(1);
        HitCount = 0;
    }

    // KnockBack 타이머
    IEnumerator KnockBackTimer(Rigidbody rigidbody, float dst)
    {
        // 정해진 넉백 적용 최대 타임.
        float knockBackTime = 4f;

        // 시작 시간.
        float startTime = Time.time;
        // 시작 위치.
        Vector3 startPos = rigidbody.position;

        // 매번 위치.
        Vector3 nowPos = rigidbody.position;

        rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 1);

        // 목표 거리까지 계속해서 감.
        while (sqrDstToStartKnockBack < Mathf.Pow(dst,2) && Time.time < startTime + knockBackTime)
        {
            nowPos = rigidbody.position;

            // 넉백 적용.
            rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * knockBackspeed);

            // 거리 계산
            sqrDstToStartKnockBack = (startPos - nowPos).sqrMagnitude;
            print("Knock Applied");

            yield return new WaitForSeconds(Time.deltaTime);
        }

    }

    // Taunt 타이머
    IEnumerator TauntTimer(Enemy enemy, float msTime)
    {
        // 도발 상태 적용.
        enemy.setTaunt(true);

        // 타겟을 플레이어로 변경.
        enemy.setTragetToPlayer();

        yield return new WaitForSeconds(msTime/1000);

        // 도발 상태 해제.
        enemy.setTaunt(false);
    }

    // Roar 타이머
    IEnumerator RoarTimer(Enemy enemy, Rigidbody rigidbody, float dst)
    {
        // 정해진 넉백 적용 최대 타임.
        float knockBackTime = 4f;

        // 시작 시간.
        float startTime = Time.time;
        // 시작 위치.
        Vector3 startPos = rigidbody.position;

        // 매번 위치.
        Vector3 nowPos = rigidbody.position;

        // 적 상태를 넉백 상태로 변경.
        enemy.setStateToKnockBack();

        // 첫번째 넉백 적용.
        rigidbody.MovePosition(enemy.transform.position - enemy.getVectorToPlayer().normalized * Time.deltaTime * 1);


        // 목표 거리까지 계속해서 감.
        while (sqrDstToStartKnockBack < Mathf.Pow(dst, 2) && Time.time < startTime + knockBackTime)
        {
            nowPos = rigidbody.position;

            // 넉백 적용.
            rigidbody.MovePosition(enemy.transform.position - enemy.getVectorToPlayer().normalized * Time.deltaTime * knockBackspeed);

            // 거리 계산
            sqrDstToStartKnockBack = (startPos - nowPos).sqrMagnitude;
            print("Knock Applied");

            yield return new WaitForSeconds(Time.deltaTime);
        }

        sqrDstToStartKnockBack = 0;

        // 적 상태를 다시 원상 복귀.
        if (enemy.Chasing == true)
        {
            enemy.setStateToChasing();
        }

    }
}
