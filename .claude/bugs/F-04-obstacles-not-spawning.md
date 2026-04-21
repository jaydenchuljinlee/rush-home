---
feature: F-04
status: open
severity: major
created: 2026-04-21
---

# 장애물이 스폰되지 않음

## 증상
게임 Playing 상태에서 12초 이상 경과 후에도 ObstaclePool 하위에 스폰된 장애물 인스턴스가 0개임.
씬에는 GroundObstacle, AirObstacle이 루트 레벨에 프리팹 템플릿으로만 존재하고, 실제 스폰된 클론이 없음.

## 에러 로그
현재 세션(SuiteAutoStarter 사용 후)에서 장애물 관련 에러 없음.

## 오브젝트 상태
- **ObstaclePool**: children 없음 (풀에 인스턴스 없음)
- **ObstacleSpawner**: 컴포넌트만 존재, 스폰 없음
- **GroundObstacle** / **AirObstacle**: 루트 레벨에만 존재 (프리팹 레퍼런스용 씬 배치 오브젝트)
- 비활성 포함 검색 시에도 ObstaclePool 하위 인스턴스 0개

## 스크린샷
타이머 00:11.02, Playing 상태 12초 경과 시점. 화면에 장애물 없음.

## 판정 기준 위반
- 기대: 씬에 GroundObstacle 또는 AirObstacle 인스턴스 1개 이상 존재 (5초 대기 후)
- 실제: 인스턴스 0개

## 추정 원인
1. ObstacleSpawner가 GameManager.Instance.CurrentState를 체크하는데, 이전 세션의 GameManager.Instance null 이슈로 스폰 로직이 한 번도 실행되지 않았을 수 있음
2. ObstaclePool의 프리팹 레퍼런스가 Inspector에서 할당되지 않았을 수 있음 (GroundObstacle/AirObstacle이 풀의 Prefabs 필드에 연결되어 있지 않음)
3. ObstacleSpawner의 spawnInterval 초기화 문제 또는 조건 미충족

## 수정 방향
1. ObstaclePool Inspector에서 groundObstaclePrefab, airObstaclePrefab 필드에 씬의 GroundObstacle/AirObstacle 프리팹 연결 여부 확인
2. ObstacleSpawner.cs의 스폰 조건 로직 확인 및 수정
3. 씬의 GroundObstacle/AirObstacle을 프리팹으로 전환 후 올바른 위치에 참조 설정
