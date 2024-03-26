using Godot;
using Godot.Collections;

public class WordDetail_Page : Control
{
    public Control Previous_Page_Ref = null;
    public int pKey = -1;
    
    public override void _Ready()
    {
        Node Database_Ref = GetNode("/root/Database");

        Array words_Array = (Array)Database_Ref.Call("get_words", new Array{pKey});

        // Array res_pKeys = (Array)words_Array[0];
        string res_kanji = (string)((Array)words_Array[1])[0];
        string res_kana = (string)((Array)words_Array[2])[0];

        Array definitions_Array = (Array)Database_Ref.Call("get_wordDefinitions", pKey);

        Array definitions_pKeys = (Array)definitions_Array[0];
        Array definitions_text = (Array)definitions_Array[1];

        Array groups_Array = (Array)Database_Ref.Call("get_groupsByKeys", Database_Ref.Call("get_groupKeysWithWord", pKey));
        Array groups_pKeys = (Array)groups_Array[0];
        Array groups_text = (Array)groups_Array[1];

        GetNode<Label>("VBoxContainer/KanjiForm_Container/Label").Text = res_kanji;
        GetNode<Label>("VBoxContainer/KanaForm_Container/Label").Text = res_kana;

        ScrollBox_Segment definitions_ScrollBox_Ref = GetNode<ScrollBox_Segment>("VBoxContainer/Definitions_Container/Definitions_VBox/Definitions_Scroll");
        Array<Control> definitionLabels = new Array<Control>();
        definitionLabels.Resize(definitions_pKeys.Count);
        for (int i = 0; i < definitions_pKeys.Count; ++i) {
            Label text_Label = new Label { Text = (string)definitions_text[i] };
            definitionLabels[i] = text_Label;
        }
        definitions_ScrollBox_Ref.setup(definitionLabels);

        ScrollBox_Segment groups_ScrollBox_Ref = GetNode<ScrollBox_Segment>("VBoxContainer/Groups_Container/HBoxContainer/Groups_ScrollBox");
        Array<Control> groupsLabels = new Array<Control>();
        groupsLabels.Resize(groups_pKeys.Count);
        for (int i = 0; i < groups_pKeys.Count; ++i) {
            Label text_Label = new Label { Text = (string)groups_text[i] };
            groupsLabels[i] = text_Label;
        }
        groups_ScrollBox_Ref.setup(groupsLabels);

        RequestReady();
    }


    public void _on_EditWord_Button_Tapped()
    {
        EditWord_Page EditWord_Page_Ref = GD.Load<PackedScene>("res://Scenes/EditWord_Page.tscn").Instance<EditWord_Page>();
        EditWord_Page_Ref.Previous_Page_Ref = this;
        EditWord_Page_Ref.pKey = pKey;

        GetNode("/root").CallDeferred("add_child",EditWord_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);
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
