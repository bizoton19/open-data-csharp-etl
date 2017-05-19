
$neissfiles = get-childitem "G:\USERS\EXIS\ASalomon\bigData\neiss-raw-tsv\*" -include  "*.tsv" -Exclude  @("*.tsv.old", "*.old", "*_old.tsv", "*_tsv.old")


foreach($file in $neissfiles){
  $counter++
  $fileName = $file.Name
  $outputPath = "G:\USERS\EXIS\ASalomon\BigData\json\"
   $jsonFile =Get-Content $file | ConvertTo-Json 
   echo $jsonFile
   $jsonFile | out-file "$OutputPath$fileName" -NoTypeInformation -Append
   



}