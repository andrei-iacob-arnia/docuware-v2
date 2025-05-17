# Define certificate path and password
$certFolder = "C:\Work\DocuWareNew\Certificate"
$certName = "VirtualPdfPrinterPSA"
$pfxPath = Join-Path $certFolder "$certName.pfx"
$certPassword = ConvertTo-SecureString -String "MySecurePassword123!" -Force -AsPlainText

# Create folder if it doesn't exist
if (-not (Test-Path $certFolder)) {
    New-Item -Path $certFolder -ItemType Directory | Out-Null
}

# Create self-signed certificate in the CurrentUser store
$cert = New-SelfSignedCertificate -Type CodeSigningCert `
    -Subject "CN=$certName" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyExportPolicy Exportable `
    -KeySpec Signature `
    -NotAfter (Get-Date).AddYears(5)

# Export to .pfx
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $certPassword

Write-Output "`n✅ Certificate created successfully at:`n$pfxPath"
