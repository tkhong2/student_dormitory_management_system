$baseUrl = "http://localhost:5002/api"

try {
    $studentLoginBody = @{
        Username = "student"
        Password = "Student@123"
    } | ConvertTo-Json
    $studentLoginResp = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $studentLoginBody -ContentType "application/json"
    $studentToken = $studentLoginResp.token

    $createBody = @{
        RoomId = 101
        RoomNumber = "A101"
        BuildingName = "Building A"
        RequestedByStudentId = 1
        RequestedByStudentName = "Nguyen Van A"
        Title = "Broken lamp"
        Description = "Need to replace the lamp."
        Category = "Electric"
        Priority = "High"
    } | ConvertTo-Json -Compress
    $headers = @{ "Authorization" = "Bearer $studentToken" }
    
    $createResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests" -Method Post -Headers $headers -Body $createBody -ContentType "application/json; charset=utf-8"
    $requestId = $createResp.id
    Write-Host "Create success! ID: $requestId"

    $staffLoginBody = @{
        Username = "staff"
        Password = "Staff@123"
    } | ConvertTo-Json
    $staffLoginResp = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $staffLoginBody -ContentType "application/json"
    $staffToken = $staffLoginResp.token

    $updateBody = @{
        NewStatus = "InProgress"
        Note = "Working on it"
    } | ConvertTo-Json -Compress
    $staffHeaders = @{ "Authorization" = "Bearer $staffToken" }
    
    $updateResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests/$requestId/status" -Method Put -Headers $staffHeaders -Body $updateBody -ContentType "application/json; charset=utf-8"
    Write-Host "Update success!"
    
    $getResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests/$requestId" -Method Get -Headers $staffHeaders
    Write-Host ($getResp | ConvertTo-Json -Depth 5)

} catch {
    Write-Host "Error occurred:"
    Write-Host $_.Exception.Response.StatusCode
    $stream = $_.Exception.Response.GetResponseStream()
    if ($stream) {
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Body: $responseBody"
    }
}