# ------------------ CONFIG ------------------
$infPath        = "C:\Work\DocuWareNew\PrinterDriver\VirtualPdfPrinter.inf"
$printerDriverDir = Split-Path $infPath
$catOutputPath  = Join-Path $printerDriverDir "VirtualPdfPrinter.cat"
$pfxPath        = "C:\Work\DocuWareNew\Certificate\VirtualPdfPrinterPSA.pfx"
$pfxPassword    = "MySecurePassword123!"  # Replace with your actual password

# Full paths to tools
$inf2catPath    = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\inf2cat.exe"
$signtoolPath   = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x86\signtool.exe"

$printerName    = "Virtual PDF Printer"
$driverName     = "Microsoft XPS Document Writer v4"
$portName       = "PORTPROMPT:"
# -------------------------------------------

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

# 4. Add the actual printer queue
Write-Host "Creating printer queue..." -ForegroundColor Cyan

if (-not (Get-Printer -Name $printerName -ErrorAction SilentlyContinue)) {
    Add-Printer -Name $printerName -DriverName $driverName -PortName $portName
    Write-Host "✅ Printer '$printerName' created."
}

Write-Host "`n✅ All done! You should now see '$printerName' in the print dialog." -ForegroundColor Green
