using System;
using System.Timers;
using Photon.SocketServer;

namespace BattleServer.Models
{
    public class BotCharacter : Character
    {
        private readonly int _interval = 10000;
        private Timer timer;


        public BotCharacter(Action<int, SendParameters> hitEvent, Action dieEvent) : base(hitEvent, dieEvent)
        {
        }


        private void OnHitInterval(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var sendParameters = new SendParameters {ChannelId = 1};
            Hit(sendParameters);
        }

        public override void Dispose()
        {
            timer.Enabled = false;
            timer?.Dispose();
        }

        public override void Init()
        {
            timer = new Timer {Interval = _interval};
            timer.Elapsed += OnHitInterval;
            timer.AutoReset = true;
            timer.Enabled = true;
            base.Init();
        }
    }
}