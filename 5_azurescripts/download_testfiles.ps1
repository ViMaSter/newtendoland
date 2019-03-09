$File = "$PSScriptRoot\testresources\FruitData.exbin"
$ftp = "ftp://${env:testAssetsFTPUsername}:${env:testAssetsFTPPassword}@$env:testAssetsFTPHostname/FruitData.exbin"

"ftp url: $ftp"

$webclient = New-Object System.Net.WebClient
$uri = New-Object System.Uri($ftp)

"Downloading $File..."

$webclient.DownloadFile($uri, $File)