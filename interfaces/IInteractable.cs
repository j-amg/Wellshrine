public partial interface IInteractable
{
    public bool Highlighted {get; set;}
    public bool Active {get; set;}
    public void Interact(){}
}