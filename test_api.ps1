$body = @{
    TourId = 1
    StartDate = "2026-08-01T00:00:00"
    EndDate = "2026-08-01T00:00:00"
    MaxParticipants = 20
    AvailableSeats = 20
    ActualAdultPrice = 1000000
    ActualChildPrice = 700000
    GuideName = "Test Guide"
    Status = "Active"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5023/odata/TourSchedules" -Method Post -Body $body -ContentType "application/json" -ErrorAction Stop
