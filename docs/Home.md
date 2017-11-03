**Project Description**
WiiCursor is a program that enables you to control the mouse cursor with a Wiimote. The buttons on the Wiimote can be configured to fire different mouse and keyboard events.  

WiiCursor is written and conceived by Jesper Niedermann

# General Idea

If you want your Wiimote to control a specific program on your PC, e.g. PowerPoint, Word or Excel, you have to combine a lot of googling with luck to find exactly what you are looking for. 

_WiiCursor_ solves this problem by making each button on your Wiimote configurable so that it can simulate a KeyPress or a Mouse Click anywhere on your screen. Using this approach you can in theory control any program with your Wiimote.

The project includes an example program to control in this way, called [DrawCursor](DrawCursor). The Combination of DrawCursor and WiiCursor enables you to draw on the desktop with the Wiimote.

# Requirements

* Windows XP - it might work on Windows Vista but I haven't tested it.
* .NET 3.5
* Some Bluetooth stack (not all will work)

# Usage
_WiiCursor_ uses the WiiMoteLib by Brian Peek, which can be found at [http://www.codeplex.com/WiiMoteLib](http://www.codeplex.com/WiiMoteLib).

When you have installed _WiiCursor_, using the current[release](release_21245), the first thing you have to do, is to connect your WiiMote using Bluetooth. Brian peek shows how in this article  [http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx](http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx) under the heading “Getting connected”.

Secondly you should place a Wii Sensor bar under your screen. I recommend that you buy a battery driven sensor bar for this. I have bought one from [Logic3](http://www.logic3.com/details/?prod=341) which works fine.  You can also build one yourself as shown [here](http://doctabu.livejournal.com/64758.html). 

As the least appealing alternative you can use the one that comes with the Wii, but this means that you have to have it connected to your Wii and your Wii has to be turned on, to provide the power. This way your Wii is used as a rather weird looking power adapter.

When you start _WiiCursor_ the only thing that happens is that the _WiiCursor_ Icon appears in the System Tray in the lower right corner. If the Sensor bar is turned on, you immediately get to control the mouse cursor with the Wiimote if it is connected. If you press a button on the Wiimote what will happen depends on the chosen configuration.
 
# Configuration

There are some default configurations included. And you can yourself add more. There is a default one for Powerpoint presentations, and a default one for [DrawCursor](DrawCursor) which is a demo program for the _WiiCursor_ that I have included in this project for fun. It is a program for drawing on the desktop using the mouse (or the Wiimote in this case). [DrawCursor](DrawCursor) can be used standalone.

In the Installation directory, typically C:\Program Files\JERN\WiiCursor\ there is a folder called WCXML. In this directory there is a Schema file “WCConfiguration.xsd” and some XML files. The XML files have to comply with the Schema. For each XML file in the WCXML folder a submenu is added to the Configuration Menu of the WiiCursor Tray Icon. An example file for PowerPoint presentations is shown here:

{code:xml}
<?xml version="1.0" encoding="utf-8" ?>
<WCConfiguration>
  <Name>Powerpoint Support</Name>
  <Default>false</Default>
  <ConnectionWaitTime>5000</ConnectionWaitTime>
  <ConnectionRetries>50</ConnectionRetries>
  <Keys>
    <A>MouseLeftButtonDown</A>
    <B>KeyEscape</B>
    <Plus>MouseLeftClick</Plus>
    <Minus>KeyArrowUp</Minus>
    <Home>KeyF5</Home>
    <One>Exit</One>
    <Two>Pause</Two>
    <Up></Up>
    <Down></Down>
    <Left></Left>
    <Right>KeyF7</Right>
  </Keys>
</WCConfiguration>
{code:xml}

As you can see the XML file has 4 basic settings and a 5th which contains an entry for each button on the Wiimote.
# Name: The name of the Submenu.
# Default: Denotes if the file contains the default configuration of _WiiCursor_. I.e. the one chosen by the system when _WiiCursor_starts. One and only one file has to be default. This is actually not validated by the program. If the Default setting is not present in the file, it means the same as if Default is set to false.
# ConnectionWaitTime: If the Wiimote is not connected via Bluetooth the _WiCursor_program will fail to connect to the Wiimote when started. This setting denotes how many milliseconds the program will wait before trying to connect again.
# ConnectionRetries: This indicates how many times the program will try to connect to the wiimote before giving up. I.e. with the above settings the program will try for 50 x 5000 milliseconds which equals 250000 milliseconds, or a bit more than 4 minutes.

Each button on the Wiimote can be configured to fire an event, e.g. simulating that a mousebutton or a key was pressed. Each button can in fact fire more than one event. You can seperate events by a + sign. So <B>keyEscape+Exit</B> means that pressing _B_ on the wiimote will send the Escape Key to the Desktop, and then terminate the program.

In the table below I explain all the events currently available. I plan to add more in the [future](future). All Key and Mouse simulations will happen at the cursor location.

You should almost always configure one of the buttons to fire the "Pause" event. This is because "Pause" will allow you to interact normally with the desktop in case of panic.

|| Name of event || Explanation ||
| 0-255 | Numerical  value corresponding to an ASCII code. E.g 65 means “A” |
| Pause | This event temporarily stops the Wiimote from interacting with the Desktop. This means that you get a chance to use the normal mouse. Triggering Pause once more will make the Wiimote work again. |
| Exit | _Exit_ terminates the WiiCursor program. |
| KeyEscape | Sends the Escape key to the desktop. |
| KeyDelete | Sends the Delete key to the desktop. |
| KeyF1 | Sends the F1 key to the desktop. |
| KeyF2 | Sends the F2 key to the desktop. |
| KeyF3 | Sends the F3 key to the desktop. |
| KeyF4 | Sends the F4 key to the desktop. |
| KeyF5 | Sends the F5 key to the desktop. |
| KeyF6 | Sends the F6 key to the desktop. |
| KeyF7 | Sends the F7 key to the desktop. |
| KeyF8 | Sends the F8 key to the desktop. |
| KeyF9 | Sends the F9 key to the desktop. |
| KeyF10 | Sends the F10 key to the desktop. |
| KeyF11 | Sends the F11 key to the desktop. |
| KeyF12 | Sends the F12 key to the desktop. |
| KeyArrowUp | Sends the arrow up key to the desktop. |
| KeyArrowDown | Sends the arrow down key to the desktop. |
| KeyArrowLeft | Sends the arrow right key to the desktop. |
| KeyArrowRight | Sends the arrow left key to the desktop. |
| MouseLeftButtonDown | Sends a mouse left button down event to the desktop. The Key stays “down” until the key on the Wiimote is released. |
| MouseLeftClick | Sends a mouse left click to the desktop. |
| MouseLeftDoubleClick | Sends a mouse left double-click to the desktop (can also be simulated by pressing a button that activates MouseLeftClick twice). |
| MouseRightButtonDown | Sends a mouse right button down event to the desktop. The key stays “down” until the key on the Wiimote is released. |
| MouseRightClick | Sends a mouse right click to the desktop. |

_WiCursor_comes with three examples of XML configuration. The most important is WCForDrawCursor.xml for drawing on the screen with [DrawCursor](DrawCursor) and the one shown above, WCPowerPoint.xml, for presenting powerpoints.

# The Amazing Logo

If you wonder what inspired the _WiiCursor_ Logo, just think about the initials of Wii Cursor.

# Further Reading

This [document](Home_ReadMeWiiCursor.pdf) recaps the information above in a printable form, and this [powerpoint](Home_WiiCursor.pptx) explains, in a cartoonish way, how the Wiimote works with the sensorbar and bluetooth.