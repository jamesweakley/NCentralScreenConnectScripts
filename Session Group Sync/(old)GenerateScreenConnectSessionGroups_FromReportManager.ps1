$ErrorActionPreference="Continue";
$sqlConnectionString="Server=reportmanagerserver;Database=ods_n_central_ds1;User ID=username;Password=password;"

write-host "Performing SQL query"

$cn = New-Object System.Data.SQLClient.SQLConnection

$cn.ConnectionString = $sqlConnectionString

$cn.Open()
$sql = "SELECT customername FROM customer "
$sql+="WHERE deleted=0 "
$sql+="AND parentid>1 "
$sql+="ORDER BY customername "


$cmd = new-object System.Data.SqlClient.SqlCommand 

$cmd.Connection = $cn
$cmd.CommandText = $sql
$cmd.CommandTimeout = 600

$result = $cmd.ExecuteReader()
$xml='<?xml version="1.0"?><SessionGroups xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><SessionGroup SessionFilter="" IsSystem="true" SessionType="Support" Name="All Sessions"/>'
$xml+='<SessionGroup SessionFilter="Host = $USERNAME" IsSystem="false" SessionType="Support" Name="My Sessions"/>'
$xml+='<SessionGroup SessionFilter="" IsSystem="true" SessionType="Meeting" Name="All Meetings"/>'
$xml+='<SessionGroup SessionFilter="Host = $USERNAME" IsSystem="false" SessionType="Meeting" Name="My Meetings"/>'
$xml+='<SessionGroup SessionFilter="" IsSystem="true" SessionType="Access" Name="All Machines"/>'

$result | % { 
    $customerName=$_.getString(0)
    $customerName = [Security.SecurityElement]::Escape($customerName)
    write-host "$customerName"
    $xml+="<SessionGroup SessionFilter=`"CustomProperty1 = '$customerName'`" IsSystem=`"false`" SessionType=`"Access`" Name=`"$customerName`"/>"
}
$xml+='</SessionGroups>'
$cn.Close()
Remove-Item "C:\Program Files (x86)\ScreenConnect\App_Data\SessionGroup.previous.xml"
Rename-Item "C:\Program Files (x86)\ScreenConnect\App_Data\SessionGroup.xml" "C:\Program Files (x86)\ScreenConnect\App_Data\SessionGroup.previous.xml"
$xml | Out-File "C:\Program Files (x86)\ScreenConnect\App_Data\SessionGroup.xml"

Restart-Service "ScreenConnect Session Manager" -force




