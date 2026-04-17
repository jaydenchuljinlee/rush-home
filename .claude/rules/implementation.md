# 구현 패턴

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
