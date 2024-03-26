using Godot;
using Godot.Collections;

public class EditGroup_Page : Control
{

    public int pKey = -1;

    public override void _Ready()
    {
        
        if (pKey == -1) {
            Control DeleteGroup_Container_Ref = GetNode<Control>("PanelContainer/VBoxContainer/DeleteGroup_Container");
            Color c = DeleteGroup_Container_Ref.Modulate;
            c.a = 0;
            DeleteGroup_Container_Ref.Modulate = c;
            DeleteGroup_Container_Ref.GetChild<Button>(0).Disabled = true;
        } else {
            Node Database_Ref = GetNode("/root/Database");
            Array gKey_asArray = new Array();
            gKey_asArray.Resize(1);
            gKey_asArray[0] = pKey;
            Array res = (Array)Database_Ref.Call("get_groupsByKeys", gKey_asArray);
            GetNode<LineEdit>("PanelContainer/VBoxContainer/GroupName_Container/HBoxContainer/GroupName_Input").Text = (string)((Array)res[1])[0];
        }


    }


    [Signal]
    public delegate void groupNameUpdateSignal(string new_name);
    [Signal]
    public delegate void groupCreatedSignal(int newPKey, string group_name);
    public void _on_Save_Button_Tapped() 
    {
        Node Database_Ref = GetNode("/root/Database");
        string groupName = GetNode<LineEdit>("PanelContainer/VBoxContainer/GroupName_Container/HBoxContainer/GroupName_Input").Text;

        if (pKey == -1) { // saving new group
            int group_pKey = (int)Database_Ref.Call("insert_group", groupName);
            EmitSignal(nameof(groupCreatedSignal), group_pKey, groupName);
        }
        else { // updating existing group
            EmitSignal(nameof(groupNameUpdateSignal), groupName);
            Database_Ref.Call("update_group", pKey, groupName);
        }


        GetParent().CallDeferred("remove_child", this);
        QueueFree();
    }


    public void _on_Cancel_Button_Tapped()
    {
        GetParent().CallDeferred("remove_child", this);
        QueueFree();
    }


    public void _on_DeleteGroup_Button_Tapped()
    {
        
        // confirmation message
        DeleteConfimation confimation = GD.Load<PackedScene>("res://Scenes/DeleteConfimation.tscn").Instance<DeleteConfimation>();
        confimation.Connect(nameof(DeleteConfimation.OptionSelectedSignal), this, nameof(_on_DeleteConfimation_response));
        AddChild(confimation);

    }
    private void _on_DeleteConfimation_response(bool confirm) {
        if (confirm) _on_DeleteGroup_confirm();
    }
    private void _on_DeleteGroup_confirm() {
        Node Database_Ref = GetNode("/root/Database");

        // delete group-word pairs before deleting group
        Database_Ref.Call("delete_groupWordPairs",
            Database_Ref.Call("get_groupWordPairsForGroup", pKey)
        );

        Database_Ref.Call("delete_group", pKey);


        if (GetParent() is GroupWordList_Page page) {
            GetNode("/root").CallDeferred("add_child", page.Previous_Page_Ref);
            GetNode("/root").CallDeferred("remove_child", page);
            page.QueueFree();
        }
        GetParent().CallDeferred("remove_child", this);
        QueueFree();

    }

    

}
