# 아키텍처 규칙

## 프로젝트 개요

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6 LTS (2D URP) |
| 언어 | C# |
| 장르 | 2D 사이드뷰 러너 |
| 플랫폼 | 모바일 (iOS / Android) |
| 백엔드 | PlayFab (랭킹/인증) |
| AI 연동 | Claude Code + Coplay MCP |

## 프로젝트 구조

```
Assets/
├── Art/                # 스프라이트, 애니메이션, 배경
├── Audio/              # BGM, SFX
├── Prefabs/
│   ├── Chunks/         # 레벨 청크
│   ├── Obstacles/      # 장애물 프리팹
│   └── Player/         # 플레이어 프리팹
├── Scenes/             # Main, Game, Result
├── Scripts/
│   ├── Player/         # 플레이어 컨트롤러
│   ├── Level/          # 청크 스포너, 맵 생성
│   ├── Obstacles/      # 장애물 로직
│   ├── UI/             # UI 컨트롤러
│   ├── Managers/       # GameManager, AudioManager 등 싱글톤
│   ├── Data/           # ScriptableObject 데이터 클래스
│   ├── Network/        # PlayFab 연동
│   └── Utils/          # Unity 비의존 유틸리티
├── Resources/          # 런타임 로드 리소스
├── Tests/
│   ├── EditMode/       # Edit Mode 테스트 (순수 로직)
│   └── PlayMode/       # Play Mode 테스트 (Unity 통합)
├── Packages/
└── ProjectSettings/
```

## 네이밍 컨벤션

| 항목 | 규칙 | 예시 |
|---|---|---|
| C# 클래스 | PascalCase | `PlayerController` |
| MonoBehaviour | 역할 + 접미사 | `ChunkSpawner`, `ObstaclePool` |
| Manager (싱글톤) | 도메인 + Manager | `GameManager`, `AudioManager` |
| ScriptableObject | 이름 + Data | `ObstacleData`, `DifficultyData` |
| 인터페이스 | I + PascalCase | `IDamageable`, `IPoolable` |
| 메서드 | PascalCase | `TakeDamage()`, `SpawnChunk()` |
| public 필드 | camelCase | `moveSpeed`, `jumpForce` |
| private 필드 | _camelCase | `_rigidbody`, `_isGrounded` |
| 상수 | PascalCase (C# 관례) | `MaxSpeed`, `DefaultLives` |
| 이벤트/Action | On + PascalCase | `OnPlayerDied`, `OnScoreChanged` |
| 코루틴 | 동사 + Coroutine | `SpawnCoroutine()` |

## 스크립트 분류

| 유형 | 폴더 | 역할 |
|---|---|---|
| MonoBehaviour | Scripts/{도메인}/ | 게임오브젝트에 부착되는 컴포넌트 |
| ScriptableObject | Scripts/Data/ | 설정 데이터 (속도, 난이도 곡선 등) |
| Manager (싱글톤) | Scripts/Managers/ | 전역 시스템 관리 |
| Pure C# | Scripts/Utils/ | Unity 비의존 유틸리티/헬퍼 |

## 의존성 규칙

**허용**:
- 개별 컴포넌트 -> Manager (싱글톤 또는 이벤트)
- UI 스크립트 -> Manager (이벤트 구독)
- Manager -> Manager (필요시)
- 모든 스크립트 -> ScriptableObject (데이터 읽기)

**금지**:
- Manager에서 특정 씬 오브젝트 직접 참조 -> 이벤트/콜백 사용
- MonoBehaviour에서 `Find`/`FindObjectOfType` 런타임 호출 -> Inspector 할당 또는 이벤트
- `Update()`에서 `GetComponent` 반복 호출 -> `Awake()`/`Start()`에서 캐싱
- 씬 간 데이터 전달에 static 남용 -> ScriptableObject 또는 싱글톤 Manager 사용
