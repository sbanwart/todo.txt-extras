todo.txt-extras
===============

An F# script for adding recurring tasks to a Todo.txt (http://todotxt.com/) file.

# Prerequisites

The script was written using F# and .NET version 4.5. It may run on other versions but I haven't tested it on anything else.

# Usage

The script is run using the F# interactive environment fsi.exe.

`fsi.exe recur.fsx <path to todo.txt> <path to recur.txt>`

To automate the script, set it up to run as a scheduled task in Windows.

# Configuration

Recurring tasks are kept in their own text file using a similar format the normal todo.txt file.

Recurring tasks are entered using the form:

`<frequency> [priority] [creation date] [@context] [+project] <task description>`

Example:

`daily (A) @home +chores Take out the trash`

## Frequency

Currently the script supports six frequency types, *daily*, *weekly*, *weekdays*, *weekends*, *monthly* and *yearly*.

The *daily* frequency doesn't take any additional parameters and tasks marked with it will be added to todo.txt every day.

The *weekly* frequency takes a day of the week (sunday, monday, tuesday, etc.) as a parameter. Tasks marked with this frequency will be added once a week on the designated day.

The *weekdays* frequency doesn't take any additional parameters adn tasks marked with it will be added to todo.txt every weekday. (Monday-Friday)

The *weekends* frequency is identical to *weekdays* except it adds tasks on the weekend. (Saturday and Sunday)

The *monthly* frequency takes two parameters. The first is a modifier describing the week on the month. Valid values are *first*, *second*, *third*, *fourth* and *last*. The second parameter is a day of the week. (sunday, monday, tuesday, etc.)

Example:

`monthly first saturday (B) @home +finances Balance checkbook`

The *yearly* frequency takes two parameters. The first is a month (january, february, march, etc.) and the second is the numeric day of the month.  (1, 2, 3, 28, 29, etc.)

Example:

`yearly september 15 (A) @home +finances Pay quarterly taxes`

## Dates

Dates can be entered using a special macro that will be replaced with the current date. The macro also supports adding days to the current date.

Example:

Current date: `%date%`

Current date + three days: `%date+3%`

As used in a task:

`weekly monday (A) %date% @home +chores due:%date+2% Place trash can on the curb`

# Limitations

This script has been put throught the rigorous testing process known as "Works on My Machine." Invalid recurring task definitions will either be silently ignored or cause the script to crash.

# Next Steps

* Clean up the code and improve performance.
* Improve error handling
