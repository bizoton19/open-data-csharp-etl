
#Param(
#[Parameter(Mandatory=$true)]
 # [DateTime]$startDateInput,
  #[Parameter(Mandatory=$true)]
  #[DateTime]$endDateInput,
 # [Parameter(Mandatory=$true)]
 # [string]$outputFileNameWithoutExtension
#)
[Reflection.Assembly]::LoadWithPartialName("System.Web.Extensions")
# Load the Dot Net Json Library
#[Reflection.Assembly]::LoadFile("C:\Users\asalomon\Documents\Visual Studio 2013\Projects\Prototypes\Application.Dashboard\bin\Newtonsoft.Json.dll")
$outputFileNameWithoutExtension="recallApiExtract_Full2."
$format = "csv"
$outputPath="G:\USERS\EXIS\$env:username\$outputFileNameWithoutExtension$format";

$startDate = "01/01/2000"#[DateTime]::Parse($startDateInput);
$endDate= "01/30/2015" #[DateTime]::Parse($endDateInput);
$url = "http://www.saferproducts.gov/restwebservices/recall/?format=$format";


#$ESEndPoint = "http://localhost:9200"
#$indexname = "cpsc-recalls"
#$typename = "recalls"
#$response = Invoke-RestMethod "$ESEndPoint/$indexname" -Method Put -ErrorAction SilentlyContinue 
#$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"

 
$j = invoke-restmethod "http://www.saferproducts.gov/restwebservices/recall/?format=$format" -ContentType "application/xml"
if($format -eq "json"){
$j | ConvertTo-Json -depth 4 | out-file $outputPath ;
}

elseif($format -eq "xml")
{
   $j.Save($outputPath) ;

}
else {

foreach($obj in $j){
     
    $recallDate
    if($obj.RecallDate -eq ""  $obj.RecallDate -eq null){
      $recallDate= [DateTime]::Parse($obj.RecallDate);
    }
    $recallNumber = $obj.RecallNumber
    $HazardCount = $obj.Hazards.Count
    $ProductCount = $obj.Products.Count
    $recallTitle = $obj.Title
    $recallId = $obj.RecallId
    echo $recallTitle
    
    if( $ProductCount -gt 1 ){ 
           echo "This recall Id $recallId has $ProductCount products"
           write-host "$recallTitle has $HazardCount hazards and the recall date is $recallDate "
    }

    $recallHazardDescription = @{
                                 Name = "Hazard"
                                 Expression =  {  
                                     $obj.Hazards.Name
                                   }
                                }
    $ProductName = @{
                                 Name = "ProductName"
                                 Expression =  {  
                                     $obj.Products.Name
                                    
                                   }
                                }

   $ProductDescription = @{
                                 Name = "ProductDescription"
                                 Expression =  {  
                                     $obj.Products.Description
                                    
                                   }
                                }
    $ProductModel = @{
                                 Name = "ProductModel"
                                 Expression =  {  
                                     $obj.Products.Model
                                    
                                   }
                                }
    $ProductType = @{
                                 Name = "ProductType"
                                 Expression =  {  
                                     $obj.Products.Type
                                    
                                   }
                                }
    $ProductCategoryId = @{
                                 Name = "ProductCategory"
                                 Expression =  {  
                                     $obj.Products.CategoryID
                                    
                                   }
                                }
    $ProductNumberOfUnits = @{
                                 Name = "numOfUnits"
                                 Expression =  {  
                                     $obj.Products.NumberOfUnits
                                    
                                   }
                                }


   # $Inconjusctions = @{
   #                              Name = "Hazard"
    #                             Expression =  {  
    #                                 $obj.Hazards
     #                              }
     #                           }

    $Images = @{
                                 Name = "ImageURL"
                                 Expression =  {  
                                     $obj.Images.URL
                                   }
                                }

    $Injuries = @{
                                 Name = "Injury"
                                 Expression =  {  
                                     $obj.Injuries.Name
                                   }
                                }

    $Remedies = @{
                                 Name = "Remedy"
                                 Expression =  {  
                                     $obj.Remedies.Name
                                   }
                                }

    $ProductUPCs = @{
                                 Name = "UPC"
                                 Expression =  {  
                                   $obj.ProductUPCs.UPC
                                  }
                                }
    $Retailers = @{
                                 Name = "Retailer"
                                 Expression =  {  
                                     $obj.Retailers.Name
                                   }
                                }

    $ManufacturerCountries = @{
                                 Name = "Country"
                                 Expression =  {  
                                     $obj.ManufacturerCountries.Country
                                   }
                                }

     $Manufacturers = @{
                                 Name = "Manufacturer"
                                 Expression =  {  
                                     $obj.Manufacturers.Name
                                   }
                                }
 

  
    if($recallDate -ge $startDate -and $recallDate -le $endDate){
                $obj | 
                 Select-Object RecallID,
                    Title, 
                    RecallNumber, 
                    URL, Description,
                    $recallHazardDescription, 
                    LastPublishDate,RecallDate,
                     $ProductName,$ProductDescription,$ProductModel,$ProductType,$ProductCategoryId,$ProductNumberOfUnits, $injuries,$Remedies, $manufacturers,$ManufacturerCountries, $Retailers, $Images |
                Export-Csv -Encoding UTF8 $outputPath -Append -NoTypeInformation;


    }

   
     $id = $obj.RecallID
   
    }
#$serializedRecall=  $obj | Select-Object RecallID, Title, RecallNumber, URL, Description, $recallHazardDescription,LastPublishDate,RecallDate | ConvertTo-json
  
   

    #Elastic search Load Region
    #########################################

    # $indexedEndpoint = "$ESEndPoint/$indexname/$typename/$id"
    # echo "building uri with parms $indexedEndpoint"
    # $webClient = new-object System.net.WebClient
    # $webClient.UploadString($indexedEndpoint,$serializedRecall )
    
    # invoke-restmethod -Uri $ESEndPoint -Method Post -Body $serializedRecall  -ContentType 'application/json' -TransferEncoding deflate
    # Note: Retrieve from ES with GET /my-index/mytype/_all
  
}