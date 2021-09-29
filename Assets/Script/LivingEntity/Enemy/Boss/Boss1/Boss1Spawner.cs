using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Spawner : MonoBehaviour {

    /* 객체 관리 */
    public Transform[] SpawnedPointArray;
    public Boss1 boss1Obejct;

    // 잡몹 소환을 위함 스폰 포인트
    public Spawner enemySpawner;

    // 처음 보스 체크
    bool isFirstBoss = true;

    /* 렌덤 생성 관련 변수 */
    // 보스 생성을 위한 렌덤 씨드
    public int bossWheretoSeed;
    // 소환 위치 갯수
    public int spawnCount = 6;
    // 보스 재생성 렌덤 값
    System.Random reviveRandomValue;

    // 보스의 소환 2 시전 판별 변수
    public bool illusion2_Casting;



    void Start ()
    {

        // 렌덤 씨드 값으로 렌덤값 생성.
        // 현재 렌덤성 확보를 위해 씨드를 넣지 않고, 렌덤값 생성.
        reviveRandomValue = new System.Random();

        // 적 소환 웨이브 초기화
        enemySpawner.SetInitWave();

        if (GameObject.FindGameObjectWithTag("Boss1") != null && Boss1.lifePoint >= 1 && isFirstBoss == true)
        {
            isFirstBoss = false;

            // Boss 오브젝트에 죽었을 때 처리 이벤트를 추가
            GameObject.FindGameObjectWithTag("Boss1").GetComponent<Boss1>().OnDeath += OnBossDeath;

            print("OK");
        }

    }
	
	void Update ()
    {
        // 보스의 메인이 홀에 도달하여, 환영소환2가 발동 되면,
        if (illusion2_Casting == true)
        {
            print("SPAWNER?");

            illusion2_Casting = false;

            // EnemySpawner의 웨이브를 다시 설정함.
            enemySpawner.SetReWave();

            enemySpawner.applySkillSpawn = true;

        }

        // 환영소환2의 적들이 모두 소환되었고, 그 적들이 모두 죽으면,
        if(enemySpawner.allSpawn_Complete == true && GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            // EnemySpawner는 다시 초기화
            enemySpawner.allSpawn_Complete = false;

            // 환영소환1을 다시 시전.
            ReviveBoss();
        }
    }

    /* 보스의 라이프타임과 관련된 처리 */

    // 보스가 죽었을 때 처리
    void OnBossDeath()
    {
        // 보스의 목숨을 하나 줄인다.
        DecBossLifePoint();

        // 목숨이 1이상이면,
        if (Boss1.lifePoint >= 1)
        {
            ReviveBoss();
        }

        // 목숨이 0 이하이면,
        if (Boss1.lifePoint <= 0)
        {

        }
    }

    // 보스의 목숨을 줄이는 메소드
    public void DecBossLifePoint()
    {
        Boss1.lifePoint--;
        enemySpawner.SetNextWave();
    }

    // 보스 재생성을 하는 메소드
    public void ReviveBoss()
    {
        int mainNum;
        Transform mainSpawnedPoint;

        // 본체가 소환 될 위치 정하기.
        mainNum = reviveRandomValue.Next(1, spawnCount);
        mainSpawnedPoint = SpawnedPointArray[mainNum];

        // 본체 Boss1 생성.
        Boss1 boss_Revived_Main = Instantiate(boss1Obejct, mainSpawnedPoint.transform.position, Quaternion.identity) as Boss1;
        // 본체 파괴되었을 때, 이벤트 추가
        boss_Revived_Main.OnDeath += OnBossDeath;

        // 환영소환 1을 시전 변수 True
        boss_Revived_Main.illusion_1_Casting = true;
        // 환영소환 1 시전.
        boss_Revived_Main.CreateIllusionSkill_1(reviveRandomValue, mainNum);
    }
}
