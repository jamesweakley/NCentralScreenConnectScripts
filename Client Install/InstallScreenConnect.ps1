# set this value to your ScreenConnect server URI.
$screenConnectUri="http://screenconnect.company.com"
# set this value to the first 10 characters of your ScreenConnect server key
$ScreenConnectKey='abcdefghi';

$ErrorActionPreference = 'Stop'
$localfile=$env:TEMP+"\ScreenConnect.msi"

if (Test-Path 'C:\Program Files (x86)')
{
    $config = Get-Content 'C:\Program Files (x86)\N-able Technologies\Windows Agent\config\ApplianceConfig.xml' | where-object {$_.contains("ApplianceID")}
}
else
{
    $config = Get-Content 'C:\Program Files\N-able Technologies\Windows Agent\config\ApplianceConfig.xml' | where-object {$_.contains("ApplianceID")}
}
$applianceID=$config.substring($config.IndexOf("<ApplianceID>")+13);
$applianceID=$applianceID.substring(0,$applianceID.IndexOf("</ApplianceID>"));

$uri = "$screenConnectUri/InstallFromNCentralAgent.aspx?ApplianceID=$applianceID&Key=$ScreenConnectKey";
write-host "Downloading from $uri"

$client = new-object System.Net.WebClient
$client.Headers.Add('User-Agent','Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36')
$client.DownloadFile($uri, $localfile)


& $localfile /q




