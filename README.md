# **README: Проект NPC на Unity с Поведенческим Деревом**  
# **Команда Radiation**  


## **Описание проекта**  
Данный проект реализует систему NPC (неигровых персонажей) с использованием **Behavior Tree** (дерева поведения). NPC реагируют на окружающую среду, такие как присутствие игрока или машины, перемещаются между точками, и адаптируются к изменениям погоды и времени суток.
 й
---

## **Функциональные возможности**  
- **Поведенческое дерево (Behavior Tree)** для реализации логики NPC.  
- **Реакция на игрока и машины** с разными сценариями взаимодействия.  
- **Перемещение NPC** с использованием NavMesh между заданными точками.  
- **Менеджер цикла дня и погоды** с анимацией изменения интенсивности освещения.  
- **Менеджер NPC**: генерация и удаление NPC в зависимости от расстояния до игрока и видимости.  

---

## **Установка и запуск проекта**  
1. **Установите Unity** (рекомендуемая версия 2021.3+).  
2. **Клонируйте проект:**  
   ```bash
   git clone <https://github.com/nek07/NPC-controllers.git>
   ```
3. Откройте проект в Unity.  
4. Настройте сцену:
   - Убедитесь, что добавлены объекты: **Player**, **NavMesh**, **NPC Prefabs**.
   - Настройте **NavMesh**:  
     `Window -> AI -> Navigation` → выберите объекты → **Bake**.  
5. Запустите проект, нажав **Play** в Unity.

---
## **Исходный код**  

### **1. Контроллер дерева поведения: `BehaviorTreeController.cs`**  
```csharp
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class BehaviorTreeController : MonoBehaviour
{
    private Node root;
    private NavMeshAgent agent;
    private Animator animator;
    private Renderer npcRenderer;
    public Transform[] destinations;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        npcRenderer = GetComponentInChildren<Renderer>();

        // Определение узлов и дерева
        ActionNode moveToDestination = new ActionNode(ctx => MoveToDestination(ctx));
        ConditionNode isPlayerNearby = new ConditionNode(ctx => CheckProximity("Player"));

        SequenceNode greetSequence = new SequenceNode(new List<Node> { isPlayerNearby, moveToDestination });
        root = new SelectorNode(new List<Node> { greetSequence });
    }

    private void Update()
    {
        var context = new ActionContext(agent, animator);
        root.Evaluate(context);
    }

    private NodeState MoveToDestination(ActionContext context)
    {
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            agent.SetDestination(destinations[0].position);
            context.Animator.SetBool("IsWalking", true);
            return NodeState.RUNNING;
        }
        context.Animator.SetBool("IsWalking", false);
        return NodeState.SUCCESS;
    }

    private bool CheckProximity(string tag) => Vector3.Distance(transform.position, GameObject.FindWithTag(tag).transform.position) < 5f;
}
```

### **2. Менеджер NPC: `NPSsManager.cs`**  
Вот документация для класса `NPSsManager`, который управляет созданием и удалением NPC в игровом мире. 

---

# Менеджер NPC: `NPSsManager.cs`

## Описание
Класс `NPSsManager` отвечает за управление NPC (неигровыми персонажами) в игре. Он создает NPC в заданном радиусе от игрока и удаляет NPC, которые находятся на большом расстоянии от игрока, чтобы оптимизировать производительность.

## Поля класса

### Параметры игрока
- **GameObject player**: Ссылка на объект игрока в сцене.
- **float spawnRadius**: Радиус, в пределах которого NPC могут появляться вокруг игрока.

### Параметры NPC
- **int maxNPC**: Максимальное количество NPC, которые могут быть активны одновременно.
- **List<GameObject> npcPrefabs**: Список префабов NPC, из которых можно создавать новых NPC.
- **List<GameObject> npcList**: Список текущих NPC в сцене.

## Метод `Start()`
- Вызывается при запуске игры. Инициализирует создание NPC, создавая максимальное количество NPC, указанных в `maxNPC`.

## Метод `Update()`
- Вызывается каждый кадр. Обрабатывает удаление удаленных NPC и создает новых NPC, если текущее количество NPC меньше максимального.

## Метод `SpawnRandomNPC()`
- Генерирует случайную позицию в пределах заданного радиуса от игрока.
- Проверяет, есть ли точка на NavMesh для спавна NPC с использованием метода `NavMesh.SamplePosition`.
- Создает нового NPC, используя случайный префаб из списка `npcPrefabs`, и добавляет его в `npcList`.

