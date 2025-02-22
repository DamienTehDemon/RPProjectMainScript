﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using client.Main.Vehicles;

namespace client.Main.Activities
{
    public enum TruckTypes
    {
        Hauler,
        Phantom,
        Packer
    };

    public enum LoadTypes
    {
        Tanker,
        Logs,
        Trailer,
        DockTrailer,
        Cars,
        Boat
    }

    public enum TruckingMenuTypes
    {
        TruckRentalTake,
        TruckRentalReturn,
        TrailerDestinations,
        DeliveryMenu
    }

    public class TruckingTerminal
    {

        public float X;
        public float Y;
        public float Z;
        public LoadTypes Type = LoadTypes.Trailer;

        public TruckingTerminal(float x, float y, float z, LoadTypes type)
        {
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }
    }

    public class TruckingDestination
    {
        public string Name;
        public float X;
        public float Y;
        public float Z;
        public int Payout;

        public TruckingDestination(string name, float x, float y, float z, int payout)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Payout = payout;
        }

    }

    public class Trucking : BaseScript
    {
        public static Trucking Instance;

        private Dictionary<string, int> _rentalPrices = new Dictionary<string, int>()
        {
            //["Hauler"] = 2000,
            ["Phantom"] = 2000,
            ["Packer"] = 5000
        };

        private List<TruckingTerminal> _terminals = new List<TruckingTerminal>()
        {
            new TruckingTerminal(806.66827392578f, -3025.4377441406f, 5.7421236038208f, LoadTypes.Trailer),
        }; 


        private List<Vector3> _rentals = new List<Vector3>()
        {
            new Vector3(772.487732f,-2962.51953f,5.81881475f),
        };

        private Dictionary<LoadTypes, List<TruckingDestination>> _destinations = new Dictionary<LoadTypes, List<TruckingDestination>>()
        {
            [LoadTypes.Trailer] = new List<TruckingDestination>()
            {
                new TruckingDestination("Davis Ave. Shopping Center", 242.32659912109f, -1517.1733398438f, 28.644651412964f, 300),
                new TruckingDestination("Davis Ave. 24/7 Shop", -66.943641662598f, -1742.4818115234f, 28.814619064331f, 300),
                new TruckingDestination("Innocence Blvd 24/7 Shop", 27.832950592041f, -1306.0754394531f, 28.575904846191f, 500),
                new TruckingDestination("Elgin Ave. Ammunation", 37.941116333008f, -1102.9970703125f, 28.795278549194f, 500),
                new TruckingDestination("Banner Hotel", -287.2790222168f, -1028.03125f, 29.895013809204f, 600),
                new TruckingDestination("Clinton Ave. 24/7 Shop", 367.90087890625f, 339.16262817383f, 102.79844665527f, 600),
                new TruckingDestination("Split Sides Comedy Club", -422.29284667969f, 293.78341674805f, 82.739112854004f, 800),
                new TruckingDestination("Tequilala Bar & Grill", -554.65325927734f, 302.55145263672f, 83.225646972656f, 800),
                new TruckingDestination("Bahama Mamas", -1365.8630371094f, -589.89636230469f, 29.00389289856f, 800),
                new TruckingDestination("Construciton Site, Paleto, Procopio Dr",79.707107543946f,6556.98046875f,31.302101135254f,2600),
                new TruckingDestination("Paleto Blvd Truck Stop",134.97045898438f,6622.0288085938f,31.52738571167f,2600),
                new TruckingDestination("Paleto Blvd/Great Ocen Fwy Dream View Motel",-85.227966308594f,6346.3979492188f,31.259307861328f,2600),
                new TruckingDestination("Cluckin Bell Paleto Blvd",-76.242240905762f,6269.8598632812f,31.144611358642f,3000),
                new TruckingDestination("Cluckin Bell Paleto Blvd 2",-128.8243560791f,6215.5913085938f,30.972526550292f,3000),
                new TruckingDestination("Jetsam Paleto Bay Blvd Baydoor",-249.0103302002f,6138.8979492188f,30.927858352662f,3000),
                new TruckingDestination("Great Ocean Hwy Resteraunt Hookies",-2200.3666992188f,4261.4013671875f,47.744899749756f,2750),
                new TruckingDestination("Great Ocean Hwy Shopping Center",-3149.0107421875f,1075.3111572266f,20.445943832398f,2350),
                new TruckingDestination("Inseno Road, Great Ocean 24/7",-3045.2829589844f,603.07781982422f,7.1277832984924f,2550),
                new TruckingDestination("Great Ocean Hwy Pacif,ic Bluf,f,s Country Club",-3019.8627929688f,97.129119873046f,11.395851135254f,2350),
                new TruckingDestination("Pipeline Inn, Del Perro Fwy, Great Ocean Hway",-2181.5187988282f,-381.40686035156f,13.021412849426f,2560),
                new TruckingDestination("East GAlileo Ave, Galileo Park Observatory",-412.65979003906f,1177.7895507812f,325.408203125f,2660),
                new TruckingDestination("Hollywood Sign Utility Center",813.57489013672f,1276.2419433594f,360.2700805664f,1200),
                new TruckingDestination("Bolingbroke State Prison",1865.3829345704f,2605.6430664062f,45.423557281494f,1600),
                new TruckingDestination("Sandy Shores Airfield",1731.0590820312f,3310.7241210938f,40.974891662598f,1395),
                new TruckingDestination("UTool Senora Fwy",2761.9626464844f,3468.806640625f,55.413646697998f,1800),
                new TruckingDestination("F,rankies Auto, Grapeseed",2305.3835449218f,4888.6489257812f,41.549850463868f,1903),
                new TruckingDestination("Grapeseed Airf,ield",2116.720703125f,4795.4140625f,40.845790863038f,1922),
            }
        };

        private UIMenu _menu;
        private bool _menuOpen = false;
        private bool _menuCreated = false;
        private int _menuIndex = 0;
        private TruckingMenuTypes _menuType = TruckingMenuTypes.TruckRentalTake;

        private TruckingDestination _currentDestination;

        private int _truckRental = -1;
        private string _truckRented = null;
        private int _truckTrailer = -1;
        private int destBlip;


        public Trucking()
        {
            Instance = this;
            SetupBlips();
            EventHandlers["RentTruck"] += new Action<string>(RentTruck);
            EventHandlers["AttemptReturnTruck"] += new Action(ReturnTruck);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            MenuCheck();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            DrawMarkers();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }
        

        private async Task DrawMarkers()
        {
            while (true)
            {
                foreach (var pos in _terminals)
                {
                    if (Utility.Instance.GetDistanceBetweenVector3s(new Vector3(pos.X, pos.Y, pos.Z), Game.PlayerPed.Position) < 30)
                    {
                        World.DrawMarker(MarkerType.HorizontalCircleSkinny, new Vector3(pos.X, pos.Y, pos.Z) - new Vector3(0, 0, 0.8f), Vector3.Zero, Vector3.Zero, new Vector3(5, 5, 5), Color.FromArgb(175, 255, 150, 0));
                    }
                }
                foreach (var pos in _rentals)
                {
                    if (Utility.Instance.GetDistanceBetweenVector3s(new Vector3(pos.X, pos.Y, pos.Z), Game.PlayerPed.Position) < 30)
                    {
                        World.DrawMarker(MarkerType.HorizontalCircleSkinny, new Vector3(pos.X, pos.Y, pos.Z) - new Vector3(0, 0, 0.8f), Vector3.Zero, Vector3.Zero, new Vector3(5, 5, 5), Color.FromArgb(175, 255, 150, 0));
                    }
                }

                if (_currentDestination != null)
                {
                    if (Utility.Instance.GetDistanceBetweenVector3s(new Vector3(_currentDestination.X, _currentDestination.Y, _currentDestination.Z), Game.PlayerPed.Position) < 30)
                    {
                        World.DrawMarker(MarkerType.HorizontalCircleSkinny, new Vector3(_currentDestination.X, _currentDestination.Y, _currentDestination.Z) - new Vector3(0, 0, 0.8f), Vector3.Zero, Vector3.Zero, new Vector3(3, 3, 3), Color.FromArgb(175, 255, 150, 0));
                    }
                }

                await Delay(0);
            }
        }

        private void SetupBlips()
        {
            foreach (var var in _terminals)
            {
                var blip = API.AddBlipForCoord(var.X, var.Y, var.Z);
                API.SetBlipSprite(blip, 477);
                API.SetBlipColour(blip, 5);
                API.SetBlipScale(blip, 0.8f);
                API.SetBlipAsShortRange(blip, true);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("Trucking Terminal");
                API.EndTextCommandSetBlipName(blip);
            }

            foreach (var var in _rentals)
            {
                var blip = API.AddBlipForCoord(var.X, var.Y, var.Z);
                API.SetBlipSprite(blip, 477);
                API.SetBlipColour(blip, 6);
                API.SetBlipAsShortRange(blip, true);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("Trucking Rentals");
                API.EndTextCommandSetBlipName(blip);
            }
        }

        private async void RentTruck(string truck)
        {
            var ped = Game.PlayerPed.Handle;

            var vehicle = (uint)API.GetHashKey(truck);
            API.RequestModel(vehicle);
            while (!API.HasModelLoaded(vehicle))
            {
                await Delay(1);
            }
            var coords = API.GetOffsetFromEntityInWorldCoords(ped, 0, 5.0f, 0);
            _truckRental = API.CreateVehicle(vehicle, coords.X, coords.Y, coords.Z, API.GetEntityHeading(ped), true,
                false);
            _truckRented = truck;
            Game.PlayerPed.SetIntoVehicle((Vehicle)Vehicle.FromHandle(_truckRental),VehicleSeat.Driver);
            API.SetVehicleNumberPlateText(_truckRental, "RENTAL");
            API.SetVehicleOnGroundProperly(_truckRental);
            API.SetModelAsNoLongerNeeded(vehicle);
            API.SetEntityAsMissionEntity(_truckRental, true, true);
            API.SetVehicleHasBeenOwnedByPlayer(_truckRental, true);

            var blip = API.AddBlipForEntity(_truckRental);
            API.SetBlipAsFriendly(blip, true);
            API.SetBlipSprite(blip, 225);
            API.SetBlipColour(blip, 3);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Rented Vehicle");
            API.EndTextCommandSetBlipName(blip);

            InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
            _menuCreated = false;
            InteractionMenu.Instance._interactionMenu.RemoveItemAt(_menuIndex);
            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
            API.DecorSetInt(_truckRental, "PIRP_VehicleOwner", Game.Player.ServerId);
            Entity.FromHandle(_truckRental).Position = new Vector3(807.68005371094f, -3040.2846679688f, 5.7421259880066f);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            RentalCarCheck();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private async Task RentalCarCheck()
        {
            while (_truckRental != -1)
            {
                if (API.IsEntityDead(_truckRental))
                {
                    _truckRental = -1;
                    _truckRented = null;
                    Utility.Instance.SendChatMessage("[TRUCKING]", "Your rental truck has been destroyed! You have lost your deposit!", 255, 255, 0);
                    if (_truckTrailer != -1)
                    {
                        API.DeleteVehicle(ref _truckTrailer);
                        _truckTrailer = -1;
                        EndDestination();
                        Utility.Instance.SendChatMessage("[TRUCKING]", "Your rental truck has been destroyed! So you lost your cargo!", 255, 255, 0);
                    }
                }
                await Delay(3000);
            }
        }

        private void EndDestination()
        {
            _currentDestination = null;
            API.DeleteEntity(ref _truckTrailer);
            _truckTrailer = -1;
            API.RemoveBlip(ref destBlip);
        }

        private async Task TrailerCheck()
        {

            while (_truckTrailer != -1)
            {
                if (API.IsEntityDead(_truckTrailer))
                {
                    EndDestination();
                    Utility.Instance.SendChatMessage("[TRUCKING]", "Your cargo has been destroyed!", 255, 255, 0);
                }

                var truck = API.GetVehiclePedIsIn(Game.PlayerPed.Handle, false);
                if (Game.PlayerPed.IsInVehicle() && !API.IsVehicleAttachedToTrailer(truck))
                {
                    EndDestination();
                    Utility.Instance.SendChatMessage("[TRUCKING]", "You have lost your cargo!", 255, 255, 0);
                }

                if (Utility.Instance.GetDistanceBetweenVector3s(Game.PlayerPed.Position,
                        Entity.FromHandle(_truckTrailer).Position) > 30)
                {
                    EndDestination();
                    Utility.Instance.SendChatMessage("[TRUCKING]", "You have lost your cargo!", 255, 255, 0);
                }

                await Delay(3000);
            }
        }

        private void ReturnTruck()
        {
            if (_truckRental != -1)
            {
                Debug.WriteLine(_truckRented);
                TriggerServerEvent("SuccessfulTruckRentalReturn", _truckRented);
                API.DeleteEntity(ref _truckRental);
                _truckRental = -1;
                InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
                _menuCreated = false;
                _truckRented = null;
                InteractionMenu.Instance._interactionMenu.RemoveItemAt(_menuIndex);
                InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
            }
        }

        private async Task MenuCheck()
        {
            while (true)
            {
                _menuOpen = false;
                if (_currentDestination == null)
                {
                    //Handles rental menu
                    foreach (var rental in _rentals)
                    {
                        var distance = API.Vdist(rental.X, rental.Y, rental.Z, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
                        if (distance < 15)
                        {
                            if (!API.DoesEntityExist(_truckRental) || !Game.PlayerPed.IsInVehicle())
                            {
                                _menuOpen = true;
                                _menuType = TruckingMenuTypes.TruckRentalTake;
                            }
                            else if (API.IsPedInAnyVehicle(Game.PlayerPed.Handle, false) && API.GetVehiclePedIsIn(Game.PlayerPed.Handle, false) == _truckRental)
                            {
                                _menuOpen = true;
                                _menuType = TruckingMenuTypes.TruckRentalReturn;
                            }
                        }
                    }

                    foreach (var terminal in _terminals)
                    {
                        var distance = API.Vdist(terminal.X, terminal.Y, terminal.Z, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
                        // This selects all the rental prices with a hash key converted to a string that matches the haskey converted to a string of the vehicle that the player is driving. :)
                        if (distance < 4 && API.IsPedInAnyVehicle(Game.PlayerPed.Handle, false))
                        {
                            _menuOpen = true;
                            _menuType = TruckingMenuTypes.TrailerDestinations;
                        }
                    }
                }
                else
                {
                    var distance = API.Vdist(_currentDestination.X, _currentDestination.Y, _currentDestination.Z, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
                    if (distance < 8)
                    {
                        _menuOpen = true;
                        _menuType = TruckingMenuTypes.DeliveryMenu;
                    }
                }
                if (_menuOpen && !_menuCreated)
                {
                    var buttons = new List<UIMenuItem>();
                    switch (_menuType)
                    {
                        case TruckingMenuTypes.TruckRentalTake:
                            _menu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(
                                InteractionMenu.Instance._interactionMenu, "Truck Rentals", "Open a menu to rent trucks for the trucking activity.", new PointF(5, Screen.Height / 2));
                            //Loop trhough the keys of the rental prices and create ui elements for all of them.
                            foreach (var var in _rentalPrices.Keys)
                            {
                                var button = new UIMenuItem(var + " ~g~$" + _rentalPrices[var] + "", "Rent a " + var + " for " + _rentalPrices[var]);
                                _menu.AddItem(button);
                                buttons.Add(button);
                                var i = buttons.Count - 1;
                                _menu.OnItemSelect += (sender, item, index) =>
                                {
                                    if (item == buttons[i])
                                    {
                                        TriggerServerEvent("TruckingRentalRequest", var);
                                    }
                                };
                            }
                            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                            _menuCreated = true;
                            _menuIndex = InteractionMenu.Instance._interactionMenu.MenuItems.Count - 1;
                            break;


                        case TruckingMenuTypes.TruckRentalReturn:
                            _menu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(
                                InteractionMenu.Instance._interactionMenu, "Truck Rentals", "Open a menu to rent trucks for the trucking activity.", new PointF(5, Screen.Height / 2));
                            var returnButton = new UIMenuItem("Return Truck!", "Return the truck that you rented and get your deposit back!");
                            _menu.AddItem(returnButton);
                            _menu.OnItemSelect += (sender, item, index) =>
                            {
                                TriggerServerEvent("TruckReturnRequest");
                            };
                            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                            _menuCreated = true;
                            _menuIndex = InteractionMenu.Instance._interactionMenu.MenuItems.Count - 1;
                            break;


                        case TruckingMenuTypes.TrailerDestinations:
                            _menu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(
                                InteractionMenu.Instance._interactionMenu, "Trucking Terminal", "Open a menu to be dispatched to to a destination as a trucker.", new PointF(5, Screen.Height / 2));
                            //Loop trhough the keys of the rental prices and create ui elements for all of them.
                            foreach (var var in _destinations[LoadTypes.Trailer])
                            {
                                var button = new UIMenuItem(var.Name + " ~g~$" + var.Payout, "Deliver trailer to " + var.Name + " for $" + var.Payout);
                                _menu.AddItem(button);
                                buttons.Add(button);
                                var i = buttons.Count - 1;
                                _menu.OnItemSelect += (sender, item, index) =>
                                {
                                    if (item == buttons[i])
                                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                                        SetupDestination(var, LoadTypes.Trailer);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                                        InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
                                        _menuCreated = false;
                                        InteractionMenu.Instance._interactionMenu.RemoveItemAt(_menuIndex);
                                        InteractionMenu.Instance._interactionMenuPool.RefreshIndex();

                                    }
                                };
                            }
                            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                            _menuCreated = true;
                            _menuIndex = InteractionMenu.Instance._interactionMenu.MenuItems.Count - 1;
                            break;


                        case TruckingMenuTypes.DeliveryMenu:
                            var deliverButton = new UIMenuItem("Finish your delivery!", "Drop off your delivery at the destination and recieve your compensation!");
                            InteractionMenu.Instance._interactionMenu.AddItem(deliverButton);
                            InteractionMenu.Instance._interactionMenu.OnItemSelect += (sender, item, index) =>
                            {
                                if (item == deliverButton)
                                {
                                    TriggerServerEvent("CompletedTruckerMission", _currentDestination.Name);
                                    EndDestination();
                                    InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
                                    _menuCreated = false;
                                    InteractionMenu.Instance._interactionMenu.RemoveItemAt(_menuIndex);
                                    InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                                }
                            };
                            InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                            _menuCreated = true;
                            _menuIndex = InteractionMenu.Instance._interactionMenu.MenuItems.Count - 1;
                            break;
                    }
                }
                else if (!_menuOpen && _menuCreated)
                {
                    _menuCreated = false;
                    var i = 0;
                    foreach (var item in InteractionMenu.Instance._interactionMenu.MenuItems)
                    {
                        if (item == _menu.ParentItem)
                        {
                            InteractionMenu.Instance._interactionMenu.RemoveItemAt(i);
                            break;
                        }
                        i++;
                    }
                    InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                }
                await Delay(1000);
            }


        }


        private async Task SetupDestination(TruckingDestination dest, LoadTypes type)
        {
            _currentDestination = dest;
            var ped = Game.PlayerPed.Handle;
            Game.PlayerPed.CurrentVehicle.Position = new Vector3(807.68005371094f, -3040.2846679688f, 5.7421259880066f);
            var truck = "Trailers2";
            var rdm = new Random();
            var rdmInt = rdm.Next(3);
            //Determine the traielr to use.
            switch (type)
            {
                case LoadTypes.Trailer:
                    rdmInt = rdm.Next(3);
                    switch (rdmInt)
                    {
                        case 1:
                            truck = "Trailers";
                            break;
                        case 2:
                            truck = "Trailers2";
                            break;
                        case 3:
                            truck = "Trailers3";
                            break;
                    }
                    break;
                case LoadTypes.Tanker:
                    rdmInt = rdm.Next(2);
                    switch (rdmInt)
                    {
                        case 1:
                            truck = "Tanker";
                            break;
                        case 2:
                            truck = "Tanker2";
                            break;
                    }
                    break;
                case LoadTypes.Logs:
                    truck = "TrailerLogs";
                    break;
                case LoadTypes.DockTrailer:
                    truck = "DockTrailer";
                    break;
                case LoadTypes.Cars:
                    truck = "TR4";
                    break;
                case LoadTypes.Boat:
                    truck = "TR3";
                    break;
            }
            var vehicle = (uint)API.GetHashKey(truck);
            API.RequestModel(vehicle);
            while (!API.HasModelLoaded(vehicle))
            {
                await Delay(1);
            }
            var coords = API.GetOffsetFromEntityInWorldCoords(ped, 0, -5.0f, 0);
            _truckTrailer = API.CreateVehicle(vehicle, coords.X, coords.Y, coords.Z, API.GetEntityHeading(ped), true,
                false);
            API.SetEntityAsMissionEntity(_truckTrailer, true, true);
            API.SetVehicleOnGroundProperly(_truckTrailer);
            API.SetModelAsNoLongerNeeded(vehicle);
            var trailirBlip = API.AddBlipForEntity(_truckTrailer);
            API.SetBlipAsFriendly(trailirBlip, true);
            API.SetBlipSprite(trailirBlip, 225);
            API.SetBlipColour(trailirBlip, 3);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Trailer");
            API.EndTextCommandSetBlipName(trailirBlip);
            
            API.AttachVehicleToTrailer(Game.PlayerPed.CurrentVehicle.Handle, _truckTrailer, 10);

            destBlip = API.AddBlipForCoord(dest.X, dest.Y, dest.Z);
            API.SetBlipSprite(destBlip, 315);
            API.SetBlipColour(destBlip, 5);
            API.SetBlipScale(destBlip, 0.5f);
            API.SetBlipAsShortRange(destBlip, true);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Trucking Destination");
            API.EndTextCommandSetBlipName(destBlip);
            API.SetBlipRoute(destBlip, true);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            TrailerCheck();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

    }

}
