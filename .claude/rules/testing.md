# 테스트 규칙

## 테스트 프레임워크
- Unity Test Framework (NUnit 기반)
- Assembly Definition 필요: `Tests/EditMode/`, `Tests/PlayMode/`

## 테스트 유형

| 유형 | 위치 | 용도 | 실행 조건 |
|---|---|---|---|
| Edit Mode | `Assets/Tests/EditMode/` | 순수 C# 로직, ScriptableObject | Unity 에디터 불필요 |
| Play Mode | `Assets/Tests/PlayMode/` | MonoBehaviour, 물리, 씬 통합 | Unity 에디터 필요 |

## Edit Mode 테스트 대상
- 순수 계산 로직 (점수 계산, 난이도 곡선, 데이터 검증)
- ScriptableObject 데이터 무결성
- 유틸리티 클래스
- 상태 머신 전이 로직

## Play Mode 테스트 대상
- 플레이어 이동/점프/슬라이딩 동작
- 충돌 감지 (장애물 히트 판정)
- 오브젝트 풀링 스폰/반환
- UI 상호작용
- 씬 전환

## 테스트 작성 원칙
- `[Test]` 메서드명은 한글로, `성공_` / `실패_` 접두사 사용
- AAA 패턴: Arrange -> Act -> Assert
- 하나의 테스트는 하나의 동작만 검증
- MonoBehaviour 테스트는 `[UnityTest]`로 코루틴 기반 실행
- 프레임 대기가 필요하면 `yield return null` 또는 `yield return new WaitForFixedUpdate()`

## 테스트 실행

```bash
# Unity 에디터 내
# Window > General > Test Runner

# CLI (Unity 배치모드)
Unity -batchmode -runTests -testPlatform EditMode -projectPath . -testResults results.xml
Unity -batchmode -runTests -testPlatform PlayMode -projectPath . -testResults results.xml
```

## 테스트 예시 구조

```csharp
// Edit Mode
[TestFixture]
public class DifficultyCalculatorTest
{
    [Test]
    public void 성공_30초경과시_보통난이도_반환()
    {
        // Arrange
        var calculator = new DifficultyCalculator();
        // Act
        var result = calculator.GetDifficulty(30f);
        // Assert
        Assert.AreEqual(Difficulty.Normal, result);
    }
}

// Play Mode
[UnityTest]
public IEnumerator 성공_점프시_플레이어가_지면에서_떨어짐()
{
    // Arrange - 플레이어 프리팹 인스턴스 생성
    // Act - 점프 입력
    yield return new WaitForSeconds(0.5f);
    // Assert - Y 위치 확인
}
```
