using System;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class MQTTClient : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_InputField ServerAddressInput;
        public TMP_InputField PortInput;
        public Button ConnectButton;
        public Button DisconnectButton;

        // mostly a wrapper for: https://github.com/dotnet/MQTTnet/blob/release/4.x.x/Samples/Client/Client_Connection_Samples.cs
        // Notice we use the 4.x branch because dotnet of unity (:

        IMqttClient mqttClient;
        GUIState guiState;

        string ServerAddress => ServerAddressInput.text;
        int ServerPort => int.Parse(PortInput.text);

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
        }

        void Start()
        {
            ServerAddressInput.text = "20.240.40.232";
            PortInput.text = "1884";
            ConnectButton.onClick.AddListener(ConnectToBroker);
            DisconnectButton.onClick.AddListener(DisconnectFromBroker);
            ConnectionInputsInteractable(true);
        }

        void ConnectionInputsInteractable(bool interactable)
        {
            ConnectButton.interactable = interactable;
            ServerAddressInput.interactable = interactable;
            PortInput.interactable = interactable;
            DisconnectButton.interactable = !interactable;
        }

        async void ConnectToBroker()
        {
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(host: ServerAddress, port: ServerPort).Build();
            guiState.Log($"Connecting to {ServerAddress}:{ServerPort} ...");
            MqttClientConnectResult response = null;
            try
            {
                ConnectionInputsInteractable(false);
                response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            }
            catch (MqttCommunicationTimedOutException)
            {
                ConnectionInputsInteractable(true);
                guiState.Log($"Timeout while trying to connect to {ServerAddress}:{ServerPort}");
                return;
            }
            catch (OperationCanceledException)
            {
                ConnectionInputsInteractable(true);
                guiState.Log($"Connection to {ServerAddress}:{ServerPort} was canceled");
                return;
            }

            if(response is null || response.ResultCode != MqttClientConnectResultCode.Success)
            {
                ConnectionInputsInteractable(true);
                guiState.Log($"Failed to connect to {ServerAddress}:{ServerPort}, result code == {response.ResultCode}");
                return;
            }
            guiState.Log($"Connected to broker on {ServerAddress}:{ServerPort}!");
        }

        async void DisconnectFromBroker()
        {
            var mqttFactory = new MqttFactory();
            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();
            try
            {
                await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
            catch (MqttClientNotConnectedException)
            {
                guiState.Log($"Not connected to broker on {ServerAddress}:{ServerPort}!");
                ConnectionInputsInteractable(true);
                return;
            }
            ConnectionInputsInteractable(true);
            guiState.Log($"Disconnected from broker on {ServerAddress}:{ServerPort}!");
        }

    

        


    }
        
}