Clear-Host

@"
GenerateScreenGroups.ps1

This script generates ScreenGroups.xml for ScreenConnect

This version of the script uses the N-Central API to export the list
of customers, using the defined username and password from web.config
in the ScreenConnect program folder.  This version does not require
ReportManager to be installed.

This version also includes the changes to the SessionGroup.xml file
that makes the active support and access sessions visible in their
own filter group.

20160312	2.0 - Introduce optional SubGroup feature available
		in ScreenConnect 5.4+ as a means to break out
		N-Central 10.1 sites into their own subgroups.

Created by:	Jon Czerwinski, Cohn Consulting Corporation
			Based on work by James Weakly, jweakley@diamondgroup.net.au
Date:		March 12, 2016
Version:	2.0

"@

$ErrorActionPreference="Continue";
$ScreenConnect = "C:\Program Files (x86)\ScreenConnect"
$webConfigFile = "$ScreenConnect\web.config"


#
# Set up the static beginning of the SessionGroup.xml file
#
$xml = '<?xml version="1.0"?>'
$xml += '<SessionGroups xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
$xml += '<SessionGroup Name="All Sessions" SessionType="Support" IsSystem="true" SessionFilter="" />'
$xml += '<SessionGroup Name="My Sessions" SessionType="Support" IsSystem="false" SessionFilter="Host = $USERNAME" />'
$xml += '<SessionGroup Name="Active Support Sessions" SessionType="Support" IsSystem="false" SessionFilter="HostConnectedCount &gt; 0 AND GuestConnectedCount &gt; 0" />'
$xml += '<SessionGroup Name="All Meetings" SessionType="Meeting" IsSystem="true" SessionFilter="" />'
$xml += '<SessionGroup Name="My Meetings" SessionType="Meeting" IsSystem="false" SessionFilter="Host = $USERNAME" />'
$xml += '<SessionGroup Name="All Machines" SessionType="Access" IsSystem="true" SessionFilter="" />'
$xml += '<SessionGroup Name="Active Access Sessions" SessionType="Access" IsSystem="false" SessionFilter="HostConnectedCount &gt; 0 and GuestConnectedCount &gt;0" />'


#
# Fetch the N-Central API username and password from web.config
#
[xml]$webConfig = Get-Content $webConfigFile
$serverHost = ($webconfig.configuration.appsettings.add | where {$_.key -eq 'NCentralServerAddress'}).value
$username = ($webconfig.configuration.appsettings.add | where {$_.key -eq 'NCentralAPIUsername'}).value
$password = ($webconfig.configuration.appsettings.add | where {$_.key -eq 'NCentralAPIPassword'}).value


#
# Generate a pseudo-unique namespace to use with the New-WebServiceProxy and 
# associated types.
#
# By controlling the namespace, the script becomes portable and is not
# dependent upon the endpoint url the webservice is connecting.  However, this
# introduces another complexity because once the namespace is defined within a
# powershell session, it cannot be reused, nor can it be undefined.  As long as
# all the calls are made to the existing webserviceproxy, then everything would be
# OK. But, if you try to rerun the script without closing and reopening the
# powershell session, you will get an error.
#
# One way around this is to create a unique namespace each time the script is run.
# We do this by using the last 'word' of a GUID appended to our base namespace 'NAble'.
# This means our type names for parameters (such as T_KeyPair) now have a dynamic
# type.  We could pass types to each new-object call using "$NWSNameSpace.T_KeyPair",
# and I find it more readable to define our 'dynamic' types here and use the typenames
# in variables when calling New-Object.
#
$NWSNameSpace = "NAble" + ([guid]::NewGuid()).ToString().Substring(25)
$KeyPairType = "$NWSNameSpace.T_KeyPair"
$KeyValueType = "$NWSNameSpace.T_KeyValue"

$bindingURL = $serverHost + "/dms/services/ServerEI?wsdl"
$secpassword = ConvertTo-SecureString $password -AsPlainText -Force
$creds = New-Object System.Management.Automation.PSCredential ("\$username", $secpassword)
$nws = New-Webserviceproxy $bindingURL -credential $creds -Namespace ($NWSNameSpace)


#
# Create CustomerData type to hold customer name, id, and parent
#
Add-Type -TypeDefinition @"
public class CustomerData {
	public string ID;
	public string Name;
	public string ParentID;
	public bool Toplevel;
	}
