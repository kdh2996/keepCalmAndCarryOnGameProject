using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHallWall : MonoBehaviour
{

    // 보스 스폰 하는 포인트
    public Boss1Spawner spawner;

    

    void Start()
    {

    }

    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        // 투명 벽에 닿으면 파괴.
        if (other.tag == "Boss1")
        {
            print("== Boss1 COLLIDE WALL ==");
            spawner.illusion2_Casting = true;

            Destroy(other.gameObject);
        }
    }

}
