﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using client.Main;
using client.Main.Items;

namespace client.Main.Users.Inventory
{
    public class Item
    {
        public int Id = 1;
        public string Name = "Test Item";
        public string Description = "This is a item for testing. LUL. NOOB.";
        public int BuyPrice = 100;
        public int SellPrice = 100;
        public int Weight = 10;
        public bool Illegal = false;
    }

    public class InventoryUI : BaseScript
    {
        public static InventoryUI Instance;
        public List<Item> Inventory = new List<Item>();

        private UIMenu _menu;

        private Dictionary<string, int> quantitys = new Dictionary<string, int>();
        private Dictionary<int, UIMenuItem> _menuItems = new Dictionary<int, UIMenuItem>();

        private UIMenuItem _weight = null;
        private UIMenuItem _cashItem = null;
        private UIMenuItem _bankItem = null;
        private UIMenuItem _untaxedItem = null;

        public InventoryUI()
        {
            EventHandlers["RefreshInventoryItems"] += new Action<List<dynamic>,int,int,int,int,int>(RefreshItems);
            EventHandlers["RefreshMoney"] += new Action<int,int,int>(RefreshMoney);
            Instance = this;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            SetupUI();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private async Task SetupUI()
        {   
            while (InteractionMenu.Instance == null)
            {
                await Delay(0);
            }
            _menu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(InteractionMenu.Instance._interactionMenu, "Inventory", "Access your inventory", new PointF(5, Screen.Height / 2));
            InteractionMenu.Instance._menus.Add(_menu);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns>The amount of the item that the player has in thier inventory.</returns>
        public int HasItem(string itemName)
        {
            var amount = 0;
            foreach (Item item in Inventory)
            {
                if (item.Name == itemName)
                {
                    amount++;
                }
            }
            return amount;
        }

        private bool HasItem(int itemId)
        {
            foreach (Item item in Inventory)
            {
                if (item.Id == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        private async void RefreshItems(List<dynamic> Items, int cash, int bank, int untaxed, int maxinv, int curinv)
        {
            while (_menu == null)
            {
                await Delay(0);
            }
            _menu.Clear();
            Inventory.Clear();
            quantitys.Clear();
            //Cast the inventory items as items that are passed as dynamics.
            Inventory = Items.Select( x => new Item { Name = x.Name, Description = x.Description, BuyPrice = x.BuyPrice,
                SellPrice = x.SellPrice, Weight = x.Weight, Illegal = x.Illegal, Id = x.Id}).ToList();
            foreach (Item item in Inventory)
            {
                if (quantitys.ContainsKey(item.Name))
                {
                    quantitys[item.Name] = quantitys[item.Name] + 1;
                }
                else
                {
                    quantitys.Add(item.Name, 1);
                }
            }

            _weight = new UIMenuItem("~o~"+curinv+"kg/"+maxinv+ "kg", "Current inventory weight and maximum weight.");
            _cashItem = new UIMenuItem("~g~$" + cash, "How much legal cash you have on your character.");
            _bankItem = new UIMenuItem("~b~$" + bank, "How much money you have in your bank account.");
            _untaxedItem = new UIMenuItem("~r~$" + untaxed, "How much illegal cash you have on your character.");
            var _giveMoenyButton = new UIMenuItem("~p~Give Closest Player Money", "Give Closest Player Money.");

            _menu.AddItem(_weight);
            _menu.AddItem(_cashItem);
            _menu.AddItem(_bankItem);
            _menu.AddItem(_untaxedItem);
            _menu.AddItem(_giveMoenyButton);
            
            _menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == _giveMoenyButton)
                {
                    InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput(
                                "Amount of money to transfer from your wallet.", "", 10,
                                delegate (string amountS)
                                {
                                    if (Int32.TryParse(amountS, out var amount))
                                    {
                                        Utility.Instance.GetClosestPlayer(out var info);
                                        if (info.Dist < 5)
                                        {
                                            TriggerServerEvent("TransferCash", amount, API.GetPlayerServerId(info.Pid));
                                        }
                                    }
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                }
            };

            foreach (var itemID in quantitys.Keys)
            {
                //Look in the list for a entryr matching the ID the nget the name from that row.
                var itemName = Inventory.Find(x => x.Name == itemID).Name;
                var itemDesc = Inventory.Find(x => x.Name == itemID).Description;
                //Set the name of the sub menu title to the item name and the amount there is.
                var itemMenu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(_menu, itemName + ".x" + quantitys[itemID],itemDesc, new PointF(5, Screen.Height / 2));
                var itemUseButton = new UIMenuItem("Use Item");
                var itemDropButton = new UIMenuItem("Drop Item");
                var itemGiveButton = new UIMenuItem("Give Item");
                itemMenu.AddItem(itemUseButton);
                itemMenu.AddItem(itemDropButton);
                itemMenu.AddItem(itemGiveButton);
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                itemMenu.OnItemSelect += async (sender, item, index) =>
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                {
                    if (item == itemUseButton)
                    {
                        InventoryProcessing.Instance.Process(item, sender);
                    }
                    else if (item == itemDropButton)
                    {
                        InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        Utility.Instance.KeyboardInput("How many items should be dropped", "", 2, async delegate(string s) {
                            Game.PlayerPed.Task.PlayAnimation("mp_arresting", "a_uncuff");
                            await Delay(1000);
                            Game.PlayerPed.Task.ClearAll();
                            TriggerServerEvent("dropItem", itemName, Convert.ToInt16(s));
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        itemMenu.Visible = false;
                        _menu.Visible = true;
                        InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                    }
                    else if (item == itemGiveButton)
                    {
                        InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
                        ClosestPlayerReturnInfo output;
                        Utility.Instance.GetClosestPlayer(out output);
                        if (output.Dist < 5)
                        {
                            var pid = API.GetPlayerServerId(output.Pid);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput("How many items should be given", "", 2, new Action<string>((string result) =>
                                TriggerServerEvent("giveItem", pid , itemName, Convert.ToInt16(result))
                            ));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            if (itemMenu.Visible)
                            {
                                itemMenu.Visible = false;
                                _menu.Visible = true;
                            }
                            else
                            {
                                itemMenu.Visible = false;
                                _menu.Visible = false;
                            }
                            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                        }
                        else
                        {
                            TriggerEvent("chatMessage", "INVENTORY", new[] { 0, 255, 0 }, "No player is close enough to give anything to them!");
                        }
                    }
                };
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            Weapons.Instance.RefreshWeapons();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private void RefreshMoney(int cash, int bank, int untaxed)
        {
            _cashItem.Text = "~g~$" + cash;
            _bankItem.Text = "~b~$" + bank;
            _untaxedItem.Text = "~r~$" + untaxed;
        }

    }
}
 