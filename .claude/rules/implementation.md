# 구현 패턴

## SOLID 원칙 & 재사용성

### 새로 만들기 전에 재사용 가능한 것을 찾는다
- 새 클래스/컴포넌트를 만들기 전에 **기존 코드에서 동일 역할을 하는 것이 있는지 먼저 탐색**한다
- 비슷한 역할의 클래스가 이미 존재하면 **확장하거나 인터페이스로 추상화**하여 재사용한다
- 동일 역할의 시스템을 2개 만들지 않는다 (예: ObstacleSpawner + PatternSpawner 동시 활성 금지)

### SRP (단일 책임 원칙)
- 하나의 클래스/컴포넌트는 **하나의 이유로만 변경**되어야 한다
- 데이터(SO) / 로직(MonoBehaviour) / 표현(UI) 분리

### OCP (개방-폐쇄 원칙)
- 새 기능 추가 시 **기존 코드 수정 없이 확장** 가능하게 설계
- 예: 새 장애물 타입 추가 시 ObstacleSpawner를 수정하지 않고 프리팹만 추가
- ScriptableObject로 데이터를 분리하면 코드 변경 없이 밸런스 조정 가능

### LSP (리스코프 치환 원칙)
- 인터페이스 구현체는 **계약을 깨지 않아야** 한다
- 예: `IPoolable` 구현 시 OnSpawnFromPool/OnReturnToPool 모두 올바르게 동작

### ISP (인터페이스 분리 원칙)
- 큰 인터페이스보다 **작고 구체적인 인터페이스**를 선호
- 예: IPoolable (스폰/반환), IDamageable (피격) 분리

### DIP (의존성 역전 원칙)
- 구체 클래스 직접 참조보다 **인터페이스나 이벤트**로 소통
- Manager의 static Instance 대신 `GameManager.IsPlaying` 같은 **null-safe 헬퍼** 제공
- `FindFirstObjectByType` 런타임 호출 금지 → Inspector 할당 또는 Start()에서 1회 캐싱

### 물리/밸런스 파라미터는 동적 계산
- jumpDuration, clearance 등 **다른 값에서 파생되는 값은 하드코딩 금지**
- Start()에서 원본 값(jumpForce, gravityScale)을 읽어 자동 계산
- 원본 값이 변경되면 파생값도 자동 반영되도록 설계

## MonoBehaviour 컴포넌트 (`Scripts/{도메인}/`)
- 하나의 컴포넌트는 하나의 책임만 담당 (SRP)
- 컴포넌트 참조는 `[SerializeField] private`로 Inspector 할당
- `GetComponent`는 `Awake()`에서 캐싱, `Update()`에서 호출 금지
- 생명주기 순서: `Awake` -> `OnEnable` -> `Start` -> `FixedUpdate` -> `Update` -> `LateUpdate`
- 물리 연산은 `FixedUpdate()`, 입력/카메라는 `Update()`/`LateUpdate()`

## Manager 싱글톤 (`Scripts/Managers/`)
- 기존 프로젝트 내 싱글톤 패턴을 먼저 확인 후 동일 방식 적용
- `DontDestroyOnLoad` 사용 시 씬 전환 중복 생성 방지
- 게임 상태 변경은 이벤트(`Action`, `UnityEvent`)로 알림
- Manager 종류: `GameManager`(상태), `AudioManager`(사운드), `UIManager`(UI), `PoolManager`(오브젝트 풀링)

## ScriptableObject (`Scripts/Data/`)
- 게임 밸런스 데이터 (속도, 점프력, 난이도 곡선 등)
- `[CreateAssetMenu]` 어트리뷰트로 에디터에서 생성 가능하게
- 런타임에 수정하지 않음 (읽기 전용 데이터)

## 오브젝트 풀링
- 자주 생성/파괴되는 오브젝트 (장애물, 청크, 이펙트)는 풀링 사용
- `IPoolable` 인터페이스: `OnSpawnFromPool()`, `OnReturnToPool()`
- Unity 6 빌트인 Object Pool 또는 커스텀 PoolManager 사용
- **풀 원본(템플릿) 오브젝트 규칙**:
  - 씬에 배치된 풀 원본 오브젝트는 **Initialize() 없이 Update()가 동작하지 않도록** 가드 필수
  - 또는 풀 원본을 **프리팹 에셋만 참조**하고 씬 오브젝트로 배치하지 않는다
  - 실수 사례: Obstacle 템플릿(X=100)이 Initialize() 없이 Update()가 실행되어 화면에 등장

## 입력 처리 (`Scripts/Player/`)
- Unity Input System (New) 사용 권장
- 모바일: 터치 입력 (탭 = 점프, 아래 스와이프 = 슬라이딩)
- 에디터: 키보드 입력으로 테스트 가능하게 추상화

## 물리 & 충돌 (`Scripts/Obstacles/`)
- 2D 물리: `Rigidbody2D` + `Collider2D`
- 장애물 감지: `OnTriggerEnter2D` (트리거 콜라이더)
- 지면 감지: `Physics2D.Raycast` 또는 `BoxCast`
- 레이어 분리: Player, Ground, Obstacle, Trigger

## PlayFab 연동 (`Scripts/Network/`)
- 모든 PlayFab 호출은 `Network/` 폴더의 래퍼 클래스를 통해서만
- 콜백 패턴 또는 async/await 패턴 통일
- 오프라인 폴백 처리 포함

## 코루틴 vs async/await
- Unity 생명주기에 묶인 작업 (프레임 단위 대기, WaitForSeconds): 코루틴
- 네트워크/IO 작업: async/await (UniTask 사용 시)
- 둘 다 가능한 경우 프로젝트 기존 패턴을 따름
