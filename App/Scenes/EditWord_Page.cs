using System.Drawing.Text;
using Godot;
using Godot.Collections;

public class EditWord_Page : Control
{
    public Control Previous_Page_Ref = null;
    ScrollBox_Segment Definitions_ScrollBox_Ref = null;  
    PackedScene EditDefinition_Segment_PackedScene = null;    


    public int pKey = -1;

    
    public override void _Ready()
    {
        Definitions_ScrollBox_Ref = GetNode<ScrollBox_Segment>("EditWord_VBox/Definitions_Container/Definitions_VBox/Definitions_Scroll");
        EditDefinition_Segment_PackedScene = GD.Load<PackedScene>("res://Scenes/EditDefinition_Segment.tscn");
        
        if (pKey == -1) {
            Control DeleteWord_Container_Ref = GetNode<Control>("EditWord_VBox/DeleteWord_Container");
            Color c = DeleteWord_Container_Ref.Modulate;
            c.a = 0;
            DeleteWord_Container_Ref.Modulate = c; 
            DeleteWord_Container_Ref.GetChild<Button>(0).Disabled = true;

            Array<Control> editDefinitionSegments = new Array<Control>();
            editDefinitionSegments.Resize(1);
            editDefinitionSegments[0] = EditDefinition_Segment_PackedScene.Instance<Control>();
            Definitions_ScrollBox_Ref.setup(editDefinitionSegments);
        } else {
            Node Database_Ref = GetNode("/root/Database");

            Array wordForms = (Array)Database_Ref.Call("get_words", new Array(pKey));
            GetNode<LineEdit>("EditWord_VBox/KanjiForm_Container/HBoxContainer/KanjiForm_Input").Text = (string)((Array)wordForms[1])[0];
            GetNode<LineEdit>("EditWord_VBox/KanaForm_Container/HBoxContainer/KanaForm_Input").Text = (string)((Array)wordForms[2])[0];
        
            Array definitions = (Array)((Array)Database_Ref.Call("get_wordDefinitions", pKey))[1];
            Array<Control> editDefinitionSegments = new Array<Control>();
            editDefinitionSegments.Resize(definitions.Count);
            for (int i=0; i<definitions.Count; ++i) {
                EditDefinition_Segment defSegment = EditDefinition_Segment_PackedScene.Instance<EditDefinition_Segment>();
                defSegment.Definition = (string)definitions[i];
                editDefinitionSegments[i] = defSegment;
            }
            Definitions_ScrollBox_Ref.setup(editDefinitionSegments);

            Array groups = (Array)Database_Ref.Call("get_groupKeysWithWord", pKey);
            GroupsSelectBox_Segment GroupsSelect_Input_Ref = GetNode<GroupsSelectBox_Segment>("EditWord_VBox/GroupsSelect_Container/HBoxContainer/GroupsSelectBox");
            GroupsSelect_Input_Ref.SetSelectedGroups(new Array<int>(groups));
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


    public void _on_Save_Button_Tapped()
    {
        Node Database_Ref = GetNode("/root/Database");
        GroupsSelectBox_Segment GroupsSelect_Input_Ref = GetNode<GroupsSelectBox_Segment>("EditWord_VBox/GroupsSelect_Container/HBoxContainer/GroupsSelectBox");

        string kanji = GetNode<LineEdit>("EditWord_VBox/KanjiForm_Container/HBoxContainer/KanjiForm_Input").Text;
        string kana = GetNode<LineEdit>("EditWord_VBox/KanaForm_Container/HBoxContainer/KanaForm_Input").Text;
        if (pKey == -1) { // saving new word
            int word_pKey = (int)Database_Ref.Call("insert_word", kanji, kana);
            
            Array<Control> definitions = Definitions_ScrollBox_Ref.getNodesArray();
            int definitionCount = definitions.Count;
            for (int i=0; i<definitionCount; ++i) {
                string definition_text = ((EditDefinition_Segment)definitions[i]).Definition;
                int def_pKey = (int)Database_Ref.Call("insert_definition", word_pKey, definition_text);
            }

            for (int i=0; i<GroupsSelect_Input_Ref.groupsSelected.Count; ++i) if 
            (GroupsSelect_Input_Ref.groupsSelected[i]) {
                int group_pKey = GroupsSelect_Input_Ref.buttonIndicesToPKeys[i];
                int pair_pKey = (int)Database_Ref.Call("insert_groupWordPair", word_pKey, group_pKey);
            }
            
        }        
        else { // updating existing word

            // update kanji & kana
            Database_Ref.Call("update_word", pKey, kanji, kana);

            // update definitions - easiest to delete and recreate all definitions
            Database_Ref.Call("delete_definitions",
                ((Array)Database_Ref.Call("get_wordDefinitions", pKey))[0]
            );

            Array<Control> definitions = Definitions_ScrollBox_Ref.getNodesArray();
            int definitionCount = definitions.Count;
            for (int i=0; i<definitionCount; ++i) {
                string definition_text = ((EditDefinition_Segment)definitions[i]).Definition;
                int def_pKey = (int)Database_Ref.Call("insert_definition", pKey, definition_text);
            }

            // update group pairings - delete and recreate all pairs
            Database_Ref.Call("delete_groupWordPairs",
                Database_Ref.Call("get_groupWordPairsForWord", pKey)
            );

            for (int i=0; i<GroupsSelect_Input_Ref.groupsSelected.Count; ++i) if 
            (GroupsSelect_Input_Ref.groupsSelected[i]) {
                int group_pKey = GroupsSelect_Input_Ref.buttonIndicesToPKeys[i];
                int pair_pKey = (int)Database_Ref.Call("insert_groupWordPair", pKey, group_pKey);
            }

        }


        GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
        QueueFree();
    }

    public void _on_Cancel_Button_Tapped() 
    {
        GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
        QueueFree();
    }


    public void _on_DeleteWord_Button_Tapped() {
        
        // confirmation message
        DeleteConfimation confimation = GD.Load<PackedScene>("res://Scenes/DeleteConfimation.tscn").Instance<DeleteConfimation>();
        confimation.Connect(nameof(DeleteConfimation.OptionSelectedSignal), this, nameof(_on_DeleteConfimation_response));
        AddChild(confimation);

    }
    private void _on_DeleteConfimation_response(bool confirm) {
        if (confirm) _on_DeleteWord_confirm();
    }
    private void _on_DeleteWord_confirm() {
        Node Database_Ref = GetNode("/root/Database");

        // delete group-word pairs before deleting word
        Database_Ref.Call("delete_groupWordPairs",
            Database_Ref.Call("get_groupWordPairsForWord", pKey)
        );

        // delete associated definitions before deleting word
        Database_Ref.Call("delete_definitions",
            ((Array)Database_Ref.Call("get_wordDefinitions", pKey))[0]
        );

        Database_Ref.Call("delete_word", pKey);

        if (Previous_Page_Ref is WordDetail_Page prev_page) {
            GetNode("/root").CallDeferred("add_child", prev_page.Previous_Page_Ref);
        } else GetNode("/root").CallDeferred("add_child", Previous_Page_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
        QueueFree();

    }


    public void _on_AddDefinition_Button_Tapped()
    {
        Definitions_ScrollBox_Ref.addNodeToEnd(EditDefinition_Segment_PackedScene.Instance<Control>());
    }


    

}
