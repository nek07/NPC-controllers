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

    // Убедитесь, что массив точек назначения установлен в инспекторе
    public Transform[] destinations; // Массив точек назначения
    public Transform anyObject; // Убедитесь, что это установлено в инспекторе
    public Transform NPC; // Убедитесь, что это установлено в инспекторе

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

        // Инициализация проверок
        isPlayerNearbyCheck = gameObject.AddComponent<IsPlayerNearby>();
        Debug.Log("IsPlayerNearby component added.");

        isCarNearbyCheck = gameObject.AddComponent<IsCarNearby>();
        Debug.Log("IsCarNearby component added.");

        // Перемешиваем массив точек назначения
        ShuffleDestinations();

        // Узлы действия
        ActionNode moveToDestination = new ActionNode(context => MoveToDestination(context));
        Debug.Log("MoveToDestination action node created.");

        ActionNode greetPlayer = new ActionNode(context => GreetPlayer(context));
        Debug.Log("GreetPlayer action node created.");

        ActionNode avoidCar = new ActionNode(context => AvoidCar(context));
        Debug.Log("AvoidCar action node created.");

        // Узлы условий
        ConditionNode isPlayerNearby = new ConditionNode(context => isPlayerNearbyCheck.Check());
        Debug.Log("IsPlayerNearby condition node created.");

        ConditionNode isCarNearby = new ConditionNode(context => isCarNearbyCheck.Check());
        Debug.Log("IsCarNearby condition node created.");

        // Секвенции и селекторы
        SequenceNode greetSequence = new SequenceNode(new List<Node> { isPlayerNearby, greetPlayer });
        Debug.Log("Greet sequence created.");

        SequenceNode carCrashSequence = new SequenceNode(new List<Node> { isCarNearby, avoidCar });
        Debug.Log("Car crash sequence created.");

        SelectorNode mainSelector = new SelectorNode(new List<Node> { carCrashSequence, greetSequence, moveToDestination });
        Debug.Log("Main selector created.");

        // Основное дерево
        root = mainSelector;
        if (root == null)
        {
            Debug.LogError("Behavior tree root is null! Check node initialization.");
        }
        else
        {
            Debug.Log("Behavior tree initialized successfully.");
        }
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

        // Вызываем Evaluate для корневого узла
        NodeState state = root.Evaluate(actionContext, conditionContext);

        // Обработка состояния, если необходимо
        switch (state)
        {
            case NodeState.RUNNING:
                Debug.Log("Behavior tree is running.");
                break;
            case NodeState.SUCCESS:
                Debug.Log("Behavior tree completed successfully.");
                break;
            case NodeState.FAILURE:
                Debug.Log("Behavior tree failed.");
                break;
        }
    }

    // Действие: движение к цели с использованием NavMeshAgent
    private NodeState MoveToDestination(ActionContext context)
    {
        if (currentDestinationIndex >= shuffledDestinations.Count)
        {
            // Если все цели достигнуты, просто вернём SUCCESS
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
            return NodeState.RUNNING;
        }

        context.Animator.SetBool("IsWalking", false); // Останавливаем анимацию
        currentDestinationIndex++; // Переходим к следующей цели
        return NodeState.SUCCESS; // Достигли цели
    }

    // Действие: приветствие игрока
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
