function Get-RawEbayData ( $ebaySearchUrl){
           $ebaySearchUrl.EndPointSpecification #| out-file c:\spec.txt -Append
           $finalUrl= [string]::Concat($ebaySearchUrl.EndpointRoot,$ebaySearchUrl.EndpointSpecification)
           $ebayProducts =  Invoke-RestMethod $finalUrl
           echo $ebayProducts.GetType()  | out-null
           
          
           $ebayProductList = @()
           


           $items = $ebayProducts.findItemsByKeywordsResponse[0].searchResult[0].item
           $count = $items.Length
           if($count -gt 0){
           echo "Found $count products matching CPSC Recalled Product" | out-null
           for($i = 0 ; $i -lt $items.Length;$i++){
              $item = $items[$i]
             
              $ebayProduct = New-Object System.Object
              $ebayProduct | Add-Member  -NotePropertyName ItemId -NotePropertyValue $item.itemId -TypeName String
              $ebayProduct | Add-Member  -NotePropertyName Title -NotePropertyValue $item.title -TypeName String
              $ebayProduct | Add-Member  -NotePropertyName CategoryId -NotePropertyValue $item.PrimaryCategory.CategoryId
              $ebayProduct | Add-Member  -NotePropertyName CategoryName -NotePropertyValue $item.PrimaryCategory.CategoryName
              $ebayProduct | Add-Member  -NotePropertyName ImageGalleryUrl -NotePropertyValue $item.galleryURL -TypeName String
              $ebayProduct | Add-Member  -NotePropertyName ItemListingUrl -NotePropertyValue $item.viewItemUrl -TypeName String
              $ebayProduct | Add-Member  -NotePropertyName SellerLocation -NotePropertyValue $item.location -TypeName String
              $ebayProduct | Add-Member -NotePropertyName SellerCountry -NotePropertyValue $item.country -TypeName String
              $ebayProduct | Add-Member  -NotePropertyName IsActive -NotePropertyValue $item.active -TypeName String
              $ebayProduct | Add-Member -NotePropertyName PossibleCpscProductMatch -NotePropertyValue $ebaySearchUrl.SafeQuery -TypeName String
              
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

        $j = invoke-restmethod "https://www.saferproducts.gov/RestWebServices/recall/?format=json" 
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
$newEbayOutputLocation= "E:\sparkData\output\ebay\"
#Hdfs-Mkdir  -hostname sddev001 -port 50070 -hdfsPath "/CPSC/ebay-products" -user "hadoop" 
#New-item -type Directory "E:\sparkData\output\ebay"
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
       $fileName = "EbayData"
       foreach($ebayProd in $ebayDataList)
       {
       echo $ebayDataList.GetType().Name
       $type= $ebayProd.GetType().Name
    if($type -ne "String"){
           echo $ebayProd.ItemId.GetType().Name
           #$convertedObject = ConvertTo-Json $ebayProd | out-file "$newEbayOutputLocation$fileName.json"
           $convertedObjectcsv =   $ebayProd | Export-Csv "$newEbayOutputLocation$fileName.csv" -Append  -NoClobber -NoTypeInformation
           
           #$convertedObject = [System.Text.Encoding]::Unicode.GetBytes($convertedObject)
           #Hdfs-Put -hostname sddev001 -port 50070 -hdfsPath "/CPSC/ebay-products/$fileName.json" -user "hadoop" -data $convertedObject 
      }
      
      }
      }