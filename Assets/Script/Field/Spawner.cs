using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    public PlayerManager playerManger;

    //웨이브 활성화/비활성화
    public bool applySpawn;
    //스킬 웨이브 활성화/비활성화
    public bool applySkillSpawn;

    // 현재 웨이브 레퍼런스
    Wave currentWave;
    // 현재 웨이브 횟수
    int currentWaveNum;

    // 남아있는 적 스폰 수
    int enemiesRemainingToSpawn;
    // 살아있는 적의 수
    int enemiesReaminingAlive;
    // 다음 번 스폰 시간
    float nextSpawnTime;

    // 모든 적 스폰이 완료되었는지 확인하는 변수
    public bool allSpawn_Complete = false;



    void Start()
    {
        if (applySpawn == true)
        {
            NextWave();
        }

        if (GameObject.Find("PlayerManger") != null)
        {
            playerManger = GameObject.Find("PlayerManger").GetComponent<PlayerManager>();
            print("==FindPlayerManager==");
        }
    }

    void Update()
    {
        if (applySpawn == true)
        {
            // 적을 스폰하는 부분.
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                // 적 스폰
                Enemy spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity) as Enemy;
                // 죽을 시, OnDeath 이벤트로 호출된, OnEnemyDeath 메소드로 알림 받음
                spawnedEnemy.OnDeath += OnEnemyDeath;

            }
        }

        if (applySkillSpawn == true)
        {
            // 적을 스폰하는 부분.
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                // 적 스폰
                Enemy spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity) as Enemy;
                // 죽을 시, OnDeath 이벤트로 호출된, OnEnemyDeath 메소드로 알림 받음
                spawnedEnemy.OnDeath += OnEnemyDeath_Skill;
            }
        }

    }

    // 죽었음을 알려주는 메소드
    void OnEnemyDeath()
    {
        enemiesReaminingAlive--;
        
        // 플레이어의 킬 수를 올려줌.
        playerManger.KillCount_Inc();

        // 살아남은 적이 없으면, 다음 웨이브 실행.
        if (enemiesReaminingAlive == 0)
        {
            NextWave();
        }
    }

    // 스킬로 소환된 적이 죽었음을 알려주는 메소드
    void OnEnemyDeath_Skill()
    {
        enemiesReaminingAlive--;

        // 플레이어의 킬 수를 올려줌.
        playerManger.KillCount_Inc();

        // 살아남은 적이 없으면, 모든 적을 소환 했음을 알림.
        if (enemiesReaminingAlive == 0)
        {
            // 모두 스폰 되었음을 알림.
            allSpawn_Complete = true;

            // 스폰을 중지 시킴.
            applySkillSpawn = false;
        }
    }

    // 다음 번 웨이브 발생 메소드
    void NextWave()
    {
        currentWaveNum++;
        print("wave" + currentWaveNum);

        // 현제 웨이브 설정.
        if (currentWaveNum - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNum - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesReaminingAlive = enemiesRemainingToSpawn;
        }
    }

    // 다시 한 번 웨이브를 반복하는 메소드
    void ReWave()
    {
        // 현제 웨이브 설정.
        if (currentWaveNum - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNum - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesReaminingAlive = enemiesRemainingToSpawn;
        }
    }

    // 별도로 웨이브 초기 설정을 하는 메소드
    public void SetInitWave()
    {
        NextWave();
    }

    // 별도로 다음 웨이브 설정을 하는 메소드
    public void SetNextWave()
    {
        NextWave();
    }

    // 별도로 다시 웨이브 설정을 하는 메소드
    public void SetReWave()
    {
        ReWave();
    }

    // 내부에 웨이브 정보를 저장할 클래스.
    [System.Serializable]
    public class Wave
    {
        // 생성 적 수
        public int enemyCount;
        // 스폰 간격
        public float timeBetweenSpawns;
    }

}
