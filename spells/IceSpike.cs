using Godot;
using System;

public partial class IceSpike : Node3D
{
    public void Cast()
    {
        GD.Print("cast");
        // base.Cast(player);
        // foreach (IDamageable enemy in GetNode<CollisionShape3D>("CollisionShape3D").GetOverlappingBodies().Cast<IDamageable>()) enemy.Damage(d);
		// foreach (Area3D area in shockCollision.GetOverlappingAreas()) if (area is Projectile p) p.Destroy();
		// camera.GetNode<AnimatedSprite3D>("shock").Play("cycle");
		// Tween tween = GetTree().CreateTween();
		// tween.TweenProperty(camera.GetNode<AnimatedSprite3D>("shock"), "modulate", new Color(0,0,0,0), .25).From(new Color(1,1,1,.75f));
    }   
    	//Global.Singleton.PlaySound2D(castShock);

}
