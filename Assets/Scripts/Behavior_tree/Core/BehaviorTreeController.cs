using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class BehaviorTreeController : MonoBehaviour
{
    private Node root;
    private NavMeshAgent agent;
    private Animator animator;
    private IsPlayerNearby isPlayerNearbyCheck;
    private IsCarNearby isCarNearbyCheck;
    private IsDogNearby isDogNearbyCheck; // Предполагается, что это тоже часть вашего кода
    private Renderer npcRenderer; // Новый компонент Renderer для отключения/включения моделей

    // Убедитесь, что массив точек назначения установлен в инспекторе
    public Transform[] destinations; // Массив точек назначения
    public Transform anyObject; // Убедитесь, что это установлено в инспекторе
    public Transform NPC; // Убедитесь, что это установлено в инспекторе
    public Camera playerCamera; // Камера игрока

    private List<Transform> shuffledDestinations = new List<Transform>(); // Перемешанные точки назначения
    private int currentDestinationIndex = 0; // Индекс текущей точки назначения

    private void Start()
    {
        // Инициализация NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on the NPC!");
            return;
        }

        // Инициализация Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on the NPC!");
            return;
        }

        // Инициализация Renderer для отключения моделей
        npcRenderer = GetComponentInChildren<Renderer>();
        if (npcRenderer == null)
        {
            Debug.LogError("Renderer not found on the NPC!");
            return;
        }

        // Инициализация проверок
        isPlayerNearbyCheck = gameObject.AddComponent<IsPlayerNearby>();
        isCarNearbyCheck = gameObject.AddComponent<IsCarNearby>();

        // Перемешиваем массив точек назначения
        ShuffleDestinations();

        // Узлы действия
        ActionNode moveToDestination = new ActionNode(context => MoveToDestination(context));
        ActionNode greetPlayer = new ActionNode(context => GreetPlayer(context));
        ActionNode avoidCar = new ActionNode(context => AvoidCar(context));

        // Узлы условий
        ConditionNode isPlayerNearby = new ConditionNode(context => isPlayerNearbyCheck.Check());
        ConditionNode isCarNearby = new ConditionNode(context => isCarNearbyCheck.Check());

        // Секвенции и селекторы
        SequenceNode greetSequence = new SequenceNode(new List<Node> { isPlayerNearby, greetPlayer });
        SequenceNode carCrashSequence = new SequenceNode(new List<Node> { isCarNearby, avoidCar });

        SelectorNode mainSelector = new SelectorNode(new List<Node> { carCrashSequence, greetSequence, moveToDestination });

        // Основное дерево
        root = mainSelector;
    }

    private void ShuffleDestinations()
    {
        shuffledDestinations.Clear();
        shuffledDestinations.AddRange(destinations);
        for (int i = 0; i < shuffledDestinations.Count; i++)
        {
            Transform temp = shuffledDestinations[i];
            int randomIndex = Random.Range(i, shuffledDestinations.Count);
            shuffledDestinations[i] = shuffledDestinations[randomIndex];
            shuffledDestinations[randomIndex] = temp;
        }
    }

    private void Update()
    {
        if (root == null)
        {
            Debug.LogError("Cannot evaluate behavior tree; root is null!");
            return;
        }

        // Создание контекстов
        var actionContext = new ActionContext(agent, animator, anyObject, destinations);
        var conditionContext = new ConditionContext(anyObject, NPC);

        // Проверка видимости
        CheckVisibility();

        // Вызываем Evaluate для корневого узла
        NodeState state = root.Evaluate(actionContext, conditionContext);
    }

    private void CheckVisibility()
    {
        if (npcRenderer == null || playerCamera == null) return;

        // Проверка, находится ли NPC в поле зрения камеры
        Vector3 viewPos = playerCamera.WorldToViewportPoint(transform.position);
        bool isVisible = (viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0);

        npcRenderer.enabled = isVisible; // Отключить/включить рендерер
    }

    // Действие: движение к цели с использованием NavMeshAgent
    private NodeState MoveToDestination(ActionContext context)
    {
        if (currentDestinationIndex >= shuffledDestinations.Count)
        {
            return NodeState.SUCCESS;
        }

        agent.isStopped = false;

        if (shuffledDestinations[currentDestinationIndex] == null)
        {
            return NodeState.FAILURE;
        }

        Transform destination = shuffledDestinations[currentDestinationIndex];

        if (Vector3.Distance(transform.position, destination.position) > agent.stoppingDistance)
        {
            agent.SetDestination(destination.position);
            context.Animator.SetBool("IsWalking", true);
            context.Animator.SetBool("IsDancing", false);
            return NodeState.RUNNING;
        }

        context.Animator.SetBool("IsWalking", false);
        currentDestinationIndex++;
        return NodeState.SUCCESS;
    }

    private NodeState GreetPlayer(ActionContext context)
    {
        isPlayerNearbyCheck.React();
        agent.isStopped = true;
        context.Animator.SetBool("IsWalking", false);
        context.Animator.SetBool("IsDancing", true);
        return NodeState.SUCCESS;
    }

    private NodeState AvoidCar(ActionContext context)
    {
        agent.isStopped = true;
        context.Animator.SetBool("IsWalking", false);
        context.Animator.SetBool("isAvoided", true);
        Debug.Log("АААА БЕГИТЕ МАШИНАА АЫВАЫВЫВА ПИЬЛАРАААС");
        return NodeState.SUCCESS;
    }
}
