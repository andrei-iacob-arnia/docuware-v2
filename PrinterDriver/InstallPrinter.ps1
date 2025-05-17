# ------------------ CONFIG ------------------
$infPath        = "C:\Work\DocuWare\docuware-v2\PrinterDriver\VirtualPdfPrinter.inf"
$printerDriverDir = Split-Path $infPath
$catOutputPath  = Join-Path $printerDriverDir "VirtualPdfPrinter.cat"
$pfxPath        = "C:\Work\DocuWare\docuware-v2\Certificate\VirtualPdfPrinterPSA.pfx"
$pfxPassword    = "MySecurePassword123!"  # Replace with your actual password

# Full paths to tools
$inf2catPath    = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\inf2cat.exe"
$signtoolPath   = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\signtool.exe"

$printerName    = "Virtual PDF Printer"
$driverName     = "Virtual PDF Printer" #"Microsoft XPS Document Writer v4"
$portName       = "PORTPROMPT:"
# -------------------------------------------

# 0. Cleanup existing printer and driver
Write-Host "Removing existing printer and driver if they exist..." -ForegroundColor Yellow

# Remove printer queue if it exists
if (Get-Printer -Name $printerName -ErrorAction SilentlyContinue) {
    Remove-Printer -Name $printerName
    Write-Host "Removed existing printer: $printerName"
}

# Find installed driver matching printer name
$driverPublishedName = $null
$drivers = pnputil /enum-drivers | Out-String -Stream
$currentBlock = @()
foreach ($line in $drivers) {
    if ($line -match "^Published Name\s*:\s*(oem\d+\.inf)") {
        if ($currentBlock.Count -gt 0) {
            $blockText = $currentBlock -join "`n"
            if ($blockText -like "*$printerName*") {
                $driverPublishedName = $currentName
                break
            }
            $currentBlock.Clear()
        }
        $currentName = $Matches[1]
    }
    $currentBlock += $line
}
if (-not $driverPublishedName -and $currentBlock.Count -gt 0) {
    $blockText = $currentBlock -join "`n"
    if ($blockText -like "*$printerName*") {
        $driverPublishedName = $currentName
    }
}

if ($driverPublishedName) {
    pnputil /delete-driver $driverPublishedName /uninstall /force
    Write-Host "Removed driver package: $driverPublishedName"
}

# 1. Generate .cat file using inf2cat
Write-Host "Generating .cat file..." -ForegroundColor Cyan
Push-Location $printerDriverDir
& $inf2catPath /driver:. /os:10_X64,10_X86 /uselocaltime
Pop-Location

if (!(Test-Path $catOutputPath)) {
    Write-Error "❌ Failed to generate CAT file."
    exit 1
}

# 2. Sign the .cat file using signtool
Write-Host "Signing .cat file..." -ForegroundColor Cyan
& $signtoolPath sign `
    /f $pfxPath `
    /p $pfxPassword `
    /tr http://timestamp.digicert.com `
    /td sha256 `
    /fd sha256 `
    $catOutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ signtool failed."
    exit 1
}

# 3. Install the printer driver
Write-Host "Installing driver package..." -ForegroundColor Cyan
pnputil /add-driver $infPath /install

# 3.5 Register driver in print spooler
Add-PrinterDriver -Name $driverName
#Write-Host "`n📋 Registering driver in print spooler..." -ForegroundColor Cyan
#try {
#    Add-PrinterDriver -Name $driverName
#    Write-Host "✅ Driver '$driverName' registered successfully."
#} catch {
#    Write-Warning "⚠️  Driver may already be registered or registration failed: $_"
#}

# 4. Add the actual printer queue
Write-Host "Creating printer queue..." -ForegroundColor Cyan
if (-not (Get-Printer -Name $printerName -ErrorAction SilentlyContinue)) {
    Add-Printer -Name $printerName -DriverName $driverName -PortName $portName
    Write-Host "✅ Printer '$printerName' created."
}

Write-Host "`n✅ All done! You should now see '$printerName' in the print dialog." -ForegroundColor Green
