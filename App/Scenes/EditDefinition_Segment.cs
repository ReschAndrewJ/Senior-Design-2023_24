using Godot;

public class EditDefinition_Segment : HBoxContainer
{


private string definition = "";
public string Definition { 
    get { return definition; }
    set {
        definition = value;
        GetNode<TextEdit>("Definition_Container/Definition_Input").Text = value;
    }
}

public void _on_RemoveDefinition_Button_Tapped() 
{
    Node parent = GetParent();
    if (parent is ScrollBox_Segment scrollbox) {
        scrollbox.removeNode(this);
    } else {
        GetParent().CallDeferred("remove_child", this);
        QueueFree();
    }
}

public void _on_Definition_Input_text_changed()
{
    definition = GetNode<TextEdit>("Definition_Container/Definition_Input").Text;
}


}
