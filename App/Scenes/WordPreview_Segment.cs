using Godot;

public class WordPreview_Segment : MarginContainer
{
    private int pKey = -1;

    public void _on_Word_Button_Tapped() {
        EmitSignal(nameof(button_tapped_signal), pKey);
    }
    [Signal]
    public delegate void button_tapped_signal(int pKey);


    public void setup(int pKey, string kanji, string kana) 
    {
        this.pKey = pKey;
        GetNode<Label>("VBoxContainer/Kanji_Container/Kanji_Label").Text = kanji;
        GetNode<Label>("VBoxContainer/Kana_Container/Kana_Label").Text = kana;


    }




}
