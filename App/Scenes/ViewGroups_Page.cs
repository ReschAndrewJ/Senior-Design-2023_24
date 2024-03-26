using Godot;
using Godot.Collections;

public class ViewGroups_Page : Control
{
    public Control Previous_Page_Ref = null;
    ScrollBox_Segment Groups_ScrollBox_Ref = null;

    (int,float)? scrollbox_cachedTopTuple = null;


    public override void _Ready()
    {
        Groups_ScrollBox_Ref = GetNode<ScrollBox_Segment>("GroupsList_Scroll");

        Node Database_Ref = GetNode("/root/Database");
        
        Array groups_res = (Array)Database_Ref.Call("get_groups");
        Array groups_pKeys = (Array)groups_res[0];
        Array groups_Names = (Array)groups_res[1];

        Array<Control> groupPreviews = new Array<Control>();
        groupPreviews.Resize(groups_pKeys.Count);

        PackedScene Preview_Segment_PackedScene = GD.Load<PackedScene>("res://Scenes/GroupPreview_Segment.tscn");
        for (int i = 0; i < groups_pKeys.Count; ++i) {
            GroupPreview_Segment Preview_Segment_Ref = Preview_Segment_PackedScene.Instance<GroupPreview_Segment>();
            Preview_Segment_Ref.setup((int)groups_pKeys[i], (string)groups_Names[i], Database_Ref);
            groupPreviews[i] = Preview_Segment_Ref;
            Preview_Segment_Ref.Connect(nameof(GroupPreview_Segment.button_tapped_signal), this, nameof(_on_GroupPreview_Tapped));
        }
        Groups_ScrollBox_Ref.setup(groupPreviews, scrollbox_cachedTopTuple);
    }


    public override void _ExitTree()
    {
        base._ExitTree();

        Groups_ScrollBox_Ref.setup(new Array<Control>());

        RequestReady();
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

    public void _on_AddGroup_Button_Tapped()
    {
        EditGroup_Page EditGroup_Page_Ref = GD.Load<PackedScene>("res://Scenes/EditGroup_Page.tscn").Instance<EditGroup_Page>();
        EditGroup_Page_Ref.Connect(nameof(EditGroup_Page.groupCreatedSignal),this,nameof(_on_AddGroup_groupCreated));

        AddChild(EditGroup_Page_Ref);
    }
    private void _on_AddGroup_groupCreated(int group_pKey, string group_name) {
        GroupPreview_Segment Preview_Segment_Ref = GD.Load<PackedScene>("res://Scenes/GroupPreview_Segment.tscn").Instance<GroupPreview_Segment>();
        Preview_Segment_Ref.setup(group_pKey, group_name, GetNode("/root/Database"));
        Groups_ScrollBox_Ref.addNodeToEnd(Preview_Segment_Ref);
        Preview_Segment_Ref.Connect(nameof(GroupPreview_Segment.button_tapped_signal), this, nameof(_on_GroupPreview_Tapped));
    }


    public void _on_GroupPreview_Tapped(int group_pKey) {
        GroupWordList_Page GroupWordList_Page_Ref = GD.Load<PackedScene>("res://Scenes/GroupWordList_Page.tscn").Instance<GroupWordList_Page>();
        GroupWordList_Page_Ref.Previous_Page_Ref = this;
        GroupWordList_Page_Ref.group_key = group_pKey;

        scrollbox_cachedTopTuple = Groups_ScrollBox_Ref.getFirstChildPosTuple();
        Groups_ScrollBox_Ref.setup(new Array<Control>());
        GetNode("/root").CallDeferred("add_child", GroupWordList_Page_Ref);
        GetNode("/root").CallDeferred("remove_child",this);
    }


}
