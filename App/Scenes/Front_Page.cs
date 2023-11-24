using Godot;
using System;

public class Front_Page : Control
{
    
    LineEdit searchBarRef = null;
    Label WordCountLabelRef = null;
    Label DefCountLabelRef = null;
    Label GroupCountLabelRef = null;
    
    readonly string WordCountLabel_text = "Words: {0}";
    readonly string DefCountLabel_text = "Definitions: {0}";
    bool ready_complete = false;
    
    public override void _Ready()
    {
        searchBarRef = GetNode<LineEdit>("VBoxContainer/SearchBar_Container/SearchBar_Input");
        WordCountLabelRef = GetNode<Label>("HBoxContainer/WordCount_Container/WordCount_Label");
        DefCountLabelRef = GetNode<Label>("HBoxContainer/DefinitionCount_Container2/DefinitionCount_Label");
        GroupCountLabelRef = GetNode<Label>("HBoxContainer/GroupCount_Container3/GroupCount_Label");

        Node Database_Ref = GetNode("/root/Database");
        int savedWordCount = (int)Database_Ref.Call("get_wordcount");
        WordCountLabelRef.Text = string.Format(WordCountLabel_text, savedWordCount);
        int savedDefCount = (int)Database_Ref.Call("get_definitioncount");
        DefCountLabelRef.Text = string.Format(DefCountLabel_text, savedDefCount);
        ready_complete = true;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        
        if (ready_complete) {
            Node Database_Ref = GetNode("/root/Database");
            int savedWordCount = (int)Database_Ref.Call("get_wordcount");
            WordCountLabelRef.Text = string.Format(WordCountLabel_text, savedWordCount);
            int savedDefCount = (int)Database_Ref.Call("get_definitioncount");
            DefCountLabelRef.Text = string.Format(DefCountLabel_text, savedDefCount);
        }
    }



    public void _on_AddWord_Button_button_down()
    {
        EditWord_Page EditWord_Page_Ref = GD.Load<PackedScene>("res://Scenes/EditWord_Page.tscn").Instance<EditWord_Page>();
        EditWord_Page_Ref.Previous_Page_Ref = this;

        GetNode("/root").CallDeferred("add_child",EditWord_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);
    }


    public void _on_ViewGroups_Button_button_down()
    {
        ViewGroups_Page ViewGroups_Page_Ref = GD.Load<PackedScene>("res://Scenes/ViewGroups_Page.tscn").Instance<ViewGroups_Page>();
        ViewGroups_Page_Ref.Previous_Page_Ref = this;

        GetNode("/root").CallDeferred("add_child",ViewGroups_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);
    }


    public void _on_ConfirmSearch_Button_button_down()
    {

        string searchValue = searchBarRef.Text;
        
        SearchWordList_Page SearchWordList_Page_Ref = GD.Load<PackedScene>("res://Scenes/SearchWordList_Page.tscn").Instance<SearchWordList_Page>();
        SearchWordList_Page_Ref.Previous_Page_Ref = this;
        SearchWordList_Page_Ref.GetNode<LineEdit>("VBoxContainer/SearchBar_Container/SearchBar_Input").Text = searchValue;

        GetNode("/root").CallDeferred("add_child",SearchWordList_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);

    }


    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == MainLoop.NotificationWmGoBackRequest) {
            GetTree().Notification(MainLoop.NotificationWmQuitRequest);
        }
    }

}
