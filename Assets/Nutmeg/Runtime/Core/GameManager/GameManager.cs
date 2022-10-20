using System;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using UnityEngine;

namespace Nutmeg.Runtime.Core.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Main { get; private set; }

        public static GameState CurrentGameState { get; private set; }

        public static Action<GameState, GameState> onGameStateChange;

        public static PlayerCharacter selectedCharacter; 
        
        private void Start()
        {
            Main = this;
            DontDestroyOnLoad(this);
        }

        public static bool ChangeGameState(GameState newGameState)
        {
            if (newGameState < CurrentGameState)
                return false;
            
            onGameStateChange?.Invoke(CurrentGameState, newGameState);

            CurrentGameState = newGameState;
            return true;
        }
    }

    public enum GameState
    {
        Booting = 100, 
        Loading = 50,
        InGame = 0,
        InMenu = 0,
        Paused = 0,
        Saving = 80,
        Exit = 30
    }
}
