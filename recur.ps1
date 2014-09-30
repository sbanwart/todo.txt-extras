
$delimiter = " "

function Parse-Recur($fileName)
{
    Get-Content $fileName | ForEach-Object {
        Parse-Line $_ 
    }
}

function Parse-Line($line)
{
    #$line.split() | ForEach-Object { Write-Host $_ }
    $t = Get-Token($line)
    $r = Get-Remainder($line)
    Write-Host "|$t|"
    Write-Host "|$r|"
}

function Get-Token($line)
{
    $line.Substring(0, $line.IndexOf($delimiter))
}

function Get-Remainder($line)
{
    $line.Substring($line.IndexOf($delimiter) + 1)
}

$file = '.\recur.txt'

Parse-Recur $file
