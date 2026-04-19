# HomeRun 프로젝트 진행 현황

> 마지막 업데이트: 2026-04-19

---

## 현재 환경

| 항목 | 상태 |
|---|---|
| macOS | 15.7.4 (Sequoia), Apple Silicon (arm64) |
| RAM | 16 GB |
| 디스크 여유 | 약 145 GB |
| Git | 2.39.5 |
| Git LFS | 3.7.1 |
| Homebrew | 설치됨 |
| Node.js | v24.10.0 |
| Claude Code | 2.1.42 |
| gh CLI | 2.50.0 |
| IDE | JetBrains Rider |
| Python | 3.11.8 (Anaconda) |
| uvx | 0.11.7 |
| Unity Hub | 설치 완료 |
| Unity Editor | Unity 6 LTS 설치 완료 |
| Coplay MCP | 연동 완료 |

---

## 개발 환경 세팅 진행 상황

### 완료

- [x] GitHub 리포지토리 생성 (`rush-home`)
- [x] Git LFS 설치 및 초기화 (`brew install git-lfs && git lfs install`)
- [x] uvx 설치 (`brew install uv` -- Coplay MCP 의존성)
- [x] Unity 전용 `.gitignore` 생성 (Library/, Temp/, Builds/ 등 제외)
- [x] Git LFS `.gitattributes` 생성 (png, wav, mp3, fbx 등 바이너리 에셋 추적)
- [x] `.claude/` 에이전트/규칙/커맨드를 Unity 프로젝트에 맞게 재구성
  - agents: feature-planner, feature-implementer, test-runner, code-validator, feature-orchestrator, test-failure-analyzer
  - rules: architecture.md, implementation.md, testing.md, build.md
  - commands: feature-maker.md
- [x] `.claude/plans/` 디렉토리 생성

### 대기 (수동 설치 필요)

- [x] **Unity Hub 설치** -- brew install --cask unity-hub
- [x] **Unity 6 LTS 설치** -- Unity Hub > Installs > Install Editor
- [x] **HomeRun Unity 프로젝트 생성** -- Unity Hub > New Project > Universal 2D
- [x] **Rider 연결** -- Unity > Settings > External Tools > Rider
- [x] **Coplay MCP 등록**
- [ ] **Unity에 Coplay 패키지 설치** -- Package Manager > Add from git URL:
  ```
  https://github.com/CoplayDev/unity-plugin.git#beta
  ```
- [ ] **PlayFab 계정 생성** -- https://playfab.com

---

## Feature 로드맵

### 기반

| # | Feature | 상태 | 완료 기준 |
|---|---------|------|----------|
| F-01 | 씬 기초 세팅 | 완료 | 플레이 시 에러 없이 Ready 상태 대기 |
| F-02 | 달리는 지면 | 완료 | 입력 시 지면이 끊김 없이 스크롤 |

### 게임플레이

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-03 | 플레이어 캐릭터 | F-02 | 완료 | Space로 점프, 아래로 슬라이딩 동작 |
| F-04 | 장애물 시스템 | F-03 | 완료 | 장애물 등장 + 충돌 시 게임오버 |
| F-06 | 난이도 곡선 | F-04 | 대기 | 30초/60초/120초마다 체감 난이도 상승 |

### UI

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-05 | 인게임 HUD / 게임오버 | F-04 | 완료 | 시작/플레이/게임오버 전체 흐름 동작 |
| F-09 | 메인메뉴 씬 | F-05 | 대기 | 메인메뉴에서 게임 씬으로 전환 |

### 디자인

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-07 | 배경 파랄랙스 | F-02 | 대기 | 3겹 배경이 서로 다른 속도로 스크롤 |
| F-08 | 캐릭터 애니메이션 | F-03 | 대기 | 상태별 애니메이션 재생 |

### 사운드

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-10 | 사운드 시스템 | F-05 | 대기 | 게임 시작 시 BGM, 액션/충돌 시 SFX |

### 서버

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-11 | PlayFab 리더보드 | F-05 | 대기 | 게임오버 시 순위 등록, TOP 10 조회 |

### 폴리싱

| # | Feature | 선행 | 상태 | 완료 기준 |
|---|---------|------|------|----------|
| F-12 | 모바일 폴리싱 | 전체 | 대기 | 모바일 디바이스에서 정상 플레이 |

---

## 의존성 다이어그램

```
F-01 씬 기초 세팅 ✅
 └── F-02 달리는 지면 ✅
      ├── F-03 플레이어 캐릭터 ✅
      │    ├── F-04 장애물 시스템 ✅
      │    │    ├── F-05 인게임 HUD / 게임오버 ✅
      │    │    │    ├── F-09 메인메뉴 씬
      │    │    │    ├── F-10 사운드 시스템
      │    │    │    └── F-11 PlayFab 리더보드
      │    │    └── F-06 난이도 곡선
      │    └── F-08 캐릭터 애니메이션
      └── F-07 배경 파랄랙스
 F-12 모바일 폴리싱 (전체 완료 후)
```

## 최소 플레이 가능 단위

**F-01 ~ F-05 완료 시**: 달리고, 점프/슬라이딩하고, 장애물에 부딪혀 게임오버, 재시작하는 완전한 게임 루프.

---

## 다음 할 일

1. F-06 난이도 곡선 구현
2. F-07 배경 파랄랙스
3. F-08 캐릭터 애니메이션
4. F-09 메인메뉴 씬
