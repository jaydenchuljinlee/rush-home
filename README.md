# HomeRun

> 퇴근 후 집에 도착하기까지, 이 도시의 온갖 장애물을 뚫고 가장 빠르게 귀가하라.

## 개요

**HomeRun**은 퇴근길을 배경으로 한 2D 사이드뷰 엔드리스/타임어택 러너 게임입니다.
플레이어는 회사에서 출발해 자동으로 달리며, 점프/슬라이딩/회피로 장애물을 극복하고 가장 빠르게 집에 도착하는 것이 목표입니다.

## 기술 스택

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6 LTS (2D URP) |
| 언어 | C# |
| 플랫폼 | 모바일 (iOS / Android) |
| 백엔드 | PlayFab (인증, 리더보드) |
| AI 보조 | Claude Code + Coplay MCP |
| 버전 관리 | Git + GitHub (Git LFS) |

## 핵심 기능 (MVP)

- 캐릭터 자동 달리기 / 점프 / 슬라이딩
- 랜덤 청크 기반 맵 생성
- 난이도 곡선 (시간 경과에 따라 상승)
- 장애물 3~5종
- 완주 시간 측정 + 글로벌 랭킹 (PlayFab)
- 기본 UI (메인메뉴, 게임오버, 결과 화면)

## 게임 플레이

### 조작
- **모바일**: 탭 = 점프, 아래 스와이프 = 슬라이딩
- **에디터**: Space/Up = 점프, Down = 슬라이딩

### 난이도
| 구간 | 난이도 | 설명 |
|---|---|---|
| 0~30초 | 쉬움 | 장애물 1~2개, 간격 넓음 |
| 30~60초 | 보통 | 장애물 3~4개, 복합 패턴 |
| 60~120초 | 어려움 | 연속 장애물, 정밀 타이밍 |
| 120초~ | 지옥 | 최대 속도, 패턴 예측 불가 |

## 프로젝트 구조

```
HomeRun/
├── Assets/
│   ├── Art/              # 스프라이트, 애니메이션, 배경
│   ├── Audio/            # BGM, SFX
│   ├── Prefabs/          # 프리팹 (청크, 장애물, 플레이어)
│   ├── Scenes/           # Main, Game, Result
│   ├── Scripts/
│   │   ├── Player/       # 플레이어 컨트롤러
│   │   ├── Level/        # 청크 스포너, 맵 생성
│   │   ├── Obstacles/    # 장애물 로직
│   │   ├── UI/           # UI 컨트롤러
│   │   ├── Managers/     # GameManager, AudioManager 등
│   │   ├── Data/         # ScriptableObject 데이터
│   │   ├── Network/      # PlayFab 연동
│   │   └── Utils/        # 유틸리티
│   └── Resources/
├── Packages/
└── ProjectSettings/
```

## 개발 로드맵

| Phase | 내용 | 상태 |
|---|---|---|
| 1 | 프로토타입 (이동/점프/장애물/게임오버) | 진행 중 |
| 2 | 핵심 시스템 (청크 맵, 난이도, 오브젝트 풀링) | 대기 |
| 3 | 아트 & UX (에셋, 애니메이션, UI, 사운드) | 대기 |
| 4 | 서버 연동 (PlayFab 인증/리더보드) | 대기 |
| 5 | 폴리싱 & 출시 준비 | 대기 |

---

## AI 에이전트 파이프라인

이 프로젝트는 **Claude Code + Coplay MCP** 기반의 자동화된 개발 파이프라인으로 구현됩니다.
사용자는 슬래시 커맨드로 파이프라인을 실행하고, 에이전트들이 계획 → 구현 → 테스트 → 검증을 자동 수행합니다.

### 전체 파이프라인 흐름도

```
                          User Commands
  ┌──────────────┬──────────────┬──────────────┬──────────────┐
  │              │              │              │              │
  ▼              ▼              ▼              ▼              ▼
/feature      /bug           /play        /feature     /git-pull
 -maker       -fixer       (7 modes)      -check       -request
  │              │                           │              │
  │              │                       Git Status      Create PR
  │              │                       Merge/Clean     via gh CLI
  │              │
  ▼              ▼
┌────────────────────┐    ┌────────────────────┐
│ feature-            │    │ bug-fix-            │
│   orchestrator      │    │   orchestrator      │
│   (6 Phases)        │    │   (3 Phases)        │
└─────────┬──────────┘    └─────────┬──────────┘
          │                         │
          └────────────┬────────────┘
                       ▼
  ┌────────────────────────────────────────────────────┐
  │                  Sub-Agent Pool                    │
  │                                                    │
  │  ┌──────────────────┐    ┌──────────────────┐      │
  │  │ feature-planner   │    │ bug-diagnostician │      │
  │  │ Plan & Design     │    │ Reproduce & Diag  │      │
  │  └──────────────────┘    └──────────────────┘      │
  │                                                    │
  │  ┌─────────────────────────────────────────┐       │
  │  │          feature-implementer            │       │
  │  │          Code Implementation (shared)   │       │
  │  └─────────────────────────────────────────┘       │
  │                                                    │
  │  ┌──────────────────┐    ┌──────────────────┐      │
  │  │ test-runner       │    │ test-failure-     │      │
  │  │ Write & Run Tests │    │   analyzer        │      │
  │  └──────────────────┘    └──────────────────┘      │
  │                                                    │
  │  ┌──────────────────┐    ┌──────────────────┐      │
  │  │ code-validator    │    │ play-verifier     │      │
  │  │ Convention Check  │    │ Unity Play Test   │      │
  │  └──────────────────┘    └──────────────────┘      │
  │                                                    │
  └────────────────────────────────────────────────────┘
```

### Feature 구현 파이프라인 (`/feature-maker`)

