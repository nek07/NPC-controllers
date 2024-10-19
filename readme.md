# TEAM Radiation
## NPC для городского окружения


### Система поведения NPC на основе Behavior Tree - Структура и компоненты
Behavior Tree (дерево поведения) — это структура данных, которая широко используется для описания поведения NPC в играх. В этой системе было реализовано дерево поведения и контроллер NPC для управления их действиями, взаимодействиями и условиями в игровом мире.

Система Behavior Tree состоит из следующих основных компонентов:

### 1. Node (Узел)
Это базовый класс, который представляет любое поведение в дереве. Каждый узел в дереве поведения либо выполняет действие, либо проверяет условие, либо управляет последовательностью или выбором других узлов.

### Ключевые методы:
Evaluate(): Метод, который возвращает текущее состояние узла: выполнение (RUNNING), успех (SUCCESS) или неудача (FAILURE).
```
public abstract class Node
{
    public abstract NodeState Evaluate();
}
```
### 2. ActionNode (Узел действия)
Этот узел отвечает за выполнение конкретного действия NPC. Примером может быть движение NPC к цели или выполнение анимации приветствия игрока.

### Ключевые особенности:
Принимает функцию действия и выполняет её.
Возвращает результат действия (SUCCESS, FAILURE или RUNNING).
```
public class ActionNode : Node
{
    private Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        return action();
    }
}
```
### 3. ConditionNode (Узел условия)
Этот узел проверяет определённые условия в игровой среде. Например, можно проверить, находится ли игрок рядом с NPC, есть ли свободное пространство для остановки под крышей во время дождя или выполняется ли какое-либо другое условие.

### Ключевые особенности:
Возвращает успех (SUCCESS), если условие выполнено, или неудачу (FAILURE), если не выполнено.
```
public class ConditionNode : Node
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Evaluate()
    {
        return condition() ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
```
### 4. SequenceNode (Узел последовательности)
Этот узел выполняет дочерние узлы по порядку. Если одно из действий завершилось неудачей (FAILURE), вся последовательность также завершится неудачей. Если все действия успешны, последовательность возвращает успех.

### Ключевые особенности:
Идеально для создания линейных цепочек поведения (например, проверка условий, выполнение действий).

```

public class SequenceNode : Node
{
    private List<Node> nodes;

    public SequenceNode(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in nodes)
        {
            NodeState result = node.Evaluate();
            if (result != NodeState.SUCCESS)
            {
                return result;
            }
        }
        return NodeState.SUCCESS;
    }
}
```
## 5. SelectorNode (Узел селектора)
Этот узел выполняет дочерние узлы по очереди до тех пор, пока один из них не завершится успешно (SUCCESS). Если все узлы завершились неудачей, селектор возвращает неудачу.

### Ключевые особенности:
Хорош для выбора из нескольких вариантов поведения (например, приоритетные действия).

```
public class SelectorNode : Node
{
    private List<Node> nodes;

    public SelectorNode(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in nodes)
        {
            NodeState result = node.Evaluate();
            if (result == NodeState.SUCCESS)
            {
                return result;
            }
        }
        return NodeState.FAILURE;
    }
}
```
Конечно! Вот объяснение кода для документации, которое описывает его функциональность, структуру и используемые принципы:

---

# NPSsManager

## Описание
Класс `NPSsManager` управляет созданием и поведением NPC (неигровых персонажей) в игровой среде. Он отвечает за спавн NPC вокруг игрока, их удаление, а также за управление взаимодействиями с определёнными местами.

## Поля класса

### Заголовки и параметры
- **Player param**
  - `GameObject player`: Ссылка на объект игрока, вокруг которого будут спавниться NPC.
  - `float spawnRadius`: Радиус вокруг игрока, в пределах которого будут спавниться NPC.

- **Optimitization param**
  - `float maxNpcDistance`: Максимальное расстояние, на котором NPC могут находиться от игрока, после чего они будут удалены.
  - `bool deleteNonVisibleNpc`: Опция для удаления невидимых NPC (не используется в текущей реализации, но предусмотрена для будущего).
  - `bool allowObjectPolling`: Опция, позволяющая включить/выключить "пуллинг" объектов, то есть временное отключение NPC, находящихся далеко от игрока.
  - `float poolSize`: Максимальное расстояние, на котором NPC активны в режиме "пуллинга".

- **NPC param**
  - `int maxNPC`: Максимальное количество NPC, которые могут быть активны одновременно.
  - `List<GameObject> npcPrefabs`: Список префабов NPC для спавна.
  - `List<GameObject> npcList`: Список активных NPC в сцене.

- **InteractionPlaces**
  - `bool allowInteraction`: Опция для разрешения взаимодействия NPC с определёнными местами.
  - `Transform[] interactionPlaces`: Массив мест, с которыми NPC могут взаимодействовать.

## Метод `Start()`
- Инициализирует список NPC и спавнит NPC в случайных позициях вокруг игрока.
- Если разрешено взаимодействие, каждому NPC назначаются случайные места взаимодействия.

## Метод `Update()`
- Проверяет и удаляет NPC, находящихся на слишком большом расстоянии от игрока.
- Если количество NPC меньше максимального, создаёт новых NPC.
- Управляет "пуллингом" объектов для экономии ресурсов.

