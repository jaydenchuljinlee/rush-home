---
feature: F-04
status: resolved
resolution: 테스트 인프라 수정 — suite에 GameManager.StartGame() 자동 호출 추가
severity: minor
created: 2026-04-19
---

# 장애물 시스템 — Ready 상태에서 장애물 미스폰

## 증상
play_game 실행 후 13초간 플레이했으나 ObstaclePool 하위에 스폰된
장애물 인스턴스가 0개. GroundObstacle, AirObstacle은 씬 루트에
원본 오브젝트로만 존재.

## 에러 로그
없음 (이번 세션 신규 에러 없음)

## 오브젝트 상태
씬 내 Obstacle 관련 오브젝트:
- ObstaclePool (Transform, ObstaclePool) — 자식 없음
- ObstacleSpawner (Transform, ObstacleSpawner)
- GroundObstacle (씬 루트, 프리팹 원본)
- AirObstacle (씬 루트, 프리팹 원본)

## 스크린샷
13초 시점 화면 — 플레이어만 존재, 장애물 없음

## 판정 기준 위반
- 기대: 씬에 GroundObstacle 또는 AirObstacle 활성 인스턴스 1개 이상 존재
- 실제: 스폰된 인스턴스 0개 (프리팹 원본만 씬에 존재)

## 근본 원인 추정
ObstacleSpawner가 GameState.Playing 상태에서만 동작하므로,
게임이 Ready 상태를 유지하는 suite 자동화 환경에서 장애물이 스폰되지 않음.
게임 자체의 버그가 아닌 테스트 자동화 한계일 수 있음.

## 수정 방향
1. ObstacleSpawner.Update()의 GameState 조건 확인
2. suite 테스트에 게임 시작 트리거 추가 검토 (F-02와 동일 원인)
3. play-suite.md 판정 기준을 "Playing 상태 전환 후" 조건으로 업데이트
