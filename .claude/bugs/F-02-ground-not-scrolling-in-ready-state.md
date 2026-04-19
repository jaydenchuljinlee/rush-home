---
feature: F-02
status: resolved
resolution: 테스트 인프라 수정 — suite에 GameManager.StartGame() 자동 호출 추가
severity: minor
created: 2026-04-19
---

# 달리는 지면 — Ready 상태에서 스크롤 미동작

## 증상
play_game 실행 후 13초간 플레이했으나 GroundTile_0의 localPosition.x가
0.0 → 0.0으로 변화 없음. 지면이 스크롤되지 않음.

## 에러 로그
없음 (이번 세션 신규 에러 없음)

## 오브젝트 상태
- GroundScroller: Transform 컴포넌트만 존재 (GroundScroller 스크립트 확인 필요)
- GroundTile_0.localPosition.x: 0.0 (플레이 전후 동일)
- GroundTile_1, GroundTile_2 미확인

## 스크린샷
캡처 시점(5초, 13초) 화면 동일 — 지면 타일 세그먼트 경계선 위치 변화 없음

## 판정 기준 위반
- 기대: Playing 상태에서 GroundScroller 하위 오브젝트의 X 위치가 변화
- 실제: 게임이 Ready 상태를 유지하여 지면 스크롤 미발생

## 근본 원인 추정
suite 테스트가 게임 시작 입력(Space)을 주입하지 않아 GameManager가
Ready 상태에 머물고, GroundScroller가 Playing 상태에서만 동작하므로
스크롤이 발생하지 않음. 게임 자체의 버그가 아닌 테스트 자동화 한계일 수 있음.

## 수정 방향
1. GroundScroller가 Ready 상태에서도 스크롤되는지 코드 확인
2. suite 테스트에 게임 시작 트리거(execute_script로 Space 입력) 추가 검토
3. play-suite.md 판정 기준을 "Playing 상태 전환 후" 조건으로 업데이트
