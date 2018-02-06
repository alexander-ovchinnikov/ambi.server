using System.Collections.Generic;
using BattleServer.Events;
using BattleServer.Models;
using BattleServer.Storage;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace BattleServer.Networking
{
    public class UnityClient : ClientPeer //Character
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private BotCharacter _enemy;

        private Character _player;
        private UserDB _user;
        private readonly UserManager _userManager;


        public UnityClient(InitRequest initRequest, UserManager userManager) : base(initRequest)
        {
            _userManager = userManager;
        }


        private void SendStatsEvent()
        {
            ResetStats();
            var sendParameters = new SendParameters();
            var eventData = new EventData((byte) EventCodes.StatsInit);
            eventData.Parameters = new Dictionary<byte, object>();
            eventData.Parameters[(byte) BattleEvent.Params.PlayerHealth] = _player.Health;
            eventData.Parameters[(byte) BattleEvent.Params.EnemyHealth] = _enemy.Health;
            SendEvent(eventData, sendParameters);
        }

        private void InitEvent(Dictionary<byte, object> operationRequestParameters, SendParameters sendParameters)
        {
            string id = null;
            if (operationRequestParameters.ContainsKey((byte) Events.InitEvent.Params.Id))
            {
                var val = operationRequestParameters[(byte) Events.InitEvent.Params.Id]; //.ToString();
                if (val != null) id = (string) val;
            }

            if (string.IsNullOrEmpty(id))
                id = _userManager.GetUserID();
            _user = _userManager.Load(id);
            _player = new Character(PlayerHitEvent, PlayerDieEvent);
            _enemy = new BotCharacter(EnemyHitEvent, EnemyDieEvent);

            var eventData = new EventData((byte) EventCodes.InitEvent)
            {
                Parameters = new Dictionary<byte, object>
                {
                    [(byte) Events.InitEvent.Params.Id] = _user.Id,
                    [(byte) Events.InitEvent.Params.Wins] = _user.Wins,
                    [(byte) Events.InitEvent.Params.Loses] = _user.Losts
                }
            };
            SendEvent(eventData, sendParameters);
        }

        private void EnemyHitEvent(int damage, SendParameters sendParameters)
        {
            _player.GetHit(damage);
            var eventData = new EventData((byte) EventCodes.EnemyHit)
            {
                Parameters = new Dictionary<byte, object>
                {
                    [(byte) BattleEvent.Params.PlayerHealth] = _player.Health,
                    [(byte) BattleEvent.Params.EnemyHealth] = _enemy.Health
                }
            };
            SendEvent(eventData, sendParameters);
        }

        private void EnemyDieEvent()
        {
            var sendParameters = new SendParameters();
            _user.Win();
            var eventData = new EventData((byte) EventCodes.EnemyDie) {Parameters = new Dictionary<byte, object>()};
            SendEvent(eventData, sendParameters);
        }

        private void PlayerDieEvent()
        {
            var sendParameters = new SendParameters();
            _user.Lost();
            var eventData = new EventData((byte) EventCodes.PlayerDie);
            eventData.Parameters = new Dictionary<byte, object>();
            SendEvent(eventData, sendParameters);
        }

        private void ResetStats()
        {
            _player.Init();
            _enemy.Init();
        }

        private void PlayerHitEvent(int damage, SendParameters sendParameters)
        {
            _enemy.GetHit(damage);
            var eventData = new EventData((byte) EventCodes.PlayerHit)
            {
                Parameters = new Dictionary<byte, object>
                {
                    [(byte) BattleEvent.Params.PlayerHealth] = _player.Health,
                    [(byte) BattleEvent.Params.EnemyHealth] = _enemy.Health
                }
            };
            SendEvent(eventData, sendParameters);
        }

        private void SendHitEvent(SendParameters sendParameters)
        {
            _player.Hit(sendParameters);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case (byte) RequestCodes.InitRequest:
                    InitEvent(operationRequest.Parameters, sendParameters);
                    break;
                case (byte) RequestCodes.StatsInitRequest: //(byte) GameRequests.PlayerHit:
                    SendStatsEvent();
                    break;
                case (byte) RequestCodes.PlayerHitRequest: //(byte) GameRequests.PlayerHit:
                    SendHitEvent(sendParameters);
                    break;

                default:
                    Log.Debug("Got unkknown request: " + operationRequest.OperationCode);
                    break;
            }
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            _player.Dispose();
            _enemy.Dispose();
            Dispose(true);
        }
    }
}