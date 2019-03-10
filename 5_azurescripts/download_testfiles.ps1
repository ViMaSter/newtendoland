$list = New-Object System.Collections.Generic.List[string];
$list.Add("EnemyData.exbin");
$list.Add("FruitData.exbin");
$list.Add("StageData.exbin");
$list.Add("MapData99.exbin");

for ($i = 0; $i -lt 60; $i++)
{
    $formattedIndex = "{0:00}" -f $i;
    $list.Add("MapData$formattedIndex.exbin");
}
 
foreach ($item in $list) {
    $file = "$PSScriptRoot\testresources\$item"
    $ftp = "ftp://${env:testAssetsFTPUsername}:${env:testAssetsFTPPassword}@$env:testAssetsFTPHostname/$item"

    $webclient = New-Object System.Net.WebClient
    $uri = New-Object System.Uri($ftp)

    "Downloading $file..."

    $webclient.DownloadFile($uri, $file)
}