## Метод `RemoveFarNPCs()`
- Проходит по списку текущих NPC в обратном порядке и удаляет тех NPC, которые находятся на расстоянии более 70 единиц от игрока.
- Удаляет NPC как из игрового мира, так и из списка `npcList`.

## Используемые принципы ООП
- **Инкапсуляция**: Все данные и методы, связанные с управлением NPC, находятся в одном классе.
- **Абстракция**: Скрывает детали реализации спавна NPC и управления их количеством.
- **Наследование и полиморфизм**: Возможно расширение этого класса для создания специализированных менеджеров NPC с уникальным поведением.


```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPSsManager : MonoBehaviour
{
    [Header("Player param")] 
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnRadius = 10f;
    
    [Header("NPC param")] 
    [SerializeField] private int maxNPC = 10;
    [SerializeField] private List<GameObject> npcPrefabs;
    private List<GameObject> npcList = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < maxNPC; i++) { SpawnRandomNPC(); }
    }

    private void Update()
    {
        RemoveFarNPCs();
        if (npcList.Count < maxNPC) { SpawnRandomNPC(); }
    }

    private void SpawnRandomNPC()
    {
        Vector3 spawnPos = player.transform.position + Random.insideUnitSphere * spawnRadius;
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)], hit.position, Quaternion.identity);
            npcList.Add(npc);
        }
    }

    private void RemoveFarNPCs()
    {
        for (int i = npcList.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(player.transform.position, npcList[i].transform.position) > 70f)
            {
                Destroy(npcList[i]);
                npcList.RemoveAt(i);
            }
        }
    }
}
```



### **1. Интерфейсы: `IAction`, `ICondition`, `INearby`**  
```csharp
public interface IAction
{
    NodeState Execute(ActionContext context);
}

public interface ICondition
{
    bool Evaluate(ConditionContext context);
}

public interface INearby
{
    bool IsNearbyByTag(string tag); 
    void React(); 
}
```

---

### **2. Менеджер цикла дня: `DayCycleManager.cs`**  
```csharp
using System;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    [Header("Day Time")]
    public bool AutoChange = false;
    [Range(0, 1)] [SerializeField] private float _timeOfDay;
    [SerializeField] private float dayDuration = 30f;
    [SerializeField] private Light Sun;
    [SerializeField] private AnimationCurve sunCurve;
    private float sunIntensity;

    [Header("Rain")]
    [SerializeField] private bool isRaining;
    [SerializeField] private GameObject rainParticle;

    private void Start()
    {
        SetEvening();
        sunIntensity = Sun.intensity;
    }

    private void Update()
    {
        SetRain(isRaining);
        if (AutoChange)
        {
            _timeOfDay += Time.deltaTime / dayDuration;
            if (_timeOfDay >= 1) _timeOfDay -= 1;
        }
        Sun.transform.localRotation = Quaternion.Euler(_timeOfDay * 360, 180, 0);
        Sun.intensity = sunIntensity * sunCurve.Evaluate(_timeOfDay);
    }

    public void SetMorning() { _timeOfDay = 0.25f; }
    public void SetEvening() { _timeOfDay = 0.5f; }
    public void SetNight() { _timeOfDay = 0.8f; }

    public void SetRain(bool state)
    {
        isRaining = state;
        rainParticle.SetActive(state);
    }
}
```

---


---



---

## **Классы и структуры дерева поведения**  
- **Node**: Базовый класс для узлов.  
- **ActionNode**: Узел действия.  
- **ConditionNode**: Узел условия.  
- **SequenceNode**: Выполняет действия последовательно.  
- **SelectorNode**: Выполняет одно из нескольких действий.  

---

## **Навигация и перемещение NPC**  
Используется **NavMesh** для передвижения NPC по игровой карте. Убедитесь, что на сцене есть правильно настроенная навигационная сетка (NavMesh).  

---

## **Цикл дня и погоды**  
Менеджер дня (`DayCycleManager`) отвечает за изменения времени суток и погоды. Включите/выключите дождь с помощью метода `SetRain()`.

---

## **Заключение**  
Этот проект демонстрирует, как с помощью поведенческого дерева можно управлять логикой NPC и взаимодействием с окружающим миром в Unity. Вы можете расширить функциональность, добавив больше узлов или усложнив логику дерева.

---

## **Контакты**  
Если у вас есть вопросы или предложения, свяжитесь с автором проекта.
