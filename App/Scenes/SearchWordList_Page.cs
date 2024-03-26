using Godot;
using Godot.Collections;

public class SearchWordList_Page : Control
{
    public Control Previous_Page_Ref = null;
    public ScrollBox_Segment WordPreviews_Scroll_Ref = null;

    (int,float)? scrollbox_cachedTopTuple = null;


    public override void _Ready()
    {
        WordPreviews_Scroll_Ref = GetNode<ScrollBox_Segment>("VBoxContainer/WordsList_Container/WordPreviews_Scroll");
        
        Search();
        RequestReady();
    }


    void Search() {
        string search_val = GetNode<LineEdit>("VBoxContainer/SearchBar_Container/SearchBar_Input").Text;

        // get all possible matches from database
        // sort by closeness

        Node Database_Ref = GetNode("/root/Database");

        Array resultsA = (Array)Database_Ref.Call("get_wordsByKanaKanjiSearch", search_val); // results are ordered by word key
        Array resultsB = (Array)Database_Ref.Call("get_wordsByDefinitionSearch", search_val); //

        Array resultsA_WordPKeys = (Array)resultsA[0];
        Array resultsA_Kanji = (Array)resultsA[1];
        Array resultsA_Kana = (Array)resultsA[2];

        Array resultsB_WordPKeys = (Array)resultsB[0];
        Array resultsB_Kanji = (Array)resultsB[1];
        Array resultsB_Kana = (Array)resultsB[2];
        Array resultsB_DefinitionText = (Array)resultsB[3];

        // combine results by pkey, take best score for each 

        Array<int> unique_res_keys = new Array<int>();
        Array<string> unique_res_kanji = new Array<string>();
        Array<string> unique_res_kana = new Array<string>();
        Array<int> unique_res_score = new Array<int>();
        
        int resA_i = 0;
        int resB_i = 0;
        int unique_i = 0;

        int resA_len = resultsA_WordPKeys.Count;
        int resB_len = resultsB_WordPKeys.Count;

        if (resA_i < resA_len && resB_i < resB_len) {
            if ((int)resultsA_WordPKeys[resA_i] < (int)resultsB_WordPKeys[resB_i]) {
                unique_res_keys.Add((int)resultsA_WordPKeys[resA_i]);
                unique_res_kanji.Add((string)resultsA_Kanji[resA_i]);
                unique_res_kana.Add((string)resultsA_Kana[resA_i]);
                int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
                int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
                unique_res_score.Add(kanjiScore < kanaScore ? kanjiScore : kanaScore);
                ++resA_i;
            } else {
                unique_res_keys.Add((int)resultsB_WordPKeys[resB_i]);
                unique_res_kanji.Add((string)resultsB_Kanji[resB_i]);
                unique_res_kana.Add((string)resultsB_Kana[resB_i]);
                int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
                unique_res_score.Add(defScore);
                ++resB_i;
            }
        } else if (resA_i < resA_len) {
            unique_res_keys.Add((int)resultsA_WordPKeys[resA_i]);
            unique_res_kanji.Add((string)resultsA_Kanji[resA_i]);
            unique_res_kana.Add((string)resultsA_Kana[resA_i]);
            int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
            int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
            unique_res_score.Add(kanjiScore < kanaScore ? kanjiScore : kanaScore);
            ++resA_i;
        } else if (resB_i < resB_len) {
            unique_res_keys.Add((int)resultsB_WordPKeys[resB_i]);
            unique_res_kanji.Add((string)resultsB_Kanji[resB_i]);
            unique_res_kana.Add((string)resultsB_Kana[resB_i]);
            int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
            unique_res_score.Add(defScore);
            ++resB_i;
        }

        while (resA_i < resA_len && resB_i < resB_len) {
            if ((int)resultsA_WordPKeys[resA_i] < (int)resultsB_WordPKeys[resB_i]) {       
                if ((int)unique_res_keys[unique_i] != (int)resultsA_WordPKeys[resA_i]) {
                    unique_res_keys.Add((int)resultsA_WordPKeys[resA_i]);
                    unique_res_kanji.Add((string)resultsA_Kanji[resA_i]);
                    unique_res_kana.Add((string)resultsA_Kana[resA_i]);
                    int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
                    int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
                    unique_res_score.Add(kanjiScore < kanaScore ? kanjiScore : kanaScore);
                    ++unique_i;
                } else {
                    int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
                    int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
                    int lesser = kanjiScore < kanaScore ? kanjiScore : kanaScore;
                    unique_res_score[unique_i] = lesser < unique_res_score[unique_i] ? lesser : unique_res_score[unique_i];
                }
                ++resA_i;
            } else {
                if ((int)unique_res_keys[unique_i] != (int)resultsB_WordPKeys[resB_i]) {
                    unique_res_keys.Add((int)resultsB_WordPKeys[resB_i]);
                    unique_res_kanji.Add((string)resultsB_Kanji[resB_i]);
                    unique_res_kana.Add((string)resultsB_Kana[resB_i]);
                    int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
                    unique_res_score.Add(defScore);
                    ++unique_i;
                } else {
                    int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
                    unique_res_score[unique_i] = defScore < unique_res_score[unique_i] ? defScore : unique_res_score[unique_i];
                }
                ++resB_i;
            }
        }

        while (resA_i < resA_len) {
            if ((int)unique_res_keys[unique_i] != (int)resultsA_WordPKeys[resA_i]) {
                unique_res_keys.Add((int)resultsA_WordPKeys[resA_i]);
                unique_res_kanji.Add((string)resultsA_Kanji[resA_i]);
                unique_res_kana.Add((string)resultsA_Kana[resA_i]);
                int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
                int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
                unique_res_score.Add(kanjiScore < kanaScore ? kanjiScore : kanaScore); 
                ++unique_i;
            } else {
                int kanjiScore = Score((string)resultsA_Kanji[resA_i], search_val);
                int kanaScore = Score((string)resultsA_Kana[resA_i], search_val);
                int lesser = kanjiScore < kanaScore ? kanjiScore : kanaScore;
                unique_res_score[unique_i] = lesser < unique_res_score[unique_i] ? lesser : unique_res_score[unique_i];
            }
            ++resA_i;
        }

        while (resB_i < resB_len) {
            if ((int)unique_res_keys[unique_i] != (int)resultsB_WordPKeys[resB_i]) {
                unique_res_keys.Add((int)resultsB_WordPKeys[resB_i]);
                unique_res_kanji.Add((string)resultsB_Kanji[resB_i]);
                unique_res_kana.Add((string)resultsB_Kana[resB_i]);
                int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
                unique_res_score.Add(defScore);
                ++unique_i;
            } else {
                int defScore = Score((string)resultsB_DefinitionText[resB_i], search_val);
                unique_res_score[unique_i] = defScore < unique_res_score[unique_i] ? defScore : unique_res_score[unique_i];
            }
            ++resB_i;
        }

        // sort scored results
        
        Array<int> sortedIndices = indexSort(unique_res_score);
        Array<int> sorted_keys = new Array<int>();
        Array<string> sorted_kanji = new Array<string>();
        Array<string> sorted_kana = new Array<string>();
        sorted_keys.Resize(sortedIndices.Count);
        sorted_kanji.Resize(sortedIndices.Count);
        sorted_kana.Resize(sortedIndices.Count);

        for (int i=0; i<sortedIndices.Count; ++i) {
            sorted_keys[i] = unique_res_keys[sortedIndices[i]];
            sorted_kanji[i] = unique_res_kanji[sortedIndices[i]];
            sorted_kana[i] = unique_res_kana[sortedIndices[i]];
        }

        // create WordPreview nodes

        PackedScene WordPreview_PackedScene = GD.Load<PackedScene>("res://Scenes/WordPreview_Segment.tscn");
        Array<Control> WordPreview_Segments = new Array<Control>();
        WordPreview_Segments.Resize(unique_res_keys.Count);

        for (int i=0; i<unique_res_keys.Count; ++i) {
            WordPreview_Segment WordPreview_Segment_Ref = WordPreview_PackedScene.Instance<WordPreview_Segment>();
            //WordPreview_Segment_Ref.setup(unique_res_keys[i], unique_res_kanji[i], unique_res_kana[i]);
            WordPreview_Segment_Ref.setup(sorted_keys[i], sorted_kanji[i], sorted_kana[i]);
            
            WordPreview_Segment_Ref.Connect(nameof(WordPreview_Segment.button_tapped_signal), this, nameof(on_WordPreview_Button_Tapped));

            WordPreview_Segments[i] = WordPreview_Segment_Ref;
        }

        WordPreviews_Scroll_Ref.setup(WordPreview_Segments, scrollbox_cachedTopTuple);
        scrollbox_cachedTopTuple = null;
    }


