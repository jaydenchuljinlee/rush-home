---
status: fixed
---
# Bug: F-05 GameOver 시 NullReferenceException (GameUIController)

## 발견일
2026-04-21

## 심각도
High

## 증상
`GameManager.GameOver()` 호출 시 `GameUIController.HandleGameStateChanged` (line 76)에서 NullReferenceException 발생.

```
NullReferenceException: Object reference not set to an instance of an object
  at GameUIController.HandleGameStateChanged (GameState state) [line 76]
  at GameManager.ChangeState (GameState newState) [line 94]
  at GameManager.GameOver ()
```

## 원인
`GameUIController.cs:76`:
```csharp
case GameState.GameOver:
    ...
    if (finalTimeText != null)
        finalTimeText.text = FormatTime(GameManager.Instance.ElapsedTime); // <- 여기서 Instance가 null
```

`GameManager.Instance`가 null인 상태에서 `HandleGameStateChanged`가 호출될 때 발생한다.

## 재현 조건
- Phase 2 (일반 모드) play_game 후 execute_script로 `gm.StartGame()` 직접 호출
- 이 경우 GameManager.Instance는 `null` (execute_script는 Editor thread, Instance는 PlayMode runtime)
- `GameManager.ChangeState()` → `OnGameStateChanged?.Invoke(state)` → `GameUIController.HandleGameStateChanged` 호출
- `HandleGameStateChanged` 내부에서 `GameManager.Instance`가 null → NullReferenceException

## 런타임 재현 가능성
정상 플레이 시에도 드물게 발생 가능. `GameManager.OnEnable()`에서 PlayerController.OnPlayerHit를 구독하고, `HandlePlayerHit()`에서 `GameOver()`를 호출하는 경로는 동일. 단, 정상 Play 시에는 Instance가 항상 설정되어 있으므로 일반적으로는 발생하지 않음.

## 수정 방향
`GameUIController.cs` line 76:
```csharp
// Before
finalTimeText.text = FormatTime(GameManager.Instance.ElapsedTime);

// After
if (GameManager.Instance != null)
    finalTimeText.text = FormatTime(GameManager.Instance.ElapsedTime);
```

또는 `HandleGameStateChanged`의 GameOver 케이스에서 ElapsedTime을 파라미터로 받지 않고 이벤트 발생 전에 값을 캡처하도록 수정.

## 관련 파일
- `Assets/Scripts/UI/GameUIController.cs` (line 75-76)
