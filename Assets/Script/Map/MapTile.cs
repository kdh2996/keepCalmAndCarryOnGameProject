using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    /* 장애물 관련 제어 변수 */

    // 장애물 적용 판별
    public bool isObstacle = false;
    // 장애물
    public Transform obstaclePrefab;
    // 새로운 장애물
    public Transform newObstacle;


    /* 데미지 관련 제어 변수 */

    // 데미지 적용 판별
    public bool isDamage = false;
    // 데미지 적용 충격 박스
    public Transform filedDamageBox;
    // 새로운 데미지 충격 박스
    public Transform newfiledDamageBox;

    // 점멸 판별 변수
    public bool isFlesh = false;



    private void Start()
    {
        ApplyTile();
    }

    private void Update()
    {
        // 타일 필드 데미지 관련 부분.
        ApplyDamage();
    }


    // 독립 타일 적용사항.
    public void ApplyTile()
    {
        // 장애물 관련 부분.
        ApplyObstacle();
    }
 
    // 장애물과 관련된 사항 적용 메소드.
    public void ApplyObstacle()
    {
        /* 장애물을 생성하는 부분 */
        if (isObstacle) // 체크시 바로 생성.
        {
            // 오브젝트 아래에 장애물이 없으면, 장애물 생성.
            if (!newObstacle)
            {
                newObstacle = Instantiate(obstaclePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            }
        }
        else if (!isObstacle) // 체크 해제시 바로 파괴.
        {
            // 장애물이 있으면, 장애물 파괴.
            if (newObstacle)
            {
                DestroyImmediate(newObstacle.gameObject);
            }
        }

    }


    /* 타일에 데미지 관련 사항을 적용하는 부분 */

    // 데미지와 관련된 사항 적용 메소드.
    public void ApplyDamage()
    {
        if (isDamage && isFlesh == false)
        {
            StartCoroutine(ApplyDamageToTile());
        }     
    }

    // 데미지와 관련된 사항을 적용하는 Coroutine
    IEnumerator ApplyDamageToTile()
    {
        isFlesh = true;

        // 해당 적용을 하는 데 걸리는 시간 측정. (경과 시간)
        float applyTimer = 0;
        // 데미지 적용 전 얼마나 지연할 지 측정. (대기 시간)
        float damageDelay = 1;

        // 빛나는 속도
        float tileFleshSpeed = 4;

        // 타일 색
        Material tileMat = transform.GetComponent<Renderer>().material;
        // 원래 색 저장.
        Color initialColor = tileMat.color;
        // 반짝일 색 저장.
        Color fleshColor = Color.red;

        /* 타일 점멸 하는 부분 */

        while (applyTimer < damageDelay)
        {
            //print("flesh");
            // 색깔 보간
            tileMat.color = Color.Lerp(initialColor, fleshColor, Mathf.PingPong(applyTimer * tileFleshSpeed, 1));

            applyTimer += Time.deltaTime;
            yield return null;
        }

        tileMat.color = Color.red;
        // 타일 점멸이 끝날 시, 데미지 박스 생성.
        CreateDamageBox();
    }

    // 타일에 데미지를 입히는 박스를 생성 하는 메소드
    public void CreateDamageBox()
    {
        /* 데미지를 적용하는 부분 */
        if (isDamage)
        {
            // 오브젝트 아래에 충격박스가 없으면, 충격박스 생성.
            if (!newfiledDamageBox)
            {
                newfiledDamageBox = Instantiate(filedDamageBox, transform.position + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            }

        }
        else if (!isDamage)
        {
            // 충격박스가 있으면, 장애물 파괴.
            if (newfiledDamageBox)
            {
                DestroyImmediate(newfiledDamageBox.gameObject);
            }
        }
    }

}
