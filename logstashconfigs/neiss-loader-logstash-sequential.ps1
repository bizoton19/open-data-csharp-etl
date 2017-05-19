function CreateConfigFileForYearFile ($yearFileName){
  $originalConfContent = get-content .\neiss-tsv_to_elasticsearch.conf 
  echo "retreived content from $yearFileName"
  echo "replacing file name in config and creating new config for file"
  $newContent = $originalConfContent.Replace("*.tsv","$yearFileName").Replace("cpscneiss","cpscneiss-$yearFileName") | out-file ".\$yearFileName.conf" -NoClobber ascii -Force
  $loadCmd = "logstash.bat -f .\$yearFileName.conf" | out-file ".\loadcmd.bat" -NoClobber ascii  -Force
 
  
}

function CreateIndexAndLoadYearFiles(){
  $files = get-childitem "G:\USERS\EXIS\ASalomon\bigData\neiss-raw-tsv\*" -include  "*.tsv"
  foreach($file in $files){
    
    $fileName = $file.Name
    $configName = "$fileName.conf"
    echo "processing $fileName"
    CreateConfigFileForYearFile $fileName
    echo "successfully created config file for $fileName"
    echo "Executing logstash for loading into elastic search"
    $process = start-process -WorkingDirectory ".\" -Wait "loadcmd.bat"  -PassThru 
    $process.WaitForExit()
    if($process.ExitCode -eq 0 ){
   
   
    echo  "Process exited with code of $exCode"
    echo "successfully loaded all content for $fileName to elastic search"
    echo "removing $configName from directory"
    Remove-Item  $configName
    echo "removed $configName from directory"
    }
    
  }
  echo " Processing complete"
  PAUSE
  
}

CreateIndexAndLoadYearFiles
PAUSE