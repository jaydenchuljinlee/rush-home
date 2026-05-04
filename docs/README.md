# 퇴근길 서바이벌 문서 인덱스

이 폴더는 `RushHome` 프로젝트의 초기 기획과 단계별 구현 계획을 관리한다.
현재 방향은 완성 에셋보다 와이어프레임/그레이박스 플레이 검증을 우선한다.

현재 MVP 목표는 하나의 `GameScene`에서 거리 기준으로 Stage 1-4를 순차 진행하고, 집의 침대 트리거에 도달해 결과 화면까지 보는 것이다.

## 문서 구성

- [00_GAME_PLAN.md](./00_GAME_PLAN.md): 전체 게임 기획, 보완 제안, 개발 원칙
- [01_FOUNDATION.md](./01_FOUNDATION.md): 프로젝트 기반, 플레이어, 카메라, 게임 상태
- [02_CORE_LOOP.md](./02_CORE_LOOP.md): 3레인 이동, 장애물, 난이도, 점수 루프
- [03_STAGE_OFFICE.md](./03_STAGE_OFFICE.md): Stage 1 사무실 기획 및 구현
- [04_STAGE_LOBBY.md](./04_STAGE_LOBBY.md): Stage 2 엘리베이터/로비 기획 및 구현
- [05_STAGE_COMMUTE.md](./05_STAGE_COMMUTE.md): Stage 3 지하철/도심 기획 및 구현
- [06_STAGE_HOME_AND_POLISH.md](./06_STAGE_HOME_AND_POLISH.md): Stage 4 집, 연출, 사운드, 출시 전 정리

## 권장 진행 순서

1. `01_FOUNDATION.md`로 씬과 공통 시스템을 안정화한다.
2. `02_CORE_LOOP.md`로 반복 플레이가 가능한 최소 재미를 만든다.
3. `03_STAGE_OFFICE.md`부터 `06_STAGE_HOME_AND_POLISH.md`까지 스테이지를 순차 확장한다.
4. 모든 단계에서 프리미티브, 단색 머티리얼, 라벨 오브젝트를 먼저 사용한다.
5. 재미와 동선이 검증된 뒤 실제 이미지, 모델, 애니메이션, 사운드를 교체한다.
