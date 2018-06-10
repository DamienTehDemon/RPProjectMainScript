﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace roleplay.Main.HUD
{
    public class VehicleHUD : BaseScript
    {
        public static VehicleHUD Instance;

        private Dictionary<string,string> _zones = new Dictionary<string, string>()
        {
            ["AIRP"] = "Los Santos International Airport",
            ["ALAMO"] = "Alamo Sea",
            ["ALTA"] = "Alta",
            ["ARMYB"] = "Fort Zancudo",
            ["BANHAMC"] = "Banham Canyon Dr",
            ["BANNING"] = "Banning",
            ["BEACH"] = "Vespucci Beach",
            ["BHAMCA"] = "Banham Canyon",
            ["BRADP"] = "Braddock Pass",
            ["BRADT"] = "Braddock Tunnel",
            ["BURTON"] = "Burton",
            ["CALAFB"] = "Calafia Bridge",
            ["CANNY"] = "Raton Canyon",
            ["CCREAK"] = "Cassidy Creek",
            ["CHAMH"] = "Chamberlain Hills",
            ["CHIL"] = "Vinewood Hills",
            ["CHU"] = "Chumash",
            ["CMSW"] = "Chiliad Mountain State Wilderness",
            ["CYPRE"] = "Cypress Flats",
            ["DAVIS"] = "Davis",
            ["DELBE"] = "Del Perro Beach",
            ["DELPE"] = "Del Perro",
            ["DELSOL"] = "La Puerta",
            ["DESRT"] = "Grand Senora Desert",
            ["DOWNT"] = "Downtown",
            ["DTVINE"] = "Downtown Vinewood",
            ["EAST_V"] = "East Vinewood",
            ["EBURO"] = "El Burro Heights",
            ["ELGORL"] = "El Gordo Lighthouse",
            ["ELYSIAN"] = "Elysian Island",
            ["GALFISH"] = "Galilee",
            ["GOLF"] = "GWC and Golfing Society",
            ["GRAPES"] = "Grapeseed",
            ["GREATC"] = "Great Chaparral",
            ["HARMO"] = "Harmony",
            ["HAWICK"] = "Hawick",
            ["HORS"] = "Vinewood Racetrack",
            ["HUMLAB"] = "Humane Labs and Research",
            ["JAIL"] = "Bolingbroke Penitentiary",
            ["KOREAT"] = "Little Seoul",
            ["LACT"] = "Land Act Reservoir",
            ["LAGO"] = "Lago Zancudo",
            ["LDAM"] = "Land Act Dam",
            ["LEGSQU"] = "Legion Square",
            ["LMESA"] = "La Mesa",
            ["LOSPUER"] = "La Puerta",
            ["MIRR"] = "Mirror Park",
            ["MORN"] = "Morningwood",
            ["MOVIE"] = "Richards Majestic",
            ["MTCHIL"] = "Mount Chiliad",
            ["MTGORDO"] = "Mount Gordo",
            ["MTJOSE"] = "Mount Josiah",
            ["MURRI"] = "Murrieta Heights",
            ["NCHU"] = "North Chumash",
            ["NOOSE"] = "N.O.O.S.E",
            ["OCEANA"] = "Pacific Ocean",
            ["PALCOV"] = "Paleto Cove",
            ["PALETO"] = "Paleto Bay",
            ["PALFOR"] = "Paleto Forest",
            ["PALHIGH"] = "Palomino Highlands",
            ["PALMPOW"] = "Palmer-Taylor Power Station",
            ["PBLUFF"] = "Pacific Bluffs",
            ["PBOX"] = "Pillbox Hill",
            ["PROCOB"] = "Procopio Beach",
            ["RANCHO"] = "Rancho",
            ["RGLEN"] = "Richman Glen",
            ["RICHM"] = "Richman",
            ["ROCKF"] = "Rockford Hills",
            ["RTRAK"] = "Redwood Lights Track",
            ["SANAND"] = "San Andreas",
            ["SANCHIA"] = "San Chianski Mountain Range",
            ["SANDY"] = "Sandy Shores",
            ["SKID"] = "Mission Row",
            ["SLAB"] = "Stab City",
            ["STAD"] = "Maze Bank Arena",
            ["STRAW"] = "Strawberry",
            ["TATAMO"] = "Tataviam Mountains",
            ["TERMINA"] = "Terminal",
            ["TEXTI"] = "Textile City",
            ["TONGVAH"] = "Tongva Hills",
            ["TONGVAV"] = "Tongva Valley",
            ["VCANA"] = "Vespucci Canals",
            ["VESP"] = "Vespucci",
            ["VINE"] = "Vinewood",
            ["WINDF"] = "Ron Alternates Wind Farm",
            ["WVINE"] = "West Vinewood",
            ["ZANCUDO"] = "Zancudo River",
            ["ZP_ORT"] = "Port of South Los Santos",
            ["ZQ_UAR"] = "Davis Quartz"
        };

        private Dictionary<int, string> _directions = new Dictionary<int, string>()
        {
            [0] = "N",
            [45] = "NW",
            [90] = "W",
            [135] = "SW",
            [180] = "S",
            [225] = "SE",
            [270] = "E",
            [315] = "NE",
            [360] = "N"
        };

        private string _direction = "N";

        public VehicleHUD()
        {
            Instance = this;
            Tick += new Func<Task>(async delegate
            {
                var pid = Game.PlayerPed.Handle;
                if (API.IsPedInAnyVehicle(pid, false))
                {
                    var veh = API.GetVehiclePedIsIn(pid, false);
                    var kmh = API.GetEntitySpeed(veh) * 3.6f;
                    var mph = API.GetEntitySpeed(veh) * 2.236936f;
                    var plateVeh = API.GetVehicleNumberPlateText(veh);
                    var pos = API.GetEntityCoords(pid,true); 
                    var streetOne = new uint();
                    var streetTwo = new uint();
                    API.GetStreetNameAtCoord(pos.X,pos.Y,pos.Z, ref streetOne,ref streetTwo);
                    var streetOneName = API.GetStreetNameFromHashKey(streetOne);
                    var streetTwoName = API.GetStreetNameFromHashKey(streetTwo);
                    var zoneName = "Unknown";
                    if (_zones.ContainsKey(API.GetNameOfZone(pos.X, pos.Y, pos.Z)))
                    {
                        zoneName = _zones[API.GetNameOfZone(pos.X, pos.Y, pos.Z)];
                    }
                    API.DisplayRadar(true);

                    foreach (var direction in _directions)
                    {
                        var tmpHeading = API.GetEntityHeading(pid);
                        if (Math.Abs(tmpHeading - direction.Key) < 22.5)
                        {
                            _direction = direction.Value;
                        }
                    }

                    Utility.Instance.DrawRct(0.118f, 0.944f, 0.037f, 0.020f, 0, 0, 0, 255);
                    Utility.Instance.DrawRct(0.0147f, 0.944f, 0.104f, 0.020f, 0, 0, 0, 255);
                    Utility.Instance.DrawTxt(0.065f, 0.944f, 1.0f, 1.0f, 0.25f, "~w~" + streetOneName + "," + streetTwoName, 255, 255, 255, 255, true);
                    Utility.Instance.DrawTxt(0.02f, 0.942f, 1.0f, 1.0f, 0.35f, "~b~" + _direction, 255, 255, 255, 255, true);
                    Utility.Instance.DrawTxt(0.128f, 0.9403f, 1.0f, 1.0f, 0.4f, "~w~" + Math.Ceiling(mph), 255, 255, 255, 255,true);
                    Utility.Instance.DrawTxt(0.135f, 0.942f, 1.0f, 1.0f, 0.3f, "~b~ mph", 255, 255, 255, 255, false);
                    
                }
                else
                {
                    var pos = API.GetEntityCoords(pid, true);
                    var streetOne = new uint();
                    var streetTwo = new uint();
                    API.GetStreetNameAtCoord(pos.X, pos.Y, pos.Z, ref streetOne, ref streetTwo);
                    var streetOneName = API.GetStreetNameFromHashKey(streetOne);
                    var streetTwoName = API.GetStreetNameFromHashKey(streetTwo);
                    var zoneName = "Unknown";
                    if (_zones.ContainsKey(API.GetNameOfZone(pos.X, pos.Y, pos.Z)))
                    {
                        zoneName = _zones[API.GetNameOfZone(pos.X, pos.Y, pos.Z)];
                    }
                    Utility.Instance.DrawRct(0.0147f, 0.944f, 0.142f, 0.020f, 0, 0, 0, 255);
                    Utility.Instance.DrawTxt(0.085f, 0.943f, 1.0f, 1.0f, 0.325f, "~w~" + streetOneName + "," + streetTwoName, 255, 255, 255, 255, true);
                    Utility.Instance.DrawTxt(0.02f, 0.942f, 1.0f, 1.0f, 0.35f, "~b~" + _direction, 255, 255, 255, 255, true);
                    API.DisplayRadar(false);
                }
            });
        }
    }
}