using Godot;
using System;

public class ViewGroups_Page : Control
{
    public Control Previous_Page_Ref = null;
 
    public override void _Ready()
    {
        
    }


    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == MainLoop.NotificationWmGoBackRequest) {
            GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
            GetNode("/root").CallDeferred("remove_child", this);
            QueueFree();
        }
    }


    public void _on_AddGroup_Button_button_down()
    {
        EditGroup_Page EditGroup_Page_Ref = GD.Load<PackedScene>("res://Scenes/EditGroup_Page.tscn").Instance<EditGroup_Page>();
        EditGroup_Page_Ref.Previous_Page_Ref = this;

        GetNode("/root").CallDeferred("add_child",EditGroup_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);
    }


}
