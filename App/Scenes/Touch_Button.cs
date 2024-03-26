using Godot;


public class Touch_Button : Button
{

    const ulong GlobalInputDelay = 50;

    static bool OnDelayCooldown = false;

  
    const float DistanceThreshold = 100*100;
    const ulong TimeThreshold = 500;
    private bool isHeld = false;
    private Vector2 startPosition = new Vector2(0,0);
    private float distanceSquare = 0;
    private ulong start_time = 0;
    private bool out_of_bounded = false;


    [Signal]
    public delegate void Tapped();



    public override void _Ready(){
        ButtonMask = 0; // prevent BaseButton from handling input
        Connect("gui_input", this, nameof(HandleInput));
    }


    public void HandleInput(InputEvent input) {
        if (Disabled) return;

        if (input is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left) {

            if (!isHeld && eventMouseButton.Pressed) { 
                isHeld = true;
                startPosition = eventMouseButton.GlobalPosition;
                distanceSquare = 0;
                out_of_bounded = false;
                start_time = Time.GetTicksMsec();
            }
            else if (isHeld && !eventMouseButton.Pressed) {
                isHeld = false;
                if (distanceSquare < DistanceThreshold 
                && Time.GetTicksMsec() - start_time < TimeThreshold
                && !out_of_bounded
                && !OnDelayCooldown) {
                    EmitTap();
                }
            }

        }
        else if (input is InputEventScreenTouch eventScreenTouch) {

            if (!isHeld && eventScreenTouch.Pressed) {
                isHeld = true;
                startPosition = eventScreenTouch.Position;
                distanceSquare = 0;
                out_of_bounded = false;
                start_time = Time.GetTicksMsec();
            }
            else if (isHeld && !eventScreenTouch.Pressed) {
                isHeld = false;
                if (distanceSquare < DistanceThreshold 
                && Time.GetTicksMsec() - start_time < TimeThreshold
                && !out_of_bounded
                && !OnDelayCooldown) {
                    EmitTap();
                }
            }

        }
        
        if (isHeld && input is InputEventMouseMotion eventMouseMotion) {
            Vector2 pos = eventMouseMotion.GlobalPosition;
            float currentDistanceSquare = (pos[0] - startPosition[0])*(pos[0] - startPosition[0]) + (pos[1] - startPosition[1])*(pos[1] - startPosition[1]);
            if (currentDistanceSquare > distanceSquare) distanceSquare = currentDistanceSquare;
            
            if (pos[0] < RectGlobalPosition[0] || pos[0] > RectGlobalPosition[0] + RectSize[0] ||
            pos[1] < RectGlobalPosition[1] || pos[1] > RectGlobalPosition[1] + RectSize[1]) {
                out_of_bounded = true;
            }
        } 
        else if (isHeld && input is InputEventScreenDrag eventScreenDrag) {
            Vector2 pos = eventScreenDrag.Position;
            float currentDistanceSquare = (pos[0] - startPosition[0])*(pos[0] - startPosition[0]) + (pos[1] - startPosition[1])*(pos[1] - startPosition[1]);
            if (currentDistanceSquare > distanceSquare) distanceSquare = currentDistanceSquare;

            if (pos[0] < RectGlobalPosition[0] || pos[0] > RectGlobalPosition[0] + RectSize[0] ||
            pos[1] < RectGlobalPosition[1] || pos[1] > RectGlobalPosition[1] + RectSize[1]) {
                out_of_bounded = true;
            }
        } 

    }

    private void EmitTap() {
        OnDelayCooldown = true;
        if (ToggleMode) Pressed = !Pressed;
        EmitSignal(nameof(Tapped));
        
        Cooldown(GetTree());
    }

    
    private static async void Cooldown(SceneTree tree) {
        Object obj = new Object();
        await obj.ToSignal(tree.CreateTimer(GlobalInputDelay / 1000f, false), "timeout");
        obj.Free();
        OnDelayCooldown = false;
    }
    

}
