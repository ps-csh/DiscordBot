using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Timers;
using System.Threading.Tasks;
using DiscordBot.Bot;
using DiscordBot.Utility;
using DiscordBot.DiscordAPI.Structures;
using Microsoft.Extensions.Options;
using DiscordBot.Configuration;

//DELETE:
// 1. Fields, Constants 
// 2. Constructors 
// 3. Events 
// 4. Properties 
// 5. Methods 
// 6. Nested Types 

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Handles messages to and from the Discord Gateway,
    /// which includes heartbeats, messages, etc.
    /// </summary>
    public class DiscordGatewayClient
    {
        public enum OpCode : int
        {
            Dispatch = 0,
            Heartbeat,
            Identify,
            StatusUpdate,
            VoiceStateUpdate,
            //OpCode 5 is not used by Discord API
            Resume = 6,
            Reconnect,
            RequestGuildMembers,
            InvalidSession,
            Hello,
            HeartbeatAcknowledge
        }

        public delegate void OnMessageCallback(DiscordGatewayPayload payload);

        private WebSocket webSocket;

        private OnMessageCallback messageCallback;
        private OnMessageCallback readyCallback;
        private OnMessageCallback guildCreateCallback;

        private string token;

        private Timer heartbeatTimer;
        private int? lastHeartbeatSequence = null;
        private bool heartbeatAcknowledged;

        private readonly ILogger logger;

        //private DiscordApiClient apiClient;

        public DiscordGatewayClient(IOptions<BotSecretsConfiguration> configuration, ILogger logger)
        {
            this.logger = logger;

            token = configuration.Value.Token;
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Token was null");
            }

            //TODO: Move gateway uri to config
            //webSocket = new WebSocket(gatewayUri, default, default, OnOpen);
            webSocket = new WebSocket("wss://gateway.discord.gg");

            // Set websocket callbacks
            webSocket.Opened += (OnOpen);
            webSocket.Closed += (OnClose);
            webSocket.MessageReceived += (OnMessage);
            webSocket.Error += (OnError);
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

        private void OnOpen(object sender, EventArgs e)
        {
            DiscordIdentifyPayload identifyPayload = new DiscordIdentifyPayload(token,
                new DiscordIdentifyPayload.ConnectionProperties { Device = ".Net" },
                (int)(DiscordIdentifyPayload.GatewayIntents.Guilds 
                | DiscordIdentifyPayload.GatewayIntents.GuildMembers
                | DiscordIdentifyPayload.GatewayIntents.GuildPresences // Required for user list
                | DiscordIdentifyPayload.GatewayIntents.GuildMessages
                | DiscordIdentifyPayload.GatewayIntents.DirectMessages
                | DiscordIdentifyPayload.GatewayIntents.MessageContent));

            //TODO: Create an Identify class to send this data instead of manual JSON LINQ
            JObject sendPacket = new JObject();
            sendPacket["op"] = 2;
            sendPacket["d"] = JObject.FromObject(identifyPayload);
            //sendPacket["d"]["token"] = token;
            //sendPacket["d"]["v"] = 1;
            //sendPacket["d"]["properties"] = new JObject();
            //sendPacket["d"]["properties"]["device"] = ".Net";
            //sendPacket["large_threshold"] = 50; // Number of offline members this client will receive
            //sendPacket["compress"] = false;
            //TODO: send Gateway Intents

            
            

            webSocket.Send(sendPacket.ToString());

            logger.LogInfo(sendPacket.ToString());
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

                // Opcodes 2, 3, 4, 6, and 8 are Send only, so they don't need to be handled here
                switch ((OpCode)gatewayPayload.Opcode)
                {
                    case OpCode.Dispatch:
                        HandleEvent(gatewayPayload);
                        break;
                    case OpCode.Heartbeat:
                        break;
                    case OpCode.Reconnect:
                        break;
                    case OpCode.InvalidSession:
                        logger.LogWarning("OpCode 9: Invalid Session received.");
                        break;
                    case OpCode.Hello:
                        OnHelloReceived(gatewayPayload);
                        break;
                    case OpCode.HeartbeatAcknowledge:
                        heartbeatAcknowledged = true;
                        break;
                    default:
                        break;
                }

                //DELETE: This is only for testing
                if (gatewayPayload?.EventData != null)
                {
                    logger.LogInfo(gatewayPayload.EventData.ToString());
                }

            }
            catch(Exception ex)
            {
                logger.LogException(ex);
                logger.LogWarning(e?.Message);
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            heartbeatTimer?.Stop();

            logger.LogInfo(e.ToString());
        }

        //Note - WebSocket4Net is based on SuperSocket 2.0
        private void OnError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            //Logger.LogWarning(e.Exception.Message);
            logger.LogException(e.Exception);
        }

        private void HandleEvent(DiscordGatewayPayload payload)
        {
            //TODO: Is try-catch needed here?
            try
            {
                messageCallback?.Invoke(payload);
            }
            catch(Exception ex)
            {
                logger.LogException(ex);
            }
            //string sender = payload["s"]
            //messageCallback.Invoke()
        }

        private void OnHelloReceived(DiscordGatewayPayload helloPayload)
        {
            try
            {
                //TODO: Define a Hello event structure instead of using JSON LINQ
                int interval = helloPayload.EventData["heartbeat_interval"].ToObject<int>();

                if (interval == 0)
                {
                    // TEMP - in case interval was not set, use a default
                    interval = 10000;
                }

                StartHeartbeat(interval);
            }
            catch (Exception ex)
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

        public void AddMessageCallback(OnMessageCallback callback)
        {
            messageCallback += callback;
        }

        public void AddGuildCreateCallback(OnMessageCallback callback)
        {
            guildCreateCallback += callback;
        }
    }
}
