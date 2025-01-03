using System;
using Cysharp.Threading.Tasks;
using Meta.Net.NativeWebSocket;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Scripts
{
    public class WebBasedController : MonoBehaviour
    {
        public event Action<bool> OnLedStatusChanged;
    
        [SerializeField] private string webUrl = "http://192.168.100.18:5000";
        [SerializeField] private string socketUrl = "ws://192.168.100.18:5000/ws";
    
        private WebSocket _websocket;

        public void TurnLedOnButton()
        {
            TurnLedOn().AsAsyncUnitUniTask();
        }

        public void TurnLedOffButton()
        {
            TurnLedOff().AsAsyncUnitUniTask();
        }

        private async void Start()
        {
            await SetupWebSocket();
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _websocket?.DispatchMessageQueue();
#endif
        }

        private async void OnDestroy()
        {
            if (_websocket == null) 
                return;
        
            Debug.Log("Close WEB SOCKET ON DESTROY");
            await _websocket.Close();
        }

        private async void OnApplicationQuit()
        {
            if (_websocket == null) 
                return;
        
            Debug.Log("Close WEB SOCKET ON APP QUITE");
            await _websocket.Close();
        }

        private async UniTask SetupWebSocket()
        {
            _websocket = new WebSocket(socketUrl);

            if (_websocket == null)
            {
                Debug.LogError("Cannot create WebSocket!");
                return;
            }

            _websocket.OnOpen += () =>
            {
                Debug.Log("_websocket is opened");
            };

            _websocket.OnError += (e) =>
            {
                Debug.LogError("_websocket error: " + e);
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("_websocket is closed");
            };

            _websocket.OnMessage += OnMessageReceived;

            await _websocket.Connect();
        }

        private void OnMessageReceived(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);

            message = message.TrimEnd('\0');

            if (string.Equals(message, StaticConstants.LedOnStatus))
            {
                Debug.Log("LED STATUS CHANGED: ON");
                OnLedStatusChanged?.Invoke(true);
                return;
            }

            if (string.Equals(message, StaticConstants.LedOffStatus))
            {
                Debug.Log("LED STATUS CHANGED: OFF");
                OnLedStatusChanged?.Invoke(false);
                return;
            }

            Debug.LogWarning($"Unknown socket event is received: {message}");
        }

        private async UniTask TurnLedOn()
        {
            using UnityWebRequest www = UnityWebRequest.Post(
                webUrl + StaticConstants.WebCommandLedOn,
                "", 
                "application/json");

            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("COMMAND: LED is ON");
            }
            else
            {
                Debug.LogError("COMMAND: Cannot turn LED on: " + www.result.ToString());
            }
        }

        private async UniTask TurnLedOff()
        {
            using UnityWebRequest www = UnityWebRequest.Post(
                webUrl + StaticConstants.WebCommandLedOff,
                "",
                "application/json");

            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("COMMAND: LED is OFF");
            }
            else
            {
                Debug.LogError("COMMAND: Cannot turn LED off: " + www.result.ToString());
            }
        }
    }
}
