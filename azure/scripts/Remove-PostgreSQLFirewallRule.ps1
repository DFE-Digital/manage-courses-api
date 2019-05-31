<#
    .SYNOPSIS
    Remove an existing Azure PostgreSQL server firewall rule

    .DESCRIPTION
    Remove an existing Azure PostgreSQL server firewall rule

    .PARAMETER ResourceGroupName
    The resource group where the Azure PostgreSQL server resides

    .PARAMETER ServerName
    The Azure PostgreSQL server name

    .PARAMETER FirewallRuleName
    The name given to the firewall rule

    .EXAMPLE
    $TestParams = @{
        ResourceGroupName = "ServerResourceGroup"
        ServerName = $ServerName
        FirewallRuleName = "agent-ip"
    }
    ./Remove-PostgreSQLFirewallRule.ps1 @TestParams
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
    [String]$FirewallRuleName
)

try {
    
    # Build resource name for firewall rule
    $ResourceName = "$ServerName/$FirewallRuleName"

    # Build object for PS command
    $FirewallProperties = @{
        ResourceGroupName = $ResourceGroupName
        ResourceType      = 'Microsoft.DBforPostgreSQL/servers/firewallRules'
        ResourceName      = $ResourceName
        ApiVersion        = '2017-12-01'
    }
    
    Remove-AzResource @FirewallProperties -Force
}
catch {
    throw "$_"
}
