using System;
using System.Threading;
using System.Threading.Tasks;
// Because we have a "Task" under SmarcGUI too... meh.
using SystemTask = System.Threading.Tasks.Task;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Packets;
using MQTTnet.Protocol;

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
        public TMP_InputField ContextInput;
        public Toggle SubToSimToggle;
        public Toggle SubToRealToggle;

        public Button ConnectButton;
        public Button DisconnectButton;

        // mostly a wrapper for: https://github.com/dotnet/MQTTnet/blob/release/4.x.x/Samples/Client/Client_Connection_Samples.cs
        // Notice we use the 4.x branch because dotnet of unity (:

        IMqttClient mqttClient;
        GUIState guiState;

        public string Context => ContextInput.text;

        string ServerAddress => ServerAddressInput.text;
        int ServerPort => int.Parse(PortInput.text);

        MQTTPublisher[] publishers;

        void Awake()
        {
            guiState = FindFirstObjectByType<GUIState>();
            ContextInput.text = "smarcsim";
            publishers = FindObjectsByType<MQTTPublisher>(FindObjectsSortMode.None);
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
            ContextInput.interactable = interactable;
            SubToRealToggle.interactable = interactable;
            SubToSimToggle.interactable = interactable;
            DisconnectButton.interactable = !interactable;
        }


        SystemTask OnMsgReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString();

            // wara stuff is formatted like: smarc/unit/subsurface/simulation/sam1/heartbeat
            // {context}/unit/{air,ground,surface,subsurface}/{real,simulation,playback}/{agentName}/{topic}
            var topicParts = topic.Split('/');
            var context = topicParts[0];
            var domain = topicParts[2];
            var realism = topicParts[3];
            var agentName = topicParts[4];
            var messageType = topicParts[5];

            switch(messageType)
            {
                case "heartbeat":
                    Debug.Log($"Hearbeat from: {agentName}");
                    break;
                default:
                    Debug.Log($"Received uhandled message on MQTT topic: {topic}");
                    break;
            }
            return SystemTask.CompletedTask;
        }

        void OnConnetionMade()
        {
            if(SubToRealToggle.isOn) SubToHeartbeats("real");
            if(SubToSimToggle.isOn) SubToHeartbeats("simulation");
            foreach(var publisher in publishers)
            {
                publisher.StartPublishing();
            }
        }

        void OnconnectionLost()
        {
            foreach(var publisher in publishers)
            {
                publisher.StopPublishing();
            }
        }


        async void ConnectToBroker()
        {
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(host: ServerAddress, port: ServerPort).Build();

            mqttClient.ApplicationMessageReceivedAsync += OnMsgReceived;

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

            OnConnetionMade();
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
            OnconnectionLost();
        }

    
        public async void Publish(string topic, string payload)
        {
            if(mqttClient is null || !mqttClient.IsConnected) return;

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();


            try
            {
                await mqttClient.PublishAsync(message, CancellationToken.None);
            }
            catch (MqttCommunicationTimedOutException)
            {
                guiState.Log($"Timeout while trying to publish message to {ServerAddress}:{ServerPort}");
                return;
            }
            catch (OperationCanceledException)
            {
                guiState.Log($"Publishing message to {ServerAddress}:{ServerPort} was canceled");
                return;
            }
        }

        async void SubToHeartbeats(string realism)
        {
            guiState.Log("Subscribing to heartbeats...");

            var topic = $"{Context}/unit/+/{realism}/+/heartbeat";
            var mqttFactory = new MqttFactory();
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Debug.Log($"MQTT client subscribed to topic: {topic}");
        }

        
        

        


    }
        
}