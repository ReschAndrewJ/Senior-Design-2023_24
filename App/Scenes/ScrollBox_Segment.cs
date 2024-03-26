using Godot;
using Godot.Collections;

public class ScrollBox_Segment : Control
{
    
    Array<Control> Nodes_Array = new Array<Control>();
    int firstChildIndex = 0;
    const int MaxNodesInTree = 10;
    const int NodesBuffer = 1;
    const int SpaceBetweenNodes = 20;
    const float MOUSE_SCROLL_SCALE = 50;
    const ulong ACTION_DELAY = 10;
    ulong last_action;

    public override void _Ready(){
        Connect("gui_input", this, nameof(handle_input));
    }

    public (int,float) getFirstChildPosTuple() {
        return (firstChildIndex, 
        (Nodes_Array.Count > firstChildIndex) ? Nodes_Array[firstChildIndex].RectPosition[1]
            : (Nodes_Array.Count - 1 > 0) ? Nodes_Array.Count - 1 : 0
        );
    }

    public Array<Control> getNodesArray() {return Nodes_Array.Duplicate();}

    
    public void setup(Array<Control> nodes, (int,float)? start=null) {
        if (Time.GetTicksMsec() - last_action < ACTION_DELAY) return;
        last_action = Time.GetTicksMsec();
        
        foreach (Node child in GetChildren()) { 
            CallDeferred("remove_child", child); 
            child.QueueFree();    
        }

        int startIndex = (start == null) ? 0 : start.Value.Item1;
        float startOffset = (start == null) ? 0 : start.Value.Item2;
        if (start != null) firstChildIndex = startIndex;

        startIndex = (startIndex < nodes.Count - MaxNodesInTree - 1) ? startIndex : nodes.Count - MaxNodesInTree - 1; 
        startIndex = (startIndex > 0) ? startIndex : 0;

        Nodes_Array = nodes;
        float offset = startOffset;
        for (int i=0; i<MaxNodesInTree; ++i) {
            if (nodes.Count <= startIndex + i) break;
            Control child = nodes[startIndex + i];
            AddChild(child);
            child.RectPosition = new Vector2(child.RectPosition[0], offset);
            child.RectSize = new Vector2(RectSize[0], child.RectSize[1]);
            offset += child.RectSize[1] + SpaceBetweenNodes;
        }

        last_action = Time.GetTicksMsec();
    }

    public void addNodeToEnd(Control node) {
        if (Time.GetTicksMsec() - last_action < ACTION_DELAY) return;
        last_action = Time.GetTicksMsec();

        Nodes_Array.Add(node);
        int childCount = GetChildCount();
        if (childCount < MaxNodesInTree) {
            if (childCount != 0) {
                Control above_child = (Control)GetChild(childCount-1);
                AddChild(node);
                node.RectPosition = new Vector2(node.RectPosition[0], above_child.RectPosition[1] + above_child.RectSize[1] + SpaceBetweenNodes);
                node.CallDeferred("_set_size", above_child.RectSize); 
            }
            else {
                AddChild(node);
                node.RectSize = new Vector2(RectSize[0], node.RectSize[1]);
            }
        }
    }

    // node will be fully de-instanced after calling
    public void removeNode(Control node) {
        if (Time.GetTicksMsec() - last_action < ACTION_DELAY) return;
        last_action = Time.GetTicksMsec();

        for (int i=0; i<Nodes_Array.Count; ++i) {
            if (Nodes_Array[i].Name == node.Name) {
                removeNodeI(i);
                break;
            }
        }
    }
    private void removeNodeI(int index) {
        Control node = Nodes_Array[index];
        if (index >= firstChildIndex && index < firstChildIndex+MaxNodesInTree) {
            bool addToEnd = index+MaxNodesInTree < Nodes_Array.Count;
            int relativeIndex = index - firstChildIndex;

            for (int i=relativeIndex+1; i<MaxNodesInTree && firstChildIndex+i<Nodes_Array.Count; ++i) {
                int offset = i==0 ? 0 : SpaceBetweenNodes;
                Control shiftNode = GetChild<Control>(i);
                shiftNode.RectPosition = new Vector2(shiftNode.RectPosition[0], shiftNode.RectPosition[1] - node.RectSize[1] - offset);
            }

            if (addToEnd) {
                Control added_child = Nodes_Array[firstChildIndex+MaxNodesInTree];
                Control above_child = GetChild<Control>(MaxNodesInTree-1);
                CallDeferred("add_child", added_child);
                added_child.RectPosition = new Vector2(added_child.RectPosition[0], above_child.RectPosition[1] + above_child.RectSize[1] + SpaceBetweenNodes);
                added_child.CallDeferred("_set_size", above_child.RectSize);
            }
        }
        Nodes_Array.RemoveAt(index);
        CallDeferred("remove_child", node);
        node.QueueFree();
    }

