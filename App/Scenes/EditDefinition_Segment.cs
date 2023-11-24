using Godot;
using System;

public class EditDefinition_Segment : HBoxContainer
{

public string Definition {get; private set;}

public void _on_RemoveDefinition_Button_button_down() 
{
    GetParent().CallDeferred("remove_child", this);
    QueueFree();
}

public void _on_Definition_Input_text_changed()
{
    Definition = GetNode<TextEdit>("Definition_Container/Definition_Input").Text;
}


}
