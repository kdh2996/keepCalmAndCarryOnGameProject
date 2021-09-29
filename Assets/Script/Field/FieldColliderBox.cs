using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class FieldColliderBox : MonoBehaviour {

    /* 피격에 대한 변수 */

    // 필드 데미지
    public float damage = 1;
    // 추가 데미지
    public float additionDamage = 1f;

    // 플레이어에 대한 Transform
    Transform PlayerTransform;
    // 적에 대한 Transform.
    Transform EnemyTransform;

    // 피격 대상 관련 정보.
    Material SkinMaterial;
    Color OriginColor;

    // 피격 후 겉 색깔 변화
    bool skinColorChange = false;
    // 피격 후 겉 색깔 회복
    bool skinRecove = false;

    // 적 색깔 저장.
    Color EnemyColor;
    // 플레이어 색깔 저장.
    Color PlayerColor;


    /* 데미지에 대한 변수 */

    // 즉발성 데미지 형.
    public bool isDamageOnce = false;
    // 지속형 데미지 형.
    public bool isDamageOverTime = false;

    // 지속형 데미지 적용 순간.
    public bool isDOTApply = false;
    // 지속형 데미지 적용 시간.
    public float DotmsTime;

    // 지속형 데미지 사이클 돌았는지 확인.
    bool isOneCycleRoutine = false;


    /* 이동 감속에 대한 변수 */
    
    // 슬로우 적용 여부
    public bool isSlowApplied = false;
    // 슬로우 정도
    public float slowRate = 0f;


    /* 박스 라이프 타임에 대한 변수 */

    // 박스의 생성 여부
    public bool isCreated = false;
    // 데미지 박스 라이프 타임.
    public float msBoxLifeTime;

    // 박스의 죽었는지 여부
    bool isDeathOn = false;


    /* 효과 */

    // 폭렬마법 효과 추가 여부
    public bool isExplosionBlaze_On = false;
    // 폭렬마법 적용 시간
    public float explosionBlaze_AppliedTime = 0f;
    // 폭렬마법 데미지 주기
    public float explosionBlaze_DamagePeriod = 0f;
    // 폭렬마법 추가 데미지
    public float explosionBlaze_AdditionalDamage = 0f;

    // 토네이도형 끌어당김 적용 여부
    public bool isTornado_Draw_On = false;



    void Update()
    {
        // 생성시 수명 적용.
        if(isCreated == false)
        {
            isCreated = true;
            StartCoroutine(BoxLifeTime(msBoxLifeTime));
        }

        // 지속형 데미지 적용시
        if(isDamageOverTime == true)
        {
            // 타이머 사이클이 지나면 다시 Routine 시작.
            if(isOneCycleRoutine == true)
            {
                isOneCycleRoutine = false;

                StartCoroutine(DOTCoroutine(DotmsTime));
            }

        }
        
        // 수정 필요. (매번 적 태그로 검사)
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            EnemyTransform = GameObject.FindGameObjectWithTag("Enemy").transform;
            EnemyColor = EnemyTransform.GetComponent<Renderer>().material.color;
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (PlayerTransform != null)
        {
            PlayerColor = PlayerTransform.GetComponent<Renderer>().material.color;
        }

        // 피격후 색깔 변경
        if (skinColorChange == true)
        {
            StartCoroutine(ChangeSkin());
        }

        // 색 변경 후 복구
        if (skinRecove == true)
        {
            StartCoroutine(ReCoverSkin());
        }
    }


    /* 피격 후 처리 */
    // 피격 후 색깔 변경
    IEnumerator ChangeSkin()
    {
        yield return null;
        // 빨갠색으로 변경.
        SkinMaterial.color = Color.red;
        skinRecove = true;
    }

    // 피격 후 색깔 복구
    IEnumerator ReCoverSkin()
    {
        yield return new WaitForSeconds(0.5f);
        SkinMaterial.color = OriginColor;

        skinColorChange = false;
        skinRecove = false;
    }


    /* 박스 라이프 타임 과련 처리 */
    // 파괴하는 메소드
    public void DestroyFieldColliderBox()
    {
        Destroy(this.gameObject);
    }

    // 박스의 라이프 타임을 설정하는 메소드
    public void setLifeTime(float msTime)
    {
        msBoxLifeTime = msTime;
    }

    // 라이프 타임.
    IEnumerator BoxLifeTime(float msTime)
    {
        yield return new WaitForSeconds(msTime / 1000);

        isDeathOn = true;
        DestroyFieldColliderBox();
    }


    /* 데미지 관련 처리 */
    // 데미지를 설정하는 메소드
    public void setDamage(float param_damage)
    {
        damage = param_damage;
    }

    // 추가 데미지를 설정하는 메소드
    public void setAdditionalDamage(float add_Damage)
    {
        additionDamage = add_Damage;
    }

    // DOT 메소드
    public void setDamageOverTime(float msTime)
    {
        isDamageOverTime = true;
        DotmsTime = msTime;

        StartCoroutine(DOTCoroutine(DotmsTime));
    }

    // DOT 루틴. (몇 초당 DOT를 적용할 것 인가.)
    IEnumerator DOTCoroutine(float DotmsTimer)
    {
        yield return new WaitForSeconds(DotmsTimer / 1000);

        // print("??");

        // 피격 판정이 가능하도록 하는 변수 On.
        isDOTApply = true;
        // 한 사이클 지났음을 표시.
        isOneCycleRoutine = true;
    }


    /* 이동 감소 관련 처리 */
    public void setSlowSpeed(float rate)
    {
        slowRate = rate;
    }

    // 폭렬불꽃 설정.
    public void setExplosionBlazeApplied(float appliedTime, float additionalDamage, float damagePeriod)
    {
        isSlowApplied = true;
        isExplosionBlaze_On = true;

        explosionBlaze_AppliedTime = appliedTime;
        explosionBlaze_AdditionalDamage = additionalDamage;
        explosionBlaze_DamagePeriod = damagePeriod;

    }


    /* 필드에 직접적으로 피격 판정 1) OnTrigger 사용 */
    // 물리 연산 없이 (겹쳐서) 데미지 판정을 위하여 OnTrigger사용.

    private void OnTriggerEnter(Collider other)
    {
        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        // 즉발성일 경우.
        if(isDamageOnce == true)
        {
            // 피격 대상이 'Enemy'일 경우
            // 필드 공격의 적에 대한 피격 판정은 현재는 제외한다.
            if (other.tag == "Enemy" && this.tag == "Player_FieldColliderBox")
            {
                if (isDamageOnce == true)
                {
                    Debug.Log("(Once FieldAttack) Enemy Hit!");
                    damageableObject.TakeDamage(damage);
                    OriginColor = EnemyColor;

                    // 피격으로 색깔 변경.
                    skinColorChange = true;

                    this.isDamageOnce = false;
                }
            }

            // 피격 대상이 'Player'일 경우
            if (other.tag == "Player" && this.tag == "Enemy_FieldColliderBox")
            {
                if (isDamageOnce == true)
                {
                    Debug.Log("(FiledAttack) Player Hit!");
                    damageableObject.TakeDamage(damage);
                    OriginColor = PlayerColor;

                    // 피격으로 색깔 변경.
                    skinColorChange = true;
                }
            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(other.tag == "Enemy" || other.tag == "Player"))
        {
            return;
        }

        Idamageable damageableObject = other.GetComponent<Idamageable>();
        SkinMaterial = other.GetComponent<Renderer>().material;

        // DOT 적용일 경우,
        if (isDamageOverTime == true)
        {
            // DOT가 적용될 경우에 데미지를 적용한다.
            if (isDOTApply == true)
            {
                // 피격 대상이 'Enemy'일 경우
                // 필드 공격의 적에 대한 피격 판정은 현재는 제외한다.
                if (other.tag == "Enemy" && this.tag == "Player_FieldColliderBox")
                {
                    Debug.Log("(DOT FieldAttack) Enemy Hit!");
                    damageableObject.TakeDamage(damage);

                    // ExplosionBlaze 효과에 따른 각종 효과 적용.
                    if(isExplosionBlaze_On == true)
                    {
                        ThirdPersonCharacter enemyCharacter = other.GetComponent<ThirdPersonCharacter>();
                        Enemy enemy = other.GetComponent<Enemy>();

                        if(isSlowApplied == true && isExplosionBlaze_On == true && enemy.isAppliedExplosionSpell == false)
                        {
                            enemy.isAppliedExplosionSpell = true;
                            StartCoroutine(ExplosionBlazeRoutine(enemy, enemyCharacter, explosionBlaze_AppliedTime, explosionBlaze_AdditionalDamage, explosionBlaze_DamagePeriod));
                        }
                    }

                    // TornadoBlaze 효과에 따라 중첩데미지 적용.
                    if(isTornado_Draw_On == true)
                    {
                        // 중첩 추가 데미지 적용.
                        Enemy collidedenemy = other.GetComponent<Enemy>();

                        collidedenemy.IncCount_TornadoBlaze();

                        // 3번 중첩 스택이 쌓이면 추가 데미지 적용.
                        if (collidedenemy.getCount_TornadoBlaze() == 3)
                        {
                            collidedenemy.InitCount_TornadoBlaze();
                            damageableObject.TakeDamage(additionDamage);

                            print("(Tornado Add Damage) Enemy has Additional Damage!");
                        }
                    }

                    OriginColor = EnemyColor;

                    // 피격으로 색깔 변경.
                    skinColorChange = true;

                    this.isDamageOnce = false;

                    isDOTApply = false;
                }

                // 피격 대상이 'Player'일 경우
                if (other.tag == "Player" && this.tag == "Enemy_FieldColliderBox")
                {
                    Debug.Log("(FiledAttack) Player Hit!");
                    damageableObject.TakeDamage(damage);
                    OriginColor = PlayerColor;

                    // 피격으로 색깔 변경.
                    skinColorChange = true;

                    isDOTApply = false;
                }

            }

        }

        // 효과 적용

        // 토네이도 끌어당김 적용.

        if(isTornado_Draw_On == true)
        {
            // 피격 대상이 'Enemy'일 경우
            if (other.tag == "Enemy" && this.tag == "Player_FieldColliderBox")
            {
                // Debug.Log("(Tornado Draw) Enemy Drag on Tornado");

                // 토네이도형 끌어당김 Routine 적용.
                if(other.transform != null)
                {
                    StartCoroutine(tornadoDragRoutine(other.transform));
                }
            }
        }

    }


    /* 필드에 직접적으로 피격 판정 2) OnCollision 사용 
    // 현재 사용하지 않음.

    private void OnCollisionEnter(Collision collision)
    {
        print("FiledCollide!");

        // 피격 데미지를 받지 않는 대상일 경우, 메소드 바로 탈출.
        if (!(collision.collider.tag == "Enemy" || collision.collider.tag == "Player"))
        {
            return;
        }

        Idamageable damageableObject = collision.collider.GetComponent<Idamageable>();
        SkinMaterial = collision.collider.GetComponent<Renderer>().material;

        // 피격 대상이 'Enemy'일 경우
        // 필드 공격의 적에 대한 피격 판정은 현재는 제외한다.

        if (collision.collider.tag == "Enemy")
        {
            Debug.Log("Enemy Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = EnemyColor;


            // 피격으로 색깔 변경.
            skinColorChange = true;
        }

        // 피격 대상이 'Player'일 경우
        if (collision.collider.tag == "Player")
        {
            Debug.Log("(FiledAttack) Player Hit!");
            damageableObject.TakeDamage(damage);
            OriginColor = PlayerColor;

            // 피격으로 색깔 변경.
            skinColorChange = true;
        }
    }

    */


    /* 효과 */
    // 토네이도형 끌어당김 적용.
    public void setTornadoDraw()
    {
        isTornado_Draw_On = true;
    }

    // 토네이도형 끌어당김 Routine
    IEnumerator tornadoDragRoutine(Transform transform)
    {
        // Collider Box가 없어질 때 까지 토네이도형 끌어당김 적용.

        while(this.gameObject != null)
        {
            if(transform != null)
            {
                DrawToColliderBox(transform);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    // 토네이도형 끌어당김을 실행하는 메소드
    public void DrawToColliderBox(Transform draggedTransform)
    {
        if(this.gameObject != null && draggedTransform.gameObject != null)
        {
            draggedTransform.position = this.transform.position;
        }
    }


    // 폭렬 불꽃 적용 메소드
    IEnumerator ExplosionBlazeRoutine(Enemy enemy, ThirdPersonCharacter character, float durationTime, float damage, float damagePeriod)
    {
        float startTime = Time.time;
        float appliedTime = startTime + durationTime/1000;

        float characterInitSpeed = character.getMoveSpeedMultiplier();

        character.SetMoveSpeedMultiplier((100 - slowRate) / 100);

        while (Time.time <= appliedTime && enemy != null)
        {
            print("== Enemy is Applied ExplosionSpell DOT==");
            enemy.TakeDamage(damage);

            yield return new WaitForSeconds(damagePeriod/1000);
        }

        print("== ExplosionSpell Additional Effect END==");
        enemy.isAppliedExplosionSpell = false;
        character.SetMoveSpeedMultiplier(characterInitSpeed);

    }


}
