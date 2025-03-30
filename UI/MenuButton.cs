using Godot;
public partial class MenuButton : TextureButton
{
	[Export]
	public AudioStream focus;
	[Export]
	public AudioStream pressed;
	[Export]
	public TextureRect select;
	[Export]
	public Label label;
	[Export]
	public string text;
	public bool autoFocussed = false;
	Global globals;
	public override void _Ready()
	{
		HideSelect();
		label.Text = text;
		FocusMode = FocusModeEnum.All;
		FocusEntered += OnFocusEntered;
		FocusExited += OnFocusExited;
		Pressed += OnPressed;

	}
    private void OnPressed() => Global.Singleton.PlaySound2D(pressed);
    private void OnFocusExited() => HideSelect();
    private void OnFocusEntered()
    {
		Global.Singleton.PlaySound2D(pressed);
        ShowSelect(autoFocussed);
    }

    public void ShowSelect(bool autofocussed)
	{
		//if (!autofocussed) Global.Singleton.PlaySound(focus);
		autoFocussed = false;
		select.Visible = true;
		
	}
	public void HideSelect()
	{
		select.Visible = false;
	}
}
