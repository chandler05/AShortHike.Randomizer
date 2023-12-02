﻿using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Items;
using AShortHike.Randomizer.Notifications;
using AShortHike.Randomizer.Settings;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class Randomizer
    {
        private readonly ConnectionHandler _connection = new();
        private readonly ItemHandler _items = new();
        private readonly NotificationHandler _notifications = new();
        private readonly SettingsHandler _settings = new();
        private readonly DataStorage _data = new();

        private string _currentScene;

        public ConnectionHandler Connection => _connection;
        public ItemHandler Items => _items;
        public NotificationHandler Notifications => _notifications;
        public SettingsHandler Settings => _settings;
        public DataStorage Data => _data;

        public MultiworldSettings MultiworldSettings { get; set; } = new();

        public void OnSceneLoaded(string scene)
        {
            if (scene == "GameScene")
            {
                _items.LoadChestObjects();
                _items.ReplaceWorldObjectsWithChests();
                _connection.SendAllLocations();
            }
            else
            {
                _connection.Disconnect();
            }
            
            if (scene == "TitleScene")
            {
                _settings.SetupInputUI();
            }

            _currentScene = scene;
        }

        public void Update()
        {
            // Debugging
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                Main.Log("Giving cheat items!");
                Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), 1, false);
            }
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                Main.Log("Removing cheat items!");
                Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), -1, false);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Main.Log("Removing cheat items!");
                Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("SilverFeather"), -1, false);
            }

            if (_currentScene == "GameScene")
            {
                _connection.UpdateReceivers();
                _notifications.UpdateNotifications();
            }

            // Chest angle testing
            //if (lastChest != null)
            //{
            //    float currAngle = lastChest.localEulerAngles.y;
            //    float factor = 10;
            //    if (Input.GetKeyDown(KeyCode.Equals))
            //    {
            //        currAngle = Mathf.Round((currAngle + factor) / factor) * factor;
            //        lastChest.rotation = Quaternion.Euler(0, currAngle, 0);
            //        Main.LogError("New rotation angle: " + currAngle);
            //    }
            //    else if (Input.GetKeyDown(KeyCode.Minus))
            //    {
            //        currAngle = Mathf.Round((currAngle - factor) / factor) * factor;
            //        lastChest.rotation = Quaternion.Euler(0, currAngle, 0);
            //        Main.LogError("New rotation angle: " + currAngle);
            //    }
            //}
        }

        public void CheckForHelpGoal()
        {
            if (MultiworldSettings.goal != GoalType.Help)
                return;

            var tags = Singleton<GlobalData>.instance.gameData.tags;

            foreach (string tag in _flagsForHelpGoal)
            {
                if (!tags.GetBool(tag))
                    return;
            }

            Connection.SendGoal(GoalType.Help);
        }

        // Chest angle testing
        public Transform lastChest;

        public void OnConnect()
        {
        }

        public void OnDisconnect()
        {
        }

        private readonly string[] _flagsForHelpGoal = new string[]
        {
            "Opened_ToughBirdNPC (1)[9]",       // Give coins to tough bird salesman
            "Opened_Frog_StandingNPC[0]",       // Trade toy shovel
            "Opened_CamperNPC[1]",              // Return camping permit
            "Opened_DeerKidBoat[0]",            // Complete boat challenge
            "Opened_Bunny_WalkingNPC (1)[0]",   // Return headband to rabbit
            "Opened_SittingNPC[0]",             // Purchase sunhat
            "Opened_Goat_StandingNPC[0]",       // Return watch to camper
            "Opened_StandingNPC[0]",            // Cheer up artist
            "Opened_LittleKidNPCVariant (1)[0]",// Collect 15 shells for the kid
            "Opened_AuntMayNPC[0]",             // Give shell necklace to Ranger May
            "FoxClimbedToTop",                  // Help fox up the mountain
        };
    }
}
