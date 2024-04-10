
## Sections

[Installation](#Installation)

[Adding Words](#Adding_Words)

[Adding Groups](#Adding_Groups)

[Searching For Words](#Searching_For_Words)

[Viewing Groups](#Viewing_Groups)

[Editing Words & Groups](#Editing_Words_&_Groups)

[FAQ](#FAQ)

<a id="Installation"></a>
## Installation 

Note: This app requires the ability to type Japanese characters. 
A virtual keyboard can be enabled through Settings -> System -> Languages & input -> Virtual Keyboard -> Gboard -> Languages -> Add keyboard

1. Download the provided APK to your Android device
2. Run the APK file
3. When given a warning press continue
4. Press Install

<p float="left"> 
  <img src="/img/installWarning.png" width=30% />
  <img src="/img/installConfirm.png" width=30% />
  <img src="/img/installComplete.png" width=30% />
</p>

<a id="Adding_Words"></a >
## Adding Words

Words can be added from the main app page by tapping the "Add Word" button.
The opened page has several fields to customize your entry for a word. 
The first two entries are labeled "漢字" (Kanji) and "カナ" (kana). These are for you to save a word using its representations in both of Japanese's writing systems: Kanji and Hiragana/Katakana.
Next is a series of text blocks to fill in the word's definitions. More definitions can be added with the "Add Definition" button, and existing definitions can be deleted with the adjacent "Delete" button.
Last is a scroll-list of the groups that have been saved to the app (see [Adding Groups](#Adding_Groups)). The word being part of any group can be toggled on or off by tapping the corresponding entry in the list.
With all of the fields filled you save your word by pressing the "Save" button, or you can go back to the main page at any time by either pressing the "Cancel" button or using your phone's back action.

<p float="left"> 
  <img src="/img/mainPage.png" width=30% />
  <img src="/img/wordCreate.png" width=30% />
</p>

<a id="Adding_Groups"></a >
## Adding Groups

Groups can be added from the groups list page, accessible by pressing the "Groups" button on the main app page.
Adding a group only requires a name to be used as the group's label. Like when adding words, the group can saved by pressing "Save", or you can return to the groups list page by pressing either the "Cancel" button or using your phone's back action.
Words can be added to the group when they are either initially added (see [Adding Words](#Adding_Words)) or edited (see [Editing Words & Groups](#Editing_Words_&_Groups)).

<p float="left">
  <img src="/img/groups.png" width=30% /> 
  <img src="/img/groupCreate.png" width=30% />
</p>

<a id="Searching_For_Words"></a >
## Searching For Words

A word can be searched for using either the search bar on the main app page and pressing the "Search" button, or by using the search bar on the search results page and pressing that page's "Search" button.
The app will look for the searched item by the Kanji form, Hiragana/Katakana form, and the word definitions.
A word returned by the search can be tapped on to open that word's details page.

<p float="left">
  <img src="/img/mainPage.png" width=30% />
  <img src="/img/search.png" width=30% />
</p>

<a id="Viewing_Groups"></a >
## Viewing Groups

Pressing the "Groups" button from the main app page brings you to the groups list page. 
This page contains a preview of every group saved to the app, and each preview contains previews of several words from the group shown in no particular order. Tapping on a group opens a list of all words that are in that group.
Any word in the group can be tapped on to open that word's details page.

<p float="left">
  <img src="/img/groups.png" width=30% />
  <img src="/img/groupWords.png" width=30% />
</p>

<a id="Editing_Words_&_Groups"></a >
## Editing Words & Groups

To edit a word, you need to go to that word's details page. From there press the "Edit Word" button, and the page used to create a word opens with the details filled. Pressing the "Save" button on this page will update any modified information. Also, the "Delete Word" button is available to completely remove the word from the app.

<p float="left">
  <img src="/img/wordDetail.png" width=30% /> 
  <img src="/img/wordEdit.png" width=30% />
  <img src="/img/wordDelete.png" width=30% />
</p>

To edit a group, you need to go to the page for that group. Pressing the "Edit Group" button allows you to either change the name of the group or remove the group entirely from the app. 
Deleting a group will not remove the words assigned to it.

<p float="left">
  <img src="/img/groupWords.png" width=30% /> 
  <img src="/img/groupEdit.png" width=30% />
  <img src="/img/groupDelete.png" width=30% />
</p>

<a id="FAQ"></a >
## FAQ

- Is the app only for Android? Can I install it on an IPhone?
  - The app is for Android only. It may be possible to use an emulator to run it on an IPhone.

- Can I share my dictionary and groups with others?
  - The app does not currently support sharing, but will likely gain that feature if I development continues.

- Do I already need to be able to read Japanese to be able to use the app?
  - You should be able to read at least hiragana to get substantial use out of the app. I have no intention of adding a romaji word form input, but you can use romaji instead of hiragana/katakana if you really want to.

- Could the app be used for languages other than Japanese?
  - The app could be modified for other languages, however it is speciallized for Japanese. For example, having multiple written forms of words is generally specific to Japanese, and the search system is made to work with Japanese conjugation. If development continues, the app would be made to be more speciallized towards Japanese, rather than more general.

- How big is the app?
  - Approximately 95 MB

- Why is the app so large?
  - The application was built in "Debug Mode" and is not fully optimized. Additionally, the app is built using a game engine, meaning there are a large number of unused features provided by the engine that may introduce file bloat. 