## Метод `SpawnRandomNPC()`
- Спавнит NPC в случайной позиции вокруг игрока, используя NavMesh для нахождения подходящей точки.
- Если разрешен "пуллинг", новый NPC будет временно отключён.

## Метод `RemoveFarNPCs()`
- Удаляет NPC, которые находятся за пределами заданного максимального расстояния от игрока.

## Метод `ObjectPolling()`
- Управляет состоянием активности NPC в зависимости от расстояния до игрока. NPC, находящиеся слишком далеко, отключаются, а те, которые ближе, активируются.

## Используемые принципы ООП
- **Инкапсуляция**: Все данные и функциональность, относящиеся к NPC, инкапсулированы в этом классе, что упрощает управление ими.
- **Абстракция**: Класс управляет сложными процессами спавна и управления NPC, скрывая детали реализации от других классов.
- **Наследование**: Класс может быть расширен для добавления новых типов NPC или функциональности без изменения базовой логики.
- **Полиморфизм**: Неявно используется в методах работы с различными префабами NPC, которые могут наследовать общие характеристики.

---

Вот документация для класса `BehaviorTreeController`, которая подробно объясняет его функциональность, структуру и используемые принципы.

---

# BehaviorTreeController

## Описание
Класс `BehaviorTreeController` управляет поведением NPC (неигровых персонажей) в игровой среде с использованием дерева поведения. Он включает в себя различные действия и условия, которые определяют, как NPC реагируют на окружающую среду, такие как нахождение игрока, наличие машины и погодные условия, такие как дождь.

## Поля класса

### Поля для управления NPC
- **Node root**: Корневой узел дерева поведения, который управляет логикой NPC.
- **NavMeshAgent agent**: Компонент для навигации NPC по игровому миру.
- **Animator animator**: Компонент анимации для управления анимациями NPC.
- **Renderer npcRenderer**: Компонент рендеринга, который используется для управления видимостью NPC.

### Поля для условий и действий
- **IsPlayerNearby isPlayerNearbyCheck**: Проверка, находится ли игрок рядом с NPC.
- **IsCarNearby isCarNearbyCheck**: Проверка, находится ли машина рядом с NPC.
- **IsDogNearby isDogNearbyCheck**: Проверка, находится ли собака рядом с NPC (хотя этот компонент не используется в текущей версии).
- **Transform[] destinations**: Массив целевых точек, к которым NPC может двигаться.
- **Transform anyObject**: Общий объект, к которому могут обращаться узлы.
- **Transform NPC**: Ссылка на самого NPC.
- **Camera playerCamera**: Камера игрока, используемая для проверки видимости NPC.
- **DayCycleManager dayCycleManager**: Менеджер дня и ночи, который управляет погодными условиями.

### Поля для управления целями
- **List<Transform> shuffledDestinations**: Перемешанный список целевых точек для случайного движения NPC.
- **int currentDestinationIndex**: Индекс текущей целевой точки, к которой движется NPC.

## Метод `Start()`
- Инициализирует компоненты NPC (NavMeshAgent, Animator, Renderer).
- Создаёт проверки для нахождения игрока и машины.
- Перемешивает массив точек назначения.
- Определяет действия (движение, приветствие, избегание машины и укрытие от дождя) и условия (близость игрока, наличие машины и дождь).
- Настраивает дерево поведения с использованием последовательностей и селекторов.

## Метод `ShuffleDestinations()`
- Перемешивает массив целевых точек, чтобы обеспечить случайное движение NPC.

## Метод `Update()`
- Проверяет, корректен ли корень дерева поведения.
- Создаёт контексты для действий и условий.
- Проверяет видимость NPC с помощью метода `CheckVisibility()`.
- Вызывает оценку корневого узла дерева поведения.

## Метод `CheckVisibility()`
- Проверяет, находится ли NPC в поле зрения камеры игрока, и управляет видимостью рендерера.

## Действия

### MoveToDestination(ActionContext context)
- Двигает NPC к текущей цели с использованием NavMeshAgent.
- Если NPC достиг целевой точки, увеличивает индекс текущей цели.

### GreetPlayer(ActionContext context)
- Реагирует на приближение игрока, останавливает движение NPC и запускает анимацию танца.

### AvoidCar(ActionContext context)
- Останавливает NPC и запускает анимацию избегания машины.

### HideFromRain(ActionContext context)
- Останавливает NPC и запускает анимацию укрытия от дождя.

## Используемые принципы ООП
- **Инкапсуляция**: Все данные и функции, относящиеся к поведению NPC, находятся в одном классе, что упрощает управление и модификацию.
- **Абстракция**: Упрощает взаимодействие с различными компонентами и узлами, скрывая детали реализации.
- **Наследование**: Этот класс может быть расширен для создания специализированных NPC с уникальным поведением.
- **Полиморфизм**: Упрощает использование различных узлов и действий, позволяя легко добавлять новые типы поведения без изменения основного кода.

---

Эта документация может быть полезна для других разработчиков, работающих с вашим проектом, а также для вас в будущем. Если нужно внести изменения или добавить дополнительную информацию, дайте знать!
