# MAUI Boggle App
An app to try out Microsoft's new MAUI development platform, adapted from the code for the 
[Upgrade your app with MVVM concepts](https://learn.microsoft.com/dotnet/maui/tutorials/notes-mvvm/) 
tutorial, which added MVVM to a MAUI note taking tutorial.

One can choose from a collection of 4x4, 5x5 and 6x6 Boggle games sold in real life, each featuring 
the collection of dice included with the game. The selected Boggle board can then be loaded with each 
die randomly located and oriented, or letters can be added to the board manually. The board can then 
be solved, using words from the Collins Scrabble Words dictionary, downloaded from 
http://pages.cs.wisc.edu/~o-laughl/csw15.txt. Solutions are ordered by length. A solution can be 
selected to show its location on the board.

See the About page for a list of issues. 

## Installation Instructions
Note: if you don't want to install the app, you can watch this video 
[Boggle on Android](https://drive.google.com/file/d/1fNZHHJ5NsjgETRqWXWAG02mX-SnTekN_/view?usp=drive_link). 
In this exciting video, the user peruses the list of supported games, choosing the Big Boggle 
Classic. She then taps the Game icon below to switch to the game board. On this page she selects the 
Scramble button up top to arrange the board, then goes to the Words tab. (She could have also entered 
letters manually if she were playing on a real Boggle set...) Next she taps Solve to find all the words...
733 points, not bad, with the winning word HOWITZERS for 11 points. 

Although MAUI can build apps for Android, iOS and Windows, as well as macOS, this app supports only 
Android and Windows. Normally an app would be discovered and installed from either the Windows Store 
or the Android Store, but as this app is not intended for widespread distribution, it is packaged and 
delivered as a "side loading" app, requiring a bit more effort to install. 

### Windows
To install on Windows:
1. Download the Boggle_0.0.3.0_Test.zip file
2. Unzip the zip file (double-click the file and click the Extract All button on the toolbar)
3. When the extraction is complete, open the Boggle_0.0.3.0_Test folder
4. Double-click the Boggle_0.0.3.0_x64.msix file (which may not show the file extension .msix)
5. If your computer doesn't know what an msix file is, first install the Microsoft App Installer from the 
Windows Store (https://apps.microsoft.com/store/detail/app-installer/9NBLGGH4NNS1?hl=en-us&gl=us),
then double click the msix file again
6. If the installer tells you, "This app package's publisher certificate could not be verified..." it means that 
you need to tell your computer to trust the certificate with which the installer was digitally signed
    1. Close the installer
    2. Double-click on the Boggle_0.0.3.0_x64.cer file
    3. Click Open then Install Certificate... 
    4. Choose Local Machine, Next
    5. Provide the administrator password as necessary to the UAC dialog, Yes
    6. Choose "Place all certificates in the following store", Browse... choose "Trusted Root Certification 
    Authorities", OK
    7. Next, Finish, OK, OK
    8. Double-click the msix file again
7. Click Install
8. Click Launch

### Android
To install on Android:
1. Tap the com.albert challenge.boggle-0.0.3.apk file
2. Tap Install
3. Tap Open

Note that when Boggle is initializing and loading games, it first writes any missing games to the app's 
storage. This means that if a user were to (somehow) change a game's json file in app storage, those 
changes would not be overwritten when Boggle is opened. One side effect of this is that if an update to 
the Boggle app is released which includes changes to the game json files, they won't be loaded unless 
the app's storage is cleared. On Windows this is accomplished by uninstalling the app, on Android the 
files must be deleted via "Clear storage" in the app's App info page. 

## Usage
Tap the About tab for play instructions and a list of notes about the app (including known issues, etc.).
