# file-backup
Essentially, this is a POC of mirroring a full backup from one location to a destination location. Needs lots of work, but meets basic backup needs for files that don't change a lot.

Mirrors files and directories to specified destination

Setup:

** Setup a new profile in the app.config file **


```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="BackupDirectory" value="\\192.168.0.32\Public\VORTECES\Backup 2016-06-21"/>
    <add key="OriginalDirectory" value="D:\MyCloud"/>
    <add key="ProfileName" value="MyCloud" />
  </appSettings>
</configuration>
