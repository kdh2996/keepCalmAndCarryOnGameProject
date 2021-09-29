using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour {

    // 이동속도
    Vector3 velocity;
    // 충돌에 영향을 받는 오브젝트를 생성
    Rigidbody myRigidbody;


    void Start() {
        myRigidbody = GetComponent<Rigidbody>();

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

    //적을 향해 보는 시각 설정 메소드
    public void LookAtToEnemy(Transform target)
    {
        transform.LookAt(target.position);
    }

    public void Rotate()
    {
        transform.Rotate(transform.position.x, 360 * Time.deltaTime,  transform.position.z);
    }

    // 정기적이고 짧게 반복 실행 필요 메소드
    void FixedUpdate()
    {
        //2018.08.18 플레이어 모델링 적용으로 미사용

        // 새로운 위치 할당 
        // : 이전 위치 + 속력 * FixedUpdate 호출된 시간
        //myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);
    }
}
