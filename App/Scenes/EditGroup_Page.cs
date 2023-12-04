using Godot;
using System;

public class EditGroup_Page : Control
{
    public Control Previous_Page_Ref = null;

    public int pKey = -1;
 
    public override void _Ready()
    {
        
        if (pKey == -1) {
            Control DeleteGroup_Container_Ref = GetNode<Control>("VBoxContainer/DeleteGroup_Container");
            Color c = DeleteGroup_Container_Ref.Modulate;
            c.a = 0;
            DeleteGroup_Container_Ref.Modulate = c;
            DeleteGroup_Container_Ref.GetChild<Button>(0).Disabled = true;
        } 
    }


    public void _on_Save_Button_button_down() 
    {
        Node Database_Ref = GetNode("/root/Database");
        String groupName = GetNode<LineEdit>("VBoxContainer/GroupName_Container/HBoxContainer/GroupName_Input").Text;

        if (pKey == -1) { // saving new group
            int group_pKey = (int)Database_Ref.Call("insert_group", groupName);
        }
        else { // updating existing group
        }


        GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
        QueueFree();
    }


    public void _on_Cancel_Button_button_down()
    {
        GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
        QueueFree();
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
