

function Hdfs-Put {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][byte[]]$data,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$user,
        [Parameter(Mandatory=$False)][ValidateSet('open', 'append', 'overwrite')][string]$mode = 'open'
    )
         
    if (!(Test-Path $localPath)) { throw "$localPath does not exist" }
    if ($hdfsPath -notmatch '^/') { throw "hdfsPath must start with a /" }
    $method = 'PUT'
    $uri = "http://${hostname}:$port/webhdfs/v1${hdfspath}?op=CREATE&overwrite=false&user.name=$user"
    if ($mode -match 'append') { $uri = "http://${hostname}:$port/webhdfs/v1${hdfspath}?op=APPEND&user.name=$user"; $method = 'POST' }
    if ($mode -match 'overwrite') { $uri = "http://${hostname}:$port/webhdfs/v1${hdfspath}?op=CREATE&overwrite=true&user.name=$user" }
    # webHDFS Create operation requires two requests. The first is sent without data and redirects
    # to node name and port where data should be sent
    $wr = [System.Net.WebRequest]::Create($uri)
    $wr.Method = $method
    $wr.AllowAutoRedirect = $false
    $response = $wr.GetResponse()
    if ($response.StatusCode -ne 'TemporaryRedirect') {
        throw 'Error: Expected temporary redirect, got ' + $response.StatusCode
    }
    $wr = [System.Net.WebRequest]::Create($response.Headers['Location'])
    $wr.Method = $method
    $wr.ContentLength = $data.Length
    $requestBody = $wr.GetRequestStream()
    $requestBody.Write($data, 0, $data.Length)
    $requestBody.Close()
 
    # Return the reponse from webHDFS to the caller
    $wr.GetResponse()
}
 
function Hdfs-Get {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$False)][string]$user,
        [Parameter(Mandatory=$False)][long]$offset = 0,
        [Parameter(Mandatory=$False)][long]$length = 67108864
    )
         
    $uri = "http://${hostname}:$port/webhdfs/v1${hdfspath}?op=OPEN&offset=$offset&length=$length"
   
    if ($user) { $uri += '&user.name=' + $user }
    $wr = [System.Net.WebRequest]::Create($uri)
    $response = $wr.GetResponse()
    $responseStream = $response.GetResponseStream()
    $br = New-Object System.IO.BinaryReader($responseStream)
    $br.ReadBytes($response.ContentLength)
    $br.Close()
    $responseStream.Close()
}
 
function Hdfs-List {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath
    )
    if ($hdfsPath -notmatch '^/') { throw "hdfsPath must start with a /" }
    $fileStatus= Invoke-RestMethod -Method Get -Uri "http://${hostname}:$port/webhdfs/v1${hdfsPath}?op=LISTSTATUS"
    foreach ($item in $fileStatus.FileStatuses.FileStatus) {
        echo $item.PathSuffix
        $item.accessTime = Convert-FromEpochTime $item.accessTime
        $item.modificationTime = Convert-FromEpochTime $item.modificationTime
        $item
    }
}
 
function Hdfs-Remove {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$user,
        [switch]$recurse
    )
    if ($hdfsPath -notmatch '^/') { throw "hdfsPath must start with a /" }
    if ($recurse) { $rec = 'true' } else { $rec = 'false' }
    $result = Invoke-RestMethod -Method Delete -Uri "http://${hostname}:$port/webhdfs/v1${hdfsPath}?op=DELETE&recursive=$rec&user.name=$user"
    $result.boolean
}
 
function Hdfs-Mkdir {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$user,
        [Parameter(Mandatory=$False)][string]$permission
    )
    if ($hdfsPath -notmatch '^/') { throw "hdfsPath must start with a /" }
    
    if ($permission) {
        $result = Invoke-RestMethod -Method Put -Uri "http://${hostname}:$port/webhdfs/v1${hdfsPath}?op=MKDIRS&permission=$permission&user.name=$user" }
    else { $result = Invoke-RestMethod -Method Put -Uri "http://${hostname}:$port/webhdfs/v1${hdfsPath}?op=MKDIRS&user.name=$user" }
    $result.boolean
}
 
function Hdfs-Rename {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$hdfsNewPath,
        [Parameter(Mandatory=$True)][string]$user
    )
    if ($hdfsPath -notmatch '^/') { throw "hdfsPath must start with a /" }
    if ($hdfsNewPath -notmatch '^/') { throw "hdfsNewPath must start with a /" }
    $result = Invoke-RestMethod -Method Put -Uri "http://${hostname}:$port/webhdfs/v1${hdfsPath}?op=RENAME&user.name=$user&destination=$hdfsNewPath"
    $result.boolean
}
 
function Convert-FromEpochTime ([long]$epochTime) {
    [TimeZone]::CurrentTimeZone.ToLocalTime(([datetime]'1/1/1970').AddSeconds($epochTime/1000))
}
 
