using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Map))]

public class FieldAttacker : MonoBehaviour {

    /* 데미지 관련 전달 변수 
     
    // 데미지 적용 전 얼마나 지연할 지 측정. (대기 시간)
    public float filedDelay;
    // 빛나는 속도
    public float FieldFleshSpeed;

    */

    // 맵 객체.
    Map map;

    // 에디터로 지정된 공격 타일
    public Coord[] AttackTile;
    // 맵 초기화되고 나서 즉각 발생
    int once = 0;



    void Start()
    {
        map = FindObjectOfType<Map>();
    }

    void Update()
    {
        if (once == 0)
        {
            AttackFiled(AttackTile);
            once++;
        }
    }


    // 필드에 공격을 실행하는 메소드
    public void AttackFiled(Coord[] attackTile)
    {
        Transform[,] Tiles = map.GetTileMap();

        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(0); y++)
            {

                for (int i = 0; i < attackTile.Length; i++)
                {
                    //print(Tiles[x, y].position);

                    // 만약, 지정한 위치 타일이 있다면,
                    if (Tiles[x, y].position == new Vector3((float)(attackTile[i].x), 0, (float)(attackTile[i].y)))
                    {
                        Tiles[x, y].GetComponent<MapTile>().isDamage = true;
                    }

                }
            }
        }

    }

}
