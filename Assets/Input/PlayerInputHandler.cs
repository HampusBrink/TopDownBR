using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")] 
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "PlayerActions";
    
    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string mousePos = "MousePos";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string dodge = "Dodge";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string altSkill = "AltSkill";

    private InputAction _moveAction;
    private InputAction _mousePosAction;
    private InputAction _sprintAction;
    private InputAction _dodgeAction;
    private InputAction _attackAction;
    private InputAction _altSkillAction;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePos { get; private set; }
    public float SprintValue { get; private set; }
    public bool DodgeTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }
    public float AltSkillValue { get; private set; }
    
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        _moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        _mousePosAction = playerControls.FindActionMap(actionMapName).FindAction(mousePos);
        _sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        _dodgeAction = playerControls.FindActionMap(actionMapName).FindAction(dodge);
        _attackAction = playerControls.FindActionMap(actionMapName).FindAction(attack);
        _altSkillAction = playerControls.FindActionMap(actionMapName).FindAction(altSkill);
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        // Move
        _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => MoveInput = Vector2.zero;
        
        // Move
        _mousePosAction.performed += context => MousePos = context.ReadValue<Vector2>();
        _mousePosAction.canceled += context => MousePos = Vector2.zero;
        
        // Sprint
        _sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        _sprintAction.canceled += context => SprintValue = 0f;
        
        // Dodge
        _dodgeAction.performed += context => DodgeTriggered = true;
        _dodgeAction.canceled += context => DodgeTriggered = false;
        
        // Attack
        _attackAction.performed += context => AttackTriggered = true;
        _attackAction.canceled += context => AttackTriggered = false;
        
        // Alt skill, I don't know how to handle if it should be hold or not
        _altSkillAction.performed += context => AltSkillValue = context.ReadValue<float>();
        _altSkillAction.canceled += context => AltSkillValue = 0f;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _mousePosAction.Enable();
        _sprintAction.Enable();
        _dodgeAction.Enable();
        _attackAction.Enable();
        _altSkillAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _mousePosAction.Disable();
        _sprintAction.Disable();
        _dodgeAction.Disable();
        _attackAction.Disable();
        _altSkillAction.Disable();
    }
}
