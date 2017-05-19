
$neissfiles = get-childitem "G:\USERS\EXIS\ASalomon\BigData\neiss-raw-tsv" -include  "*.tsv" -Exclude  @("*.tsv.old", "*.old", "*_old.tsv", "*_tsv.old")

$totalFileCount = $neissfiles.Count | echo


foreach($file in $neissfiles)
{
  $fileName = $file.FullName
  $exit = Start-Process -Wait "C:\MongoDB\bin\mongoimport.exe" -NoNewWindow  -ArgumentList @("/db BigData","/collection neiss-data" , "/file $fileName", "/type tsv", "/headerline")
  $exit.WaitForExit()
  $code = $exit.ExitCode
  $progress = "loaded $fileName into localhost Mongodb neiss-data collection with an exit code of $code " | out-file -FilePath C:\exittools\BigData\MongoDbLoadSummary.txt -Append

}