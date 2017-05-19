$cols = @("DocumentNumber","IncidentReportNumber", "TaskTypeId","IncidentReportId","LockedDateTime","AssignedOnDateTime","DueDateTime", "Status","AssignedTo", "AssignedBy", "LockedBy", "Assignment", "TaskId", "FinishDateTime", "AverageCompletionDays","ExpectedCompletionDate", "IsHighPriority")
"<schema name = ""CpsrmsTasks"" version=""1.0"">"|  out-file c:\solrSchemaFields.txt -Append
foreach($col in $cols)
{
   #"<field column=""$col"" name=""$col"" />" | out-file c:\solrFieldds.txt -Append
   
   "<field  name=""$col"" type=""string"" indexed=""true"" stored=""true"" required=""true""/>" | out-file c:\solrSchemaFields.txt -Append
   

}
"</schema>" | out-file c:\solrSchemaFields.txt -Append
