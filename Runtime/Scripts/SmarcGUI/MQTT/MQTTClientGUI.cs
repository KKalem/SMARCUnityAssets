using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmarcGUI
{
    public class MQTTClientGUI : MonoBehaviour
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
            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            ConnectionInputsInteractable(false);

            if(response.ResultCode != MqttClientConnectResultCode.Success)
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
            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            ConnectionInputsInteractable(true);
            guiState.Log($"Disconnected from broker on {ServerAddress}:{ServerPort}!");
        }

    

        


    }
        
}