    public void _on_ConfirmSearch_Button_Tapped() {
        Search();
    }


    public void on_WordPreview_Button_Tapped(int pKey) {
        WordDetail_Page WordDetailPage_Ref = GD.Load<PackedScene>("res://Scenes/WordDetail_Page.tscn").Instance<WordDetail_Page>();
        WordDetailPage_Ref.Previous_Page_Ref = this;
        WordDetailPage_Ref.pKey = pKey;

        scrollbox_cachedTopTuple = WordPreviews_Scroll_Ref.getFirstChildPosTuple();
        WordPreviews_Scroll_Ref.setup(new Array<Control>());

        GetNode("/root").CallDeferred("add_child", WordDetailPage_Ref);
        GetNode("/root").CallDeferred("remove_child", this);
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



    private int Score(string s1, string s2) {

        string left = s1.ToLower();
        string right = s2.ToLower();

        int maxlen = 0;
        for (int i=0; i < left.Length(); ++i) {
            for (int j=0; j<right.Length(); ++j) {
                int len = 0; int cL=i; int cR=j;
                while (cL < left.Length() && cR < right.Length()) {
                    if (left[cL] == right[cR]) { ++len; ++cL; ++cR; }
                    else break;
                }
                if (len > maxlen) maxlen = len;
            }
        }
        
        int score = left.Length() > right.Length() ? left.Length() - maxlen : right.Length() - maxlen;

        return score;
    }

    private Array<int> indexSort(Array<int> ScoresArray) {
        
        Array<int> indices = new Array<int>();
        indices.Resize(ScoresArray.Count);
        for (int i=0; i<ScoresArray.Count; ++i) {
            indices[i] = i;
        }

        return mergeSort(indices, ScoresArray);
    }

    private Array<int> mergeSort(Array<int> indexSubarray, Array<int> ScoresArray) {

        Array<int> left;
        Array<int> right;

        if (indexSubarray.Count <= 1) return indexSubarray;
        if (indexSubarray.Count == 2) {
            left = new Array<int>{indexSubarray[0]};
            right = new Array<int>{indexSubarray[1]};
        } else {
            int midpoint = indexSubarray.Count / 2;
            left = new Array<int>();
            right = new Array<int>();
            left.Resize(midpoint);
            right.Resize(indexSubarray.Count-midpoint);
            for (int i=0; i<midpoint; ++i) left[i] = indexSubarray[i];
            for (int i=midpoint; i<indexSubarray.Count; ++i) right[i-midpoint] = indexSubarray[i];
            
            left = mergeSort(left, ScoresArray);
            right = mergeSort(right, ScoresArray);
        }

        Array<int> returnIndexArray = new Array<int>();
        returnIndexArray.Resize(indexSubarray.Count);

        int ret_i = 0;
        int left_i = 0;
        int right_i = 0;

        while (left_i < left.Count && right_i < right.Count) {
            if (ScoresArray[left[left_i]] <= ScoresArray[right[right_i]]) {
                returnIndexArray[ret_i] = left[left_i];
                ++left_i;
                ++ret_i;
            } else {
                returnIndexArray[ret_i] = right[right_i];
                ++right_i;
                ++ret_i;
            }
        }
        while (left_i < left.Count) {
            returnIndexArray[ret_i] = left[left_i];
            ++left_i;
            ++ret_i;
        }
        while (right_i < right.Count) {
            returnIndexArray[ret_i] = right[right_i];
            ++right_i;
            ++ret_i;
        }

        return returnIndexArray;
    }
    
}

