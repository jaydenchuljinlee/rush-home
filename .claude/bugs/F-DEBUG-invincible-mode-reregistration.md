---
status: fixed
---
# Bug: F-DEBUG 무적 모드 ON 후 OnPlayerHit 재등록 문제

## 발견일
2026-04-21

## 심각도
Low (에디터 전용 기능)

## 증상
SuiteAutoStarter에서 `PlayerController.OnPlayerHit` 필드를 null로 설정하여 무적 모드를 활성화해도, 이후 DebugCheatManager 검증 시 OnPlayerHit backing field가 **non-null**로 확인됨.

## 재현 방법
1. play_game 후 SuiteAutoStarter 부착 (무적 ON: OnPlayerHit = null)
2. 수초 후 SuiteDebugCheck.Execute() 실행
3. 결과: `PlayerController.OnPlayerHit backing field = non-null`

## 원인 분석
`GameManager.cs:43`:
```csharp
private void OnEnable()
{
    PlayerController.OnPlayerHit += HandlePlayerHit;
}
```

GameManager의 `OnEnable()`이 SuiteAutoStarter의 `Start()` 이전에 실행된다:
- `GameManager.OnEnable()` → `OnPlayerHit += HandlePlayerHit` (등록)
- `SuiteAutoStarter.Start()` → `OnPlayerHit = null` (제거)

그러나 이후 다른 컴포넌트의 OnEnable 또는 PlayMode 재활성화 과정에서 재등록될 가능성이 있다. 정확한 재등록 시점은 추가 조사 필요.

## 영향
- 무적 모드가 의도한 대로 동작하지 않을 수 있음
- Phase 1 suite 실행 중 장애물 충돌 시 GameOver가 발생할 수 있음
- Phase 1 스크린샷에서 장애물이 플레이어를 통과하는 것이 확인되어 실제로는 무적이 유지됐을 가능성도 있음 (물리 충돌은 정상, 이벤트만 제거)

## 수정 방향
SuiteAutoStarter에서 null 설정 후 GameManager의 OnEnable 재등록을 막으려면:
- SuiteAutoStarter.Start()에서 `field.SetValue(null, null)` 호출 시점을 늦추거나
- `DebugCheatManager`의 무적 토글 로직과 연동하거나
- GameManager의 OnEnable에서 SuiteAutoStarter 존재 시 등록을 스킵하는 조건 추가

## 관련 파일
- `Assets/Scripts/Utils/SuiteAutoStarter.cs`
- `Assets/Scripts/Managers/GameManager.cs` (OnEnable)
- `Assets/Scripts/Debug/DebugCheatManager.cs`
