using IngameDebugConsole;
using Unity.Netcode;
using UnityEngine;

public class MoneyManager : NetworkBehaviour
{
    public static MoneyManager Main;

    private int balance = 0;
    
    private void Awake()
    {
        Main = this;
    }

    void Start()
    {
        DebugLogConsole.AddCommand<int>("Money.Set", "Set Money", SetBalanceConsole, "amount");
        DebugLogConsole.AddCommand<int>("Money.Add", "Add Money", AddBalanceConsole, "amount");
        DebugLogConsole.AddCommand<int>("Money.Subtract", "Subtract Money", SubtractBalanceConsole, "amount");
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
    
    private void AddBalanceConsole(int amount)
    {
        AddBalance(amount);
    }

    private void SubtractBalanceConsole(int amount)
    {
        SubtractBalance(amount);
    }


    public int Balance => balance;

    private bool SetBalance(int amount)
    {
        if (amount < 0)
            return false;
        
        balance = amount;
        if (IsServer || IsHost)
            UpdateClients();
        return true;
    }

    public bool AddBalance(int amount)
    {
        if (!EnsureServer())
            return false;

        if (amount < 0)
            return false;

        balance += amount;
        UpdateClients();
        return true;
    }

    public bool SubtractBalance(int amount)
    {
        if (!EnsureServer())
            return false;

        if (amount < 0)
            return false;

        if (balance - amount < 0)
            return false;

        balance -= amount;
        UpdateClients();
        return true;
    }

    public bool CanAfford(int price)
    {
        return balance - price >= 0;
    }


    private void UpdateClients()
    {
        if (!EnsureServer())
            return;
        
        UpdateClientsClientRpc(balance);
    }
    
    [ClientRpc]
    private void UpdateClientsClientRpc(int newBalance)
    {
        balance = newBalance;
    }
}
