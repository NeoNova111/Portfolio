// GENERATED AUTOMATICALLY FROM 'Assets/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""CharacterControlls"",
            ""id"": ""50994374-c55c-4fc8-b435-65f9e8eec004"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""50886552-be9c-47e6-a04f-881a25d94a30"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""bff300f8-979f-4a83-a68e-28b5d6a10f83"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2c379a2f-2d46-4f4e-8629-18a731055dd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LockOn"",
                    ""type"": ""Button"",
                    ""id"": ""d9cb06f8-0e28-4c2f-b595-06e2f716b0ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""4e8e22b5-80cd-455f-9220-9774aba9ea47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TargetLeft"",
                    ""type"": ""Button"",
                    ""id"": ""57b8d6bd-96ce-44c2-83b1-80bef74dd11a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TargetRight"",
                    ""type"": ""Button"",
                    ""id"": ""e206d467-ba65-406c-bff7-b4ce6896f62c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ContextButton"",
                    ""type"": ""Button"",
                    ""id"": ""31a698e2-3374-41ef-872e-e4b5b701e55f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookAround"",
                    ""type"": ""Value"",
                    ""id"": ""48e76dd9-4419-4b21-9439-fa09df14c643"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Debugmode"",
                    ""type"": ""Button"",
                    ""id"": ""6b8b792f-0a3a-4a32-a0ca-2fb1454fa6d8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""607d544c-fef8-4a16-8ddf-b80fc49a09c6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Return"",
                    ""type"": ""Button"",
                    ""id"": ""5dc8a78e-6d9d-4649-9b4b-307327b0e423"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0ea46128-9ca6-43cd-9964-834ac1fc70d7"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a517c923-533a-4ae9-af7e-68a115748d70"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""033631ea-48be-4fb7-9d54-827cb000b89c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88b6ce45-9fd0-474f-97a0-7098637f6641"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2384925d-2b72-42b1-833c-701e49280e26"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""00f083a9-6c5c-4ce1-be7d-cf27207aa2f6"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d066976e-f143-4c4c-a1c5-7ab48e68328b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""607c8d51-97db-42dc-9f80-7eeadfa0f999"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d289e85c-b500-417e-a03d-fb2209edb3e9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""175aca6b-3355-4d68-9c86-d6d65fcedf99"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""514e3eaa-a4cf-41d1-8141-9e4b6137481a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66ff448a-2ce4-4cc8-a628-cc58ecbf7dd7"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b4eeda7-709f-468a-a934-f9eef2a977ea"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""054a0520-9af4-4aa4-8929-e03fa733d4ea"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ContextButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5829a0bb-de61-4c0c-a0e0-a264e4c913ff"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ContextButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""a1f33d1a-0b54-4fca-9dc8-95d947565baf"",
                    ""path"": ""Dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e1d1099d-3753-4c65-9b8d-8699e33a6f87"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c58d7eb5-54dd-41c9-bed0-dcc5e2bfa996"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ba3745d6-28ad-4192-8d6a-9dc3b460859a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f2ab9be5-5e5d-4a0c-99b8-ef7572516fcb"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""22f4f1a0-a880-4a79-8c52-5ef444eca1d5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""644ef50d-84e6-4fc3-9a76-a7af5b0e9a14"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""66c1bb03-1160-49f7-898f-fc6663f2330d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""72973a71-c41f-4a09-b75b-19e43c21dfa7"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b6267e35-aeff-4629-8cd4-7755cdf03321"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65a4e297-c87c-477f-8abe-d797221ea847"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookAround"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""481ce97d-65f5-47b4-addf-900a4832287c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debugmode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96ecd622-a749-46a0-91ac-a4dad68946ce"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Debugmode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c943169-97e7-4c18-8ca1-6675994d2557"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f0572ee4-dc94-4e88-bfc5-3ece57f73940"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ffaee59-d7f4-455d-83d9-041d9478fdf6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Return"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78763d45-56a3-4bea-9945-ac9e72b9d2d4"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ContextButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82281f6f-550a-489d-a4d4-a59780d9c002"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ContextButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mouse & Keyboard"",
            ""bindingGroup"": ""Mouse & Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // CharacterControlls
        m_CharacterControlls = asset.FindActionMap("CharacterControlls", throwIfNotFound: true);
        m_CharacterControlls_Movement = m_CharacterControlls.FindAction("Movement", throwIfNotFound: true);
        m_CharacterControlls_Run = m_CharacterControlls.FindAction("Run", throwIfNotFound: true);
        m_CharacterControlls_Jump = m_CharacterControlls.FindAction("Jump", throwIfNotFound: true);
        m_CharacterControlls_LockOn = m_CharacterControlls.FindAction("LockOn", throwIfNotFound: true);
        m_CharacterControlls_Attack = m_CharacterControlls.FindAction("Attack", throwIfNotFound: true);
        m_CharacterControlls_TargetLeft = m_CharacterControlls.FindAction("TargetLeft", throwIfNotFound: true);
        m_CharacterControlls_TargetRight = m_CharacterControlls.FindAction("TargetRight", throwIfNotFound: true);
        m_CharacterControlls_ContextButton = m_CharacterControlls.FindAction("ContextButton", throwIfNotFound: true);
        m_CharacterControlls_LookAround = m_CharacterControlls.FindAction("LookAround", throwIfNotFound: true);
        m_CharacterControlls_Debugmode = m_CharacterControlls.FindAction("Debugmode", throwIfNotFound: true);
        m_CharacterControlls_Menu = m_CharacterControlls.FindAction("Menu", throwIfNotFound: true);
        m_CharacterControlls_Return = m_CharacterControlls.FindAction("Return", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CharacterControlls
    private readonly InputActionMap m_CharacterControlls;
    private ICharacterControllsActions m_CharacterControllsActionsCallbackInterface;
    private readonly InputAction m_CharacterControlls_Movement;
    private readonly InputAction m_CharacterControlls_Run;
    private readonly InputAction m_CharacterControlls_Jump;
    private readonly InputAction m_CharacterControlls_LockOn;
    private readonly InputAction m_CharacterControlls_Attack;
    private readonly InputAction m_CharacterControlls_TargetLeft;
    private readonly InputAction m_CharacterControlls_TargetRight;
    private readonly InputAction m_CharacterControlls_ContextButton;
    private readonly InputAction m_CharacterControlls_LookAround;
    private readonly InputAction m_CharacterControlls_Debugmode;
    private readonly InputAction m_CharacterControlls_Menu;
    private readonly InputAction m_CharacterControlls_Return;
    public struct CharacterControllsActions
    {
        private @PlayerInput m_Wrapper;
        public CharacterControllsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_CharacterControlls_Movement;
        public InputAction @Run => m_Wrapper.m_CharacterControlls_Run;
        public InputAction @Jump => m_Wrapper.m_CharacterControlls_Jump;
        public InputAction @LockOn => m_Wrapper.m_CharacterControlls_LockOn;
        public InputAction @Attack => m_Wrapper.m_CharacterControlls_Attack;
        public InputAction @TargetLeft => m_Wrapper.m_CharacterControlls_TargetLeft;
        public InputAction @TargetRight => m_Wrapper.m_CharacterControlls_TargetRight;
        public InputAction @ContextButton => m_Wrapper.m_CharacterControlls_ContextButton;
        public InputAction @LookAround => m_Wrapper.m_CharacterControlls_LookAround;
        public InputAction @Debugmode => m_Wrapper.m_CharacterControlls_Debugmode;
        public InputAction @Menu => m_Wrapper.m_CharacterControlls_Menu;
        public InputAction @Return => m_Wrapper.m_CharacterControlls_Return;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControlls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControllsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControllsActions instance)
        {
            if (m_Wrapper.m_CharacterControllsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMovement;
                @Run.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnRun;
                @Jump.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnJump;
                @LockOn.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLockOn;
                @LockOn.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLockOn;
                @LockOn.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLockOn;
                @Attack.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
                @TargetLeft.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetLeft;
                @TargetLeft.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetLeft;
                @TargetLeft.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetLeft;
                @TargetRight.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetRight;
                @TargetRight.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetRight;
                @TargetRight.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnTargetRight;
                @ContextButton.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnContextButton;
                @ContextButton.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnContextButton;
                @ContextButton.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnContextButton;
                @LookAround.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLookAround;
                @LookAround.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLookAround;
                @LookAround.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnLookAround;
                @Debugmode.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDebugmode;
                @Debugmode.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDebugmode;
                @Debugmode.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDebugmode;
                @Menu.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMenu;
                @Return.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnReturn;
                @Return.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnReturn;
                @Return.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnReturn;
            }
            m_Wrapper.m_CharacterControllsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @LockOn.started += instance.OnLockOn;
                @LockOn.performed += instance.OnLockOn;
                @LockOn.canceled += instance.OnLockOn;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @TargetLeft.started += instance.OnTargetLeft;
                @TargetLeft.performed += instance.OnTargetLeft;
                @TargetLeft.canceled += instance.OnTargetLeft;
                @TargetRight.started += instance.OnTargetRight;
                @TargetRight.performed += instance.OnTargetRight;
                @TargetRight.canceled += instance.OnTargetRight;
                @ContextButton.started += instance.OnContextButton;
                @ContextButton.performed += instance.OnContextButton;
                @ContextButton.canceled += instance.OnContextButton;
                @LookAround.started += instance.OnLookAround;
                @LookAround.performed += instance.OnLookAround;
                @LookAround.canceled += instance.OnLookAround;
                @Debugmode.started += instance.OnDebugmode;
                @Debugmode.performed += instance.OnDebugmode;
                @Debugmode.canceled += instance.OnDebugmode;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @Return.started += instance.OnReturn;
                @Return.performed += instance.OnReturn;
                @Return.canceled += instance.OnReturn;
            }
        }
    }
    public CharacterControllsActions @CharacterControlls => new CharacterControllsActions(this);
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse & Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface ICharacterControllsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLockOn(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnTargetLeft(InputAction.CallbackContext context);
        void OnTargetRight(InputAction.CallbackContext context);
        void OnContextButton(InputAction.CallbackContext context);
        void OnLookAround(InputAction.CallbackContext context);
        void OnDebugmode(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnReturn(InputAction.CallbackContext context);
    }
}
