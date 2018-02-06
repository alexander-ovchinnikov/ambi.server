using System;
using Photon.SocketServer;

namespace BattleServer.Models
{
    public class Character : IDisposable
    {
        public const int MaxHealth = 100;


        public Character(Action<int, SendParameters> onHit, Action onDie)
        {
            hitEvent += onHit;
            dieEvent += onDie;
            Init();
        }

        public int Health { get; private set; } = 100;
        private int Damage { get; } = 10;

        public virtual void Dispose()
        {
        }

        public event Action<int, SendParameters> hitEvent;
        public event Action dieEvent;

        public void Hit(SendParameters sendParameters)
        {
            if (CanHit())
                hitEvent.Invoke(Damage, sendParameters);
        }

        public virtual void Init()
        {
            Health = MaxHealth;
        }


        public bool CanHit()
        {
            return Health > 0;
        }

        public void GetHit(int damage)
        {
            Health -= damage;
            if (Health <= 0) dieEvent();
        }
    }
}