    public void handle_input(InputEvent input) {

        if (input is InputEventMouseButton mouse_input)
        {
            if (mouse_input.GlobalPosition[1] < RectGlobalPosition[1] ||
            mouse_input.GlobalPosition[1] > RectGlobalPosition[1] + RectSize[1]) return;

            if (mouse_input.ButtonIndex == (int)ButtonList.WheelUp) scroll(mouse_input.Factor * MOUSE_SCROLL_SCALE);
            if (mouse_input.ButtonIndex == (int)ButtonList.WheelDown) scroll(-mouse_input.Factor * MOUSE_SCROLL_SCALE);

        }
        if (input is InputEventScreenDrag drag_input)
        {
            if (drag_input.Position[1] <= 0 || drag_input.Position[1] >= RectSize[1]) return;

            Vector2 direction = drag_input.Relative;
            scroll(direction[1]);
        }
    }


    private void scroll(float distance) {
        if (Time.GetTicksMsec() - last_action < ACTION_DELAY) return;
        
        Array children = GetChildren();
        
        if (children.Count == 0) return;
        if (distance > 0 && firstChildIndex==0 && (
            Nodes_Array[0].RectGlobalPosition[1] > RectGlobalPosition[1] || 
            Nodes_Array[0].RectGlobalPosition[1] + distance > RectGlobalPosition[1])
            ) return;
        if (distance < 0 && firstChildIndex+children.Count==Nodes_Array.Count && (
            Nodes_Array[Nodes_Array.Count-1].RectGlobalPosition[1]+Nodes_Array[Nodes_Array.Count-1].RectSize[1] < RectGlobalPosition[1]+RectSize[1] ||
            Nodes_Array[Nodes_Array.Count-1].RectGlobalPosition[1]+Nodes_Array[Nodes_Array.Count-1].RectSize[1] + distance < RectGlobalPosition[1]+RectSize[1])
            ) return;
        last_action = Time.GetTicksMsec();

        // update positions
        for (int i=0; i<children.Count; ++i) {
            Control child = (Control)children[i];
            child.RectPosition = new Vector2(child.RectPosition[0], child.RectPosition[1] + distance);
        }

        // if applicable, add and remove children control nodes
        if (children.Count < MaxNodesInTree) return;

        Control upper_buffer_node = (Control)children[NodesBuffer-1]; // buffer 5 -> index 4
        Control lower_buffer_node = (Control)children[MaxNodesInTree-NodesBuffer]; // buffer 5 -> index 15
        
        // scroll up
        if (distance > 0 && upper_buffer_node.RectGlobalPosition[1] > RectGlobalPosition[1]) {
            // remove end nodes, add lead nodes            

            int num_to_exchange = NodesBuffer;
            if (firstChildIndex < NodesBuffer) num_to_exchange = firstChildIndex;

            for (int i=0; i<num_to_exchange; ++i) {
                CallDeferred("remove_child", Nodes_Array[firstChildIndex+MaxNodesInTree-NodesBuffer+i]);
            }

            for (int i=0; i<num_to_exchange; ++i) {
                Control added_child = Nodes_Array[firstChildIndex-1-i];
                CallDeferred("add_child", added_child);
                CallDeferred("move_child", added_child, 0);
                Control below_child = Nodes_Array[firstChildIndex-i];
                added_child.RectPosition = new Vector2(added_child.RectPosition[0], below_child.RectPosition[1] - below_child.RectSize[1] - SpaceBetweenNodes);
                added_child.CallDeferred("_set_size", below_child.RectSize);
            }

            firstChildIndex -= num_to_exchange;
        }
        // scroll down
        if (distance < 0 && lower_buffer_node.RectGlobalPosition[1] + lower_buffer_node.RectSize[1] < RectGlobalPosition[1] + RectSize[1]) {
            // remove lead nodes, add end nodes

            int num_to_exchange = NodesBuffer;
            if (Nodes_Array.Count-firstChildIndex-MaxNodesInTree < NodesBuffer) {
                num_to_exchange = Nodes_Array.Count-firstChildIndex-MaxNodesInTree;
            }

            for (int i=0; i<num_to_exchange; ++i) {
                CallDeferred("remove_child", Nodes_Array[firstChildIndex+i]);
            }

            for (int i=0; i<num_to_exchange; ++i) {
                Control added_child = Nodes_Array[firstChildIndex+MaxNodesInTree+i];
                CallDeferred("add_child", added_child);
                Control above_child = Nodes_Array[firstChildIndex+MaxNodesInTree+i-1];
                added_child.RectPosition = new Vector2(added_child.RectPosition[0], above_child.RectPosition[1] + above_child.RectSize[1] + SpaceBetweenNodes);
                added_child.CallDeferred("_set_size", above_child.RectSize);
            }

            firstChildIndex += num_to_exchange;
        }
    }

    public override bool _ClipsInput()
    {
        return true;
    }

}
