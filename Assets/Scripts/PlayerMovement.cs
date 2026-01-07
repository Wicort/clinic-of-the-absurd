using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Walking Sway")]
    [SerializeField] private float swayAngle = 4f;
    [SerializeField] private float swayHalfCycle = 0.25f;

    [Header("Idle Breathing (Scale)")]
    [SerializeField] private float idleScaleMin = 0.96f;
    [SerializeField] private float idleBreathingSpeed = 2f;

    [Header("References")]
    [SerializeField] private Transform visuals;

    public Transform GetVisuals() => visuals;

    // ← Теперь храним ссылку на Input Actions
    private PlayerControls inputActions;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Vector3 visualsDefaultLocalScale;

    private Tweener swayTweener;
    private bool isBreathing = false;
    private float breathingTimer = 0f;

    private bool IsDialogueOpen => DialogueBoxUI.Instance?.IsShowing == true;

    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (visuals == null)
        {
            Debug.LogError("Visuals not assigned!");
            return;
        }

        visualsDefaultLocalScale = visuals.localScale;
        inputActions = new PlayerControls();
        
        Instance = this;
    }

    private void OnEnable()
    {
        inputActions.Player.Move.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();

        if (Instance == this)
            Instance = null;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (IsDialogueOpen)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void Update()
    {
        if (visuals == null) return;

        // 🔥 Ключевое исправление: учитываем диалог при выборе анимации
        bool isCurrentlyMoving = !IsDialogueOpen && moveInput.magnitude > 0.1f;

        if (isCurrentlyMoving)
        {
            isBreathing = false;
            breathingTimer = 0f;

            if (swayTweener == null || !swayTweener.IsActive())
            {
                StartSway();
            }
        }
        else
        {
            swayTweener?.Complete();
            swayTweener = null;

            // Возвращаем поворот в 0
            if (visuals.localEulerAngles.z != 0f)
            {
                visuals.DORotate(Vector3.zero, swayHalfCycle * 0.8f)
                    .SetEase(Ease.OutSine);
            }

            isBreathing = true;
        }

        // Обновляем "дыхание" только если не в диалоге
        if (isBreathing)
        {
            breathingTimer += Time.deltaTime;
            float scaleOffset = Mathf.Sin(breathingTimer * idleBreathingSpeed) * (1f - idleScaleMin);
            float targetScaleY = 1f - scaleOffset;
            visuals.localScale = new Vector3(
                visualsDefaultLocalScale.x,
                targetScaleY,
                visualsDefaultLocalScale.z
            );
        }
        else
        {
            if (visuals.localScale != visualsDefaultLocalScale)
            {
                visuals.localScale = visualsDefaultLocalScale;
            }
        }
    }

    private void StartSway()
    {
        swayTweener = visuals.DORotate(
            new Vector3(0, 0, swayAngle),
            swayHalfCycle * 2f,
            RotateMode.FastBeyond360
        ).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void TeleportTo(Transform target)
    {
        if (target == null) return;
        rb.linearVelocity = Vector2.zero;
        transform.position = target.position;
    }

    public void TeleportTo(Vector3 position)
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = position;
    }
}
