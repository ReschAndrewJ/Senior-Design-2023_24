using Godot;
using System;

public class EditGroup_Page : Control
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
    

}
