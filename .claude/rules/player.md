# 플레이어 세팅 규칙

## 위치와 콜라이더 정합성
- CapsuleCollider center=(0,0,0)일 때 발 위치 = position.y - height/2
- **도로 윗면 y와 발 y가 반드시 일치**해야 한다 — 역산해서 position.y를 결정
- 위치를 임의로 설정하면 공중에 뜨거나 바닥에 묻힌다
- 씬에 저장된 Rigidbody 값과 Start()에서 설정하는 값이 다를 수 있다 — 씬 저장 값도 반드시 동기화

## Ground Check
- SphereCast/Raycast의 시작점과 거리가 실제 발 위치에서 바닥까지 닿는지 **수치로 검증**한다
- 예: 시작 y=1.5, 거리=0.8, 반지름=0.3 → 바닥 도달 = 1.5-0.8-0.3 = 0.4 → y=0에 안 닿음 (실패)
- `col.bounds.min.y`는 월드 좌표 기준 캡슐 바닥 — 이것을 기준으로 사용하는 것이 안전

## 점프 높이
- 최대 높이 = v² / (2 × g × gravityMultiplier). 상승 중 추가 중력이 있으면 분모에 포함
- 상승 중 추가 중력 없이 하강만 강화하면 → 체공 시간이 길어 실제보다 높아 보인다
- "머리 높이만큼 점프" = 발이 약 0.5~1유닛 상승 (캐릭터 전체 높이가 아님)

## 3D 모델 & 애니메이션
- 플레이어 비주얼은 Player 자식 `PlayerModel`에 FBX를 배치한다 (Player 자체에는 콜라이더/Rigidbody만)
- Animator는 PlayerModel에 부착, PlayerController에서 `GetComponentInChildren<Animator>()`로 참조
- FBX 애니메이션 클립은 기본적으로 **Loop Time이 꺼져있다** — 달리기/걷기는 반드시 Loop Time을 켜야 한다 (안 하면 클립 한 번 재생 후 정지)
- 점프 애니메이션은 Loop Time OFF, 대신 **재생 속도를 실제 물리 체공 시간에 맞춘다**
  - 체공 시간 = 상승(v / g×upMult) + 하강(√(2h / g×fallMult))
  - 속도 배율 = 애니메이션 길이 / 체공 시간

## Animator 전환 규칙
- Jump → Run 전환을 반드시 추가한다 (Jump → Idle만 있으면 착지 후 Idle을 거치며 끊김 발생)
- Jump → Run: `has_exit_time=false`, 조건 `IsGrounded=true AND Speed>0.1`
- Jump → Idle: `has_exit_time=false`, 조건 `IsGrounded=true AND Speed<0.1`
- AnyState → Jump: Trigger 기반, `has_exit_time=false`

## 모델 회전 (이동 방향 바라보기)
- 모델 회전은 반드시 **LateUpdate**에서 처리한다 — Update에서 하면 Animator가 덮어쓴다
- 실제 프레임별 이동 속도 `(currentLateralSpeed, 0, currentSpeedZ)`로 방향을 계산한다
- `MoveToLane()`에서 `(현재X - 이전X) / deltaTime`으로 lateralSpeed를 구한다
- 이동 없을 때는 `Quaternion.identity`로 서서히 정면 복귀

## 죽음 처리 (Dying → GameOver)
- 점프 중 피격 시 즉시 GameOver가 아니라 `Dying` 상태로 전환 → 착지 후 GameOver
- Dying 중: 입력 비활성, 중력만 적용, 이동 속도 감속
- 착지 감지 시 `GameManager.PlayerLanded()` 호출 → GameOver + `Time.timeScale = 0`
- `Time.timeScale = 0`이므로 UI 애니메이션은 `Time.unscaledDeltaTime` 사용 필수
- Restart 시 `Time.timeScale = 1f` 복구 필수

## Input System
- `com.unity.inputsystem` 패키지가 있으면 `Input.GetKey()` 등 구 API가 동작하지 않는다
- `ProjectSettings.asset`의 `activeInputHandler: 2` (Both)로 설정 필요
- 이 변경은 Unity 에디터 재시작이 필요하다
