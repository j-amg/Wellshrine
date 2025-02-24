using Godot;
public partial class MenuButton : TextureButton
{
	[Export]
	public string text;
	[Export]
	public AudioStream focus;
	[Export]
	public AudioStream pressed;
	private TextureRect select;
	public bool autoFocussed = false;
	Global globals;
	public override void _Ready()
	{
		select = GetNode<TextureRect>("select");
		GetNode<Label>("Label").Text = text;
		HideSelect();
		FocusMode = FocusModeEnum.All;
		FocusEntered += OnFocusEntered;
		FocusExited += OnFocusExited;
		//Pressed += OnPressed;

	}
    //private void OnPressed() => Global.Singleton.PlaySound(pressed);
    private void OnFocusExited() => HideSelect();
    private void OnFocusEntered() => ShowSelect(autoFocussed);
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