"@


#
# Set up and execute the query for Service Organizations
#
$KeyPairs = @()
$KeyPair = New-Object -TypeName $KeyPairType
$KeyPair.Key = 'listSOs'
$KeyPair.Value = "true"
$KeyPairs += $KeyPair

$rc = $nws.customerList($username, $password, $KeyPairs)


#
# Set up the SO array, then populate
#
$SOs = @()

foreach ($SO in $rc) {
	$SOInfo = @{}
	foreach ($item in $SO.Info) {$SOInfo[$item.key] = $item.Value}

	$Customer = New-Object CustomerData
	$Customer.ID = $SOInfo["customer.customerid"]
	$Customer.Name = $SOInfo["customer.customername"]
	$Customer.ParentID = $SOInfo["customer.parentid"]
	
	$Script:SOs += $Customer

	Remove-Variable SOInfo
	}

Remove-Variable KeyPairs


#
# Set up and execute the query for Customers
#
$KeyPairs = @()
$KeyPair = New-Object -TypeName $KeyPairType
$KeyPair.Key = 'listSOs'
$KeyPair.Value = "false"
$KeyPairs += $KeyPair

$rc = $nws.customerList($username, $password, $KeyPairs)


#
# Set up the Customers array, then populate
#
$Customers = @()

foreach ($Customer in $rc) {
	$CustomerInfo = @{}
	foreach ($item in $Customer.Info) {$CustomerInfo[$item.key] = $item.Value}

	$Customer = New-Object CustomerData
	$Customer.ID = $CustomerInfo["customer.customerid"]
	$Customer.Name = $CustomerInfo["customer.customername"]
	$Customer.ParentID = $CustomerInfo["customer.parentid"]
	ForEach ($SO in $SOs) {
		If ($SO.ID -eq $Customer.ParentID) {
			$Customer.Toplevel = $true
			break
			}
		}
	
	$Script:Customers += $Customer

	Remove-Variable CustomerInfo
	}

Remove-Variable KeyPairs
Remove-Variable SOs

#
# Write each customer out to the SessionGroup as its own
# Access organization
#
$Customers | Where {$_.Toplevel -eq $true} | Foreach {
	
    $customerName =  $customerName = [Security.SecurityElement]::Escape($_.Name)
	
    write-host "$customerName"
#
# If you want to use the ScreenConnect SubGroup feature to break out the devices according to their
# site, then uncomment the following line and comment out the corresponding line below.
#
# Note - Using this feature introduces an (empty) subgroup for all organizations where the subgroup field
# isn't set.
#
#    $Script:xml += "<SessionGroup SubgroupExpressions=`"customProperty2`" SessionFilter=`"CustomProperty1 = '$customerName'`" IsSystem=`"false`" SessionType=`"Access`" Name=`"$customerName`"/>"

#
# Comment out the line below if you're using the SubGroup feature above.
#
    $Script:xml += "<SessionGroup SubgroupExpressions=`"`" SessionFilter=`"CustomProperty1 = '$customerName'`" IsSystem=`"false`" SessionType=`"Access`" Name=`"$customerName`"/>"
	}


#
# Add Remaining Custom Session Groups
#
$xml += @'
<SessionGroup Name="Client - Ramah - Accounting2" SessionType="Access" IsSystem="false" SessionFilter="SessionID = '112d0620-59bd-4728-827b-511291db7161'"/>
'@


#
# Complete SessionGroup XML file.
#
$xml += '</SessionGroups>'


#
# Remove old backup, if present, then backup existing ScreenGroup.xml file
# before writing out new file.  Finally, restart the ScreenConnect Session
# Manager to read in the new Screen Groups.
#
If (Test-Path ("$ScreenConnect\App_Data\SessionGroup.previous.xml")) {
	Remove-Item "$ScreenConnect\App_Data\SessionGroup.previous.xml"
	}
	
Rename-Item "$ScreenConnect\App_Data\SessionGroup.xml" "$ScreenConnect\App_Data\SessionGroup.previous.xml"

$xml | Out-File "$ScreenConnect\App_Data\SessionGroup.xml"

Restart-Service "ScreenConnect Session Manager" -force
