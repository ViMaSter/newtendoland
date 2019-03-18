# Nintendo Yoshi's Fruit Cart Map Editor
[![Build Status](https://vimaster.visualstudio.com/newtendoland/_apis/build/status/ViMaSter.newtendoland?branchName=master)](https://vimaster.visualstudio.com/newtendoland/_build/latest?definitionId=1&branchName=master)

## Running tests
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
