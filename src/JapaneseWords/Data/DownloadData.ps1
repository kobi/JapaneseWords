#note: this function will not work if the file extension is not zip. Not my bad.
function Extract-File-From-Zip($zipFile, $oneFolderInZip, $fileNameInZip , $destination)
{
    $shell = new-object -com shell.application
    $zip = $shell.NameSpace($zipFile)

    if($oneFolderInZip){
        $oneFolder = $zip.Items() | Where-Object {$_.IsFolder -and $_.Name -eq $oneFolderInZip} | Select-Object -Unique
        # if we need deeper level in the folder, we need to use `$oneFolder.GetFolder.Items() | {}`.
        $folderInZip = $oneFolder.GetFolder;
    }
    else {
        $folderInZip = $zip
    }
    $fileInZip = $folderInZip.Items() | Where-Object {-not $_.IsFolder -and $_.Name -eq $fileNameInZip} | Select-Object -Unique

    # 0x14 - override and silent http://stackoverflow.com/a/5711383/7586
    $shell.Namespace($destination).copyhere($fileInZip, 0x14)
}


function Download-Jar-Rename-Zip($extensionUrl, $jarNameInExtension, $folder, $targetFileName)
{
    $targetExtensionZipFilePath = [System.IO.Path]::Combine($targetFolder, $targetFileName)

    $client = new-object System.Net.WebClient
    $client.DownloadFile($extensionUrl, $targetExtensionZipFilePath)
    $client.Dispose()

    return $targetExtensionZipFilePath;
}

$scriptFileFullPath = $MyInvocation.MyCommand.Path
$targetFolder = Split-Path $scriptFileFullPath

$extensionZip = Download-Jar-Rename-Zip -extensionUrl "https://addons.mozilla.org/firefox/downloads/latest/2471/addon-2471-latest.xpi" -folder $targetFolder -targetFileName "rikaichan.jar.zip"

Extract-File-From-Zip -zipFile $extensionZip -oneFolderInZip "chrome" -fileNameInZip "rikaichan.jar" -destination $targetFolder
Remove-Item $extensionZip

$extensionJarPath = [System.IO.Path]::Combine($targetFolder, "rikaichan.jar")
$extensionJarPathZip = $extensionJarPath + ".zip"
Rename-Item -Path $extensionJarPath -NewName $extensionJarPathZip
$extensionJarPath = $extensionJarPathZip

Extract-File-From-Zip -zipFile $extensionJarPath -oneFolderInZip "content" -fileNameInZip "kanji.dat" -destination $targetFolder
Extract-File-From-Zip -zipFile $extensionJarPath -oneFolderInZip "content" -fileNameInZip "kanji-copyright.txt" -destination $targetFolder
Extract-File-From-Zip -zipFile $extensionJarPath -oneFolderInZip "content" -fileNameInZip "radicals.dat" -destination $targetFolder
Extract-File-From-Zip -zipFile $extensionJarPath -oneFolderInZip "content" -fileNameInZip "deinflect.dat" -destination $targetFolder

Remove-Item $extensionJarPath

$dictionaryExtensionJarPath = Download-Jar-Rename-Zip -extensionUrl "https://addons.mozilla.org/firefox/downloads/latest/398350/addon-398350-latest.xpi" -folder $targetFolder -targetFileName "rikaichan.dictionary.jar.zip"

Extract-File-From-Zip -zipFile $dictionaryExtensionJarPath -fileNameInZip "dict.sqlite" -destination $targetFolder
Extract-File-From-Zip -zipFile $dictionaryExtensionJarPath -fileNameInZip "dict-copyright.txt" -destination $targetFolder

Remove-Item $dictionaryExtensionJarPath