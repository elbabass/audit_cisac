Param
(
  [Parameter (Mandatory=$true)]
  [String] $RGName,

  [Parameter (Mandatory=$true)]
  [String] $AccountName,

  [Parameter (Mandatory=$true)]
  [String] $DBName,

  [Parameter (Mandatory=$true)]
  [Int] $Throughput
)

$databaseResourceName = $AccountName + "/sql/" + $DBName + "/throughput"

$Conn = Get-AutomationConnection -Name AzureRunAsConnection
Add-AzureRMAccount -ServicePrincipal -Tenant $Conn.TenantID -ApplicationId $Conn.ApplicationID -CertificateThumbprint $Conn.CertificateThumbprint

$properties = @{
    "resource"=@{"throughput"=$Throughput}
}

Set-AzureRmResource -ResourceType "Microsoft.DocumentDb/databaseAccounts/apis/databases/settings" -ApiVersion "2015-04-08" -ResourceGroupName $RGName -Name $databaseResourceName -PropertyObject $properties -Force