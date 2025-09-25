using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float runSpeed = 10f;
    private float baseSpeed;
    private int slowCount = 0;
    public float laneOffset = 3f;
    public float jumpForce = 5f;
    public float laneChangeSpeed = 10f;//레인 이동속도
    private float fallTimer = 0f;//낙하 추적

    private bool isStunned = false;

    public GameObject blindOverlay;

    public VirtualJoystick joystick; // 조이스틱 연결용
    public Button jumpButton;

    public float fallMultiplier = 30f; // 낙하 가속 배율

    //private float keyHoldTime = 0f;
    private UnityEngine.Rigidbody rb;
    private SkillData currentSkill;  // 현재 선택된 스킬 저장

    [Header("UI")]
    public Slider skillCooldownSlider;  //쿨타임 시 오버레이
    private bool canUseSkill = true; // 쿨타임 체크
    public float skillCooldown = 5f; // 기본 쿨타임 (초)
    public GameObject gameOverUI;

    [Header("Effects")]
    public Material speedEffectMat;
    [Header("Jump Settings")]
    public float normalJumpHeight = 5f;   // Inspector에서 높이 조절 가능
    public float normalJumpDuration = 0.6f; // 점프 시간도 함께 조절 가능
    public float obstacleJumpHeight = 7f;
    public float obstacleJumpDuration = 0.4f;


    public float landingOffsetZ = 0.2f; // 🔹 장애물 윗면 중앙에서 앞으로 땡겨올 비율
    private int currentLane = 1;
    private Vector3 targetPosition;

    private bool isGrounded = true;
    private bool hasJumped = false;
    private bool isBlocked = false;


    public float fallSpeed = 10f; // 낙하 속도
    public Transform respawnPoint;

    private bool isDead = false;
    private float invincibleTimer = 0f;

    public GameObject buffEffectPrefab;   // 버프 이펙트
    public GameObject debuffEffectPrefab;

    [Header("Effects")]

    private GameObject activeEffect;
    public GameObject stunEffectPrefab;   // 🔹 Inspector에서 연결할 스턴 이펙트 프리팹
    private GameObject activeStunEffect;  // 현재 실행 중인 이펙트

    public Vector3 stunOffset = new Vector3(0, 0.3f, 0); // Inspector에서 조절 가능

    [Header("Camera")]
    public CinemachineCamera cineCam;   // 🔹 인스펙터에서 CinemachineCamera 드래그
    private float defaultFOV = 40f;
    private Coroutine fovCoroutine;
    public UniversalRendererData rendererData;
    private ScriptableRendererFeature speedFeature;

    private float obstacleHeight = 0f;

    private void Awake()
    {
        // 런타임 전용 인스턴스 생성
      
            speedEffectMat = new Material(speedEffectMat);
    }


    void Start()
    {
       
        rb = GetComponent<UnityEngine.Rigidbody>();
        baseSpeed = runSpeed;

        rb.freezeRotation = true;
        animator.SetInteger("animation", 18);
        UpdateTargetPosition();
        jumpButton.onClick.AddListener(Jump);

        if (isDead)
        {
            RespawnInstant();
        }

        // RendererFeature 찾기
        foreach (var f in rendererData.rendererFeatures)
            Debug.Log($"RendererFeature: {f.name}");

        speedFeature = rendererData.rendererFeatures
            .Find(f => f.name.Contains("FullScreenPassRendererFeature"));

        if (speedFeature != null)
            Debug.Log("[OK] SpeedFeature 연결 성공!");
        else
            Debug.LogError("[ERROR] SpeedFeature 못 찾음");

        //SetSpeedEffect(false);

        if (speedEffectMat != null)
            speedEffectMat.SetFloat("_Distortion", 0f);
    }

    // ElementDataLoader에서 스킬을 전달받을 함수
    public void SetSkill(SkillData skill)
    {
        currentSkill = skill;
        Debug.Log($"[플레이어] 스킬 세팅 완료 → {skill.Name}");
    }
    // 본체 클릭 시 스킬 발동
    private void OnMouseDown()
    {
        UseSkill();
    }
    public void UseSkill()
    {
        if (!canUseSkill || currentSkill == null)
        {
            Debug.Log("스킬 사용 불가 (쿨타임 중이거나 스킬 없음)");
            return;
        }

        Debug.Log($"[스킬 발동] {currentSkill.Name} - {currentSkill.Description}");

        // 스킬 효과 자동 실행
        ApplySkillEffect(currentSkill);
        // 쿨타임 적용
        StartCoroutine(SkillCooldownRoutine());
    }
    private void ApplySkillEffect(SkillData skill)
    {
        // 자기 자신 버프형
        if (skill.TargetType == 1)
        {
            // BuffId1 적용 → 이속 증가
            if (skill.BuffId1 == 321040) // 물 Lv1
                StartCoroutine(ApplySpeedBuff(1.4f, skill.Duration));
            else if (skill.BuffId1 == 321060) // 물 Lv2
                StartCoroutine(ApplySpeedBuff(1.6f, skill.Duration));
            else if (skill.BuffId1 == 321080) // 물 Lv3
                StartCoroutine(ApplySpeedBuff(1.8f, skill.Duration));
            else if (skill.BuffId1 == 312100) // 장애물 무시
                StartCoroutine(ApplyInvincibility(skill.Duration));

            // 공기 스킬 (ElementId == 3)
            else if (skill.ElementId == 3)
            {
                if (skill.BuffId1 == 311040) // 공기 Lv1
                    StartCoroutine(ApplySpeedBuff(1.4f, skill.Duration));
                else if (skill.BuffId1 == 311060) // 공기 Lv2
                    StartCoroutine(ApplySpeedBuff(1.6f, skill.Duration));
                else if (skill.BuffId1 == 311080 && skill.BuffId2 == 0) // 공기 Lv3
                    StartCoroutine(ApplySpeedBuff(1.8f, skill.Duration));
                else if (skill.BuffId1 == 311080 && skill.BuffId2 == 312100) // 공기 Lv4
                {
                    StartCoroutine(ApplySpeedBuff(1.8f, skill.Duration));
                    StartCoroutine(ApplyInvincibility(skill.Duration));
                }
            }
        }
        // 적 대상 디버프/공격형
        else if (skill.TargetType == 2)
        {
            if (skill.ElementId == 2) // 불 원소
            {
                // DOT (초당 피해)
                StartCoroutine(ApplyDamageOverTime(skill.Damage, skill.Duration));
            }
            else if (skill.ElementId == 4) // 흙 원소
            {
                // 즉발 데미지
                EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(skill.Damage);
                }
                Debug.Log($"[즉시 피해] 흙 스킬 {skill.Name} → {skill.Damage} 데미지");
            }
            else
            {
                Debug.LogWarning($"[스킬 미구현] {skill.ElementId} 원소는 효과가 정의되지 않음");
            }
        }
    }

    private IEnumerator ApplySpeedBuff(float multiplier, float duration)
    {
        float original = runSpeed;
        runSpeed = baseSpeed * multiplier;

        // 🔹 효과용 Material 활성화
        //SetSpeedEffect(true);


        // 🔹 카메라 줌인 (FOV 30으로)
        if (cineCam != null)
        {
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
           
            fovCoroutine = StartCoroutine(ChangeFOV(30f, 0.5f)); // 0.5초 동안 줌인
        }
        // 🔹 쉐이더 강도 올리기
        StartCoroutine(AnimateSpeedShader(0.4f, 0.5f));
        Debug.Log($"[버프] 이속 {multiplier * 100}% ({duration}초)");
        
        yield return new WaitForSeconds(duration);

        runSpeed = original;
        //SetSpeedEffect(false);  // 끄기
      
        Debug.Log("[버프 종료] 기본 속도로 복귀");
        // 🔹 카메라 줌아웃 (기본값으로 되돌림)
        if (cineCam != null)
        {
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovCoroutine = StartCoroutine(ChangeFOV(defaultFOV, 0.5f)); // 0.5초 동안 복구
        }
        // 🔹 쉐이더 강도 내리기
        StartCoroutine(AnimateSpeedShader(0f, 0.5f));
    }
    //private void SetSpeedEffect(bool enabled)
    //{
    //    Debug.Log($"[SetSpeedEffect 호출됨] enabled={enabled}, speedFeature={(speedFeature != null ? speedFeature.name : "NULL")}");

    //    if (speedFeature == null)
    //    {
    //        Debug.LogWarning("[SetSpeedEffect] speedFeature 아직 연결 안됨");
    //        return;
    //    }

    //    speedFeature.SetActive(enabled);
    //    Debug.Log($"[RendererFeature 적용됨] {speedFeature.name} → {enabled}");
    //}

    // 배율 → 타겟 FOV 계산 함수
    private float CalculateTargetFOV(float multiplier)
    {
        // multiplier = 1 → defaultFOV
        // multiplier = 1.8 → defaultFOV - 10 (즉 30)
        float minMultiplier = 1f;
        float maxMultiplier = 1.8f;
        float minFOV = defaultFOV - 10f; // 최대로 줄일 값 (예: 30)
        float maxFOV = defaultFOV;       // 기본값 (예: 40)

        // 선형 보간
        float t = Mathf.InverseLerp(minMultiplier, maxMultiplier, multiplier);
        return Mathf.Lerp(maxFOV, minFOV, t);
    }
    private IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        float startFOV = cineCam.Lens.FieldOfView;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cineCam.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            yield return null;
        }

        cineCam.Lens.FieldOfView = targetFOV; // 보정
    }

    private IEnumerator ApplyInvincibility(float duration)
    {
        Debug.Log($"[무적] 장애물 무시 {duration}초");
        // 예시: isBlocked 무시 처리
        bool prev = isBlocked;
        isBlocked = false;

        yield return new WaitForSeconds(duration);

        isBlocked = prev;
        Debug.Log("[무적 종료]");
    }

    private IEnumerator ApplyDamageOverTime(int damagePerSecond, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
            foreach (var enemy in enemies)
            {
                enemy.TakeDamage(damagePerSecond);
            }

            Debug.Log($"[도트 피해] 초당 {damagePerSecond} 데미지 적용");

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
        Debug.Log("[도트 피해 종료]");
    }

    private IEnumerator SkillCooldownRoutine()
    {
        canUseSkill = false;

        // 슬라이더 초기화
        if (skillCooldownSlider != null)
        {
            skillCooldownSlider.maxValue = skillCooldown;
            skillCooldownSlider.value = 0f;
        }
        float time = 0f;


        while (time < skillCooldown)
        {
            time += Time.deltaTime;

            if (skillCooldownSlider != null)
            {
                //차오름
                skillCooldownSlider.value = time;
            }

            yield return null;
        }

        if (skillCooldownSlider != null)
            skillCooldownSlider.value = skillCooldown;
        canUseSkill = true;
        Debug.Log("[쿨타임 종료] 스킬 재사용 가능");
    }

    public void ApplySlow(float multiplier)
    {

        runSpeed = baseSpeed * multiplier;
        PlayEffect(debuffEffectPrefab);
        Debug.Log($"[슬로우 적용] {multiplier * 100}% 속도로 변경");

    }

    public void RemoveSlow()
    {
        StopEffect();
        runSpeed = baseSpeed;
        Debug.Log("[슬로우 종료] 기본 속도로 복구");
    }
    // 🔹 바위 전용: 슬로우 + 스턴 이펙트만
    public void ApplyRockSlow(float multiplier, float duration)
    {
        StopCoroutine(nameof(RockSlowCoroutine));
        StartCoroutine(RockSlowCoroutine(multiplier, duration));
    }
    private IEnumerator RockSlowCoroutine(float multiplier, float duration)
    {
        runSpeed = baseSpeed * multiplier;
        PlayStunEffect(duration);   // 🔹 스턴 이펙트만 표시 (디버프 이펙트 X)

        Debug.Log($"[바위 슬로우] {multiplier * 100}% 속도로 변경 ({duration}초)");

        yield return new WaitForSeconds(duration);

        runSpeed = baseSpeed;
        Debug.Log("[바위 슬로우 종료] 기본 속도로 복구");
    }

    private void PlayStunEffect(float duration)
    {
        // 중복 제거
        if (activeStunEffect != null)
        {
            Destroy(activeStunEffect);
            activeStunEffect = null;
        }
        if (stunEffectPrefab == null) return;
        // 캐릭터 콜라이더 높이 기준으로 머리 위 위치 계산
        float height = 1f;
        Collider col = GetComponent<Collider>();
        if (col != null)
            height = col.bounds.size.y * 0.2f;

        // 머리 위에 스턴 이펙트 생성
        activeStunEffect = Instantiate(stunEffectPrefab, transform);

        // 머리 위 (Pivot + 높이 + 오프셋)
        activeStunEffect.transform.localPosition = Vector3.up * height + stunOffset;

        // -90° 회전 적용
        activeStunEffect.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        // 크기 조절 (필요 시)
        activeStunEffect.transform.localScale = Vector3.one * 1.5f;

        // 자동 제거
        Destroy(activeStunEffect, duration);
    }
    // 🔹 일정 시간 후 자동 해제되는 슬로우 (바위 같은 경우)
    public void ApplyTimedSlow(float multiplier, float duration)
    {
        StopCoroutine(nameof(TimedSlowCoroutine)); // 중복 방지
        StartCoroutine(TimedSlowCoroutine(multiplier, duration));
    }
    private IEnumerator TimedSlowCoroutine(float multiplier, float duration)
    {
        runSpeed = baseSpeed * multiplier;
        PlayEffect(debuffEffectPrefab);
        PlayStunEffect(duration);

        Debug.Log($"[슬로우 적용] {multiplier * 100}% 속도로 변경 ({duration}초)");

        yield return new WaitForSeconds(duration);

        StopEffect();
        runSpeed = baseSpeed;
        Debug.Log("[슬로우 자동 종료] 기본 속도로 복구");

    }

    public void ApplyBuff(float multiplier)
    {
        runSpeed = baseSpeed * multiplier;

        Debug.Log($"[버프 적용] {multiplier * 100}% 속도로 변경");

    }

    public void RemoveBuff()
    {

        runSpeed = baseSpeed;
        Debug.Log("[버프 종료] 기본 속도로 복구");

    }
    private void PlayEffect(GameObject effectPrefab)
    {
        StopEffect(); // 기존 이펙트 제거
        if (effectPrefab != null)
        {
            // 캐릭터 위치 + 약간 위쪽
            Vector3 pos = transform.position + Vector3.up * 2f;
            activeEffect = Instantiate(effectPrefab, pos, Quaternion.identity, transform);
        }
    }
    private void StopEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }
    }
    void Update()
    {
        if (isStunned || isDead) return;
        
        // 좌우 레인 변경 (키보드 입력 예시: A=왼쪽, D=오른쪽)
        if (Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);

        // 레인 보간 이동 (x축만 움직임)
        Vector3 lanePos = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
        rb.position = Vector3.MoveTowards(rb.position, lanePos, laneChangeSpeed * Time.deltaTime);

        // 애니메이션: 항상 달리는 상태
        animator.SetInteger("animation", 18);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }

        if (!isGrounded && !hasJumped)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= 3f) // 🔹 3초 이상 떨어지면
            {
                GameOver();
            }
        }
        else
        {
            fallTimer = 0f; // 바닥에 닿으면 초기화
        }
    }
    private void GameOver()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        animator.SetTrigger("Die"); // 죽는 애니메이션 실행

        if (gameOverUI != null)
            gameOverUI.SetActive(true); // 🔹 게임오버 UI 표시
    }


    public void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, 0, 2); // 0~2 범위 제한
        if (newLane != currentLane)
        {
            currentLane = newLane;
            UpdateTargetPosition();
        }
    }
    void UpdateTargetPosition()
    {
        // 레인별 X 좌표 계산 (왼 -3, 중 0, 오 +3 같은 구조)
        float xPos = (currentLane - 1) * laneOffset;
        targetPosition = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    public void ApplyStun(float duration)//스턴지속
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetInteger("animation", 34);
        // 스턴 이펙트 표시
        PlayStunEffect(duration);
        Debug.Log($"[Player] 스턴 상태 ({duration}초)");
        yield return new WaitForSeconds(duration);

        isStunned = false;
        StopStunEffect();
        Debug.Log("[Player] 스턴 해제");
    }
    private void StopStunEffect()
    {
        if (activeStunEffect != null)
        {
            Destroy(activeStunEffect);
            activeStunEffect = null;
        }
    }
    public void ApplyBlind(float duration)//시야가려짐
    {
        StartCoroutine(BlindCoroutine(duration));
    }
    IEnumerator BlindCoroutine(float duration)
    {
        if (blindOverlay != null) blindOverlay.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (blindOverlay != null) blindOverlay.SetActive(false);
    }



    void Jump()
    {

        if (isDead || isStunned || hasJumped) return;

        hasJumped = true;
        isGrounded = false;
        animator.SetTrigger("Jump");

        Vector3 start = transform.position;
        RaycastHit hit = new RaycastHit();
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        if (isBlocked || Physics.Raycast(rayOrigin, Vector3.forward, out hit, 5f, LayerMask.GetMask("Obstacle")))
        {
            // 장애물 정보가 있으면 높이 계산
            float dynamicHeight = obstacleJumpHeight;       // 기본 높이
            float dynamicDuration = obstacleJumpDuration;   // 기본 시간
            Vector3 end = start + Vector3.forward; // 기본 착지 위치 (앞으로 살짝만)

            if (hit.collider != null) // 실제 장애물 검출된 경우
            {
                Bounds b = hit.collider.bounds;
                float topY = b.max.y;

                // 필요한 높이 계산 (최소 obstacleJumpHeight 보장)
                float need = (topY - transform.position.y) + 0.6f;
                dynamicHeight = Mathf.Max(obstacleJumpHeight, need);

                //  착지 지점 = 장애물 "앞/뒤 모서리" 기준으로 잡기
                float landingSide = (landingOffsetZ >= 0) ? b.max.z : b.min.z;

                // offset 비율만큼 더하기 (예: -0.3f → 뒤쪽 30%)
                float zOffset = (b.extents.z * Mathf.Abs(landingOffsetZ));

                float targetZ = (landingOffsetZ >= 0)
                    ? landingSide + zOffset   // 앞쪽 착지
                    : landingSide - zOffset;  // 뒤쪽 착지

                end = new Vector3(
                    transform.position.x,
                    topY + 0.05f,  // 살짝 띄워서 착지
                    targetZ
                );
            }

            Debug.Log($"[장애물/막힘 점프] 높이 {dynamicHeight}, 시간 {dynamicDuration}");
            StartCoroutine(BezierJump(start, end, dynamicHeight, dynamicDuration));
            return;
          
        }



        Vector3 normalEnd = start + Vector3.forward * 10f;
        Debug.Log($"[일반 점프] 높이 {normalJumpHeight}, 시간 {normalJumpDuration}");
        StartCoroutine(BezierJump(start, normalEnd, normalJumpHeight, normalJumpDuration));
    }
    private bool isJumping = false;
    IEnumerator BezierJump(Vector3 start, Vector3 end, float height, float duration)
    {
        isJumping = true;
        // 러너 로직/중력 전부 정지
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        float elapsed = 0f;

        // 베지에 컨트롤 포인트 (포물선 모양)
        Vector3 control = (start + end) / 2f + Vector3.up * (height * 1.5f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 🔹 Quadratic Bezier 공식: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            rb.MovePosition(pos);
            yield return null;
        }
            if (Physics.Raycast(end + Vector3.up * 2f, Vector3.down, out RaycastHit groundHit, 5f, LayerMask.GetMask("Obstacle", "Untagged")))
            {
                Vector3 groundPos = groundHit.point + Vector3.up * 0.05f;
                rb.MovePosition(groundPos);
            }


            isGrounded = true;
            hasJumped = false;
            isJumping = false;
            rb.isKinematic = false;
    }
    IEnumerator ParabolaJump(Vector3 start, Vector3 end, float height, float duration)
    {
        rb.isKinematic = true; // 물리 끄기
        yield return new WaitForFixedUpdate();
        rb.isKinematic = false;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 🔹 XZ는 그대로 선형 보간
            Vector3 pos = Vector3.Lerp(start, end, t);

            // 🔹 Y는 (start.y ~ end.y) 선형 보간 + 포물선 높이
            float baseY = Mathf.Lerp(start.y, end.y, t);
            float parabola = 4 * height * t * (1 - t); // 최대 height까지 뜸
            pos.y = baseY + parabola;

            rb.MovePosition(pos);
            yield return null;
        }

        isGrounded = true;
        hasJumped = false;
    }
    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        // 🔹 아래 방향 레이캐스트
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 2f))
        {
            // 바닥이나 장애물 위
            if (hit.collider.CompareTag("Untagged") || hit.collider.CompareTag("Obstacle"))
            {
                isGrounded = true;
                return;
            }
        }

        // 바닥 없으면 낙하 시작
        isGrounded = false;
    }
    // 점프 직후 추가 힘을 적용하는 코루틴
    private IEnumerator ApplyExtraJumpForce()
    {
        yield return new WaitForFixedUpdate(); // 한 프레임 대기

        // 추가 상승 힘 적용
        if (!isGrounded && rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector3.up * (jumpForce * 0.5f), ForceMode.Impulse);
        }
    }



    private void OnCollisionStay(Collision collision)
    {
        if (rb == null) return; // Rigidbody 없으면 무시
        foreach (ContactPoint contact in collision.contacts)
        {
            // 위에서 닿았고, 실제로 거의 내려오는 중일 때만 착지 처리
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f && rb.linearVelocity.y <= 0.1f)
            {
                isGrounded = true;
                hasJumped = false;
                return;
            }
        }
    }
    private void CheckBlocked()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f; // 캐릭터 중간 높이
        if (Physics.Raycast(origin, Vector3.forward, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                isBlocked = true;
                obstacleHeight = hit.collider.bounds.size.y;
                return;
            }
        }
        isBlocked = false;
        obstacleHeight = 0f;
    }
    void FixedUpdate()
    {
        if (isDead) return;

        if (isStunned)
        {
            rb.linearVelocity = Vector3.zero;  // 스턴 동안 아예 정지
            return; // 아래 이동 로직 실행 안 함
        }
        if (rb.isKinematic) return;

        // 앞으로 전진
        Vector3 vel = rb.linearVelocity;

        if (isBlocked)
        {
            vel.z = 0f; // 막혔으면 앞으로 안 나가게 고정
        }
        else
        {
            vel.z = runSpeed; // 평소엔 앞으로 진행
        }
        vel.x = 0;          // 좌우는 레인 이동으로 제어
        rb.linearVelocity = vel; // y는 그대로 유지 (점프 값 살림)

        // 좌우 이동
        Vector3 lanePos = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
        rb.MovePosition(Vector3.MoveTowards(rb.position, lanePos, laneChangeSpeed * Time.fixedDeltaTime));

        // 중력 가속 보정
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        
        CheckGround();
        SnapToGround();
        StickToGround();
    }
    private void SnapToGround()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, Vector3.down, out hit, 2f, LayerMask.GetMask("Untagged", "Obstacle")))
        {
            float groundY = hit.point.y;

            // 작은 단차(예: 0.5m 이하)는 자동 스냅
            if (rb.position.y - groundY < 0.5f)
            {
                Vector3 pos = rb.position;
                pos.y = Mathf.Lerp(rb.position.y, groundY + 0.01f, 0.5f); // 부드럽게
                rb.MovePosition(pos);
                isGrounded = true;
                hasJumped = false;
            }
        }
    }
    private void StickToGround()
    {
        // 점프 중이면 무시
        if (hasJumped) return;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, LayerMask.GetMask("Untagged", "Obstacle")))
        {
            float distance = transform.position.y - hit.point.y;

            if (distance > 0.05f && distance < 1.0f)
            {
                Vector3 pos = rb.position;
                pos.y = hit.point.y + 0.01f;
                rb.MovePosition(pos);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                isGrounded = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isBlocked = false;  // 막힘 해제
            Debug.Log("Obstacle 막힘 해제!");
        }
        // 또는 태그 없이 일반적인 충돌체에서 벗어날 때도 해제
        StartCoroutine(DelayedBlockCheck());
    }
    // 약간의 딜레이 후 블록 상태 재확인
    private IEnumerator DelayedBlockCheck()
    {
        yield return new WaitForSeconds(0.1f);

        // 주변에 장애물이 없으면 블록 해제
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 1.5f);
        bool hasNearbyObstacle = false;

        foreach (Collider col in nearbyColliders)
        {
            if (col != GetComponent<Collider>() && !col.isTrigger)
            {
                Vector3 direction = (transform.position - col.transform.position).normalized;
                if (Vector3.Dot(direction, Vector3.forward) < 0.3f)
                {
                    hasNearbyObstacle = true;
                    break;
                }
            }
        }

        if (!hasNearbyObstacle)
        {
            isBlocked = false;
        }
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (!rb.isKinematic)
            rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        animator.SetTrigger("Die");

        StartCoroutine(RespawnAfterDelay(1.9f));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnInstant();
        isDead = false;
    }
    public void OnDieAnimationEnd()
    {
        // 애니메이션 끝났을 때 호출됨
        RespawnInstant();
    }


    void RespawnInstant()
    {
        Vector3 pos = respawnPoint.position;
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out hit, 20f))
        {
            pos.y = hit.point.y + 0.01f;
        }
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;              // 물리 잠깐 끄기
        rb.MovePosition(pos);               // transform.position 대신 이거!
        StartCoroutine(ReenablePhysics());  //

        // Idle 상태로 되돌리기
        animator.ResetTrigger("Die");      // 트리거 초기화
        animator.SetInteger("animation", 34); // Idle 애니메이션 실행
        animator.Play("Idle", 0, 0f);

        isDead = false;
    }
    public void RespawnAt(Transform respawnPoint)
    {
        Vector3 pos = respawnPoint.position + Vector3.up * 5f;
        RaycastHit hit;

        if (Physics.Raycast(pos, Vector3.down, out hit, 20f))
        {
            pos = hit.point + Vector3.up * 0.1f; // 바닥 위
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;              // 물리 잠깐 끄기
        rb.MovePosition(pos);               // 여기서도 MovePosition
        StartCoroutine(ReenablePhysics());

        animator.ResetTrigger("Die");
        animator.SetInteger("animation", 34);
        animator.Play("Idle", 0, 0f);

        isDead = false;
        // 무적 시간 1초 시작
        invincibleTimer = 1f;
    }
    private IEnumerator ReenablePhysics()
    {
        yield return new WaitForFixedUpdate(); // 물리 프레임 한 번 기다린 뒤
        rb.isKinematic = false;                // 다시 활성화
    }
    private void OnTriggerEnter(Collider other)
    {
        if (invincibleTimer > 0f)
            return; // 무적 상태라면 충돌 무시

    }
    private IEnumerator AnimateSpeedShader(float targetValue, float duration)
    {
        if (speedEffectMat == null) yield break;
        float startValue = speedEffectMat.GetFloat("Distortion");
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, time / duration);
            speedEffectMat.SetFloat("Distortion", newValue);

            Debug.Log($"[Shader Distortion] {newValue}"); // 👈 값 변화 확인
            yield return null;
        }
        speedEffectMat.SetFloat("Distortion", targetValue);
    }
}
