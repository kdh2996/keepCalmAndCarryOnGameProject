using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

    /* 오브젝트 관련 변수 */
    // Player
    Player player;
    // Player와의 제곱 거리
    float SqrDstToPlayer;


    // 라이프 타임 관련 변수
    public bool isDeathOn = false;
    // 라이프 타임
    public float msLifeTime = 0;

    public bool isRangeinner = false;

    // 깃발 효과 관련 변수

    // 추가 데미지 적용 가능 거리
    public float additionalDamage_AppliedRangeDst = 0;
    // 추가 데미지
    public float additionalDamage = 0;




    void Start ()
    {

        StartCoroutine(LifeTime(msLifeTime));

    }

	void Update ()
    {
        FindAlliance();
		
	}


    /* 라이프 타임 과련 처리 */
    // 라이프 타임을 설정하는 메소드
    public void setLifeTime(float msTime)
    {
        msLifeTime = msTime;
    }

    // 파괴하는 메소드
    public void DestroyFlag()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() != null)
        {
            // 파괴되기 전에 버프가 사라지게 해야 함.
            if(isRangeinner == true)
            {
                player.meleeContoller.SubtractDamage(additionalDamage);
                player.additionalDamage -= additionalDamage;
            }
        }

        Destroy(this.gameObject);
    }

    // 라이프 타임.
    IEnumerator LifeTime(float msTime)
    {
        yield return new WaitForSeconds(msTime / 1000);

        isDeathOn = true;
        DestroyFlag();
    }


    // 아군을 찾는 메소드
    public void FindAlliance()
    {

        if(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() != null)
        {
            // 플레이어
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            // 플레이어와의 거리 계산.
            SqrDstToPlayer = (player.transform.position - transform.position).sqrMagnitude;

            if (SqrDstToPlayer <= Mathf.Pow(additionalDamage_AppliedRangeDst, 2) && isRangeinner == false)
            {
                // 안에 있을 때 한 번만 공격력이 올라가야 함.
                isRangeinner = true;

                player.meleeContoller.AddDamage(additionalDamage);
                player.additionalDamage += additionalDamage;

                print("== Flag Buff ==" );
                print("** Additional Melee Damage : " + additionalDamage + "**");

            }
            else if(SqrDstToPlayer > Mathf.Pow(additionalDamage_AppliedRangeDst, 2) && isRangeinner == true)
            {
                isRangeinner = false;

                //print("== Flag Buff End ==");
                player.meleeContoller.SubtractDamage(additionalDamage);
                player.additionalDamage -= additionalDamage;
            }
        }
       
    }

    // 추가 데미지 적용 거리 설정
    public void setAdditionalDamageRangeDst(float dst)
    {
        additionalDamage_AppliedRangeDst = dst;
    }

    // 추가 데미지 설정
    public void setAdditionalDamage(float damage)
    {
        additionalDamage = damage;
    }


}
