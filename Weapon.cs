using Godot;
using System;

public partial class Weapon : Node
	{
    		public string name;
    		public int damageMin;
    		public int damageMax;
    		public float recharge;
    		public float stunDuration;
            public float recoil;

            public static Weapon InitWeapon(string name, int damageMin, int damageMax, float recharge, float stunDuration, float recoil)
            {
                Weapon w = new();
                w.name = name;
                w.damageMin = damageMin;
                w.damageMax = damageMax;
                w.recharge = recharge;
                w.stunDuration = stunDuration;
                w.recoil = recoil;
                return w;
            }
	}