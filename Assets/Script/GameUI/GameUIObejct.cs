using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIObejct : MonoBehaviour
{

    // 라이프 타임 관련 변수
    float lifeTime;
    // 이동속도
    Vector3 velocity;



    void Update()
    {
        // 이동 적용.
        transform.position += velocity;
    }


    // 라이프 타임 설정
    public void SetLifeTime(float _lifetime)
    {
        lifeTime = _lifetime;

        StartCoroutine(LifeCycle(lifeTime));
        
    }

    // 라이프 타임 적용
    IEnumerator LifeCycle(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }

    // 이동 설정 메소드
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    // 보는 시각 설정 메소드
    public void LookAt(Vector3 lookPoint)
    {
        // Y값을 유지하여, 플레이어 객체가 기울여지지 않도록.
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);

        transform.LookAt(heightCorrectedPoint);
    }

}
