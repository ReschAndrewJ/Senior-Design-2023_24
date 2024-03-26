using Godot;
using Godot.Collections;


public class GroupWordList_Page : Control
{
    public Control Previous_Page_Ref = null;
    public int group_key = -1;

    (int,float)? scrollbox_cachedTopTuple = null;


    public override void _Ready()
    {
        ScrollBox_Segment WordPreviews_Scroll_Ref = GetNode<ScrollBox_Segment>("WordsList_Container/WordPreviews_Scroll");
        Node Database_Ref = GetNode("/root/Database");

        Array gKey_asArray = new Array();
        gKey_asArray.Resize(1);
        gKey_asArray[0] = group_key;
        Array res_group = (Array)Database_Ref.Call("get_groupsByKeys", gKey_asArray);
        GetNode<Label>("GroupName_Container/GroupName_Label").Text = (string)((Array)(res_group[1]))[0];

        Array word_keys = (Array)Database_Ref.Call("get_wordKeysInGroup", group_key);
        Array words_Array = (Array)Database_Ref.Call("get_words", word_keys);
        
        Array res_pKeys = (Array)words_Array[0];
        Array res_kanji = (Array)words_Array[1];
        Array res_kana = (Array)words_Array[2];
        
        PackedScene WordPreview_PackedScene = GD.Load<PackedScene>("res://Scenes/WordPreview_Segment.tscn");
        Array<Control> WordPreview_Segments = new Array<Control>();
        WordPreview_Segments.Resize(res_pKeys.Count);

        for (int i = 0; i < res_pKeys.Count; ++i) {
            WordPreview_Segment WordPreview_Segment_Ref = WordPreview_PackedScene.Instance<WordPreview_Segment>();
            WordPreview_Segment_Ref.setup((int)res_pKeys[i], (string)res_kanji[i], (string)res_kana[i]);
            
            WordPreview_Segment_Ref.Connect(nameof(WordPreview_Segment.button_tapped_signal), this, nameof(on_WordPreview_Button_Tapped));

            WordPreview_Segments[i] = WordPreview_Segment_Ref;
        }

        WordPreviews_Scroll_Ref.setup(WordPreview_Segments, scrollbox_cachedTopTuple);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        ScrollBox_Segment WordPreviews_Scroll_Ref = GetNode<ScrollBox_Segment>("WordsList_Container/WordPreviews_Scroll");
        scrollbox_cachedTopTuple = WordPreviews_Scroll_Ref.getFirstChildPosTuple();
        WordPreviews_Scroll_Ref.setup(new Array<Control>());

        RequestReady();
    }


    public void on_WordPreview_Button_Tapped(int pKey) {
        WordDetail_Page WordDetailPage_Ref = GD.Load<PackedScene>("res://Scenes/WordDetail_Page.tscn").Instance<WordDetail_Page>();
        WordDetailPage_Ref.Previous_Page_Ref = this;
        WordDetailPage_Ref.pKey = pKey;

        GetNode("/root").CallDeferred("add_child", WordDetailPage_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
    }


    public void _on_EditGroup_Button_Tapped() {
        EditGroup_Page EditGroupPage_Ref = GD.Load<PackedScene>("res://Scenes/EditGroup_Page.tscn").Instance<EditGroup_Page>();
        EditGroupPage_Ref.pKey = group_key;

        EditGroupPage_Ref.Connect(nameof(EditGroup_Page.groupNameUpdateSignal),this,nameof(_on_EditGroup_nameupdate));

        AddChild(EditGroupPage_Ref);
    }
    public void _on_EditGroup_nameupdate(string text) {
        GetNode<Label>("GroupName_Container/GroupName_Label").Text = text;
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



