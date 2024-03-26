using Godot;


public class DeleteConfimation : Control
{

[Signal]
public delegate void OptionSelectedSignal(bool confirm);

bool disabled = false;

public void _on_Confirm_Button_Tapped() {
    if (disabled) return;
    disabled = true;
    EmitSignal(nameof(OptionSelectedSignal), true);
    GetParent().CallDeferred("remove_child", this);
    QueueFree();
}



public void _on_Cancel_Button_Tapped() {
    if (disabled) return;
    disabled = true;
    EmitSignal(nameof(OptionSelectedSignal), false);
    GetParent().CallDeferred("remove_child", this);
    QueueFree();
}


}
