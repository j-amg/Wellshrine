using Godot;
using System;

public partial class IceRay : Node3D
{

    private Sprite3D sprite;
    public override void _Ready()
    {
        //sprite = GetNode<Sprite3D>("ray");
        Fade();
    }

    public override void _PhysicsProcess(double delta)
	{
        //float angle = sprite.GlobalPosition.AngleTo(Global.Singleton.player.GlobalPosition);
        //sprite.LookAt(Global.Singleton.player.GlobalPosition);
        //sprite.Rotation = new Vector3(0,sprite.Rotation.Y, sprite.Rotation.Z);
        //GD.Print(sprite.Rotation);
        //sprite.LookAt(Global.Singleton.player.camera.GlobalTransform.Origin, new Vector3(0,0,1));
    }


    private async void Fade()
    {
        Sprite3D raySprite = GetNode<Sprite3D>("ray");
        Sprite3D frontSprite = GetNode<Sprite3D>("frontRay");
        Tween frontTween = GetTree().CreateTween();
        Tween sideTween = GetTree().CreateTween();
        sideTween.TweenProperty(raySprite, "modulate", new Color(0,0,0,0), 1f);
        frontTween.TweenProperty(frontSprite, "modulate", new Color(0,0,0,0), 1f);
        await ToSignal(frontTween, Tween.SignalName.Finished);
        CallDeferred("queue_free");
    }
}
