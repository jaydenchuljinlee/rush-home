# Bug Report: F-05 ReadyPanel 미숨김 + GameOverPanel 하위 오브젝트 누락

## 발견일
2026-04-21

## 심각도
MEDIUM

## 재현 환경
- 브랜치: feature/difficulty-curve
- 씬: Assets/Scenes/GameScene.unity

---

## 버그 1: Playing 상태에서 ReadyPanel "START" 텍스트가 계속 표시됨

### 증상
- GameManager.StartGame() 호출 후 Playing 상태로 전환되어 타이머가 진행되는데도
  화면에 "START" 텍스트가 계속 표시된다.
- 타이머 00:18.33, 00:47.42 시점 스크린샷에서 모두 "START" 텍스트 확인됨.

### 관련 코드
`Assets/Scripts/UI/GameUIController.cs:57`
```csharp
case GameState.Playing:
    if (readyPanel != null) readyPanel.SetActive(false);  // 동작 안함
```

### 예상 원인
- `ReadyPanel` 오브젝트 자체는 비활성화되지만 `ReadyPanel/ReadyText` (TextMeshProUGUI)가
  별도 오브젝트로 존재하지 않고 Canvas 루트에 직접 붙어 있을 가능성.
- 또는 `GameUIController.readyPanel` 참조가 ReadyPanel 전체가 아닌 Image 컴포넌트만
  참조하고 있을 가능성.
- Play Mode 중 get_game_object_info 결과: ReadyPanel IsActive=true (Playing 상태임에도)

### 판정
FAIL

---

## 버그 2: GameOverPanel 하위 오브젝트(FinalTimeText, RestartButton) 누락

### 증상
- `Canvas/GameOverPanel` 오브젝트는 존재하나 하위에 자식 오브젝트가 없음.
- play-suite.md 판정 기준: `FinalTimeText 값 > 0 + RestartButton 존재` — 모두 미존재.
- GameUIController.cs에서 `finalTimeText`, `restartButton` 참조가 있으나
  씬에 해당 오브젝트가 없어 null 상태.

### 관련 오브젝트 (누락)
- `Canvas/GameOverPanel/FinalTimeText` (TextMeshProUGUI)
- `Canvas/GameOverPanel/RestartButton` (Button)

### 판정
FAIL

---

## 재현 방법
1. GameScene 열기
2. Play Mode 진입
3. GameManager.StartGame() 호출
4. 화면에서 "START" 텍스트가 사라지지 않음 확인
5. Canvas/GameOverPanel 하위 오브젝트 없음 확인

## 수정 방향
1. GameUIController의 readyPanel 참조 및 ReadyText 오브젝트 구조 점검
2. Canvas/GameOverPanel 하위에 FinalTimeText, RestartButton 오브젝트 추가
3. GameUIController Inspector에서 참조 재할당
