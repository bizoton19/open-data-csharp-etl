#start elastic search
#wait 60 seconds
#start Kibana
#future start service
Start-Process -FilePath 'C:\Program Files\Elastic\Elasticsearch\bin\elasticsearch.exe' 
Start-Sleep -Seconds 60
start-Process 'C:\Program Files\Elastic\kibana-5.5.0-windows-x86\bin\kibana.bat' -Wait
npm run dev