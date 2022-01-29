# Nintendo Yoshi's Fruit Cart Map Editor
[![Build Status](https://vimaster.visualstudio.com/newtendoland/_apis/build/status/ViMaSter.newtendoland?branchName=master)](https://vimaster.visualstudio.com/newtendoland/_build/latest?definitionId=1&branchName=master)

![image](https://user-images.githubusercontent.com/1689033/151661504-df1b3db4-317d-48ec-bba6-29ab35b93f21.png)

## Required
- Python 3 + `py` inside `PATH`
- [Uwizard](https://github.com/MrMysterio/Uwizard) to extract and re-pack gamefiles
- .wux/.wud backup of Nintendo Land ( + [WudCompress](https://github.com/TeamCemu/WudCompress), if you need to convert a `.wux`-file [compressed] into a `.wud`-file [uncompressed] for extraction)
- the title key of Nintendo Land to extract and recompress game files (note: title keys are different for each region + download/disc version)

## Recommended
- [Cemu WiiU emulator](https://cemu.info/#download) for quicker itteration
- Hexinator for easier inspection of MapData files

# Editing a map
_in progress_

# Development of the editor
1. Clone this repository AND it's submodules
2. Open `[REPO ROOT]/0_vs/tileeditor.sln`

## To inspect the data of a level on hex-basis...
...check the info inside [GAMEDATA.MD](6_file-structures/GAMEDATA.MD)

1. Locate your Nintendo Land `.wud`/`.wux`-file
2. (If it's a `.wux`, decompress it into a `.wud` using WudCompress)
3. Extract the game files
4. Extract `[EXTRACTED WUD ROOT]/data/content/Common/Package/Ysi_Cmn.pack`
5. Extract `[EXTRACTED Ysi_Cmn.pack ROOT]/Common/Scene/Ysi.szs`
6. Open any of the `.exbin` files with a text editor

## Running automated tests
Some tests require game files to be run properly. The quickest way to achieve this, is to manually extract the `Ysi.szs` file into the `/5_azurescripts/testresources/`-folder.  
Done correctly the files `/5_azurescripts/testresources/FruitData.exbin` and `/5_azurescripts/testresources/MapData.exbin` should now exist.

This process can be automated and is part of the default azure pipeline. Simply set the following three environment variables and run `/5_azurescripts/download_testfiles.ps1`:
```
testAssetsFTPHostname // hostname (+ optional subfolders) of a remote folder
                          containing FruitData.exbin and other WITHOUT trailing slash
testAssetsFTPPassword // FTP username used for authorization
testAssetsFTPUsername // FTP password used for authorization
```
**Example:** Your `FruitData.exbin` is downloadable at `ftp://example.org/gamefiles/FruitData.exbin` accessable with the username `exampleUser` and password `securePassword`. The following environment variables would be required:
```
testAssetsFTPHostname = example.org/gamefiles
testAssetsFTPPassword = exampleUser
testAssetsFTPUsername = securePassword
```
