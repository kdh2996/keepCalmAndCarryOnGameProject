using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MeleeWeapon
{
    /* 넉백 관련 변수 */

    // 넉백 스피드
    public float knockBackspeed;
    // 넉백 거리
    public float knockBackDst;

    // 처음 위치와 넉백 후 위치와의 거리.
    float sqrDstToStartKnockBack;



    /* 넉백 효과 */

    // 넉백 효과를 발생 시키는 메소드
    public void KnockbackCollider(Enemy enemy, Rigidbody rigidbody, float dst)
    {
        StartCoroutine(KnockBackTimer(enemy, rigidbody, dst));
    }

    // 넉백 타이머
    IEnumerator KnockBackTimer(Enemy enemy, Rigidbody rigidbody, float dst)
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

        rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 1);

        // 목표 거리까지 계속해서 감.
        while (sqrDstToStartKnockBack < Mathf.Pow(dst, 2) && Time.time < startTime + knockBackTime)
        {
            nowPos = rigidbody.position;

            // 넉백 적용.
            rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * knockBackspeed);

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
        Enemy enemy = other.GetComponent<Enemy>();

        // 피격 대상이 'Enemy'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Enemy" && HitCount == 1 && this.gameObject.tag == "Player_Melee")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;

            // 넉백 적용.
            if(this.getCurrentCombo() == 3)
            {
                KnockbackCollider(enemy, rigidbodyObject, knockBackDst);
            }

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
        Enemy enemy = other.GetComponent<Enemy>();


        // 피격 대상이 'Enemy'일 경우
        // 참고) 플레이어가 공격.
        if (other.tag == "Enemy" && HitCount == 1 && this.gameObject.tag == "Player_Melee")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;

            // 넉백 적용.
            if (this.getCurrentCombo() == 3)
            {
                KnockbackCollider(enemy, rigidbodyObject, knockBackDst);
            }

            HitCount--;

            // 피격으로 색깔 변경.
            skinColorChange = true;
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
}
