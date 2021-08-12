// GENERATED AUTOMATICALLY FROM 'Assets/Data/NSInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @NSInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @NSInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""NSInput"",
    ""maps"": [
        {
            ""name"": ""Flight"",
            ""id"": ""a2c0b898-4b79-400d-846b-e5d1db089b3c"",
            ""actions"": [
                {
                    ""name"": ""Sideways"",
                    ""type"": ""Button"",
                    ""id"": ""645e050a-b55f-457a-8801-09f0be1d5643"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Button"",
                    ""id"": ""14031124-8b6c-4537-8b8c-1eb41d56aba6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Forward"",
                    ""type"": ""Button"",
                    ""id"": ""3296acdf-ba89-499c-ae0d-5d43cfc66e9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pitch"",
                    ""type"": ""Value"",
                    ""id"": ""6349aca5-699b-432b-9674-0b394e4ef943"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Yaw"",
                    ""type"": ""Value"",
                    ""id"": ""6de20b41-10ac-412f-a441-27fb7f629226"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""Value"",
                    ""id"": ""1f4f1c1c-70ea-4069-90a5-8ed35ca5ea0f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hyper"",
                    ""type"": ""Button"",
                    ""id"": ""ea1b710c-772f-4cf2-a8a2-296b9d9b2a6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Button"",
                    ""id"": ""5579e713-2e9f-4e92-85e8-9b343fd1eb15"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""c157b583-a4f8-4e9b-8ac3-894b1d37254e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7b3286f8-c3e1-40c4-bc51-a56194250798"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""44193fd6-3a6e-485d-91c8-80548d8f65a1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""6fc39b38-e396-4a03-8459-eb5e94394a10"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sideways"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""1dfe8763-aa86-47dd-b6f2-7b0ca3044f79"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sideways"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""14d43b16-582d-45ee-a556-bd89b9d6e8e2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sideways"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""0ab4f9d8-7eec-4791-8b96-081fed5663ff"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a4c118de-1b3f-4cfb-a941-352ae8f89021"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7285c437-f3b7-48ad-8226-a35bb7d9ec23"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""a72a8e65-5a35-45d2-ac15-a2c7c4333f88"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d20a9ecb-4422-4e45-82f5-a535b8876aac"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4f26bf82-43d4-4631-8fef-eff060144a7b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e60908bd-3423-43a2-adb2-e0d3e94fb07c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hyper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c1a4447-a6b9-4a89-b9ce-796fd5d1c298"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""d6f1e8f5-1d5f-4792-adef-c4d1c2e69367"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pitch"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""1881c236-f4b5-46c4-a5d6-317f7a19757f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""88b9b41c-1424-452b-b291-504d552c6122"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""73c0b3d8-b11f-4639-a085-a8681894a81c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""456970ea-6f34-4880-af1c-efd9718144e2"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""adde185e-540a-457c-8b98-24f2dfad714c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Flight
        m_Flight = asset.FindActionMap("Flight", throwIfNotFound: true);
        m_Flight_Sideways = m_Flight.FindAction("Sideways", throwIfNotFound: true);
        m_Flight_Vertical = m_Flight.FindAction("Vertical", throwIfNotFound: true);
        m_Flight_Forward = m_Flight.FindAction("Forward", throwIfNotFound: true);
        m_Flight_Pitch = m_Flight.FindAction("Pitch", throwIfNotFound: true);
        m_Flight_Yaw = m_Flight.FindAction("Yaw", throwIfNotFound: true);
        m_Flight_Roll = m_Flight.FindAction("Roll", throwIfNotFound: true);
        m_Flight_Hyper = m_Flight.FindAction("Hyper", throwIfNotFound: true);
        m_Flight_Brake = m_Flight.FindAction("Brake", throwIfNotFound: true);
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

    // Flight
    private readonly InputActionMap m_Flight;
    private IFlightActions m_FlightActionsCallbackInterface;
    private readonly InputAction m_Flight_Sideways;
    private readonly InputAction m_Flight_Vertical;
    private readonly InputAction m_Flight_Forward;
    private readonly InputAction m_Flight_Pitch;
    private readonly InputAction m_Flight_Yaw;
    private readonly InputAction m_Flight_Roll;
    private readonly InputAction m_Flight_Hyper;
    private readonly InputAction m_Flight_Brake;
    public struct FlightActions
    {
        private @NSInput m_Wrapper;
        public FlightActions(@NSInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Sideways => m_Wrapper.m_Flight_Sideways;
        public InputAction @Vertical => m_Wrapper.m_Flight_Vertical;
        public InputAction @Forward => m_Wrapper.m_Flight_Forward;
        public InputAction @Pitch => m_Wrapper.m_Flight_Pitch;
        public InputAction @Yaw => m_Wrapper.m_Flight_Yaw;
        public InputAction @Roll => m_Wrapper.m_Flight_Roll;
        public InputAction @Hyper => m_Wrapper.m_Flight_Hyper;
        public InputAction @Brake => m_Wrapper.m_Flight_Brake;
        public InputActionMap Get() { return m_Wrapper.m_Flight; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FlightActions set) { return set.Get(); }
        public void SetCallbacks(IFlightActions instance)
        {
            if (m_Wrapper.m_FlightActionsCallbackInterface != null)
            {
                @Sideways.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnSideways;
                @Sideways.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnSideways;
                @Sideways.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnSideways;
                @Vertical.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnVertical;
                @Forward.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnForward;
                @Forward.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnForward;
                @Forward.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnForward;
                @Pitch.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnPitch;
                @Pitch.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnPitch;
                @Pitch.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnPitch;
                @Yaw.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnYaw;
                @Yaw.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnYaw;
                @Yaw.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnYaw;
                @Roll.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnRoll;
                @Roll.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnRoll;
                @Roll.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnRoll;
                @Hyper.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnHyper;
                @Hyper.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnHyper;
                @Hyper.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnHyper;
                @Brake.started -= m_Wrapper.m_FlightActionsCallbackInterface.OnBrake;
                @Brake.performed -= m_Wrapper.m_FlightActionsCallbackInterface.OnBrake;
                @Brake.canceled -= m_Wrapper.m_FlightActionsCallbackInterface.OnBrake;
            }
            m_Wrapper.m_FlightActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Sideways.started += instance.OnSideways;
                @Sideways.performed += instance.OnSideways;
                @Sideways.canceled += instance.OnSideways;
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
                @Forward.started += instance.OnForward;
                @Forward.performed += instance.OnForward;
                @Forward.canceled += instance.OnForward;
                @Pitch.started += instance.OnPitch;
                @Pitch.performed += instance.OnPitch;
                @Pitch.canceled += instance.OnPitch;
                @Yaw.started += instance.OnYaw;
                @Yaw.performed += instance.OnYaw;
                @Yaw.canceled += instance.OnYaw;
                @Roll.started += instance.OnRoll;
                @Roll.performed += instance.OnRoll;
                @Roll.canceled += instance.OnRoll;
                @Hyper.started += instance.OnHyper;
                @Hyper.performed += instance.OnHyper;
                @Hyper.canceled += instance.OnHyper;
                @Brake.started += instance.OnBrake;
                @Brake.performed += instance.OnBrake;
                @Brake.canceled += instance.OnBrake;
            }
        }
    }
    public FlightActions @Flight => new FlightActions(this);
    public interface IFlightActions
    {
        void OnSideways(InputAction.CallbackContext context);
        void OnVertical(InputAction.CallbackContext context);
        void OnForward(InputAction.CallbackContext context);
        void OnPitch(InputAction.CallbackContext context);
        void OnYaw(InputAction.CallbackContext context);
        void OnRoll(InputAction.CallbackContext context);
        void OnHyper(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
    }
}
