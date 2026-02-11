using Godot;
using System;

// public partial class IceSpike : Spell
// {
//     [Export]
//     public PackedScene iceray;
//     public override void Cast(Player player)
//     {
//         base.Cast(player);
//         Global.Singleton.PlaySound2D(castIce);
// 	    foreach (IDamageable enemy in iceCollision.GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(Damage.InitDamage(5, false, this));
// 	    foreach (Area3D area in iceCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
// 	    IceRay ray = iceray.Instantiate() as IceRay;
// 	    var main = GetTree().CurrentScene;
// 	    main.CallDeferred("add_child", ray);
// 	    ray.Transform = player.head.GlobalTransform;
//     }
// }
