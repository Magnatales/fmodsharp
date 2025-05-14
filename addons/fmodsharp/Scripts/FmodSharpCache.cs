using Godot;

[Tool]
[GlobalClass]
public partial class FmodSharpCache : Resource
{
    [Export] public string BankFolderPath {get; set;}
    [Export] public string BankPath {get; set;}
    [Export] public string StringsBankPath {get; set;}
    
    [Export] public string BankRelativePath {get; set;}
    [Export] public string StringsBankRelativePath {get; set;}
    
    [Export] public bool Debug {get; set;}
}
