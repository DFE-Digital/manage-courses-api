<#
    .SYNOPSIS
    Add one or more IP addresses to an Azure PostgreSQL server firewall whitelist

    .DESCRIPTION
    Add one or more IP addresses to an Azure PostgreSQL server firewall whitelist.
    If none is provided then use the IP of the current process by making a request to
    a web service which returns json that includes the request IP.

    .PARAMETER ResourceGroupName
    The resource group where the Azure PostgreSQL server resides

    .PARAMETER ServerName
    The Azure PostgreSQL server name

    .PARAMETER FirewallRuleName
    The name given to the firewall rule

    .PARAMETER IpAddresses
    Array of IP addresses

    .EXAMPLE
    $TestParams = @{
        ResourceGroupName = "ServerResourceGroup"
        ServerName = $ServerName
        FirewallRuleName = "agent-ip"
        IpAddresses = "123.123.123.123"
    }
    ./Add-PostgreSQLFirewallRule.ps1 @TestParams

    .EXAMPLE
    $TestParams = @{
        ResourceGroupName = "ServerResourceGroup"
        ServerName = $ServerName
        FirewallRuleName = "agent-ip"
    }
    ./Add-PostgreSQLFirewallRule.ps1 @TestParams
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [String]$ResourceGroupName,
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [String]$ServerName,
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [String]$FirewallRuleName,
    [Parameter(Mandatory = $false)]
    [Array]$IpAddresses
)

try {
    function New-PostgreSQLFirewallRule {
        param(
            $ResourceName,
            $ProcessIP
        )

        # Get the server location and build firewall resource name
        $ServerLocation = (Get-AzResource -ResourceGroupName $ResourceGroupName -ResourceType Microsoft.DBforPostgreSQL/servers -ResourceName $ServerName -ApiVersion 2017-12-01).Location

        # Build object for PS command
        $FirewallRuleProperties = @{
            Location          = $ServerLocation
            ResourceGroupName = $ResourceGroupName
            ResourceType      = 'Microsoft.DBforPostgreSQL/servers/firewallRules'
            ApiVersion        = '2017-12-01'
            ResourceName      = $ResourceName
            PropertyObject    = @{
                startIpAddress = "$ProcessIP";
                endIpAddress   = "$ProcessIP"
            }
        }

        New-AzResource @FirewallRuleProperties -Force -Verbose
    }

    if ($PSBoundParameters.ContainsKey("IpAddresses")) {
        foreach ($Ip in $IpAddresses) {
            $ResourceName = ("$ServerName/$FirewallRuleName-$Ip").Replace(".", "_")

            New-PostgreSQLFirewallRule -ResourceName $ResourceName -ProcessIP $Ip
        }
    }
    else {
        # Get the IP used by the current process/server in this instance our hosted agent
        $ProcessIP = Invoke-RestMethod https://ipinfo.io/json | Select-Object -ExpandProperty ip
        $ResourceName = "$ServerName/$FirewallRuleName"

        New-PostgreSQLFirewallRule -ResourceName $ResourceName -ProcessIP $ProcessIP
    }
}
catch {
    throw "$_"
}
