---
status: fixed
---
# Bug: F-06 Extreme 구간 임계값 불일치

## 발견일
2026-04-21

## 심각도
Medium

## 증상
play-suite.md에 명시된 난이도 구간과 실제 DifficultyData 에셋의 `extremeThreshold` 값이 불일치한다.

| 구간 | play-suite.md 기대값 | 실제 값 |
|------|----------------------|---------|
| Easy | 0~29초 | 0~29초 (일치) |
| Normal | 30~59초 | 30~59초 (일치) |
| Hard | 60~119초 | 60~89초 (불일치) |
| Extreme | 120초~ | 90초~ (불일치) |

## 재현 방법
1. play-suite.md F-06 항목 확인: Hard 구간이 60~119초, Extreme이 120초~로 명시
2. `Resources.FindObjectsOfTypeAll<DifficultyData>()` 로 에셋 접근
3. `extremeThreshold` 필드 값 확인: **90** (기대값 120)

## 검증 데이터 (execute_script 결과)
```
t=  60: speed=11, spawnInterval=(0.9, 2)   <- Hard 시작
t=  89: speed=11, spawnInterval=(0.9, 2)   <- Hard 구간 내
t=  90: speed=15, spawnInterval=(0.6, 1.5) <- Extreme 시작 (기대: 120초)
t= 120: speed=15, spawnInterval=(0.6, 1.5) <- Extreme (이미 90에서 시작됨)
extremeThreshold = 90
```

## 관련 파일
- `Assets/Resources/DifficultyData.asset` 또는 `Assets/ScriptableObjects/DifficultyData.asset`
- `Assets/Scripts/Data/DifficultyData.cs`

## 수정 방향
두 가지 중 하나를 선택:
1. DifficultyData 에셋의 `extremeThreshold`를 **90 → 120**으로 변경 (play-suite.md 기준 준수)
2. play-suite.md의 Hard/Extreme 구간 기대값을 실제 값(90초)으로 업데이트

설계 의도를 확인하여 어느 쪽이 정확한지 결정 필요.
