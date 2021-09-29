using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss1 : LivingEntity {

    /* 상태 관련 변수 */

    public enum State {Idle, Moving, Attacked}
    State currentState;


    /* 객체 관리 */

    // 길찾기 관리 NavMeshAgent
    NavMeshAgent pathFinder;

    // 보스 스폰 소환 전체 관리 객체
    public Transform bossSpawner;

    // 스폰 위치 배열
    public Transform[] SpawnedPointArray;
    // 스폰 된 위치 리스트
    List<int> SpawnedList = new List<int>();

    // 환영 소환을 위한 boss1 오브젝트
    public Boss1 boss1_illusionObejct;

    // 보스의 목숨.
    public static int lifePoint = 7;


    /* 이동 관련 변수 */

    // 이동속도
    public float moveSpeed = 5;


    /* 스킬 시전에 관련한 변수 */

    // 환영소환 1을 시전했는지 판별하는 변수
    public bool illusion_1_Casting;
    // 환영소환 1이 끝났음을 알리는 변수
    public bool illusion1_Complete = false;

    // 소환 위치 갯수
    public int spawnCount = 6;

    // 소환 술사의 움직임에 관련된 판별 변수
    public bool isDestination = false;


    // 환영소환 2을 시전했는지 판별하는 변수
    public bool illusion_2_Casting;
    // 환영소환 2이 끝났음을 판별하는 변수
    public bool illusion2_Complete = false;



    protected override void Start ()
    {
        base.Start();

        pathFinder = GetComponent<NavMeshAgent>();

        // 초기 상태 지정
        currentState = State.Moving;

        //StartCoroutine(MoveToDest());

        if (this.gameObject.tag == "Boss1")
        {
            //Die();
            //StartCoroutine(DeathBySecond());
        }

    }
	
	void Update ()
    {
        if (illusion1_Complete == true)
        {
            StartCoroutine(MoveToDest());
        }
    }

    /* 환영 생성 처리 */

    // 환영소환 1시전을 하는 메소드
    public void CreateIllusionSkill_1(System.Random randNum, int mainNum)
    {
        // 환영 생성.
        CreateIllusion_AlterEgo(randNum, mainNum);
    }

    // 환영소환 1에 따라, 그에 따른 보스 분신을 생성하는 메소드
    public void CreateIllusion_AlterEgo(System.Random randNum, int mainNum)
    {
        // 스폰할 환영 갯수 설정.
        int spawnIllusionCount = Boss1.lifePoint - 1;

        // 중복 숫자 체크
        // bool isDuplicatedNum = false;

        int illusionNum;
        Transform illusionSpawnedPoint;


        // 미리 스폰했던 본체 멤버 등록.
        SpawnedList.Add(mainNum);
        
        for (int i=0; i<spawnIllusionCount; i++)
        {
            print("SPAWNILLUSION COUNT " + spawnIllusionCount);

            illusionNum = NotRepeatRandom();
            illusionSpawnedPoint = SpawnedPointArray[illusionNum];

            // 환영 Boss1 생성.
            Boss1 boss_Revived_illusion = Instantiate(boss1_illusionObejct, illusionSpawnedPoint.transform.position, Quaternion.identity) as Boss1;
            boss_Revived_illusion.transform.parent = this.transform;
            boss_Revived_illusion.illusion1_Complete = true;
        }

        illusion1_Complete = true;


        /*
        // 환영 갯수 만큼 소환.
        for (int i=0; i<= spawnIllusionCount; i++)
        {
            while(true)
            {
                illusionNum = Random.Range(1, spawnCount);


                illusionSpawnedPoint = SpawnedPointArray[illusionNum];

                SpawnedIndex[i] = illusionNum;

                print("ILLUSION NUM " + illusionNum);

                isDuplicatedNum = false;

                for (int j = 0; j < i; j++)
                {
                    if (SpawnedIndex[j] == SpawnedIndex[i])
                    {
                        print("SPAWEND INDEX NUM" + SpawnedIndex[j]);

                        isDuplicatedNum = true;
                        break;
                    }
                }

                if (isDuplicatedNum == false)
                {
                    break;
                }
            }

            // 환영 Boss1 생성.
            Boss1 boss_Revived_illusion = Instantiate(boss1_illusionObejct, illusionSpawnedPoint.transform.position, Quaternion.identity) as Boss1;
            boss_Revived_illusion.transform.parent = this.transform;

            // 생성되면 좌표는 삭제.
            // SpawnedPointArray[illusionNum] = null;
            // SpawnedPointArray[illusionNum].gameObject.SetActive(false);

        }
        */

    }

    // 중복되지 않는 렌덤 가져오는 메소드
    public int NotRepeatRandom()
    {
        int num = Random.Range(0, spawnCount);

        while (SpawnedList.Contains(num))
        {
            num = Random.Range(0, spawnCount);
        }

        SpawnedList.Add(num);

        return num;
    }

    IEnumerator MoveToDest()
    {
        // 갱신 간격 만큼 루프 반복
        while (isDestination == false)
        {
            if (currentState == State.Moving)
            {
                Vector3 targetPosition = new Vector3(bossSpawner.transform.position.x, 0, bossSpawner.transform.position.y);

                //print("move?");
                illusion1_Complete = false;

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


                // 최종 목적지 도착.
                if (transform.position == targetPosition)
                {
                    isDestination = true;
                    

                }

            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    IEnumerator DeathBySecond()
    {
        yield return new WaitForSeconds(2);

        print("BOSS LIFEPOINT " + Boss1.lifePoint);
        Die();
    }

}
