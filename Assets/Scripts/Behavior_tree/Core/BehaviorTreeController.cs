using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class BehaviorTreeController : MonoBehaviour
{
    private Node root;

    public Transform anyObject;
    public Transform NPC; // Добавим ссылку на собаку
    public Transform destination;
    private NavMeshAgent agent;
    private Animator animator;
     private IsPlayerNearby isPlayerNearbyCheck;
    private void Start()
    {
        // Инициализация NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on the NPC!");
            return;
        }

        
        animator = GetComponent<Animator>();
         isPlayerNearbyCheck = gameObject.AddComponent<IsPlayerNearby>();
        // Узлы действия
        ActionNode moveToDestination = new ActionNode(context => MoveToDestination(context));
        ActionNode greetPlayer = new ActionNode(context => GreetPlayer(context));
        ConditionNode isPlayerNearby = new ConditionNode(context => isPlayerNearbyCheck.Check());
        // Узлы условий


        // Секвенции и селекторы
        SequenceNode greetSequence = new SequenceNode(new List<Node> { isPlayerNearby, greetPlayer });
        SelectorNode mainSelector = new SelectorNode(new List<Node> { greetSequence, moveToDestination });

        // Основное дерево
        root = mainSelector;
    }

    private void Update()
    {
        // Создание контекстов
        var actionContext = new ActionContext(agent, animator, anyObject, destination);
        var conditionContext = new ConditionContext(anyObject, NPC);

        root.Evaluate(actionContext, conditionContext); // Передача контекстов в дерево
    }

    // Действие: движение к цели с использованием NavMeshAgent
    private NodeState MoveToDestination(ActionContext context)
    {
        agent.isStopped = false;
      
        if (context.Destination == null)
        {
            
            return NodeState.FAILURE;
        }
      
        
        if (Vector3.Distance(transform.position, context.Destination.position) > context.Agent.stoppingDistance)
        {
            context.Agent.SetDestination(context.Destination.position);
            context.Animator.SetBool("IsWalking", true); 
            return NodeState.RUNNING;
        }

        context.Animator.SetBool("IsWalking", false); // Останавливаем анимацию
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



}
