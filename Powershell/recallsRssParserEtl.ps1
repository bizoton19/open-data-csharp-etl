 function Send-EmailTo($Jsonstring){
    $recallLog="C:\temp\recallsLogprod1.json"
    $jsonstring | ConvertTo-Json | out-file $recallLog -Append utf8

    $subject = "Recalls Published To Public Database"
    
    $body = [string]::Join([Environment]::NewLine,$Jsonstring) | Format-Table
    #$localCred = Get-Credential
    #$cred = New-Object System.Net.NetworkCredential($localCred.UserName,$localCred.Password)
    $att = new-object System.Net.Mail.Attachment($recallLog)
    Send-MailMessage -SmtpServer "172.16.1.67" -Attachments $recallLog  -From "noreply@cpsc.gov" -To "asalomon@cpsc.gov" -Subject $subject  -Body "loged recalls attached"  -BodyAsHtml  -Encoding UTF8
    
    
}

$listofRecallsToSend = New-Object System.Collections.ArrayList
$recallsEndPoint = "https://www.cpsc.gov/newsroom/cpsc-rss-feed/recalls-rss-sp"

 $htmlObject = invoke-restmethod -Uri $recallsEndPoint -Method Get 
 $descriptionHtmlJsonObject = $htmlObject.description
 $pubDate = $htmlObject.pubDate
 echo $descriptionHtmlJsonObject | Format-Table
 
 foreach($o in $descriptionHtmlJsonObject)
 {
   
  
  $s = ConvertFrom-Json $o 
  $backtoJson = ConvertTo-Json $s 
  $year = ([datetime]$s.field_rc_date.und.value).Year
  $urlseg=$s.field_rc_url_segment.und.safe_value
  $desc = [System.Web.HttpUtility]::HtmlDecode($s.field_rc_recall_description.en.value) -replace "<.*?>",''
  $imagesToLoad=[String]::Empty
  $images=$s.field_rc_images

  $imagesCount = $images.und.Count

  if($imagesCount -gt 0)
  {
    $count=0
    $finalImages = New-Object System.Collections.ArrayList
     $fileNames = $images.und.filename
    
     foreach($image in $fileNames){
     
     $image = "https://www.cpsc.gov/s3fs-public/$image"
     $finalImages.Add($image)
     
     }
     
     $imagesToLoad = [String]::Join("|",$finalImages.ToArray())
  }

  $inConjunctions = [string]::Empty
  foreach($country in $s.field_rc_in_conjunction_with.und.value)
  {

     $inConjunctions += "$country|"

  }
  
 $recall_obj = @{
          
        RecallNo = $s.field_rc_number.und.value.Replace('-','')
        RecallDate =  [String]::Format("{0:yyyy-MM-dd}",[datetime]$s.field_rc_date.und.value)
        RecallUrl = "https://www.cpsc.gov/Recalls/$year/$urlseg"
        RecallTitle=$s.title
        ConsumerContact=[System.Web.HttpUtility]::HtmlDecode($s.field_rc_consumer_contact.en.value)  -replace "<.*?>",''
        RecallDescription= $desc
        LastPublishDate= [String]::Format("{0:yyyy-MM-dd}",[datetime]::UtcNow)
        ProductName=$s.field_rc_product_name.en.value
        Remedies=[System.Web.HttpUtility]::HtmlDecode($s.field_rc_remedy.en.value) -replace "<.*?>",''
        NumberOfUnit=$s.field_rc_units.en.value
        Remedy=[System.Web.HttpUtility]::HtmlDecode($s.field_rc_remedy.en.value) -replace "<.*?>",''
        Injury=[System.Web.HttpUtility]::HtmlDecode($s.field_rc_incidents.en.value)-replace "<.*?>",''
        ManufacturerCountry=$s.field_rc_manufactured_in.und.value
        Manufacturer=$s.field_rc_manufacturers.value
        Hazard=[System.Web.HttpUtility]::HtmlDecode( $s.field_rc_hazard_description.en.value) -replace "<.*?>",''
        SoldAtLabel=$s.field_rc_sold_at_label.und.value 
        Retailer=[System.Web.HttpUtility]::HtmlDecode($s.field_rc_sold_at.en.value) -replace "<.*?>",''
        Distributor=$s.field_rc_distributors.value 
        CustomLabel=$s.field_rc_custom_label.value 
        CustomField=$s.field_rc_custom_field.value
        RemedyOption=[String]::Join("|",$s.field_rc_remedy_options.und.value)
        ImageUrl = $imagesToLoad.ToString().Trim() 
        InconjunctionCountry=$inConjunctions
        ProductDescription=""
        ProductModel=""
        UPC=""

     
}
echo $recall_obj
$postOutput = Invoke-WebRequest -Uri "http://powertester/cgi-bin/dotnetapps/RestwebModifyServices/Recall"  -method POST -Body $recall_obj -UseDefaultCredentials
 if($postOutput.StatusCode -eq 200){
    $stringJson = $recall_obj 
    $listofRecallsToSend.Add($stringJson)
    
 }
 }
 echo $listofRecallsToSend.Count
 Send-EmailTo($listofRecallsToSend.ToArray())
 

 