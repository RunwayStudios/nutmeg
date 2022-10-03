using IngameDebugConsole;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Money
{
    public class MoneyManager : NetworkBehaviour
    {
        public static MoneyManager Main;

        [SerializeField] private int startBalance = 500;
        private NetworkVariable<int> balance = new NetworkVariable<int>(0);
        [SerializeField] private UnityEvent OnBalanceChange;


        private void Awake()
        {
            Main = this;

            balance.OnValueChanged += OnBalanceChanged;
            if (!IsClient || IsHost)
                balance.Value = startBalance;
        }

        void Start()
        {
            DebugLogConsole.AddCommand<int>("Money.Set", "Set Money", SetBalanceConsole, "amount");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            balance.OnValueChanged -= OnBalanceChanged;
        }


        private void OnBalanceChanged(int previousValue, int newValue)
        {
            OnBalanceChange.Invoke();
        }

        private bool EnsureServer()
        {
            if (IsServer || IsHost) return true;

            Debug.LogError("illegal client side balance modification");
            return false;
        }

        private void SetBalanceConsole(int amount)
        {
            SetBalanceConsoleServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetBalanceConsoleServerRpc(int amount)
        {
            SetBalance(amount);
        }


        public int Balance => balance.Value;

        private bool SetBalance(int amount)
        {
            if (amount < 0)
                return false;

            balance.Value = amount;
            //if (IsServer || IsHost)
            //UpdateClients();
            return true;
        }

        public bool AddBalance(int amount)
        {
            if (!EnsureServer())
                return false;

            if (amount < 0)
                return false;

            balance.Value += amount;
            //UpdateClients();
            return true;
        }

        public bool SubtractBalance(int amount)
        {
            if (!EnsureServer())
                return false;

            if (amount < 0)
                return false;

            if (balance.Value - amount < 0)
                return false;

            balance.Value -= amount;
            //UpdateClients();
            return true;
        }

        public bool CanAfford(int price)
        {
            return balance.Value - price >= 0;
        }


        /*private void UpdateClients()
        {
            if (!EnsureServer())
                return;
        
            UpdateClientsClientRpc(balance.Value);
        }
    
        [ClientRpc]
        private void UpdateClientsClientRpc(int newBalance)
        {
            balance.Value = newBalance;
        }*/
    }
}