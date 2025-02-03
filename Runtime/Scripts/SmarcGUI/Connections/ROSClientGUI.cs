using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;

namespace SmarcGUI.Connections
{
    public class ROSClientGUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_InputField ServerAddressInput;
        public TMP_InputField PortInput;
        public Button ConnectButton;
        public Button DisconnectButton;

        ROSConnection rosCon;

        string ServerAddress => ServerAddressInput.text;
        int ServerPort => int.Parse(PortInput.text);

        void Start()
        {
            rosCon = ROSConnection.GetOrCreateInstance();
            ServerAddressInput.text = rosCon.RosIPAddress.ToString();
            PortInput.text = rosCon.RosPort.ToString();
            ConnectButton.onClick.AddListener(OnConnect);
            DisconnectButton.onClick.AddListener(OnDisconnect);
            rosCon.ShowHud = false;
        }

        void ConnectionInputsInteractable(bool interactable)
        {
            ConnectButton.interactable = interactable;
            ServerAddressInput.interactable = interactable;
            PortInput.interactable = interactable;
            DisconnectButton.interactable = !interactable;
        }

        void OnConnect()
        {
            rosCon.Connect(ServerAddress, ServerPort);
            ConnectionInputsInteractable(false);
        }

        void OnDisconnect()
        {
            rosCon.Disconnect();
            ConnectionInputsInteractable(true);
        }

    }
}