$baseUrl = "http://localhost:5002/api"

Write-Host "1. Logging in as Student..."
$studentLoginBody = @{
    Username = "student"
    Password = "Student@123"
} | ConvertTo-Json
$studentLoginResp = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $studentLoginBody -ContentType "application/json"
$studentToken = $studentLoginResp.token
Write-Host "Student token received."

Write-Host "2. Creating Maintenance Request..."
$createBody = @{
    RoomId = 101
    RoomNumber = "A101"
    BuildingName = "Building A"
    RequestedByStudentId = 1
    RequestedByStudentName = "Nguyen Van A"
    Title = "Hỏng bóng đèn"
    Description = "Bóng đèn nhà vệ sinh bị cháy, cần thay gấp."
    Category = "Electric"
    Priority = "High"
} | ConvertTo-Json
$headers = @{ "Authorization" = "Bearer $studentToken" }
$createResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests" -Method Post -Headers $headers -Body $createBody -ContentType "application/json"
$requestId = $createResp.id
Write-Host "Request created with ID: $requestId (Status: $($createResp.status))"

Write-Host "3. Logging in as Staff..."
$staffLoginBody = @{
    Username = "staff"
    Password = "Staff@123"
} | ConvertTo-Json
$staffLoginResp = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $staffLoginBody -ContentType "application/json"
$staffToken = $staffLoginResp.token
Write-Host "Staff token received."

Write-Host "4. Staff updates status to InProgress..."
$updateBody = @{
    NewStatus = "InProgress"
    Note = "Đã tiếp nhận yêu cầu, đang cử thợ qua kiểm tra."
} | ConvertTo-Json
$staffHeaders = @{ "Authorization" = "Bearer $staffToken" }
$updateResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests/$requestId/status" -Method Put -Headers $staffHeaders -Body $updateBody -ContentType "application/json"
Write-Host "Status updated to: $($updateResp.status)"

Write-Host "5. Fetching the updated request details to verify Logs..."
$getResp = Invoke-RestMethod -Uri "$baseUrl/maintenancerequests/$requestId" -Method Get -Headers $staffHeaders
$jsonOut = $getResp | ConvertTo-Json -Depth 5
Write-Host $jsonOut
