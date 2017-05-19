
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
$page=0;
$batch=0
$format="json"
$outputFileNameWithoutExtension= "IncidentdDataApiExtract_Full"

$outputPath="G:\USERS\EXIS\$env:username\$outputFileNameWithoutExtension.$format";

$startDate = [DateTime]::Parse($startDateInput);
$endDate= [DateTime]::Parse($endDateInput);
$url = "https://api.cpsc.gov/incidentData/api/IncidentReports?page=$page";


#$ESEndPoint = "http://localhost:9200"
#$indexname = "cpsc-recalls"
#$typename = "recalls"
#$response = Invoke-RestMethod "$ESEndPoint/$indexname" -Method Put -ErrorAction SilentlyContinue 
#$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
for($start=1; $start -le 563 ; $start++){
$page++
$batch+=50
$outputPath="G:\USERS\EXIS\$env:username\$outputFileNameWithoutExtension$batch.$format";
 $url = "https://api.cpsc.gov/incidentData/api/IncidentReports?page=$page";

 $url | write-host
$j = invoke-restmethod $url  -ContentType "application/json"
if($format -eq "json"){
 
$outputPath = $outputPath.Replace(".json",".file")

$j | ConvertTo-Json -depth 4 | out-file $outputPath 


}elseIf($format -eq "xml"){
    $j.save($outputPath)
 
 }else{
foreach($obj in $j){

    
    
  $Locale = @{ 
                  Name = "LocalePublicName"
                  Expression =  {  $obj.Locale.LocalePublicName  }
                }

  $SourceType = @{
                     Name = "SourceTypPublicName"
                     Expression =  {   $obj.SourceType.SourceTypePublicName }
                   }

  $SeverityType = @{
                     Name = "SeverityType"
                     Expression =  {   $obj.SeverityType.SeverityTypePublicName }
                   }

   $SeverityTypeDescription = @{
                     Name = "SeverityTypeDescription"
                     Expression =  {   $obj.SeverityType.SeverityTypeDescription }
                   }

    $Gender     = @{  
                      Name = "Gender"
                      Expression = { $obj.Gender.GenderPublicName}
                    }

  $RelationshipType = @{    
                            Name = "RelationshipType"
                            Expression = { $obj.RelationshipType.RelationshipTypePublicName}
                        }

  $ProductCategoryId = @{

                          Name="ProductCategoryId"
                           Expression =  { $obj.ProductCategory.ProductCategoryId }
                         }

 $ProductCategoryPublicName = @{

                            Name="ProductCategoryPublicName"
                            Expression =  { $obj.ProductCategory.ProductCategoryPublicName }


                               }

    $IncidentDocumentId =  @{

                           Name="IncidentDocumentId"
                           Expression =  { $obj.IncidentDocuments.DocumentId }


                           }
 
    $obj| Select-Object IncidentReportNumber,
                    IncidentReportId, 
                    IncidentReportDate, 
                    IncidentDate,
                    IncidentReportPublicationDate,
                    IncidentDescription,
                    ProductModelName,
                    ProductBrandName,
                    ProductSerialNumber,
                    ProductPurchasedDate,
                    ProductUPCCode,
                    UserStillHasProduct,
                    ProductDamagedBeforeIncident,
                    ProductModifiedBeforeIncident,
                    IncidentProductDescription,
                    ProductManufacturerCompanyId,
                    ProductManufacturerName,
                    ProductRetailCompanyName,
                    ProductRetailCompanyId,
                    UserContactedManufacturer,
                    AnswerExplanation,
                    UserPlansToContactManufacturer,
                    VictimAgeInMonths,
                    
                    $Locale,
                    $SeverityType,
                    $SeverityTypeDescription,
                    $Gender,
                    $RelationshipType,
                    $SourceType,
                    $ProductCategoryId,
                    $ProductCategoryPublicName,
                    $IncidentDocumentId  |


                   
                Export-Csv -Encoding UTF8 $outputPath -Append -NoTypeInformation;
                
 }
}
#$serializedRecall=  $obj | Select-Object RecallID, Title, RecallNumber, URL, Description, $recallHazardDescription,LastPublishDate,RecallDate | ConvertTo-json
  
   

    #Elastic search Load Region
    #########################################

    # $indexedEndpoint = "$ESEndPoint/$indexname/$typename/$id"
    # write-host "building uri with parms $indexedEndpoint"
    # $webClient = new-object System.net.WebClient
    # $webClient.UploadString($indexedEndpoint,$serializedRecall )
    
    # invoke-restmethod -Uri $ESEndPoint -Method Post -Body $serializedRecall  -ContentType 'application/json' -TransferEncoding deflate
    # Note: Retrieve from ES with GET /my-index/mytype/_all
 
}