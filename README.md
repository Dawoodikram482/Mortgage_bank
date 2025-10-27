# BuyMyHouse - Microservices Application

A cloud-based estate agency platform with 3 microservices for house listings, mortgage applications, and automated batch processing using Azure Functions.

---

## What This Project Does

**Services:**
1. **Listings API** - Manages house listings (SQL Server) with image storage in Blob Storage
2. **Mortgage API** - Handles mortgage applications (Azure Table Storage with CQRS)
3. **Azure Functions** - Processes applications and sends offer emails (timer-triggered batch jobs)

**Business Logic:**
- Evaluates mortgage applications based on income (min €20k, max loan = 5× income)
- Calculates interest rates (3.5% - 5.0%)
- Generates mortgage offer documents
- Sends email notifications
- Stores house images in Azure Blob Storage

---

## How to Run

### Prerequisites
Install these before running:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Azurite](https://www.npmjs.com/package/azurite) - `npm install -g azurite`
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) - `npm install -g azure-functions-core-tools@4`

SQL Server LocalDB is included with .NET SDK.

### Start the Application
```powershell
# Navigate to project folder
cd "C:\IT Inholland Year 4\Minor Cloud Development\BuyMyHouse"

# Run everything (Azurite + all services)
.\start-all.ps1
```

**If you see an HTTPS certificate warning:**
```powershell
dotnet dev-certs https --trust
```
Then restart the services.

### Access the Services
Once running, open these URLs:
- **Listings API Swagger:** https://localhost:5001/swagger
- **Mortgage API Swagger:** https://localhost:5002/swagger
- **Azure Functions:** http://localhost:7071

---

## How to Test

### 1. Test House Listings
Open browser: https://localhost:5001/swagger

Or use PowerShell:
```powershell
Invoke-WebRequest https://localhost:5001/api/houses
```

You should see 2 pre-seeded houses.

### 2. Test Image Upload (Optional)
Upload an image for a house using Swagger UI:
1. Go to https://localhost:5001/swagger
2. Try `POST /api/houses/{id}/images`
3. Set `id` to `1` or `2`
4. Upload any image file (jpg, png, gif)
5. The image URL will be returned and stored in Blob Storage

Or using PowerShell:
```powershell
$filePath = "C:\path\to\your\image.jpg"
$uri = "https://localhost:5001/api/houses/1/images"
$form = @{
    image = Get-Item -Path $filePath
}
Invoke-RestMethod -Uri $uri -Method Post -Form $form
```

### 3. Test Mortgage Application
**Using Swagger UI:**
1. Go to https://localhost:5002/swagger
2. Try `POST /api/mortgageapplications`
3. Use this test data:
```json
{
  "applicantEmail": "poopdi@peepdi.com",
  "applicantName": "poopoo peepee",
  "annualIncome": 600000,
  "requestedAmount": 250000,
  "houseId": 1
}
```
4. Copy the returned `applicationId`

**Using PowerShell:**
```powershell
$body = @{
    applicantEmail = "lili@paapaa.com"
    applicantName = "hula lala"
    annualIncome = 60000
    requestedAmount = 250000
    houseId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5002/api/mortgageapplications" `
    -Method POST -Body $body -ContentType "application/json"
```

### 3. Test Azure Functions (Manual Trigger)
The functions normally run on schedule (23:00 and 09:00), but you can trigger them manually:

**Process pending applications:**
```powershell
Invoke-WebRequest http://localhost:7071/api/test-process
```

**Send offer emails:**
```powershell
Invoke-WebRequest http://localhost:7071/api/test-send
```

**Check the Function logs in terminal** to see:
- Application processing results (Approved/Rejected)
- Email sending confirmations
- Generated document URLs

### 4. Verify Data Storage
**View Table Storage:**
1. Install [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/)
2. Connect to "Local Emulator"
3. Check `MortgageApplications` table - see applications with status

**View Blob Storage:**
1. In Storage Explorer, expand "Blob Containers"
2. Open `mortgage-offers` container - see generated mortgage docs
3. Open `house-images` container - see uploaded house images

---

## Expected Results

✅ **Listings API** returns 2 houses  
✅ **Mortgage API** creates applications with "Pending" status  
✅ **Process Function** approves/rejects applications (check logs)  
✅ **Send Function** logs email notifications  
✅ **Storage Explorer** shows applications in Table Storage and documents in Blob Storage

---

## Troubleshooting

**Services won't start:**
```powershell
# Check if ports are already in use
netstat -ano | findstr ":5001 :5002 :7071"

# Kill the processes if found
taskkill /PID <ProcessId> /F
```

**Azurite errors:**
```powershell
# Stop all Azurite instances
Get-Process azurite | Stop-Process -Force

# Start fresh
azurite --silent --location c:\azurite
```

**Build errors:**
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

---

**Course:** Minor Cloud Development - IT Inholland Year 4  
**Date:** 27 October 2025
