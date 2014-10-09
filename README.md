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

Currently the script supports two frequency types, *daily* and *weekly*

The *daily* frequency doesn't take any additional parameters and tasks marked with it will be added to todo.txt every day.

The *weekly* frequency takes a day of the week (sunday, monday, tuesday, etc.) as a parameter. Tasks marked with this frequency will be added once a week on the designated day.

## Dates

Dates can be entered using a special macro that will be replaced with the current date. The macro also supports adding days to the current date.

Example:

Current date: `%date%`

Current date + three days: `%date+3%`

As used in a task:

`weekly monday (A) %date% @home +chores due:%date+2% Place trash can on the curb`

# Next Steps

* Add a monthly frequency with a fuzzy day parameter. ex: monthly last saturday, monthly second tuesday
* Add the ability to specify an absolute date for infrequent tasks. This is intended for tasks that only occur quarterly or yearly.
