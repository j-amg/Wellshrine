using Godot;
using System;

public partial class Weapon : Node
	{
    		public string name;
    		public float damageMin;
    		public float damageMax;
            public float critChance;
    		public float recharge;
    		public float stunDuration;
            public float recoil;
            public string description;

            public static Weapon InitWeapon(string name, float damageMin, float damageMax, float critChance, float recharge, float stunDuration, float recoil, string description)
            {
                Weapon w = new();
                w.name = name;
                w.damageMin = damageMin;
                w.damageMax = damageMax;
                w.critChance = critChance;
                w.recharge = recharge;
                w.stunDuration = stunDuration;
                w.recoil = recoil;
                w.description = description;
                return w;
            }
	}