
$delimiter = " "

function Parse-Recur($fileName)
{
    Get-Content $fileName | ForEach-Object {
        Parse-Line $_ 
    }
}

function Parse-Line($line)
{
    $frequency = Get-Token($line)
    $remainder = Get-Remainder($line)

    switch ($frequency)
    {
        "daily" { $task = Process-Daily($remainder) }
        "weekly" { $task = Process-Weekly($remainder) }
        default { throw "Invalid frequency." }
    }

    $task
}

function Process-Daily($task)
{
    # For a daily task the remainder is the task itself
    Process-Task($task)
}

function Process-Weekly($line)
{
    $remainder = $line

    $t = (Get-Token($line) -as [int])
    $v = $t -is [int]
    Write-Host "t: $t"
    Write-Host "v: $v"
    # Determine if there is a lead time
    if ($t -is [int])
    #if (Get-Token($line) -is [int])
    {
        $leadTime = Get-Token($line)
        $leadPeriod = Get-Token(Get-Remainder($line))
        $remainder = Get-Remainder(Get-Remainder(Get-Remainder($line)))
        Write-Host "Lead Time: $leadTime $leadPeriod"
    }

    $targetDay = Get-Token($remainder)
    Write-Host "Target Day: $targetDay"
    $task = Get-Remainder($remainder)
}

function Process-Task($task)
{
    $task
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
