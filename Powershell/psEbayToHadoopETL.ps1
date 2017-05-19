 function Hdfs-Put {
    param (
        [Parameter(Mandatory=$True)][string]$hostname,
        [Parameter(Mandatory=$False)][int]$port = 50070,
        [Parameter(Mandatory=$True)][byte[]]$data,
        [Parameter(Mandatory=$True)][string]$hdfsPath,
        [Parameter(Mandatory=$True)][string]$user,
        [Parameter(Mandatory=$False)][ValidateSet('open', 'append', 'overwrite')][string]$mode = 'open'
    )
         
    #if (!(Test-Path $localPath)) { throw "$localPath does not exist" }
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
    $response
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





function Get-RawEbayData ( $ebaySearchUrl){
           $ebaySearchUrl.EndPointSpecification #| out-file c:\spec.txt -Append
           $ebayProducts =  Invoke-RestMethod $ebaySearchUrl.EndPointSpecification
           echo $ebayProducts.GetType()  | out-null
           
          
           $ebayProductList = @()
           


           $items = $ebayProducts.findItemsByKeywordsResponse[0].searchResult[0].item
           $count = $items.Length
           if($count -gt 0){
           echo "Found $count products matching CPSC Recalled Product" | out-null
           for($i = 0 ; $i -lt $items.Length;$i++){
              $item = $items[$i]
             
              $ebayProduct = New-Object System.Object
              $ebayProduct | Add-Member  -NotePropertyName ItemId -NotePropertyValue $item.itemId
              $ebayProduct | Add-Member  -NotePropertyName Title -NotePropertyValue $item.title
              $ebayProduct | Add-Member  -NotePropertyName CategoryId -NotePropertyValue $item.PrimaryCategory.CategoryId
              $ebayProduct | Add-Member  -NotePropertyName CategoryName -NotePropertyValue $item.PrimaryCategory.CategoryName
              $ebayProduct | Add-Member  -NotePropertyName ImageGalleryUrl -NotePropertyValue $item.galleryURL
              $ebayProduct | Add-Member  -NotePropertyName ItemListingUrl -NotePropertyValue $item.viewItemUrl
              $ebayProduct | Add-Member  -NotePropertyName SellerLocation -NotePropertyValue $item.location
              $ebayProduct | Add-Member -NotePropertyName SellerCountry -NotePropertyValue $item.country
              $ebayProduct | Add-Member  -NotePropertyName IsActive -NotePropertyValue $item.active
              $ebayProduct | Add-Member -NotePropertyName PossibleCpscProductMatch -NotePropertyValue $ebaySearchUrl.SafeQuery
              
              $ebayProductList += $ebayProduct
              
     
     
     
     
     
     
     
           }
           }
           
           return $ebayProductList

        }
        #http://svcs.ebay.com/services/search/FindingService/v1?OPERATION-NotePropertyName=findItemsByKeywords&SERVICE-VERSION=1.0.0&
        #SECURITY-APPNAME=Consumer-CpscReca-PRD-63890c490-5b88f7c8&GLOBAL-ID=EBAY-US&RESPONSE-DATA-FORMAT=JSON
        #&REST-PAYLOAD&keywords=hoverboard&paginationInput.entriesPerPage=10

        #http://svcs.ebay.com/services/search/FindingService/v1?OPERATION-NotePropertyName=findItemsByKeywords&SERVICE-VERSION=1.0.0&SECURITY-APPNAME=Consumer-CpscReca-PRD-63890c490-5b88f7c8&GLOBAL-ID=EBAY-US&RESPONSE-DATA-FORMAT=JSON&REST-PAYLOAD&keywords=hoverboard&paginationInput.entriesPerPage=20

        function Find-SimilarCPSCCategories($categoryName){






        }



 function Transfer-ProductDataToHadoop($data){

        }

 function  Get-CSPCRecalls(){

        $j = invoke-restmethod "http://www.saferproducts.gov/restwebservices/recall/?format=json" 
            echo $j.GetType() | Out-Null
           
            $productNameList = New-Object System.Collections.ArrayList
    foreach($obj in $j){

       echo $recallTitle | Out-Null
    
    
       foreach($product in $obj.Products){
         echo $product.Name | Out-Null
         echo $product.GetType().Name | Out-Null
         $productNameList.Add($product.Name.ToString()) | Out-Null

         }

        }
        return $ProductNameList

        }



$p = Get-CSPCRecalls
$newEbayOutputLocation= "G:USERS\EXIS\Asalomon\bigdata\ebay\"
#Hdfs-Mkdir  -hostname sddev001 -port 50070 -hdfsPath "/CPSC/ebay-products" -user "hadoop" 
New-item -type Directory "G:USERS\EXIS\Asalomon\bigdata\ebay"
for($pi = 0 ; $pi -lt $p.Length;$pi++){
#API request variables 
#http://svcs.ebay.com/services/search/FindingService/v1?OPERATION-NotePropertyName=findItemsByKeywords&SERVICE-VERSION=1.0.0&SECURITY-APPNAME=&GLOBAL-ID=EBAY-US&&keywords=hoverboard&paginationInput.entriesPerPage=10
		      $ebaySearchUrl = New-Object System.Object
              $ebaySearchUrl | Add-Member -NotePropertyName EndpointRoot -NotePropertyValue "http://svcs.ebay.com/services/search/FindingService/v1?"
              $ebaySearchUrl | Add-Member  -NotePropertyName Version -NotePropertyValue "1.0.0"
              $ebaySearchUrl | Add-Member  -NotePropertyName AppID -NotePropertyValue "Consumer-CpscReca-PRD-63890c490-5b88f7c8"
              $ebaySearchUrl | Add-Member -NotePropertyName GlobalId -NotePropertyValue "EBAY-US"
              $ebaySearchUrl | Add-Member  -NotePropertyName SearchQuery -NotePropertyValue $p[$pi];
              $ebaySearchUrl |  Add-Member  -NotePropertyName RecordsPerpage -NotePropertyValue 100
              $ebaySearchUrl | Add-Member  -NotePropertyName SafeQuery -NotePropertyValue $p[$pi];
              $ebaySearchUrl | Add-Member  -NotePropertyName OperationName -NotePropertyValue "findItemsByKeywords"
              

        #$endPoint = "http://svcs.ebay.com/services/search/FindingService/v1?";
		#$version = "1.0.0";
		#$appId = "Consumer-CpscReca-PRD-63890c490-5b88f7c8";
		#$globalId = "EBAY-US";
		#$query = $p[$pi];
        #$recordsperPage = 20
		#$safeQuery = $query;
		#$operationName = "findItemsByKeywords"
        #=====================================================================
		#Construct the finItemsByKeywords HTTp GET call
		$apicall=$endPoint;
		$apicall += "OPERATION-Name="+$ebaySearchUrl.OperationName
		$apicall += "&SERVICE-VERSION="+$ebaySearchUrl.Version
		$apicall += "&SECURITY-APPNAME="+$ebaySearchUrl.AppID
        
		$apicall += "&GLOBAL-ID="+$ebaySearchUrl.GlobalId
         $apicall += "&RESPONSE-DATA-FORMAT=JSON&REST-PAYLOAD"
        
		$apicall+= "&keywords="+$ebaySearchUrl.SafeQuery;
		$apicall += "&paginationInput.entriesPerPage="+$ebaySearchUrl.RecordsPerpage
        
        $ebaySearchUrl | Add-Member  -NotePropertyName EndpointSpecification -NotePropertyValue $apicall

       $ebayDataList = Get-RawEbayData($ebaySearchUrl);
       foreach($ebayProd in $ebayDataList)
       {
           $fileName = $ebayProd.ItemId
           $convertedObject = ConvertTo-Json $ebayProd | out-file "$newEbayOutputLocation$fileName.json"
           $convertedObjectcsv = ConvertTo-csv $ebayProd | out-file "$newEbayOutputLocation$fileName.csv"
           
           $convertedObject = [System.Text.Encoding]::Unicode.GetBytes($convertedObject)
           #Hdfs-Put -hostname sddev001 -port 50070 -hdfsPath "/CPSC/ebay-products/$fileName.json" -user "hadoop" -data $convertedObject 
      
      
      }
      }
       