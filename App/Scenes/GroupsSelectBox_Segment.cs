using Godot;
using Godot.Collections;



public class GroupsSelectBox_Segment : Control
{
    
    public Array<bool> groupsSelected = new Array<bool>();
    public readonly Array<int> buttonIndicesToPKeys = new Array<int>(); 

    public override void _Ready()
    {
        Node Database_Ref = GetNode("/root/Database");
        ScrollBox_Segment Scrollbox_Ref = GetChild<ScrollBox_Segment>(0);

        Array groups_res = (Array)Database_Ref.Call("get_groups");
        Array pKeys = (Array)groups_res[0];
        Array gNames = (Array)groups_res[1];

        groupsSelected.Resize(pKeys.Count);
        buttonIndicesToPKeys.Resize(pKeys.Count);

        Array<Control> groupButtons = new Array<Control>();
        groupButtons.Resize(pKeys.Count);

        for (int i=0; i<pKeys.Count; ++i) groupsSelected[i] = false;
        for (int i=0; i<pKeys.Count; ++i) buttonIndicesToPKeys[i] = (int)pKeys[i];  
        for (int i=0; i<pKeys.Count; ++i) {
            var button = new GroupSelectButton(i) { Text = (string)gNames[i] };
            button.Connect(nameof(GroupSelectButton.button_tapped_signal), this, nameof(on_groupSelectButton_Tapped));
            groupButtons[i] = button;
        }
        Scrollbox_Ref.setup(groupButtons);
    }

    public void SetSelectedGroups(Array<int> group_pKeys) {
        Array<Control> nodesArray = GetNode<ScrollBox_Segment>("ScrollBox").getNodesArray();
        for (int i=0; i<buttonIndicesToPKeys.Count; ++i) {
            if (group_pKeys.Contains(buttonIndicesToPKeys[i])) {
                groupsSelected[i] = true;
                ((Button)nodesArray[i]).SetPressedNoSignal(true);
            } else {
                groupsSelected[i] = false;
                ((Button)nodesArray[i]).SetPressedNoSignal(false);
            }
        }
    }

    public void on_groupSelectButton_Tapped(int index) {
        groupsSelected[index] = !groupsSelected[index];
    }


}



class GroupSelectButton : Touch_Button {
    public int Index {get; private set;}
    public GroupSelectButton(int index) {
        Index = index;
        ToggleMode = true;
        MouseFilter = MouseFilterEnum.Pass;
        Connect(nameof(Tapped),this,nameof(on_button_Tapped));
    }
    public void on_button_Tapped() {
        EmitSignal(nameof(button_tapped_signal), Index);
    }
    [Signal]
    public delegate void button_tapped_signal(int index);
}