```
/feature-maker
│
├─ Step 0: Pre-check (branch / status / PROGRESS.md)
│
└─ Step 1: feature-orchestrator
   │
   │  ┌─────────┐
   ├─ │ Phase 1 │ feature-planner ─────────── Plan (.claude/plans/*.md)
   │  └─────────┘
   │
   │  ┌─────────┐
   ├─ │ Phase 2 │ feature-implementer ─────── C# Implementation
   │  └─────────┘
   │       ▲
   │       │ Regression (max 3+2)
   │       │
   │  ┌─────────┐
   ├─ │ Phase 3 │ test-runner ─────────────── Write & Run Tests
   │  └─────────┘
   │       │
   │       ├─ PASS ──────────────────────── Next Phase
   │       ├─ FAIL [IMPL] ──────────────── Regress to Phase 2
   │       ├─ FAIL [TEST] ──────────────── Self-fix test code
   │       └─ FAIL [ENV] ───────────────── test-failure-analyzer
   │                                            │
   │                                       Classify & Route
   │
   │  ┌─────────┐
   ├─ │ Phase 4 │ code-validator ──────────── Convention & Perf Check
   │  └─────────┘
   │       │
   │       ├─ PASS ──────────────────────── Next Phase
   │       └─ FAIL ──────────────────────── Regress to Phase 2
   │
   │  ┌─────────┐
   ├─ │ Phase 5 │ play-verifier ───────────── Unity Play Test (Coplay MCP)
   │  └─────────┘
   │       │
   │       ├─ PASS ──────────────────────── Next Phase
   │       └─ FAIL ──────────────────────── Self-fix (max 2)
   │
   │  ┌─────────┐
   └─ │ Phase 6 │ Register play-suite.md ──── Final Report
      └─────────┘
```

### 버그 수정 파이프라인 (`/bug-fixer`)

```
/bug-fixer
│
├─ Step 0: Create bugfix/* branch
│
└─ Step 1: bug-fix-orchestrator
   │
   │  ┌─────────┐
   ├─ │ Phase 1 │ bug-diagnostician ───────── Reproduce & Root Cause
   │  └─────────┘                              Fix Plan (.claude/plans/*.md)
   │
   │  ┌─────────┐
   ├─ │ Phase 2 │ feature-implementer ─────── Apply Fix
   │  └─────────┘
   │       ▲
   │       │ Regression (max 3)
   │       │
   │  ┌─────────┐
   └─ │ Phase 3 │ test-runner ─────────────── Regression + Verification
      └─────────┘
           │
           ├─ PASS ──────────────────────── Final Report
           └─ FAIL [IMPL] ──────────────── Regress to Phase 2
```

### 슬래시 커맨드 요약

| 커맨드 | 설명 | 호출 에이전트 |
|--------|------|-------------|
| `/feature-maker` | 기능 구현 전체 파이프라인 | feature-orchestrator (6 phases) |
| `/bug-fixer` | 버그 진단 → 수정 → 검증 | bug-fix-orchestrator (3 phases) |
| `/feature-check` | 브랜치 상태 점검, 머지/PR 안내 | 없음 (직접 처리) |
| `/play` | Unity 게임 플레이 및 검증 | 없음 (Coplay MCP 직접 호출) |
| `/git-pull-request` | PR 초안 생성 및 GitHub PR 생성 | 없음 (gh CLI 호출) |

### `/play` 모드

| 모드 | 설명 |
|------|------|
| `check` | 3초 플레이 후 에러 로그 + 스크린샷 (기본) |
| `watch` | N초 동안 시간별로 변화 관찰 |
| `test` | 특정 기능 동작 검증 |
| `repro` | 버그 재현 및 증상 기록 |
| `free` | 플레이만 시작, 사용자 직접 조작 |
| `suite` | `.claude/play-suite.md` 기반 전체 기능 순차 검증 |
| `fix` | `.claude/bugs/` open 리포트 순차 수정 |

### 서브 에이전트 역할

| 에이전트 | 모델 | 역할 |
|----------|------|------|
| feature-planner | Opus | 기능 요구사항 분석 → 구현 계획서 작성 |
| feature-implementer | Sonnet | 계획서 기반 C# 코드 구현 (기능/버그 공용) |
| test-runner | Sonnet | Edit/Play Mode 테스트 작성 및 Coplay MCP로 실행 |
| test-failure-analyzer | Opus | 테스트 실패 근본 원인 분석 및 수정 경로 제시 |
| code-validator | Sonnet | 네이밍 컨벤션, 성능 안티패턴, 사이드 이펙트 검증 |
| play-verifier | Sonnet | Unity 에디터에서 Coplay MCP로 실제 플레이 검증 |
| bug-diagnostician | Opus | 버그 재현 → 근본 원인 분석 → 수정 계획서 작성 |

### 브랜치 전략

```
main
 └── feature/{기능명}        ← /feature-maker로 생성. 완료 시 PR → main
      └── bugfix/{버그명}    ← /bug-fixer로 생성. 완료 시 merge → feature
```

### 데이터 흐름

```
docs/PROGRESS.md          ← Feature 로드맵 및 상태 추적
.claude/plans/*.md        ← 구현/수정 계획서 (에이전트 자동 생성)
.claude/play-suite.md     ← Feature별 플레이 검증 항목
.claude/bugs/*.md         ← 버그 리포트 (suite FAIL 시 자동 생성)
```

---

## 타겟 유저

- 20~40대 직장인
- 1~3분 내 짧은 플레이를 선호하는 캐주얼 게이머
- 랭킹 경쟁에 동기부여받는 유저

## 레퍼런스

Subway Surfers, Jetpack Joyride, Vector, Geometry Dash