function Hdfs-PutFile {
   param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$localPath,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$user,
        [Parameter(Mandatory=$False)][int]$sliceSize = 67108864,
        [Parameter(Mandatory=$False)][ValidateSet('open', 'append', 'overwrite')][string]$mode = 'open'
    )
       
    try {
        $br = New-Object System.IO.BinaryReader([System.IO.File]::Open($localPath, [System.IO.FileMode]::Open))
    } catch { throw $error[0].Exception.Message }
    $total = $br.BaseStream.Length
    $sent = 0
    $firstRun = $true
   
    do {
        Write-Progress -Activity "Copying $localPath to HDFS on $hostname" -PercentComplete ($sent/$total * 100)
        $data = $br.ReadBytes($sliceSize)
        try {
            Hdfs-Put -hostname $hostname -port $port -user $user -hdfsPath $hdfsPath -data $data -mode $mode | out-null
        } catch { $br.Close(); throw $error[0].Exception.Message }
        $sent += $sliceSize
        if ($firstRun) { $firstRun = $false; $mode = 'append' }
    } while ($data.LongLength -eq $sliceSize)
    $br.Close()
}
 
function Hdfs-GetFile {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$False)][string]$user,
        [Parameter(Mandatory=$False)][string]$localPath,
        [Parameter(Mandatory=$False)][long]$length,
        [switch]$append,
        [switch]$overwrite
    )
    if ($append -and $overwrite) { throw 'Cannot use -append and -overwrite together' }
    $mode = [System.IO.FileMode]::CreateNew
    if ($append) {$mode = [System.IO.FileMode]::Append}
    if ($overwrite) {$mode = [System.IO.FileMode]::Create}
   
    try {
        $bw = New-Object System.IO.BinaryWriter([System.IO.File]::Open($localPath, $mode))
    } catch { throw $error[0].Exception.Message }
   
    $fileAttribs = Hdfs-List -hostname $hostname -hdfsPath $hdfsPath -port $port
    if (!$length) { $length = $fileAttribs.length }
    $blockSize = $fileAttribs.blockSize
    if ($length -lt $blockSize) { $blockSize = $length }
    $got = 0
   
    do {
        Write-Progress -Activity "Copying $hdfsPath to $localPath" -PercentComplete ($got/$length * 100)
       
        try {
            [byte[]]$data = Hdfs-Get -hostname $hostname -port $port -user $user -hdfsPath $hdfsPath -offset $got -length $blockSize
        } catch { $bw.Close(); throw $error[0].Exception.Message }
        try {
            $bw.Write($data)
        } catch { $bw.Close(); throw $error[0].Exception.Message }
        $got += $data.LongLength
    } while ($got -lt $length)
    $bw.Close()
}
$d =  Get-Date 
#Hdfs-Remove  -hostname sddev001 -port 50070 -hdfsPath "/CPSC/twitter-streams" -user "hadoop" -recurse
#Hdfs-GetFile -hostname sddev001 -port 50070 -hdfsPath "/CPSC/twitter-streams/tweet-728656529876424708" -user "hadoop" -localPath "C:\Big_Data\Twitter_To_File\tweet-728656529876424708.file"
#$jsonRecalls = Get-ChildItem "G:\USERS\EXIS\ASalomon\TwitterData"
#foreach($file in $jsonRecalls){
#Hdfs-Remove  -hostname sddev001 -port 50070 -hdfsPath "/CPSC/twitter-streams" -user "hadoop" -recurse
Hdfs-Mkdir  -hostname   "ec2-34-206-33-76.compute-1.amazonaws.com" -port 50070 -hdfsPath "/CPSC/recalls" 
Hdfs-Mkdir  -hostname  "ec2-34-206-33-76.compute-1.amazonaws.com" -port 50070 -hdfsPath "/CPSC/incidents" -user "hadoop" 
Hdfs-Mkdir  -hostname  "ec2-34-206-33-76.compute-1.amazonaws.com" -port 50070 -hdfsPath "/CPSC/neiss" -user "hadoop" 
Hdfs-Mkdir  -hostname  "ec2-34-206-33-76.compute-1.amazonaws.com" -port 50070 -hdfsPath "/CPSC/twitter" -user "hadoop" 
Hdfs-Mkdir  -hostname  "ec2-34-206-33-76.compute-1.amazonaws.com" -port 50070 -hdfsPath "/CPSC/ebay" -user "hadoop" 

  #Hdfs-PutFile  -hostname sddev001 -port 50070 -hdfsPath "/CPSC/twitter-streams" -user "hadoop" -localPath $file.FullName 
  #Hdfs-List -hostname sddev001  -hdfsPath "/CPSC/twitter-streams" 
#}
#echo $t