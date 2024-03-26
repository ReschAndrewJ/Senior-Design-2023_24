using Godot;
using Godot.Collections;

public class GroupPreview_Segment : MarginContainer
{
    const int preview_wordCount = 6;
    int group_pKey = -1;

    public void _on_Group_Button_Tapped() {
        EmitSignal(nameof(button_tapped_signal), group_pKey);
    }
    [Signal]
    public delegate void button_tapped_signal(int group_pKey);


    public void setup(int group_pKey, string group_Name, Node Database_Ref) {
        this.group_pKey = group_pKey;

        Array words_pKeys = (Array)Database_Ref.Call("get_wordKeysInGroup", group_pKey, preview_wordCount);
    
        GetNode<Label>("HBoxContainer/GroupName_Container/GroupName_Label").Text = group_Name;
        Label WordsPreview_Ref = GetNode<Label>("HBoxContainer/WordsPreview_Container/WordsPreview_Label");

        Array words_Array = (Array)Database_Ref.Call("get_words", words_pKeys);
        
        //Array pKeys_Array = (Array)words_Array[0];
        Array kanji_Array = (Array)words_Array[1];
        //Array kana_Array = (Array)words_Array[2];

        string preview_text = "";
        if (kanji_Array.Count != 0) preview_text = (string)kanji_Array[0];
        for (int i=1; i<kanji_Array.Count; ++i) {
            preview_text += ", " + (string)kanji_Array[i];
        }
        WordsPreview_Ref.Text = preview_text;
    }


}
