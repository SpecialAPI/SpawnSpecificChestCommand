using BepInEx;
using Gungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpawnSpecificChestCommand
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.spawnspecificchestcommand";
        public const string NAME = "Spawn Specific Chest Command";
        public const string VERSION = "1.0.0";

        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager gm)
        {
            ETGModConsole.Commands.AddUnit("spawnspecificchest", SpawnSpecificChest, ETGModConsole.GiveAutocompletionSettings);
        }

        public static void SpawnSpecificChest(string[] args)
        {
            if (args.Length < 1)
            {
                ETGModConsole.Log("Item not given!");
                return;
            }

            var id = args[0];
            if (!Game.Items.ContainsID(id))
            {
                ETGModConsole.Log($"Invalid item ID {id}!");
                return;
            }

            if (!GameManager.HasInstance || GameManager.Instance.PrimaryPlayer == null)
            {
                ETGModConsole.Log("Player doesn't exist!");
                return;
            }

            var currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
            if (currentRoom == null)
                return;

            var item = Game.Items[id];
            var rewards = GameManager.Instance.RewardManager;
            var chest = item.quality switch
            {
                PickupObject.ItemQuality.S => rewards.S_Chest,
                PickupObject.ItemQuality.A => rewards.A_Chest,
                PickupObject.ItemQuality.B => rewards.B_Chest,
                PickupObject.ItemQuality.C => rewards.C_Chest,
                PickupObject.ItemQuality.D => rewards.D_Chest,

                _ => rewards.D_Chest
            };

            var location = currentRoom.GetBestRewardLocation(new IntVector2(2, 1), Dungeonator.RoomHandler.RewardLocationStyle.PlayerCenter, true);
            var c = Chest.Spawn(chest, location, currentRoom, true);

            if(c == null)
                return;

            c.ForceUnlock();
            c.forceContentIds = [item.PickupObjectId];
        }
    }
}
