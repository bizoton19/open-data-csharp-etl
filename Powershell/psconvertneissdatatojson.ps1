
$mergedFileLocation = "G:\USERS\EXIS\ASalomon\bigData\json\"
$neissfiles = get-childitem "G:\USERS\EXIS\ASalomon\bigData\neiss-raw-tsv\*" -include  "*.tsv" -Exclude  @("*.tsv.old", "*.old", "*_old.tsv", "*_tsv.old")
$Output= @()
$totalFileCount = $neissfiles.Count | echo


foreach($file in $neissfiles)
{
$counter=0
  echo $file
  
  
  if($file.Length -gt 0){
  $fileName = $file.Name
  $fileFullName = $file.FullName

  foreach($line in [System.IO.File]::ReadLines($file.FullName)){
  $counter++   
  if($counter -gt 1){
  $temp =  $line.Split("`t")
   
 $neiss_obj = @{
  
  CpscCaseNo=$temp[0]
  trmt_date=$temp[1]
  psu_weight=$temp[3]
  stratum=$temp[4]
  age=$temp[5]
  sex=$temp[6]
  race=$temp[7]
  race_other=$temp[8]
  diag=$temp[9]
  diag_other=$temp[10]
  body_part=$temp[11]
  disposition=$temp[12]
  location=$temp[13]
  fmv= $temp[14]
  prod1=$temp[15]
  prod2=$temp[16]
  narr1=$temp[17]
  narr2=$temp[18]




 }
    
     
     $neiss_obj | convertto-json | out-file "$mergedFileLocation$fileName$counter.json" -Append
    

     }
   }
  }
}

echo "merge file created" 