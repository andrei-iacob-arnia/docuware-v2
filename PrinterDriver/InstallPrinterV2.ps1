# ------------------ CONFIG ------------------
$printerName = "Virtual PDF Printer Queue Name"
$driverName  = "Microsoft XPS Document Writer v4"
$portName    = "PORTPROMPT:"
# -------------------------------------------

Write-Host "[INFO] Removing existing printer (if it exists)..."

if (Get-Printer -Name $printerName -ErrorAction SilentlyContinue) {
    Remove-Printer -Name $printerName
    Write-Host "[INFO] Removed existing printer: $printerName"
}

# Add the printer queue using the built-in driver
Write-Host "[INFO] Adding printer using built-in driver..."
Add-Printer -Name $printerName -DriverName $driverName -PortName $portName

Write-Host "[OK] Printer '$printerName' created successfully."
Write-Host "[INFO] PSA should now trigger when this printer is used."
