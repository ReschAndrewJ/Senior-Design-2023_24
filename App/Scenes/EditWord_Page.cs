using Godot;

public class EditWord_Page : Control
{
    public Control Previous_Page_Ref = null;
    Control Definitions_VBox_Ref = null;  
    PackedScene EditDefinition_Segment_PackedScene = null;    


    public int pKey = -1;

    
    public override void _Ready()
    {
        Definitions_VBox_Ref = GetNode<Control>("EditWord_VBox/Definitions_Container/Definitions_Scroll/Definitions_VBox");
        EditDefinition_Segment_PackedScene = GD.Load<PackedScene>("res://Scenes/EditDefinition_Segment.tscn");
        Definitions_VBox_Ref.AddChild(EditDefinition_Segment_PackedScene.Instance());

        
        if (pKey == -1) {
            Control DeleteWord_Container_Ref = GetNode<Control>("EditWord_VBox/DeleteWord_Container");
            Color c = DeleteWord_Container_Ref.Modulate;
            c.a = 0;
            DeleteWord_Container_Ref.Modulate = c; 
            DeleteWord_Container_Ref.GetChild<Button>(0).Disabled = true;
        }
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


    public void _on_Save_Button_button_down()
    {
        Node Database_Ref = GetNode("/root/Database");
        GroupsSelectBox_Segment GroupsSelect_Input_Ref = GetNode<GroupsSelectBox_Segment>("EditWord_VBox/GroupsSelect_Container/HBoxContainer/GroupsSelectBox");

        string kanji = GetNode<LineEdit>("EditWord_VBox/KanjiForm_Container/HBoxContainer/KanjiForm_Input").Text;
        string kana = GetNode<LineEdit>("EditWord_VBox/KanaForm_Container2/HBoxContainer/KanaForm_Input").Text;
        if (pKey == -1) { // saving new word
            int word_pKey = (int)Database_Ref.Call("insert_word", kanji, kana);
            int definitionCount = Definitions_VBox_Ref.GetChildCount() - 1;
            for (int i=1; i<=definitionCount; ++i) {
                string definition_text = Definitions_VBox_Ref.GetChild<EditDefinition_Segment>(i).Definition;
                int def_pKey = (int)Database_Ref.Call("insert_definition", word_pKey, definition_text);
            }

            for (int i=0; i<GroupsSelect_Input_Ref.groupsSelected.Count; ++i) if 
            (GroupsSelect_Input_Ref.groupsSelected[i]) {
                int group_pKey = GroupsSelect_Input_Ref.buttonIndicesToPKeys[i];
                int pair_pKey = (int)Database_Ref.Call("insert_groupWordPair", word_pKey, group_pKey);
            }
            
        }        
        else { // updating existing word
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

    public void _on_AddDefinition_Button_button_down()
    {
        Definitions_VBox_Ref.AddChild(EditDefinition_Segment_PackedScene.Instance());
    }

}
