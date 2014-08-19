ScreenConnect client deployment
-------------------------------
See "Client Install\ScreenConnect installation.jpg" for a diagram showing how the finished product works. Do all of this at your own risk, etc etc.

1) Copy the following files to your ScreenConnect server, into C:\Program Files (x86)\ScreenConnect:

 - Client Install\InstallFromNCentralAgent.aspx
 - Client Install\InstallFromNCentralAgent.aspx.cs
 - Client Install\TellMeMyKey.aspx
 - Client Install\TellMeMyKey.aspx.cs

2) Visit http://your.screenconnect.server/TellMeMyKey.aspx and copy the 10 characters you see, ready to paste later.

3) Delete TellMeMyKey.aspx and TellMeMyKey.aspx.cs from the ScreenConnect server, these are only needed for displaying the key in the previous step.

4) Edit web.config on your ScreenConnect server at C:\Program Files (x86)\ScreenConnect. Find the appSettings section with all the <add key="x" value="y" /> entries and add the following three:

   <add key="NCentralServerAddress" value="http://your.ncentral.server" />
   <add key="NCentralAPIUsername" value="ncentral_api_user" />
   <add key="NCentralAPIPassword" value="ncentral_api_password" />
Replace the values with those of your environment, i.e. define an N-Central user that has API level permission.

5) Download and edit the top two lines of the Client Install\InstallScreenConnect.ps1 script. Put your ScreenConnect server URI on the first line, and paste in the key from step 2 into the second variable.

6) As a test, run the script on any device that has an N-Central agent but no ScreenConnect client. It should download and install ScreenConnect silently with the organization set to the N-Central customer that the agent belongs to.

7) You can now upload the script to the N-Central repository and run it on any N-Central agent.

 

ScreenConnect Session Groups Sync
---------------------------------
Continue below to configure automatic syncing of ScreenConnect session groups from your N-Central customer list.

8) Copy the Session Group Sync\GenerateScreenConnectSessionGroups.ps1 to a folder on your ScreenConnect server.

9) To be on the safe side, take a backup copy of C:\Program Files (x86)\ScreenConnect\App_Data\SessionGroup.xml

10) Run the GenerateScreenConnectSessionGroups.ps1 script on your ScreenConnect server. NOTE: This will restart the ScreenConnect services. You will also lose any existing Session Groups.

11) Assuming all looks good, you can schedule the script to run nightly to keep it all in sync.


Note: The other script "(old)GenerateScreenConnectSessionGroups_FromReportManager.ps1" was the method used in the original version of the script. This method required you to create a SQL user on your report manager server that has db_datareader access to your ods_n_central_ds1 database, then put these details into the connectionstring at the top of the script. Thanks to Jon Czerwinski, this method has been deprecated in favour of calling the N-Central server directly via the web service.