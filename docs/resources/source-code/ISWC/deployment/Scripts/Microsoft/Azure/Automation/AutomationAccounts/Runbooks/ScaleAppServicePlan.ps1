Param
(
  [Parameter (Mandatory=$true)]
  [String] $RGName,

  [Parameter (Mandatory=$true)]
  [String] $AppServicePlanName,

  [Parameter (Mandatory=$true)]
  [String] $Tier,

  [Parameter (Mandatory=$true)]
  [String] $WorkerSize
)

$Conn = Get-AutomationConnection -Name AzureRunAsConnection
Add-AzureRMAccount -ServicePrincipal -Tenant $Conn.TenantID -ApplicationId $Conn.ApplicationID -CertificateThumbprint $Conn.CertificateThumbprint

Set-AzureRmAppServicePlan -ResourceGroupName $RGName -Name $AppServicePlanName -Tier $Tier -WorkerSize $WorkerSize