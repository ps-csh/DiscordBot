using DiscordBot.Configuration;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Structures.Voice;
using DiscordBot.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using WebSocket4Net;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Handles Voice-over-IP Gateway connections.
    /// </summary>
    public class DiscordVoipGatewayClient
    {
        /*
        0	Identify	        client	            Begin a voice websocket connection.
        1	Select Protocol	    client	            Select the voice protocol.
        2	Ready	            server	            Complete the websocket handshake.
        3	Heartbeat	        client	            Keep the websocket connection alive.
        4	Session Description	server	            Describe the session.
        5	Speaking	        client and server	Indicate which users are speaking.
        6	Heartbeat ACK	    server	            Sent to acknowledge a received client heartbeat.
        7	Resume	            client	            Resume a connection.
        8	Hello	            server	            Time to wait between sending heartbeats in milliseconds.
        9	Resumed	            server	            Acknowledge a successful session resume.
        13	Client Disconnect	server	            A client has disconnected from the voice channel
         */
        public enum OpCode
        {
            Identify = 0,
            SelectProtocol,
            Ready,
            Heartbeat,
            SessionDescription,
            Speaking,
            HeartbeatAcknowledge,
            Resume,
            Hello,
            Resumed,
            //10-12 are unused in Voice Gateways
            ClientDisconnect = 13
        }

        private readonly DiscordGatewayClient gatewayClient;
        private readonly ILogger logger;

        private WebSocket webSocket;

        private string token;
        private string sessionId;
        private string userId;
        private string serverId;

        private Timer heartbeatTimer;
        private int? lastHeartbeatSequence = null;
        private bool heartbeatAcknowledged;

        public DiscordVoipGatewayClient(DiscordGatewayClient gatewayClient, BotSecretsConfiguration configuration, ILogger logger)
        {
            this.gatewayClient = gatewayClient;
            this.logger = logger;
            //this.token = configuration.Token;
        }

        public void Connect(string gatewayUri, string serverId, string sessionId, string userId, string botToken)
        {
            token = botToken;
            this.sessionId = sessionId;
            this.serverId = serverId;
            this.userId = userId;

            //webSocket = new WebSocket(gatewayUri, default, default, OnOpen);
            webSocket = new WebSocket(gatewayUri);

            // Set websocket callbacks
            //webSocket.Opened += (OnOpen);
            //webSocket.Closed += (OnClose);
            webSocket.MessageReceived += (OnMessage);
            //webSocket.Error += (OnError);
        }

        public void OpenWebSocket()
        {
            if (webSocket.State != WebSocketState.Open)
            {
                webSocket.Open();
            }
        }

        public void CloseWebSocket()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                webSocket.Close("Closed by user");

                webSocket.Dispose();
            }
        }

        private void OnOpen()
        {
            /*
                {
                  "op": 0,
                  "d": {
                    "server_id": "41771983423143937",
                    "user_id": "104694319306248192",
                    "session_id": "my_session_id",
                    "token": "my_token"
                  }
                }
             */
            JObject sendPacket = new JObject();
            sendPacket["op"] = 0;
            sendPacket["d"] = new JObject();
            sendPacket["d"]["server_id"] = serverId;
            sendPacket["d"]["user_id"] = userId;
            sendPacket["d"]["session_id"] = sessionId;
            sendPacket["d"]["token"] = token;

            // Send an Identify (OpCode 0) payload to the websocket
            // Expect Ready (OpCode 2) in return
            webSocket.Send(sendPacket.ToString(Formatting.None));
        }

        /// <summary>
        /// Any payload received from Gateway.
        /// The type of payload is defined by the Opcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                //JObject payload = JObject.Parse(e.Message);

                DiscordGatewayPayload gatewayPayload = JsonConvert.DeserializeObject<DiscordGatewayPayload>(e.Message);

                // Update the last sequence received, used when sending heartbeats
                lastHeartbeatSequence = gatewayPayload.Sequence;

                // Opcodes 0, 1, 3, and 7 are Send only, so they don't need to be handled here
                switch ((OpCode)gatewayPayload.Opcode)
                {
                    case OpCode.Ready:
                        //HandleEvent(gatewayPayload);
                        OnReady(gatewayPayload.EventData.ToObject<DiscordVoiceReadyPayload>());
                        break;
                    case OpCode.SessionDescription:
                        break;
                    case OpCode.Speaking:
                        break;
                    case OpCode.HeartbeatAcknowledge:
                        OnHeartbeatAcknowledge(gatewayPayload.EventData);
                        break;
                    case OpCode.Hello:
                        OnHello(gatewayPayload.EventData);
                        break;
                    case OpCode.Resumed:
                        break;
                    case OpCode.ClientDisconnect:
                    default:
                        logger.LogWarning($"Invalid OpCode received by VOIP Gateway: {gatewayPayload.Opcode}");
                        break;
                }

                //DELETE: This is only for testing
                if (gatewayPayload?.EventData != null)
                {
                    logger.LogInfo(gatewayPayload.EventData.ToString());
                }

            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void OnReady(DiscordVoiceReadyPayload voiceReadyPayload)
        {
            /*
             {
                "op": 2,
                "d": {
                    "ssrc": 1,
                    "ip": "127.0.0.1",
                    "port": 1234,
                    "modes": ["xsalsa20_poly1305", "xsalsa20_poly1305_suffix", "xsalsa20_poly1305_lite"],
                    "heartbeat_interval": 1
                }
            }
             */
        }

        private void OnHello(JObject helloPayload)
        {
            int interval = helloPayload["heartbeat_interval"].ToObject<int>();
            StartHeartbeat(interval);
        }

        private void OnHeartbeatAcknowledge(JObject acknowledgePayload)
        {
            try
            {
                lastHeartbeatSequence = acknowledgePayload.ToObject<int>();
                heartbeatAcknowledged = true;
            }
            catch(Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void StartHeartbeat(int interval)
        {
            heartbeatTimer = new Timer(interval);
            heartbeatTimer.Elapsed += Heartbeat;
            heartbeatTimer.Start();
            heartbeatAcknowledged = true;
        }

        private void Heartbeat(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (heartbeatAcknowledged)
                {
                    JObject payload = new JObject();
                    payload["op"] = (int)OpCode.Heartbeat;
                    payload["d"] = lastHeartbeatSequence;

                    webSocket.Send(payload.ToString());

                    heartbeatAcknowledged = false;
                }
                else
                {
                    //Heartbeat was not acknowledged.
                    //TODO: Handle heartbeat missed
                    //Discord recommends closing Gateway and attempting to reconnect
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }
}
