This is a full list of all event IDs. Use it to quickly check basic errors. The first number specifies the event catagory. The second two numbers specify the event that has happened. The third and foruth number specify any additinal (sub-)events that happened.
E.g. the event 01010

General (0xxyy)
---------------
* 1: Missing Library
  * 0: Could not resolve missing Library
  * 1: Missing library resolved

Compiler (3xxyy)
----------------
* 0: Compiler Message
  * 0: Starting compilation process
  * 1: Finished compilation process
  * 2: Compiling file
  * 3: Finished compiling file
* 1: Compiler Error
  * 0: No script files detected
  * 1: No such folder exists
  * 2: Failed to compile file
  * 3: Script error message (tells where the script contains errors)
