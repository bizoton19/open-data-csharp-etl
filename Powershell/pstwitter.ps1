$files= Get-ChildItem E:\sparkData\*.csv
$twitterMasterFile = "E:\sparkData\output\twitter.csv"
$archivedir = "E:\sparkData\Archive\"
if(!Test-Path -Path $twitterMasterFile){
$twitterHeader="user,message,source,tag" | out-file   $twitterMasterFile -Encoding utf8
}
$content=null
$counter=0
foreach($file in $files){
$fileName = $file.Name
   if((test-path -Path "$archivedir\$fileName") -ne $true ){
    $counter++
    $content =Get-Content $file.FullName
    echo "reading $file..."
    echo $content.Length
  
    echo $content
    echo "appending $content to $twitterMasterFile..."
    $out = $content | Out-File $twitterMasterFile -Append -Encoding default
    move-Item $file.FullName -Destination $archivedir

   # if($counter -eq 500){

      
     #  $counter=0

    #   break;
   # }
    #echo "deleting $file..."
    #Remove-Item $file.FullName
    
    #}
    }

}

    $content = @()