using Godot;
using Godot.Collections;

class GroupSelectButton : Button {
    public int Index {get; private set;}
    public GroupSelectButton(int index) {
        Index = index;
        ToggleMode = true;
        Connect("button_down",this,nameof(on_button_down));
    }
    public void on_button_down() {
        EmitSignal(nameof(button_down_signal), Index);
    }
    [Signal]
    public delegate void button_down_signal(int index);
}

public class GroupsSelectBox_Segment : ScrollContainer
{
    
    public Array<bool> groupsSelected = new Array<bool>();
    public readonly Array<int> buttonIndicesToPKeys = new Array<int>(); 

    public override void _Ready()
    {
        Node Database_Ref = GetNode("/root/Database");
        Node VBox_Ref = GetChild(0);

        Array groups_res = (Array)Database_Ref.Call("get_groups", true);
        Array pKeys = (Array)groups_res[0];
        Array gNames = (Array)groups_res[1];

        groupsSelected.Resize(pKeys.Count);
        buttonIndicesToPKeys.Resize(pKeys.Count);

        for (int i=0; i<pKeys.Count; ++i) groupsSelected[i] = false;
        for (int i=0; i<pKeys.Count; ++i) buttonIndicesToPKeys[i] = (int)pKeys[i];  
        for (int i=0; i<pKeys.Count; ++i) {
            var button = new GroupSelectButton(i)
            { Text = (string)gNames[i] };
            button.Connect(nameof(GroupSelectButton.button_down_signal), this, nameof(on_groupSelectButton_down));
            VBox_Ref.AddChild(button);
        }      

    }


    public void on_groupSelectButton_down(int index) {
        groupsSelected[index] = !groupsSelected[index];
    }


}
