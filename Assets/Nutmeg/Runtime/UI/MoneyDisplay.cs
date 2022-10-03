using System;
using Nutmeg.Runtime.Gameplay.Money;
using UnityEngine;
using UnityEngine.UI;

namespace Nutmeg.Runtime.UI
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private Text text;

        public void Start()
        {
            UpdateBalance();
        }

        public void UpdateBalance()
        {
            text.text = MoneyManager.Main.Balance.ToString();
        }
    }
}
