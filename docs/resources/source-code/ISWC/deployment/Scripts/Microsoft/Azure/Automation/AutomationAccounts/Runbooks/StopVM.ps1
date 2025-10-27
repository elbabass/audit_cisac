Param
(
  [Parameter (Mandatory=$true)]
  [String] $VMName,

  [Parameter (Mandatory=$true)]
  [String] $RGName
)

$Conn = Get-AutomationConnection -Name AzureRunAsConnection
Add-AzureRMAccount -ServicePrincipal -Tenant $Conn.TenantID -ApplicationId $Conn.ApplicationID -CertificateThumbprint $Conn.CertificateThumbprint

Stop-AzureRmVM -Name $VMName -ResourceGroupName $RGName -Force