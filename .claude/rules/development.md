# 개발 규칙

## SOLID 원칙

### S — Single Responsibility (단일 책임)
- 하나의 클래스/컴포넌트는 하나의 역할만 담당한다
- 예: ObstacleBase는 충돌 감지와 풀링만 담당, 효과 로직은 IObstacleEffect 구현체에 위임
- MonoBehaviour가 비대해지면 역할별로 컴포넌트를 분리한다

### O — Open/Closed (개방/폐쇄)
- 새 기능 추가 시 기존 코드를 수정하지 않고 확장한다
- 예: 새 장애물 효과 → IObstacleEffect를 구현한 새 컴포넌트 추가, ObstacleBase 수정 불필요
- 예: 새 이동 패턴 → IObstacleMovement를 구현한 새 컴포넌트 추가

### L — Liskov Substitution (리스코프 치환)
- 인터페이스 구현체는 어디서든 교체 가능해야 한다
- 예: LethalEffect와 BlockEffect는 모두 IObstacleEffect이므로 프리팹에서 자유롭게 교체 가능

### I — Interface Segregation (인터페이스 분리)
- 인터페이스는 작고 구체적으로 유지한다
- 예: IObstacleEffect(충돌 효과)와 IObstacleMovement(이동 패턴)를 하나로 합치지 않는다
- 클라이언트가 사용하지 않는 메서드를 강제하지 않는다

### D — Dependency Inversion (의존성 역전)
- 구체 클래스가 아닌 인터페이스에 의존한다
- 예: ObstacleBase는 LethalEffect가 아닌 IObstacleEffect에 의존
- GetComponent<인터페이스>()로 구현체를 주입받는다

## Unity 프로젝트 적용 지침
- 새 시스템을 만들 때 인터페이스를 먼저 정의하고, 구현체를 컴포넌트로 분리한다
- 프리팹에서 컴포넌트 조합으로 다양한 변형을 만들 수 있는 구조를 지향한다
- Manager/Singleton은 최소한으로 유지하고, 이벤트 기반 통신을 우선한다
