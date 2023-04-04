-------------------------------------------------------------------------------

MVTec Software Manager

-------------------------------------------------------------------------------

version: 1.2.1.167

-------------------------------------------------------------------------------

Quick Start

-------------------------------------------------------------------------------

-   Run the accompanying binary som (MVTec Software Manager).

-   Select the page "Available".

-   To install a software product, click its Install button.
    This will pre-select the recommended package selection. Alternatively,
    click the button Select Packages to select a custom set of packages from
    scratch.

-   Click the button Apply to start the installation.

-------------------------------------------------------------------------------

About the MVTec Software Manager

-------------------------------------------------------------------------------

The MVTec Software Manager (SOM) is an installation manager for software
packages. It starts a local web server, provides access to a remote catalog of
products, and supports the following features:

-   downloading, verifying, and installing packages

-   switching between different package versions (upgrade/downgrade)

-   offline operation once the packages have been downloaded

-   command-line operation

-   launch pad

Additionally, SOM detects MVTec products that have been installed by other
means, and allows to switch between different HALCON versions.

-------------------------------------------------------------------------------

Installing SOM

-------------------------------------------------------------------------------

SOM is a self-contained application and requires no installation at all.
Nonetheless, it will offer to install itself upon startup, and add both a
desktop icon and a start menu entry for SOM. If you choose to perform the
installation, you may delete the original SOM binary as soon as the current
session is exited. Otherwise, just close the "Welcome" dialog, and continue to
use SOM without installation.

If another version of SOM is already installed, that version will be upgraded.

Once installed, SOM can manage itself from the page "Installed". However, if
you completely uninstall SOM, it can only be re-installed as long as the
current session is still running (or by running the original binary again).

-------------------------------------------------------------------------------

Running SOM

-------------------------------------------------------------------------------

In the majority of cases SOM should only be run with the current users standard
permissions. Since SOM version 1.2, it is in general no longer required to run
SOM in system mode, i.e. with admin privileges, to install products for all
users. For more information on how SOM handles admin privileges please refer to
the section Admin privileges.

The only exception when it might still be required to start SOM with admin
privileges is, when SOM is running in headless mode or when using the command
line interface. Please see the corresponding sections Headless operation and
Command line usage for more information.

When starting SOM a web server is started and your default browser should open
the starting page automatically. If your browser does not come up, start "MVTec
Software Manager CLI", enter som, and use the displayed address in any
HTML5-compliant browser on your system.

On systems, where desktop or menu shortcuts are not available (e.g., embedded
systems), SOM can be started from the command line:

Linux

    $HOME/MVTec/SoftwareManager/som     # "MVTec Software Manager"
    $HOME/MVTec/SoftwareManager/som cli # "MVTec Software Manager CLI"

Windows

    %LOCALAPPDATA%\Programs\MVTec\SoftwareManager\som.exe     # "MVTec Software Manager"
    %LOCALAPPDATA%\Programs\MVTec\SoftwareManager\som.exe cli # "MVTec Software Manager CLI"

-------------------------------------------------------------------------------

Headless operation

-------------------------------------------------------------------------------

On systems that lack a graphical environment, or are simply not powerful enough
to run a full-blown HTML5-compliant browser, SOM can either be controlled from
the command line as described in the section Command line usage, or from the
browser of another machine. Suppose SOM is running on a headless machine
"tiny", and you want to control it from a more capable machine "bigshot". Login
on "tiny", and issue the following command to start the SOM server:

    $HOME/MVTec/SoftwareManager/som --hostname tiny -n # start server on tiny

The command line option "--hostname tiny" makes the server listen to other
machines on the network, while "-n" prevents SOM from attempting to start up a
browser. Enter the address displayed on the terminal of "tiny" into a browser
running on "bigshot".

The headless mode has the following limitations:

1.  The running SOM will not be able to gain admin privileges using Windows UAC
    or polkit. Therefore, if you want SOM to perform actions that require admin
    privileges in headless mode, you will need to start SOM with these
    privileges. On Linux based systems this can be done using sudo, and on
    Windows systems use "Run as Administrator" when starting SOM.

2.  Downloading packages requires a valid login, which in turn is only
    supported via the web interface and when the browser is running on the same
    machine as SOM. As a workaround, first log in in a SOM session on another
    machine (e.g., "bigshot"), and copy the login token to "tiny" as described
    in the Login section below.

-------------------------------------------------------------------------------

Windows Defender Firewall warning

-------------------------------------------------------------------------------

This section only applies to SOM running on a windows operating system.

Upon the first start of SOM, the following warning may be issued by Windows
Defender:

    Windows Defender Firewall has blocked some features of this app

You can safely click "Cancel" unless you intend to access the SOM web server
from another machine in your network. In that case, click "Allow access". For
security reasons, SOM only listens to localhost (the machine it is running on)
by default. To accept connections from other machines, replace "localhost" with
your machine name in the settings of SOM.

-------------------------------------------------------------------------------

Startup problems

-------------------------------------------------------------------------------

In case SOM refuses to start at all, try to run it from the command line:

-   Start "MVTec Software Manager CLI".

-   Enter the command som.

    MVTec Software Manager 1.2.1.167
    {rev-commit}
    mode: user, user: foo, os: windows
    listening on http://localhost:{CFG-port-default}

The above is the expected output from a successful SOM startup. It shows the
tool name, the version number, the commit on which this version is based, the
mode (user or system), username, operating system, and the URL to access the
current session. Any other output may indicate a problem during startup.

By default, SOM listens on port {CFG-port-default} for incoming HTTP
connections. If this port is in use on your system, use the command line option
--port, and configure another default port in the settings.

-------------------------------------------------------------------------------

Connecting through an HTTP proxy

-------------------------------------------------------------------------------

On systems that do not support a direct connection to the internet, SOM can be
instructed to connect through an HTTP proxy. Open the settings, enter your
proxy into the "HTTP Proxy" field, and confirm your change with Enter. Note
that SOM has to be restarted to actually use the proxy.

If your proxy requires authorization with a username and password, the proxy
can be prefixed with username:password@, e.g., foo:secret@server:port.
Specifying the proxy this way will keep your password stored in the settings
file. If this is undesirable, you can start a SOM session with a temporary
proxy setting:

    som -P foo:secret@server:port

You can also specify the proxy in an environment variable HTTP_PROXY, which is
used if the "HTTP Proxy" field in the settings of SOM is left empty.

For proxies with NTLM authentication, we suggest to install a forwarding proxy
on your machine, and specify that proxy in SOM. One of the following
third-party tools should work in this scenario:

-   Cntlm (proxy)

-   NTLMAPS (proxy)

-   px (proxy)

               specify this proxy in SOM
                          |
                          |           NTLM
    +------+      +----------------+   |   +------------+      +---------+
    | SOM  | <--> | localhost:3128 | <---> | proxy:3128 | <--> | Catalog |
    +------+      +----------------+       +------------+      +---------+
     server        forwarding proxy        corporate proxy      Internet

-------------------------------------------------------------------------------

Usage

-------------------------------------------------------------------------------

Upon server startup, SOM is controlled entirely from your browser. The
different pages that can be accessed via tabs in the header of each page are
described in the following sections.

Closing the browser window does not terminate the server. Thus, you can start
an installation, close the browser window, and SOM will work ahead in the
background.

-------------------------------------------------------------------------------

Admin privileges

-------------------------------------------------------------------------------

When installing packages, SOM might require admin privileges when executing the
install-hook of packages, or when extracting files to a location where the user
that started SOM does not have write permission. For tasks that require admin
privileges, SOM will try to gain these privileges by using the respective
functionality of the operating system, and it will drop these privileges as
soon as they are no longer required. On Windows systems, this will happen
through the Windows User Account Control and on Linux based systems by using
polkit.

SOM itself will never ask for any of your passwords for your local system.
Instead, it uses the operating system’s capability to request elevated rights
(polkit/UAC).

Whenever possible, actions that require admin privileges will be marked in the
frontend with the following symbol:

[admin]

    Signifies that the action will require admin privileges.

-------------------------------------------------------------------------------

Login

-------------------------------------------------------------------------------

To download products with SOM, you need to log in with your "MVLogin" account.
In order to log in, click the button MVLogin. You will also be redirected to
the login when a download is started, and you were not already logged in. After
the login, the consent for retrieving user information and downloading MVTec
products via SOM has to be given.

You can only log in via the web interface. Therefore, the command line mode can
only be used after a successful login via the web interface. Offline
installations are not affected and can still be used without login, as well as
installations of packages that have already been downloaded and are available
in the local repository.

Downloading products via SOM on a remote machine without the option to open a
browser on the device itself requires a workaround for the login. First,
execute SOM on your local machine and perform the login there. Afterwards, copy
the login token located in the SOM configuration folder (windows:
%APPDATA%\MVTec\SoftwareManager\som.login, linux: $HOME/.som/som.login) from
your local machine to the corresponding directory on the remote machine. If a
SOM instance is already running on the remote machine it needs to be restarted.
Now, you can run SOM on the remote machine using the login session created in
the first step.

-------------------------------------------------------------------------------

Installed

-------------------------------------------------------------------------------

This page lists installed products. Both products installed with SOM and
products installed with other MVTec installers are listed here. Depending on
the actual installation, additional buttons for the execution of applications
may appear for each product.

[bell]

    Signifies available updates for the corresponding product. By clicking on
    the icon, the "Manage packages" dialog is opened with the new packages
    versions selected.

[threedots]

    Customize product installed with SOM. Using this pop-up menu, the installed
    product can be updated (if new packages are available) or uninstalled
    completely. Using the entry "Manage packages" the installation may be
    customized, i.e., individual packages may be added or removed.

-------------------------------------------------------------------------------

Handling of HALCON versions

-------------------------------------------------------------------------------

This section only applies to windows

If multiple versions of HALCON are installed on your system, the active version
may be switched. Only one HALCON version can be active at any given time, i.e.,
file types are associated with the corresponding HDevelop, the environment
variables HALCONROOT, HALCONARCH, HALCONEXAMPLES, and HALCONIMAGES are set
appropriately, and the PATH variable contains the directory
%HALCONROOT%\bin\%HALCONARCH%.

The mode SOM is running in determines the type of environment variables being
set. In user mode, the environment variables for your account are being set. In
system mode, the system-wide environment variables are being set. All
user-specific environment variables take precedence over the system-wide
settings. An important exception to this rule is the variable PATH, whose
system-wide value is prepended to the user-specific one. This is relevant if
HALCON is installed for both all users and your user. The former will always be
found first when searching the PATH, thus hiding the latter.

◉

    Denotes the active HALCON version.

◎

    Denotes a version of HALCON that is not currently active. Click this button
    to register the corresponding HALCON version.

-------------------------------------------------------------------------------

Available

-------------------------------------------------------------------------------

This page provides a list of available products from the configured product
catalog. It is the starting page if no products are installed by SOM. Installed
products move over to the page "Installed". For each product you can choose to
install it just for the current user, or for all users on your system. The
latter will usually require admin privileges, see Admin privileges for details.

-------------------------------------------------------------------------------

Settings [ic settings white 48dp]

-------------------------------------------------------------------------------

The settings page provides options that are stored persistently. Commit your
changes by pressing Enter when the cursor is in a text field, or click Save
changes. Options marked with an asterisk (*) require SOM to be restarted.

Catalog URL

    Specifies the location from where the product catalog is downloaded.

    Default: https://packages.mvtec.com/som.catalog

HTTP proxy

    Specifies the address of an HTTP proxy. Leave at the default (empty) value
    for a direct internet connection.

Host name *

    Specifies the host name SOM listens on for incoming connections. The
    default value “localhost” will only accept local connections. Set this to
    the name of the machine SOM is running on to accept connections from other
    machines in your network. Please note that using the machine name will let
    SOM listen on all the machine’s IP addresses (same as specifying -H
    0.0.0.0). This might be undesired if the machine can be reached publicly.
    Alternatively, use a private IP address of the machine to prevent external
    access while still allowing internal access from other machines on the
    network.

    Default: localhost

Port number *

    Specifies the port number SOM is listening on. If port number 0 is
    specified, SOM will select a free port automatically.

    Default: {CFG-port-default}

The following entries are path names for various file locations used by SOM.
Path names may contain environment variables, which always have to be prefixed
with $. On Windows as an example, use $PROGRAMFILES instead of %PROGRAMFILES%.
Clicking on the folder button opens the corresponding directory in a file
manager.

There are two versions for the location of program files and data files. One is
for installations that are for the current user only, the other is for
installations that are for all users on the system. Once products are
installed, the installation targets in either mode should not be changed, or
havoc may ensue.

-------------------------------------------------------------------------------

Documentation [ic help outline white 48dp]

-------------------------------------------------------------------------------

This page displays the documentation of SOM.

-------------------------------------------------------------------------------

Language

-------------------------------------------------------------------------------

Here, the language of the web interface can be selected. The documentation is
available in English and German.

-------------------------------------------------------------------------------

Shutdown SOM

-------------------------------------------------------------------------------

This button shuts down the SOM web server. Any installations that are still in
progress will be aborted. If the browser window is simply closed, SOM keeps
running in the background, and the session is still accessible by
double-clicking "MVTec Software Manager" again.

-------------------------------------------------------------------------------

Notes about offline usage

-------------------------------------------------------------------------------

Once a particular package has been downloaded, it is kept in a local
repository, and it can be installed without requiring an internet connection.
Depending on the amount of installed software packages, the repository may grow
quite large over time, and the contents can safely be deleted if disk space
becomes an issue. Missing packages will be downloaded again if required unless
they are no longer provided at the remote catalog URL.

-------------------------------------------------------------------------------

Command line options

-------------------------------------------------------------------------------

SOM supports the following command line options (names in parentheses refer to
the corresponding entry in the settings):

-H string | --hostname string (Host name)

    Specifies the host name SOM listens on for incoming connections. The
    default value “localhost” will only accept local connections. Set this to
    the name of the machine SOM is running on to accept connections from other
    machines in your network. Please note that using the machine name will let
    SOM listen on all the machine’s IP addresses (same as specifying -H
    0.0.0.0). This might be undesired if the machine can be reached publicly.
    Alternatively, use -H with a private IP address of the machine to prevent
    external access while still allowing internal access from other machines on
    the network.

    Default: localhost

-P string | --proxy string (HTTP proxy)

    Specifies the address of an HTTP proxy. Leave at the default (empty) value
    for a direct internet connection.

--accept string

    Some packages contain an EULA, that must be accepted before the
    installation is performed. When trying to install such a package from the
    command line, SOM will print the EULA on the console, along with an
    identification hash specific for its contents. To acknowledge the terms of
    the EULA, pass the hash to the parameter “--accept”.

-c string | --catalog string (Catalog URL)

    Specifies the location from where the product catalog is downloaded.

    Default: https://packages.mvtec.com/som.catalog

--data string (Install target (data))

    Specifies the base directory where data packages are installed. In system
    mode, this directory should be writable by all users.

    Default (user mode): $HOME/MVTec
    Default (system mode): /opt/MVTec

-f string | --feed string

    Specifies the URL of a specific feed, i.e., a distinct product. This
    parameter is primarily used for the installation or removal of packages
    from the command line. If no additional command (list, install, or remove)
    is specified on the command line, SOM starts as usual, and opens the
    configuration page of the specified product.

-h | --help

    Print a list of command line options to the console and exit.

-n | --nogui

    If specified, SOM will not attempt to start the default browser.

-p value | --port value (Port number)

    Specifies the port number SOM is listening on. If port number 0 is
    specified, SOM will select a free port automatically.

    Default: {CFG-port-default}

--programs string (Install target (programs))

    Specifies the base directory where program packages are installed.

    Default (user mode): $HOME/MVTec
    Default (system mode): /opt/MVTec

-r string | --repository string (Repository path)

    Specifies the base directory for downloaded packages. You may specify a
    shared directory to supply downloaded packages to multiple users. However,
    it is your responsibility to ensure that two SOM sessions running at the
    same time do not download the same files concurrently. The local repository
    behaves like a browser cache. The contents of this directory can safely be
    deleted. SOM will download missing files again when needed.

    Default (user mode): $HOME/.som/repository
    Default (system mode): $HOME/.som/repository

-v | --verbose

    Output additional diagnostic messages.

-------------------------------------------------------------------------------

Command line usage

-------------------------------------------------------------------------------

In addition to be controlled from your browser, SOM can also be operated from
the command line. This enables script-driven installation and removal of
software packages. To ease access to the command line, double-click the
shortcut "MVTec Software Manager CLI". This opens a terminal that allows to run
the command som without setting the environment variable PATH.

Commands for SOM are issued in the following format:

    som <options> command [<spec>]

The <options> all start with a dash for the shorthand notation (-) and a double
dash for full option names (--). A list is given with:

    som -h

To list the available commands, enter:

    som help

The optional <spec> depends on the actual command. The following sections list
all commands with examples.

-------------------------------------------------------------------------------

som cat

-------------------------------------------------------------------------------

List the contents of the product catalog. Any of the listed feed URLs can be
selected using the option -f <url> in one of the following commands: install,
list, remove.

-------------------------------------------------------------------------------

som sync

-------------------------------------------------------------------------------

List the product catalog just like the command "som cat" but fetch any updates
from the remote catalog beforehand.

-------------------------------------------------------------------------------

som -f <url> list

-------------------------------------------------------------------------------

List both meta and base packages of the product specified with -f.

    som -f https://packages.mvtec.com/halcon/halcon-20.11-progress/halcon-20.11-progress.feed list

For each package, the name, description, and the available versions are listed.
Installed package versions are marked with an asterisk (*) in the command
output.

-------------------------------------------------------------------------------

som -f <url> install

-------------------------------------------------------------------------------

Install a given package (and all its dependencies):

    som -f 20.11-progress install rt

In this example, no complete URL has been given. SOM will automatically select
the first matching feed from the catalog. Also, a package name has been given,
but no version number has been specified. SOM always defaults to the latest
package version. To install an older package version, the version string has to
be specified explicitly (presuming a newer version of package rt existed):

    som -f 20.11-progress install rt 20.11.0.0

The above command will only actually install "HALCON 20.11 Progress" after its
"Software License Agreement" has been accepted. The last line of output repeats
your command line with an additional parameter that signifies your acceptance
of the agreement to SOM. If you want to automate your installations, you have
to figure out the acceptance hash interactively in advance like in this
example.

-------------------------------------------------------------------------------

som list

-------------------------------------------------------------------------------

List installed products. Below each product name, the feed URL and a list of
installed packages is provided. Meta packages are marked "M", and base packages
are marked "B". The remaining fields are package name, package description,
installed version, and installation directory (in that order). The fields are
separated by tabs.

-------------------------------------------------------------------------------

som -f <url> remove

-------------------------------------------------------------------------------

Remove the given package (and all packages that depend on it). In the unlikely
case that you do not want to keep your installed software (or parts of it), the
remove command lets you reverse an accidental installation. If no package is
specified, all installed packages of a product are removed.

    som -f 20.11-progress remove rt

-------------------------------------------------------------------------------

som cli

-------------------------------------------------------------------------------

Starts a terminal/command prompt with the environment variable PATH extended,
so that the command som can be executed without giving the full path. This is
equal to running "MVTec Software Manager CLI".

On unix by default XTerm (/usr/bin/xterm) is started. In order to use a
different terminal program, complement the call with the path of the desired
program:

    som cli /usr/local/st

-------------------------------------------------------------------------------

som remove

-------------------------------------------------------------------------------

Uninstalls all installed SOM packages. Usually, just like other products SOM
should be uninstalled via the browser.

-------------------------------------------------------------------------------

Uninstalling SOM

-------------------------------------------------------------------------------

You can safely uninstall SOM without affecting other products that have been
installed with it. The repository containing any downloaded packages and the
configuration file are not touched by the uninstallation and have to be removed
manually. SOM can be uninstalled in multiple ways:

-------------------------------------------------------------------------------

Using the browser

-------------------------------------------------------------------------------

Start up SOM, make sure the page "Installed" is selected, click the entry
"Uninstall" in the pop-up menu [threedots] next to the entry "Software
Manager", and confirm the uninstallation by clicking on "Apply". Afterwards,
shutdown the server to exit SOM.

-------------------------------------------------------------------------------

Using the command line

-------------------------------------------------------------------------------

Run "MVTec Software Manager CLI", and execute the following command:

    som remove

-------------------------------------------------------------------------------

Windows control panel

-------------------------------------------------------------------------------

This section only applies to windows

Open the Windows control panel, and select "Programs and Features". To start
the uninstallation, double-click on the program entry "MVTec Software Manager
(user)" (for a user installation), or "MVTec Software Manager (system)" for a
system installation.

The alternative dialog "Apps & Features" can only be used if SOM has been
installed for all users in system mode. If SOM has been installed in user mode,
it will still be listed, but trying to uninstall it from this dialog will
advise you to do the following:

    MVTec Software Manager must be uninstalled via MVTec Software Manager or
    the Windows control panel 'Programs and Features'

-------------------------------------------------------------------------------

Manual uninstallation

-------------------------------------------------------------------------------

Simply deleting the installation folder of SOM would leave non-functional menu
entries and a desktop shortcut as well as a registry entry (windows only)
behind. These would have to be deleted manually in the following directories
(user mode):

Linux

    $HOME/Desktop
    $HOME/.local/share/applications

Windows

    %USERPROFILE%\Desktop
    %APPDATA%\Microsoft\Windows\Start Menu\Programs\MVTec Software Manager

In system mode, these shortcuts are generated in the home directory of the user
who performed the installation on unix. On Windows, the shortcuts can be found
in the following directories:

    %PUBLIC%\Desktop
    %PROGRAMDATA%\Microsoft\Windows\Start Menu\Programs\MVTec Software Manager

For a user installation, run the Windows registry editor "regedit" and remove
the following key and its sub-keys:

     HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\MVTec Software Manager (user)

For a system installation, run the Windows registry editor "regedit" as
Administrator, and remove the following key and its sub-keys:

    "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\MVTec Software Manager (system)

-------------------------------------------------------------------------------

Installation database

-------------------------------------------------------------------------------

Installed packages are registered in a subdirectory som.d in each product
installation directory. The file som.feed is a JSON file that contains metadata
of the installed packages. In addition, the database directory contains a JSON
file for each installed package that lists the installed files and directories.
SOM also all the product icons and on Windows an uninstaller to this directory.

-------------------------------------------------------------------------------

Acronyms

-------------------------------------------------------------------------------

EULA

"End User License Agreement"

HTML

Hypertext Markup Language

HTTP

Hypertext Transfer Protocol

JSON

JavaScript Object Notation

LAN

Local Area Network

NTLM

NT LAN Manager

SHA

Secure Hash Algorithm

SOM

MVTec Software Manager

URI

Uniform Resource Identifier

URL

Uniform Resource Locator

-------------------------------------------------------------------------------

Release notes

-------------------------------------------------------------------------------

-------------------------------------------------------------------------------

1.2.1

-------------------------------------------------------------------------------

-   Added a button for refreshing the catalog from server to the settings
    dialog.

-   HTML entities in the RSS feed are now displayed correctly.

-   After modifying a product finishes, the progress modal now displays if the
    modification was a success. In case of an error during the modification,
    the error is displayed in the modal instead of in a notification.

-   If SOM is started with an invalid hostname, an error message is now shown.

-   When starting a product executable on linux systems, the LD_LIBRARY_PATH
    was set incorrectly. This problem has been fixed.

-   Added a new action to installed products for opening the installation
    directory in the operating systems file browser.

-   In rare cases, installation hooks were not executed properly on Windows
    because spaces in the hook command were not escaped correctly. This problem
    has been fixed.

-------------------------------------------------------------------------------

1.2

-------------------------------------------------------------------------------

-   SOM now handles installing products for all users in a more intuitive way.
    It is no longer required to start SOM with elevated rights to install
    products for all users. Instead, SOM will request the required permissions
    only when necessary, and return them as soon as they are no longer needed.
    More information can be found in the new section ??? in the documentation.

-   The SOM frontend now displays which actions will require admin rights to
    perform.

-   When leaving the package selection dialog the current selection is now
    saved for as long as the current browser tab is kept open.
    The saved selection is automatically restored when the user opens the
    dialog again by clicking on Select Packages for a new installation or
    Modify Packages for an existing installation. When the user opens the
    package selection dialog via the Install or Update buttons, the selection
    is not restored.

-   An installation that was aborted because the user was not logged in can now
    be resumed after being redirected back to SOM by the identity provider.

-   The frontend now adapts to small screen sizes.

-   SOM now checks whether another instance is running on the same port. If the
    instance was started by the same user it reuses this session, otherwise a
    warning is shown that the port is used by another user and the session is
    not reused.

-------------------------------------------------------------------------------

1.1.5

-------------------------------------------------------------------------------

-   Updating a product using the bell icon led to only a random subset of
    packages selected for an update.
    This issue has been fixed.

-   Update of the identity provider

-   The documentation link in the frontend now opens the HTML version of the
    documentation.

-   SOM can now display EULA texts in HTML format.

-------------------------------------------------------------------------------

1.1.4

-------------------------------------------------------------------------------

-   External commands (install hooks, file explorer, product executables) are
    now started in the foreground on Windows.

-   The console log accessible in the frontend now shows all log entries.

-   The frontend now shows a warning when the SOM backend is running with
    elevated rights.

-   The frontend dialog showing the progress when installing or removing
    packages is now localized.

-------------------------------------------------------------------------------

1.1.3

-------------------------------------------------------------------------------

-   Environment variable changes are now broadcast to the system on Windows.

-   The date of the copyright notice of the frontend is now up-to-date.

-------------------------------------------------------------------------------

1.1.2

-------------------------------------------------------------------------------

-   SOM now installs all icons that are part of a product alongside the program
    files.
    This solves the problem that the icons of installed products could not be
    displayed when SOM was started as an offline installer.

-   Running SOM as an offline installer, now completely ignores the current
    catalog URL setting.
    When the catalog URL was last saved by an old offline installer where SOM
    version was smaller than 1.1, the catalog URL was set to a now invalid URL.
    This led to the problem that the offline installer could not install new
    products.

-   On Windows SOM now installs the correct version of the uninstaller
    somctl.exe automatically to the som.d folder.
    For previous SOM versions, the uninstaller had to be part of the product
    feed. This is no longer necessary.

-------------------------------------------------------------------------------

1.1.1

-------------------------------------------------------------------------------

-   Setting the HTTP proxy did not work as expected.
    The command line option -P/--proxy and the frontend setting "HTTP Proxy"did
    not have any effect. This has been fixed and the option now works as
    expected.

-   In system session mode SOM cached data in the local repository’s toplevel
    folder.
    This issue has been fixed.

-   The news display in the frontend was not localized.
    Also, when "All" was selected, no news was displayed when the frontend
    language was changed. Both these issues have been fixed.

-------------------------------------------------------------------------------

1.0.0.2

-------------------------------------------------------------------------------

-   Extend output from som list.
    The "list" command now lists the installation folder of each package. To
    make parsing the output easier, the fields are separated by tabs instead of
    spaces. The package description is no longer enclosed in parentheses.

-   Always install SOM as som via pop-up dialog.
    Renaming the SOM binary caused incomplete offline installations. This
    problem has been fixed.

-   Add package field Default (ApiVersion 1.3).
    The default package is installed using the button Install on the page
    "Available".

-   Fix package selection of meta packages with multiple versions.
    The automatic selection of meta packages produced inconsistent package
    selections in some cases. This problem has been fixed.

-   Add uploaded license files for HALCON as new files by default.
    SOM used to replace existing license files when a license file of the same
    name had been selected for upload. Now, license files are added as new
    files by default. If a license file with the same name already exists, an
    increasing number will be added to the target file. It is also possible to
    overwrite an existing license file by selecting its name in the upload
    form.

-------------------------------------------------------------------------------

1.0.0.1

-------------------------------------------------------------------------------

-   Fix installation issue when initially installing SOM from catalog.
    When running the downloaded SOM binary and installing SOM from the catalog
    instead of using the installation from the pop-up greeting, the
    installation would register the wrong path for the generated shortcuts.

-   Add command line option to force user mode.

-   Enable interactive acceptance of EULA from command line.
    Installing a product from the command line that requires the acceptance of
    an EULA is now possible by entering "I accept" interactively. Previously,
    the command always had to be repeated with an additional command line
    option "--accept string". This is still supported for non-interactive
    installations from the command line.

-------------------------------------------------------------------------------

1.0.0.0

-------------------------------------------------------------------------------

-   Get rid of the bootstrap installer.
    SOM now only consists of a single binary. It can be used without installing
    it at all. Upon startup, SOM will provide an installation button in case
    you wish to install it or upgrade an existing installation.

-   SOM as an offline installer.
    SOM can be bundled with additional products for offline installation. If
    SOM detects a repository directory next to it upon startup, it will serve
    this directory as http://localhost:8000/mirror (or whatever port it uses at
    startup). In addition, the catalog for this session will be switched to the
    mirrored repository. This way, the bundled product(s) can be installed
    without requiring an internet connection at all.

-   Detect running sessions with same username but different mode.
    Upon startup, SOM re-used a running session if the username matched
    regardless of the session mode. Now, SOM also compares the session mode
    (user/system), and will start a new session if a different mode is
    requested.

-   Respect configuration changes to the installation targets.
    When changing the installation target for programs/data in the settings
    page, SOM had to be restarted to actually use them. This problem has been
    fixed.

-   Create directories only if needed.
    SOM used to create configuration/installation directories upon the first
    start of the program.

-   Change handling of installed products from multiple catalogs.
    SOM used to distinguish products by their feed URL. Now, products are
    distinguished by their BaseDir, i.e., the base directory used for the
    installation. This way, products can be mirrored at multiple locations
    without conflict (provided that the feed actually specifies the same
    product).

-   Do not display installed products on page "Available".

-   Do not display products with no packages for current architecture on page
    "Available".

-   Always show progress bars on terminal (if available).

-   Document the different methods to uninstall SOM.

-   Move button to manage packages to a drop-down menu.
    Each installed product now features a drop-down menu ⋮ to customize the
    installation. The entries allow to manage the installed packages, update
    the product (see below), or uninstall it entirely.

-   Detect updates for installed packages.
    In previous versions, SOM used to download the catalog only when started
    for the first time. Further, updates had to be initiated manually by
    clicking "Refresh from server". Now, SOM fetches the catalog from the
    server automatically upon server startup unless "Keep catalog up-to-date"is
    deactivated in the settings. Potential updates for installed products are
    now indicated on the page "Installed". If updates are available, a bell
    icon appears next to the product name. Click this icon or the entry
    "Update" in the drop-down menu of the product.

-   Change package selection to an expandable tree.
    You can no longer switch between basic and meta packages. Instead, meta
    packages are displayed as nodes, which can be expanded to show the included
    base packages in case you wish to tweak the installation.

-   Allow feeds to include external packages.
    If a package description contains the field "PackagesURL", packages from
    the referenced file are included into the current feed.

-   Slight modifications to the output format in command line mode.

-   Removal of somctl.
    The auxiliary tool "somctl" has been removed. Its functionality has been
    integrated into SOM itself.

-   Add command line option "-A" to start SOM as Administrator on Windows.

-   Remove "base and "meta" commands.
    The command "list" now lists all packages of the given feed (-f string) by
    default.

-   Add optional terminal path for the command "cli" on Linux.
    By default, "som cli" starts an XTerm from /usr/local/xterm. To use a
    different terminal, add its path to the command:

    som cli /usr/local/st

-   Require "MVTec Account" to download files.
    SOM now requires an "MVTec Account" for online installations. Downloaded
    files are associated with your login. Offline installations are not
    affected by this requirement. Once logged in, SOM stores a login token, so
    that your login is re-used in future sessions. If you want to install
    products from the command line, you have to log in from a browser session
    beforehand.

-   Display additional content for the selected product.
    The page "Installed" may display additional content for the selected
    product. Currently, this can be a link to the product documentation. The
    selected product is indicated by a slightly darker background color. To
    select another product, simply click on its background area.

-------------------------------------------------------------------------------

0.9.2.1

-------------------------------------------------------------------------------

-   Force default settings if SOM is not installed.

-------------------------------------------------------------------------------

0.9.2.0

-------------------------------------------------------------------------------

-   Extensions to the bootstrap installer.
    The bootstrap installer that installs SOM initially has been a mostly
    silent experience. It now features a minimalist graphical interface to
    provide a little more feedback of the installation process. It can also
    update older versions of SOM. The graphical interface can be suppressed
    with the command line option "-n". SOM can be uninstalled by providing the
    command line option "-u".
    If the file name of the bootstrap installer includes the string "offline",
    it expects a directory named repository next to the installer binary. The
    assumption is that this directory contains offline data for the
    installation of a certain product on machines with no internet access. The
    contents of this directory will be imported into the local repository, and
    SOM will be started so that the product can be installed. The offline
    installer will complain if it does not find the directory.

-   Changed CLI behavior if -f string is given without command.
    If a product is selected using the command line option -f string and none
    of the commands "base", "list", "install", "meta",or "remove" is given, som
    starts as usual, and opens the configuration page of the specified product.
    The old behavior (listing the meta and base packages) can be achieved using
    the command "list".

-------------------------------------------------------------------------------

0.9.1.0

-------------------------------------------------------------------------------

-   Initial public release of SOM.

-------------------------------------------------------------------------------

Legal notices

-------------------------------------------------------------------------------

SOM uses third-party software from the following projects

-------------------------------------------------------------------------------

github.com/coreos/go-oidc/v3/oidc

-------------------------------------------------------------------------------

    CoreOS Project
    Copyright 2014 CoreOS, Inc

    This product includes software developed at CoreOS, Inc.
    (http://www.coreos.com/).

    Apache License
                               Version 2.0, January 2004
                            http://www.apache.org/licenses/

       TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

       1. Definitions.

          "License" shall mean the terms and conditions for use, reproduction,
          and distribution as defined by Sections 1 through 9 of this document.

          "Licensor" shall mean the copyright owner or entity authorized by
          the copyright owner that is granting the License.

          "Legal Entity" shall mean the union of the acting entity and all
          other entities that control, are controlled by, or are under common
          control with that entity. For the purposes of this definition,
          "control" means (i) the power, direct or indirect, to cause the
          direction or management of such entity, whether by contract or
          otherwise, or (ii) ownership of fifty percent (50%) or more of the
          outstanding shares, or (iii) beneficial ownership of such entity.

          "You" (or "Your") shall mean an individual or Legal Entity
          exercising permissions granted by this License.

          "Source" form shall mean the preferred form for making modifications,
          including but not limited to software source code, documentation
          source, and configuration files.

          "Object" form shall mean any form resulting from mechanical
          transformation or translation of a Source form, including but
          not limited to compiled object code, generated documentation,
          and conversions to other media types.

          "Work" shall mean the work of authorship, whether in Source or
          Object form, made available under the License, as indicated by a
          copyright notice that is included in or attached to the work
          (an example is provided in the Appendix below).

          "Derivative Works" shall mean any work, whether in Source or Object
          form, that is based on (or derived from) the Work and for which the
          editorial revisions, annotations, elaborations, or other modifications
          represent, as a whole, an original work of authorship. For the purposes
          of this License, Derivative Works shall not include works that remain
          separable from, or merely link (or bind by name) to the interfaces of,
          the Work and Derivative Works thereof.

          "Contribution" shall mean any work of authorship, including
          the original version of the Work and any modifications or additions
          to that Work or Derivative Works thereof, that is intentionally
          submitted to Licensor for inclusion in the Work by the copyright owner
          or by an individual or Legal Entity authorized to submit on behalf of
          the copyright owner. For the purposes of this definition, "submitted"
          means any form of electronic, verbal, or written communication sent
          to the Licensor or its representatives, including but not limited to
          communication on electronic mailing lists, source code control systems,
          and issue tracking systems that are managed by, or on behalf of, the
          Licensor for the purpose of discussing and improving the Work, but
          excluding communication that is conspicuously marked or otherwise
          designated in writing by the copyright owner as "Not a Contribution."

          "Contributor" shall mean Licensor and any individual or Legal Entity
          on behalf of whom a Contribution has been received by Licensor and
          subsequently incorporated within the Work.

       2. Grant of Copyright License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          copyright license to reproduce, prepare Derivative Works of,
          publicly display, publicly perform, sublicense, and distribute the
          Work and such Derivative Works in Source or Object form.

       3. Grant of Patent License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          (except as stated in this section) patent license to make, have made,
          use, offer to sell, sell, import, and otherwise transfer the Work,
          where such license applies only to those patent claims licensable
          by such Contributor that are necessarily infringed by their
          Contribution(s) alone or by combination of their Contribution(s)
          with the Work to which such Contribution(s) was submitted. If You
          institute patent litigation against any entity (including a
          cross-claim or counterclaim in a lawsuit) alleging that the Work
          or a Contribution incorporated within the Work constitutes direct
          or contributory patent infringement, then any patent licenses
          granted to You under this License for that Work shall terminate
          as of the date such litigation is filed.

       4. Redistribution. You may reproduce and distribute copies of the
          Work or Derivative Works thereof in any medium, with or without
          modifications, and in Source or Object form, provided that You
          meet the following conditions:

          (a) You must give any other recipients of the Work or
              Derivative Works a copy of this License; and

          (b) You must cause any modified files to carry prominent notices
              stating that You changed the files; and

          (c) You must retain, in the Source form of any Derivative Works
              that You distribute, all copyright, patent, trademark, and
              attribution notices from the Source form of the Work,
              excluding those notices that do not pertain to any part of
              the Derivative Works; and

          (d) If the Work includes a "NOTICE" text file as part of its
              distribution, then any Derivative Works that You distribute must
              include a readable copy of the attribution notices contained
              within such NOTICE file, excluding those notices that do not
              pertain to any part of the Derivative Works, in at least one
              of the following places: within a NOTICE text file distributed
              as part of the Derivative Works; within the Source form or
              documentation, if provided along with the Derivative Works; or,
              within a display generated by the Derivative Works, if and
              wherever such third-party notices normally appear. The contents
              of the NOTICE file are for informational purposes only and
              do not modify the License. You may add Your own attribution
              notices within Derivative Works that You distribute, alongside
              or as an addendum to the NOTICE text from the Work, provided
              that such additional attribution notices cannot be construed
              as modifying the License.

          You may add Your own copyright statement to Your modifications and
          may provide additional or different license terms and conditions
          for use, reproduction, or distribution of Your modifications, or
          for any such Derivative Works as a whole, provided Your use,
          reproduction, and distribution of the Work otherwise complies with
          the conditions stated in this License.

       5. Submission of Contributions. Unless You explicitly state otherwise,
          any Contribution intentionally submitted for inclusion in the Work
          by You to the Licensor shall be under the terms and conditions of
          this License, without any additional terms or conditions.
          Notwithstanding the above, nothing herein shall supersede or modify
          the terms of any separate license agreement you may have executed
          with Licensor regarding such Contributions.

       6. Trademarks. This License does not grant permission to use the trade
          names, trademarks, service marks, or product names of the Licensor,
          except as required for reasonable and customary use in describing the
          origin of the Work and reproducing the content of the NOTICE file.

       7. Disclaimer of Warranty. Unless required by applicable law or
          agreed to in writing, Licensor provides the Work (and each
          Contributor provides its Contributions) on an "AS IS" BASIS,
          WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
          implied, including, without limitation, any warranties or conditions
          of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
          PARTICULAR PURPOSE. You are solely responsible for determining the
          appropriateness of using or redistributing the Work and assume any
          risks associated with Your exercise of permissions under this License.

       8. Limitation of Liability. In no event and under no legal theory,
          whether in tort (including negligence), contract, or otherwise,
          unless required by applicable law (such as deliberate and grossly
          negligent acts) or agreed to in writing, shall any Contributor be
          liable to You for damages, including any direct, indirect, special,
          incidental, or consequential damages of any character arising as a
          result of this License or out of the use or inability to use the
          Work (including but not limited to damages for loss of goodwill,
          work stoppage, computer failure or malfunction, or any and all
          other commercial damages or losses), even if such Contributor
          has been advised of the possibility of such damages.

       9. Accepting Warranty or Additional Liability. While redistributing
          the Work or Derivative Works thereof, You may choose to offer,
          and charge a fee for, acceptance of support, warranty, indemnity,
          or other liability obligations and/or rights consistent with this
          License. However, in accepting such obligations, You may act only
          on Your own behalf and on Your sole responsibility, not on behalf
          of any other Contributor, and only if You agree to indemnify,
          defend, and hold each Contributor harmless for any liability
          incurred by, or claims asserted against, such Contributor by reason
          of your accepting any such warranty or additional liability.

       END OF TERMS AND CONDITIONS

       APPENDIX: How to apply the Apache License to your work.

          To apply the Apache License to your work, attach the following
          boilerplate notice, with the fields enclosed by brackets "{}"
          replaced with your own identifying information. (Don't include
          the brackets!)  The text should be enclosed in the appropriate
          comment syntax for the file format. We also recommend that a
          file or class name and description of purpose be included on the
          same "printed page" as the copyright notice for easier
          identification within third-party archives.

       Copyright {yyyy} {name of copyright owner}

       Licensed under the Apache License, Version 2.0 (the "License");
       you may not use this file except in compliance with the License.
       You may obtain a copy of the License at

           http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing, software
       distributed under the License is distributed on an "AS IS" BASIS,
       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
       See the License for the specific language governing permissions and
       limitations under the License.

-------------------------------------------------------------------------------

github.com/google/uuid

-------------------------------------------------------------------------------

    Copyright (c) 2009,2014 Google Inc. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

github.com/k3a/html2text

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2017 Mario K3A Hros (www.k3a.me)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

github.com/mattn/go-runewidth

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright (c) 2016 Yasuhiro Matsumoto

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

github.com/mitchellh/colorstring

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright (c) 2014 Mitchell Hashimoto

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

-------------------------------------------------------------------------------

github.com/rivo/uniseg

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2019 Oliver Kuederle

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

github.com/schollz/progressbar/v3

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2017 Zack

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

github.com/spf13/cobra

-------------------------------------------------------------------------------

                                    Apache License
                               Version 2.0, January 2004
                            http://www.apache.org/licenses/

       TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

       1. Definitions.

          "License" shall mean the terms and conditions for use, reproduction,
          and distribution as defined by Sections 1 through 9 of this document.

          "Licensor" shall mean the copyright owner or entity authorized by
          the copyright owner that is granting the License.

          "Legal Entity" shall mean the union of the acting entity and all
          other entities that control, are controlled by, or are under common
          control with that entity. For the purposes of this definition,
          "control" means (i) the power, direct or indirect, to cause the
          direction or management of such entity, whether by contract or
          otherwise, or (ii) ownership of fifty percent (50%) or more of the
          outstanding shares, or (iii) beneficial ownership of such entity.

          "You" (or "Your") shall mean an individual or Legal Entity
          exercising permissions granted by this License.

          "Source" form shall mean the preferred form for making modifications,
          including but not limited to software source code, documentation
          source, and configuration files.

          "Object" form shall mean any form resulting from mechanical
          transformation or translation of a Source form, including but
          not limited to compiled object code, generated documentation,
          and conversions to other media types.

          "Work" shall mean the work of authorship, whether in Source or
          Object form, made available under the License, as indicated by a
          copyright notice that is included in or attached to the work
          (an example is provided in the Appendix below).

          "Derivative Works" shall mean any work, whether in Source or Object
          form, that is based on (or derived from) the Work and for which the
          editorial revisions, annotations, elaborations, or other modifications
          represent, as a whole, an original work of authorship. For the purposes
          of this License, Derivative Works shall not include works that remain
          separable from, or merely link (or bind by name) to the interfaces of,
          the Work and Derivative Works thereof.

          "Contribution" shall mean any work of authorship, including
          the original version of the Work and any modifications or additions
          to that Work or Derivative Works thereof, that is intentionally
          submitted to Licensor for inclusion in the Work by the copyright owner
          or by an individual or Legal Entity authorized to submit on behalf of
          the copyright owner. For the purposes of this definition, "submitted"
          means any form of electronic, verbal, or written communication sent
          to the Licensor or its representatives, including but not limited to
          communication on electronic mailing lists, source code control systems,
          and issue tracking systems that are managed by, or on behalf of, the
          Licensor for the purpose of discussing and improving the Work, but
          excluding communication that is conspicuously marked or otherwise
          designated in writing by the copyright owner as "Not a Contribution."

          "Contributor" shall mean Licensor and any individual or Legal Entity
          on behalf of whom a Contribution has been received by Licensor and
          subsequently incorporated within the Work.

       2. Grant of Copyright License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          copyright license to reproduce, prepare Derivative Works of,
          publicly display, publicly perform, sublicense, and distribute the
          Work and such Derivative Works in Source or Object form.

       3. Grant of Patent License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          (except as stated in this section) patent license to make, have made,
          use, offer to sell, sell, import, and otherwise transfer the Work,
          where such license applies only to those patent claims licensable
          by such Contributor that are necessarily infringed by their
          Contribution(s) alone or by combination of their Contribution(s)
          with the Work to which such Contribution(s) was submitted. If You
          institute patent litigation against any entity (including a
          cross-claim or counterclaim in a lawsuit) alleging that the Work
          or a Contribution incorporated within the Work constitutes direct
          or contributory patent infringement, then any patent licenses
          granted to You under this License for that Work shall terminate
          as of the date such litigation is filed.

       4. Redistribution. You may reproduce and distribute copies of the
          Work or Derivative Works thereof in any medium, with or without
          modifications, and in Source or Object form, provided that You
          meet the following conditions:

          (a) You must give any other recipients of the Work or
              Derivative Works a copy of this License; and

          (b) You must cause any modified files to carry prominent notices
              stating that You changed the files; and

          (c) You must retain, in the Source form of any Derivative Works
              that You distribute, all copyright, patent, trademark, and
              attribution notices from the Source form of the Work,
              excluding those notices that do not pertain to any part of
              the Derivative Works; and

          (d) If the Work includes a "NOTICE" text file as part of its
              distribution, then any Derivative Works that You distribute must
              include a readable copy of the attribution notices contained
              within such NOTICE file, excluding those notices that do not
              pertain to any part of the Derivative Works, in at least one
              of the following places: within a NOTICE text file distributed
              as part of the Derivative Works; within the Source form or
              documentation, if provided along with the Derivative Works; or,
              within a display generated by the Derivative Works, if and
              wherever such third-party notices normally appear. The contents
              of the NOTICE file are for informational purposes only and
              do not modify the License. You may add Your own attribution
              notices within Derivative Works that You distribute, alongside
              or as an addendum to the NOTICE text from the Work, provided
              that such additional attribution notices cannot be construed
              as modifying the License.

          You may add Your own copyright statement to Your modifications and
          may provide additional or different license terms and conditions
          for use, reproduction, or distribution of Your modifications, or
          for any such Derivative Works as a whole, provided Your use,
          reproduction, and distribution of the Work otherwise complies with
          the conditions stated in this License.

       5. Submission of Contributions. Unless You explicitly state otherwise,
          any Contribution intentionally submitted for inclusion in the Work
          by You to the Licensor shall be under the terms and conditions of
          this License, without any additional terms or conditions.
          Notwithstanding the above, nothing herein shall supersede or modify
          the terms of any separate license agreement you may have executed
          with Licensor regarding such Contributions.

       6. Trademarks. This License does not grant permission to use the trade
          names, trademarks, service marks, or product names of the Licensor,
          except as required for reasonable and customary use in describing the
          origin of the Work and reproducing the content of the NOTICE file.

       7. Disclaimer of Warranty. Unless required by applicable law or
          agreed to in writing, Licensor provides the Work (and each
          Contributor provides its Contributions) on an "AS IS" BASIS,
          WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
          implied, including, without limitation, any warranties or conditions
          of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
          PARTICULAR PURPOSE. You are solely responsible for determining the
          appropriateness of using or redistributing the Work and assume any
          risks associated with Your exercise of permissions under this License.

       8. Limitation of Liability. In no event and under no legal theory,
          whether in tort (including negligence), contract, or otherwise,
          unless required by applicable law (such as deliberate and grossly
          negligent acts) or agreed to in writing, shall any Contributor be
          liable to You for damages, including any direct, indirect, special,
          incidental, or consequential damages of any character arising as a
          result of this License or out of the use or inability to use the
          Work (including but not limited to damages for loss of goodwill,
          work stoppage, computer failure or malfunction, or any and all
          other commercial damages or losses), even if such Contributor
          has been advised of the possibility of such damages.

       9. Accepting Warranty or Additional Liability. While redistributing
          the Work or Derivative Works thereof, You may choose to offer,
          and charge a fee for, acceptance of support, warranty, indemnity,
          or other liability obligations and/or rights consistent with this
          License. However, in accepting such obligations, You may act only
          on Your own behalf and on Your sole responsibility, not on behalf
          of any other Contributor, and only if You agree to indemnify,
          defend, and hold each Contributor harmless for any liability
          incurred by, or claims asserted against, such Contributor by reason
          of your accepting any such warranty or additional liability.

-------------------------------------------------------------------------------

github.com/spf13/pflag

-------------------------------------------------------------------------------

    Copyright (c) 2012 Alex Ogier. All rights reserved.
    Copyright (c) 2012 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

golang.org/x/crypto

-------------------------------------------------------------------------------

    Copyright (c) 2009 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

golang.org/x/net

-------------------------------------------------------------------------------

    Copyright (c) 2009 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

golang.org/x/oauth2

-------------------------------------------------------------------------------

    Copyright (c) 2009 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

golang.org/x/sys

-------------------------------------------------------------------------------

    Copyright (c) 2009 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

golang.org/x/term

-------------------------------------------------------------------------------

    Copyright (c) 2009 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

gopkg.in/square/go-jose.v2

-------------------------------------------------------------------------------

                                     Apache License
                               Version 2.0, January 2004
                            http://www.apache.org/licenses/

       TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

       1. Definitions.

          "License" shall mean the terms and conditions for use, reproduction,
          and distribution as defined by Sections 1 through 9 of this document.

          "Licensor" shall mean the copyright owner or entity authorized by
          the copyright owner that is granting the License.

          "Legal Entity" shall mean the union of the acting entity and all
          other entities that control, are controlled by, or are under common
          control with that entity. For the purposes of this definition,
          "control" means (i) the power, direct or indirect, to cause the
          direction or management of such entity, whether by contract or
          otherwise, or (ii) ownership of fifty percent (50%) or more of the
          outstanding shares, or (iii) beneficial ownership of such entity.

          "You" (or "Your") shall mean an individual or Legal Entity
          exercising permissions granted by this License.

          "Source" form shall mean the preferred form for making modifications,
          including but not limited to software source code, documentation
          source, and configuration files.

          "Object" form shall mean any form resulting from mechanical
          transformation or translation of a Source form, including but
          not limited to compiled object code, generated documentation,
          and conversions to other media types.

          "Work" shall mean the work of authorship, whether in Source or
          Object form, made available under the License, as indicated by a
          copyright notice that is included in or attached to the work
          (an example is provided in the Appendix below).

          "Derivative Works" shall mean any work, whether in Source or Object
          form, that is based on (or derived from) the Work and for which the
          editorial revisions, annotations, elaborations, or other modifications
          represent, as a whole, an original work of authorship. For the purposes
          of this License, Derivative Works shall not include works that remain
          separable from, or merely link (or bind by name) to the interfaces of,
          the Work and Derivative Works thereof.

          "Contribution" shall mean any work of authorship, including
          the original version of the Work and any modifications or additions
          to that Work or Derivative Works thereof, that is intentionally
          submitted to Licensor for inclusion in the Work by the copyright owner
          or by an individual or Legal Entity authorized to submit on behalf of
          the copyright owner. For the purposes of this definition, "submitted"
          means any form of electronic, verbal, or written communication sent
          to the Licensor or its representatives, including but not limited to
          communication on electronic mailing lists, source code control systems,
          and issue tracking systems that are managed by, or on behalf of, the
          Licensor for the purpose of discussing and improving the Work, but
          excluding communication that is conspicuously marked or otherwise
          designated in writing by the copyright owner as "Not a Contribution."

          "Contributor" shall mean Licensor and any individual or Legal Entity
          on behalf of whom a Contribution has been received by Licensor and
          subsequently incorporated within the Work.

       2. Grant of Copyright License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          copyright license to reproduce, prepare Derivative Works of,
          publicly display, publicly perform, sublicense, and distribute the
          Work and such Derivative Works in Source or Object form.

       3. Grant of Patent License. Subject to the terms and conditions of
          this License, each Contributor hereby grants to You a perpetual,
          worldwide, non-exclusive, no-charge, royalty-free, irrevocable
          (except as stated in this section) patent license to make, have made,
          use, offer to sell, sell, import, and otherwise transfer the Work,
          where such license applies only to those patent claims licensable
          by such Contributor that are necessarily infringed by their
          Contribution(s) alone or by combination of their Contribution(s)
          with the Work to which such Contribution(s) was submitted. If You
          institute patent litigation against any entity (including a
          cross-claim or counterclaim in a lawsuit) alleging that the Work
          or a Contribution incorporated within the Work constitutes direct
          or contributory patent infringement, then any patent licenses
          granted to You under this License for that Work shall terminate
          as of the date such litigation is filed.

       4. Redistribution. You may reproduce and distribute copies of the
          Work or Derivative Works thereof in any medium, with or without
          modifications, and in Source or Object form, provided that You
          meet the following conditions:

          (a) You must give any other recipients of the Work or
              Derivative Works a copy of this License; and

          (b) You must cause any modified files to carry prominent notices
              stating that You changed the files; and

          (c) You must retain, in the Source form of any Derivative Works
              that You distribute, all copyright, patent, trademark, and
              attribution notices from the Source form of the Work,
              excluding those notices that do not pertain to any part of
              the Derivative Works; and

          (d) If the Work includes a "NOTICE" text file as part of its
              distribution, then any Derivative Works that You distribute must
              include a readable copy of the attribution notices contained
              within such NOTICE file, excluding those notices that do not
              pertain to any part of the Derivative Works, in at least one
              of the following places: within a NOTICE text file distributed
              as part of the Derivative Works; within the Source form or
              documentation, if provided along with the Derivative Works; or,
              within a display generated by the Derivative Works, if and
              wherever such third-party notices normally appear. The contents
              of the NOTICE file are for informational purposes only and
              do not modify the License. You may add Your own attribution
              notices within Derivative Works that You distribute, alongside
              or as an addendum to the NOTICE text from the Work, provided
              that such additional attribution notices cannot be construed
              as modifying the License.

          You may add Your own copyright statement to Your modifications and
          may provide additional or different license terms and conditions
          for use, reproduction, or distribution of Your modifications, or
          for any such Derivative Works as a whole, provided Your use,
          reproduction, and distribution of the Work otherwise complies with
          the conditions stated in this License.

       5. Submission of Contributions. Unless You explicitly state otherwise,
          any Contribution intentionally submitted for inclusion in the Work
          by You to the Licensor shall be under the terms and conditions of
          this License, without any additional terms or conditions.
          Notwithstanding the above, nothing herein shall supersede or modify
          the terms of any separate license agreement you may have executed
          with Licensor regarding such Contributions.

       6. Trademarks. This License does not grant permission to use the trade
          names, trademarks, service marks, or product names of the Licensor,
          except as required for reasonable and customary use in describing the
          origin of the Work and reproducing the content of the NOTICE file.

       7. Disclaimer of Warranty. Unless required by applicable law or
          agreed to in writing, Licensor provides the Work (and each
          Contributor provides its Contributions) on an "AS IS" BASIS,
          WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
          implied, including, without limitation, any warranties or conditions
          of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
          PARTICULAR PURPOSE. You are solely responsible for determining the
          appropriateness of using or redistributing the Work and assume any
          risks associated with Your exercise of permissions under this License.

       8. Limitation of Liability. In no event and under no legal theory,
          whether in tort (including negligence), contract, or otherwise,
          unless required by applicable law (such as deliberate and grossly
          negligent acts) or agreed to in writing, shall any Contributor be
          liable to You for damages, including any direct, indirect, special,
          incidental, or consequential damages of any character arising as a
          result of this License or out of the use or inability to use the
          Work (including but not limited to damages for loss of goodwill,
          work stoppage, computer failure or malfunction, or any and all
          other commercial damages or losses), even if such Contributor
          has been advised of the possibility of such damages.

       9. Accepting Warranty or Additional Liability. While redistributing
          the Work or Derivative Works thereof, You may choose to offer,
          and charge a fee for, acceptance of support, warranty, indemnity,
          or other liability obligations and/or rights consistent with this
          License. However, in accepting such obligations, You may act only
          on Your own behalf and on Your sole responsibility, not on behalf
          of any other Contributor, and only if You agree to indemnify,
          defend, and hold each Contributor harmless for any liability
          incurred by, or claims asserted against, such Contributor by reason
          of your accepting any such warranty or additional liability.

       END OF TERMS AND CONDITIONS

       APPENDIX: How to apply the Apache License to your work.

          To apply the Apache License to your work, attach the following
          boilerplate notice, with the fields enclosed by brackets "[]"
          replaced with your own identifying information. (Don't include
          the brackets!)  The text should be enclosed in the appropriate
          comment syntax for the file format. We also recommend that a
          file or class name and description of purpose be included on the
          same "printed page" as the copyright notice for easier
          identification within third-party archives.

       Copyright [yyyy] [name of copyright owner]

       Licensed under the Apache License, Version 2.0 (the "License");
       you may not use this file except in compliance with the License.
       You may obtain a copy of the License at

           http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing, software
       distributed under the License is distributed on an "AS IS" BASIS,
       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
       See the License for the specific language governing permissions and
       limitations under the License.

-------------------------------------------------------------------------------

gopkg.in/square/go-jose.v2/json

-------------------------------------------------------------------------------

    Copyright (c) 2012 The Go Authors. All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

       * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
       * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following disclaimer
    in the documentation and/or other materials provided with the
    distribution.
       * Neither the name of Google Inc. nor the names of its
    contributors may be used to endorse or promote products derived from
    this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
    OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
    DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
    THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

https://github.com/babel/babel

-------------------------------------------------------------------------------

    Copyright (C) 2012-2014 by various contributors (see AUTHORS)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/intlify/vue-i18n-next

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright (c) 2020 kazuya kawaguchi

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/vuejs/core

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright (c) 2018-present, Yuxi (Evan) You

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/browserslist/browserslist

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright 2014 Andrey Sitnik <andrey@sitnik.ru> and other contributors

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/browserslist/caniuse-lite

-------------------------------------------------------------------------------

    Attribution 4.0 International

    =======================================================================

    Creative Commons Corporation ("Creative Commons") is not a law firm and
    does not provide legal services or legal advice. Distribution of
    Creative Commons public licenses does not create a lawyer-client or
    other relationship. Creative Commons makes its licenses and related
    information available on an "as-is" basis. Creative Commons gives no
    warranties regarding its licenses, any material licensed under their
    terms and conditions, or any related information. Creative Commons
    disclaims all liability for damages resulting from their use to the
    fullest extent possible.

    Using Creative Commons Public Licenses

    Creative Commons public licenses provide a standard set of terms and
    conditions that creators and other rights holders may use to share
    original works of authorship and other material subject to copyright
    and certain other rights specified in the public license below. The
    following considerations are for informational purposes only, are not
    exhaustive, and do not form part of our licenses.

         Considerations for licensors: Our public licenses are
         intended for use by those authorized to give the public
         permission to use material in ways otherwise restricted by
         copyright and certain other rights. Our licenses are
         irrevocable. Licensors should read and understand the terms
         and conditions of the license they choose before applying it.
         Licensors should also secure all rights necessary before
         applying our licenses so that the public can reuse the
         material as expected. Licensors should clearly mark any
         material not subject to the license. This includes other CC-
         licensed material, or material used under an exception or
         limitation to copyright. More considerations for licensors:
        wiki.creativecommons.org/Considerations_for_licensors

         Considerations for the public: By using one of our public
         licenses, a licensor grants the public permission to use the
         licensed material under specified terms and conditions. If
         the licensor's permission is not necessary for any reason--for
         example, because of any applicable exception or limitation to
         copyright--then that use is not regulated by the license. Our
         licenses grant only permissions under copyright and certain
         other rights that a licensor has authority to grant. Use of
         the licensed material may still be restricted for other
         reasons, including because others have copyright or other
         rights in the material. A licensor may make special requests,
         such as asking that all changes be marked or described.
         Although not required by our licenses, you are encouraged to
         respect those requests where reasonable. More_considerations
         for the public:
        wiki.creativecommons.org/Considerations_for_licensees

    =======================================================================

    Creative Commons Attribution 4.0 International Public License

    By exercising the Licensed Rights (defined below), You accept and agree
    to be bound by the terms and conditions of this Creative Commons
    Attribution 4.0 International Public License ("Public License"). To the
    extent this Public License may be interpreted as a contract, You are
    granted the Licensed Rights in consideration of Your acceptance of
    these terms and conditions, and the Licensor grants You such rights in
    consideration of benefits the Licensor receives from making the
    Licensed Material available under these terms and conditions.


    Section 1 -- Definitions.

      a. Adapted Material means material subject to Copyright and Similar
         Rights that is derived from or based upon the Licensed Material
         and in which the Licensed Material is translated, altered,
         arranged, transformed, or otherwise modified in a manner requiring
         permission under the Copyright and Similar Rights held by the
         Licensor. For purposes of this Public License, where the Licensed
         Material is a musical work, performance, or sound recording,
         Adapted Material is always produced where the Licensed Material is
         synched in timed relation with a moving image.

      b. Adapter's License means the license You apply to Your Copyright
         and Similar Rights in Your contributions to Adapted Material in
         accordance with the terms and conditions of this Public License.

      c. Copyright and Similar Rights means copyright and/or similar rights
         closely related to copyright including, without limitation,
         performance, broadcast, sound recording, and Sui Generis Database
         Rights, without regard to how the rights are labeled or
         categorized. For purposes of this Public License, the rights
         specified in Section 2(b)(1)-(2) are not Copyright and Similar
         Rights.

      d. Effective Technological Measures means those measures that, in the
         absence of proper authority, may not be circumvented under laws
         fulfilling obligations under Article 11 of the WIPO Copyright
         Treaty adopted on December 20, 1996, and/or similar international
         agreements.

      e. Exceptions and Limitations means fair use, fair dealing, and/or
         any other exception or limitation to Copyright and Similar Rights
         that applies to Your use of the Licensed Material.

      f. Licensed Material means the artistic or literary work, database,
         or other material to which the Licensor applied this Public
         License.

      g. Licensed Rights means the rights granted to You subject to the
         terms and conditions of this Public License, which are limited to
         all Copyright and Similar Rights that apply to Your use of the
         Licensed Material and that the Licensor has authority to license.

      h. Licensor means the individual(s) or entity(ies) granting rights
         under this Public License.

      i. Share means to provide material to the public by any means or
         process that requires permission under the Licensed Rights, such
         as reproduction, public display, public performance, distribution,
         dissemination, communication, or importation, and to make material
         available to the public including in ways that members of the
         public may access the material from a place and at a time
         individually chosen by them.

      j. Sui Generis Database Rights means rights other than copyright
         resulting from Directive 96/9/EC of the European Parliament and of
         the Council of 11 March 1996 on the legal protection of databases,
         as amended and/or succeeded, as well as other essentially
         equivalent rights anywhere in the world.

      k. You means the individual or entity exercising the Licensed Rights
         under this Public License. Your has a corresponding meaning.


    Section 2 -- Scope.

      a. License grant.

           1. Subject to the terms and conditions of this Public License,
              the Licensor hereby grants You a worldwide, royalty-free,
              non-sublicensable, non-exclusive, irrevocable license to
              exercise the Licensed Rights in the Licensed Material to:

                a. reproduce and Share the Licensed Material, in whole or
                   in part; and

                b. produce, reproduce, and Share Adapted Material.

           2. Exceptions and Limitations. For the avoidance of doubt, where
              Exceptions and Limitations apply to Your use, this Public
              License does not apply, and You do not need to comply with
              its terms and conditions.

           3. Term. The term of this Public License is specified in Section
              6(a).

           4. Media and formats; technical modifications allowed. The
              Licensor authorizes You to exercise the Licensed Rights in
              all media and formats whether now known or hereafter created,
              and to make technical modifications necessary to do so. The
              Licensor waives and/or agrees not to assert any right or
              authority to forbid You from making technical modifications
              necessary to exercise the Licensed Rights, including
              technical modifications necessary to circumvent Effective
              Technological Measures. For purposes of this Public License,
              simply making modifications authorized by this Section 2(a)
              (4) never produces Adapted Material.

           5. Downstream recipients.

                a. Offer from the Licensor -- Licensed Material. Every
                   recipient of the Licensed Material automatically
                   receives an offer from the Licensor to exercise the
                   Licensed Rights under the terms and conditions of this
                   Public License.

                b. No downstream restrictions. You may not offer or impose
                   any additional or different terms or conditions on, or
                   apply any Effective Technological Measures to, the
                   Licensed Material if doing so restricts exercise of the
                   Licensed Rights by any recipient of the Licensed
                   Material.

           6. No endorsement. Nothing in this Public License constitutes or
              may be construed as permission to assert or imply that You
              are, or that Your use of the Licensed Material is, connected
              with, or sponsored, endorsed, or granted official status by,
              the Licensor or others designated to receive attribution as
              provided in Section 3(a)(1)(A)(i).

      b. Other rights.

           1. Moral rights, such as the right of integrity, are not
              licensed under this Public License, nor are publicity,
              privacy, and/or other similar personality rights; however, to
              the extent possible, the Licensor waives and/or agrees not to
              assert any such rights held by the Licensor to the limited
              extent necessary to allow You to exercise the Licensed
              Rights, but not otherwise.

           2. Patent and trademark rights are not licensed under this
              Public License.

           3. To the extent possible, the Licensor waives any right to
              collect royalties from You for the exercise of the Licensed
              Rights, whether directly or through a collecting society
              under any voluntary or waivable statutory or compulsory
              licensing scheme. In all other cases the Licensor expressly
              reserves any right to collect such royalties.


    Section 3 -- License Conditions.

    Your exercise of the Licensed Rights is expressly made subject to the
    following conditions.

      a. Attribution.

           1. If You Share the Licensed Material (including in modified
              form), You must:

                a. retain the following if it is supplied by the Licensor
                   with the Licensed Material:

                     i. identification of the creator(s) of the Licensed
                        Material and any others designated to receive
                        attribution, in any reasonable manner requested by
                        the Licensor (including by pseudonym if
                        designated);

                    ii. a copyright notice;

                   iii. a notice that refers to this Public License;

                    iv. a notice that refers to the disclaimer of
                        warranties;

                     v. a URI or hyperlink to the Licensed Material to the
                        extent reasonably practicable;

                b. indicate if You modified the Licensed Material and
                   retain an indication of any previous modifications; and

                c. indicate the Licensed Material is licensed under this
                   Public License, and include the text of, or the URI or
                   hyperlink to, this Public License.

           2. You may satisfy the conditions in Section 3(a)(1) in any
              reasonable manner based on the medium, means, and context in
              which You Share the Licensed Material. For example, it may be
              reasonable to satisfy the conditions by providing a URI or
              hyperlink to a resource that includes the required
              information.

           3. If requested by the Licensor, You must remove any of the
              information required by Section 3(a)(1)(A) to the extent
              reasonably practicable.

           4. If You Share Adapted Material You produce, the Adapter's
              License You apply must not prevent recipients of the Adapted
              Material from complying with this Public License.


    Section 4 -- Sui Generis Database Rights.

    Where the Licensed Rights include Sui Generis Database Rights that
    apply to Your use of the Licensed Material:

      a. for the avoidance of doubt, Section 2(a)(1) grants You the right
         to extract, reuse, reproduce, and Share all or a substantial
         portion of the contents of the database;

      b. if You include all or a substantial portion of the database
         contents in a database in which You have Sui Generis Database
         Rights, then the database in which You have Sui Generis Database
         Rights (but not its individual contents) is Adapted Material; and

      c. You must comply with the conditions in Section 3(a) if You Share
         all or a substantial portion of the contents of the database.

    For the avoidance of doubt, this Section 4 supplements and does not
    replace Your obligations under this Public License where the Licensed
    Rights include other Copyright and Similar Rights.


    Section 5 -- Disclaimer of Warranties and Limitation of Liability.

      a. UNLESS OTHERWISE SEPARATELY UNDERTAKEN BY THE LICENSOR, TO THE
         EXTENT POSSIBLE, THE LICENSOR OFFERS THE LICENSED MATERIAL AS-IS
         AND AS-AVAILABLE, AND MAKES NO REPRESENTATIONS OR WARRANTIES OF
         ANY KIND CONCERNING THE LICENSED MATERIAL, WHETHER EXPRESS,
         IMPLIED, STATUTORY, OR OTHER. THIS INCLUDES, WITHOUT LIMITATION,
         WARRANTIES OF TITLE, MERCHANTABILITY, FITNESS FOR A PARTICULAR
         PURPOSE, NON-INFRINGEMENT, ABSENCE OF LATENT OR OTHER DEFECTS,
         ACCURACY, OR THE PRESENCE OR ABSENCE OF ERRORS, WHETHER OR NOT
         KNOWN OR DISCOVERABLE. WHERE DISCLAIMERS OF WARRANTIES ARE NOT
         ALLOWED IN FULL OR IN PART, THIS DISCLAIMER MAY NOT APPLY TO YOU.

      b. TO THE EXTENT POSSIBLE, IN NO EVENT WILL THE LICENSOR BE LIABLE
         TO YOU ON ANY LEGAL THEORY (INCLUDING, WITHOUT LIMITATION,
         NEGLIGENCE) OR OTHERWISE FOR ANY DIRECT, SPECIAL, INDIRECT,
         INCIDENTAL, CONSEQUENTIAL, PUNITIVE, EXEMPLARY, OR OTHER LOSSES,
         COSTS, EXPENSES, OR DAMAGES ARISING OUT OF THIS PUBLIC LICENSE OR
         USE OF THE LICENSED MATERIAL, EVEN IF THE LICENSOR HAS BEEN
         ADVISED OF THE POSSIBILITY OF SUCH LOSSES, COSTS, EXPENSES, OR
         DAMAGES. WHERE A LIMITATION OF LIABILITY IS NOT ALLOWED IN FULL OR
         IN PART, THIS LIMITATION MAY NOT APPLY TO YOU.

      c. The disclaimer of warranties and limitation of liability provided
         above shall be interpreted in a manner that, to the extent
         possible, most closely approximates an absolute disclaimer and
         waiver of all liability.


    Section 6 -- Term and Termination.

      a. This Public License applies for the term of the Copyright and
         Similar Rights licensed here. However, if You fail to comply with
         this Public License, then Your rights under this Public License
         terminate automatically.

      b. Where Your right to use the Licensed Material has terminated under
         Section 6(a), it reinstates:

           1. automatically as of the date the violation is cured, provided
              it is cured within 30 days of Your discovery of the
              violation; or

           2. upon express reinstatement by the Licensor.

         For the avoidance of doubt, this Section 6(b) does not affect any
         right the Licensor may have to seek remedies for Your violations
         of this Public License.

      c. For the avoidance of doubt, the Licensor may also offer the
         Licensed Material under separate terms or conditions or stop
         distributing the Licensed Material at any time; however, doing so
         will not terminate this Public License.

      d. Sections 1, 5, 6, 7, and 8 survive termination of this Public
         License.


    Section 7 -- Other Terms and Conditions.

      a. The Licensor shall not be bound by any additional or different
         terms or conditions communicated by You unless expressly agreed.

      b. Any arrangements, understandings, or agreements regarding the
         Licensed Material not stated herein are separate from and
         independent of the terms and conditions of this Public License.


    Section 8 -- Interpretation.

      a. For the avoidance of doubt, this Public License does not, and
         shall not be interpreted to, reduce, limit, restrict, or impose
         conditions on any use of the Licensed Material that could lawfully
         be made without permission under this Public License.

      b. To the extent possible, if any provision of this Public License is
         deemed unenforceable, it shall be automatically reformed to the
         minimum extent necessary to make it enforceable. If the provision
         cannot be reformed, it shall be severed from this Public License
         without affecting the enforceability of the remaining terms and
         conditions.

      c. No term or condition of this Public License will be waived and no
         failure to comply consented to unless expressly agreed to by the
         Licensor.

      d. Nothing in this Public License constitutes or may be interpreted
         as a limitation upon, or waiver of, any privileges and immunities
         that apply to the Licensor or You, including from the legal
         processes of any jurisdiction or authority.


    =======================================================================

    Creative Commons is not a party to its public
    licenses. Notwithstanding, Creative Commons may elect to apply one of
    its public licenses to material it publishes and in those instances
    will be considered the “Licensor.” The text of the Creative Commons
    public licenses is dedicated to the public domain under the CC0 Public
    Domain Dedication. Except for the limited purpose of indicating that
    material is shared under a Creative Commons public license or as
    otherwise permitted by the Creative Commons policies published at
    creativecommons.org/policies, Creative Commons does not authorize the
    use of the trademark "Creative Commons" or any other trademark or logo
    of Creative Commons without its prior written consent including,
    without limitation, in connection with any unauthorized modifications
    to any of its public licenses or any other arrangements,
    understandings, or agreements concerning use of licensed material. For
    the avoidance of doubt, this paragraph does not form part of the
    public licenses.

    Creative Commons may be contacted at creativecommons.org.

-------------------------------------------------------------------------------

https://github.com/frenic/csstype

-------------------------------------------------------------------------------

    Copyright (c) 2017-2018 Fredrik Nicol

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/kilian/electron-to-chromium

-------------------------------------------------------------------------------

    Copyright 2018 Kilian Valkhof

    Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/lukeed/escalade

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) Luke Edwards <luke.edwards05@gmail.com> (lukeed.com)

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/Rich-Harris/estree-walker

-------------------------------------------------------------------------------

    Copyright (c) 2015-20 [these people](https://github.com/Rich-Harris/estree-walker/graphs/contributors)

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/NaturalIntelligence/fast-xml-parser

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2017 Amit Kumar Gupta

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/rich-harris/magic-string

-------------------------------------------------------------------------------

    Copyright 2018 Rich Harris

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/ai/nanoid

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright 2017 Andrey Sitnik <andrey@sitnik.ru>

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/chicoxyzzy/node-releases

-------------------------------------------------------------------------------

    The MIT License

    Copyright (c) 2017 Sergey Rubanov (https://github.com/chicoxyzzy)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/alexeyraspopov/picocolors

-------------------------------------------------------------------------------

    ISC License

    Copyright (c) 2021 Alexey Raspopov, Kostiantyn Denysov, Anton Verinov

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/vuejs/pinia

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright (c) 2019-present Eduardo San Martin Morote

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/postcss/postcss

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright 2013 Andrey Sitnik <andrey@sitnik.ru>

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/7rulnik/source-map-js

-------------------------------------------------------------------------------

    Copyright (c) 2009-2011, Mozilla Foundation and contributors
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

    * Neither the names of the Mozilla Foundation nor the names of project
      contributors may be used to endorse or promote products derived from this
      software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

https://github.com/mozilla/source-map

-------------------------------------------------------------------------------

    Copyright (c) 2009-2011, Mozilla Foundation and contributors
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

    * Neither the names of the Mozilla Foundation nor the names of project
      contributors may be used to endorse or promote products derived from this
      software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

-------------------------------------------------------------------------------

https://github.com/Rich-Harris/sourcemap-codec

-------------------------------------------------------------------------------

    The MIT License

    Copyright (c) 2015 Rich Harris

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/NaturalIntelligence/strnum

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2021 Natural Intelligence

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/Microsoft/TypeScript

-------------------------------------------------------------------------------

    Apache License

    Version 2.0, January 2004

    http://www.apache.org/licenses/

    TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

    1. Definitions.

    "License" shall mean the terms and conditions for use, reproduction, and distribution as defined by Sections 1 through 9 of this document.

    "Licensor" shall mean the copyright owner or entity authorized by the copyright owner that is granting the License.

    "Legal Entity" shall mean the union of the acting entity and all other entities that control, are controlled by, or are under common control with that entity. For the purposes of this definition, "control" means (i) the power, direct or indirect, to cause the direction or management of such entity, whether by contract or otherwise, or (ii) ownership of fifty percent (50%) or more of the outstanding shares, or (iii) beneficial ownership of such entity.

    "You" (or "Your") shall mean an individual or Legal Entity exercising permissions granted by this License.

    "Source" form shall mean the preferred form for making modifications, including but not limited to software source code, documentation source, and configuration files.

    "Object" form shall mean any form resulting from mechanical transformation or translation of a Source form, including but not limited to compiled object code, generated documentation, and conversions to other media types.

    "Work" shall mean the work of authorship, whether in Source or Object form, made available under the License, as indicated by a copyright notice that is included in or attached to the work (an example is provided in the Appendix below).

    "Derivative Works" shall mean any work, whether in Source or Object form, that is based on (or derived from) the Work and for which the editorial revisions, annotations, elaborations, or other modifications represent, as a whole, an original work of authorship. For the purposes of this License, Derivative Works shall not include works that remain separable from, or merely link (or bind by name) to the interfaces of, the Work and Derivative Works thereof.

    "Contribution" shall mean any work of authorship, including the original version of the Work and any modifications or additions to that Work or Derivative Works thereof, that is intentionally submitted to Licensor for inclusion in the Work by the copyright owner or by an individual or Legal Entity authorized to submit on behalf of the copyright owner. For the purposes of this definition, "submitted" means any form of electronic, verbal, or written communication sent to the Licensor or its representatives, including but not limited to communication on electronic mailing lists, source code control systems, and issue tracking systems that are managed by, or on behalf of, the Licensor for the purpose of discussing and improving the Work, but excluding communication that is conspicuously marked or otherwise designated in writing by the copyright owner as "Not a Contribution."

    "Contributor" shall mean Licensor and any individual or Legal Entity on behalf of whom a Contribution has been received by Licensor and subsequently incorporated within the Work.

    2. Grant of Copyright License. Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable copyright license to reproduce, prepare Derivative Works of, publicly display, publicly perform, sublicense, and distribute the Work and such Derivative Works in Source or Object form.

    3. Grant of Patent License. Subject to the terms and conditions of this License, each Contributor hereby grants to You a perpetual, worldwide, non-exclusive, no-charge, royalty-free, irrevocable (except as stated in this section) patent license to make, have made, use, offer to sell, sell, import, and otherwise transfer the Work, where such license applies only to those patent claims licensable by such Contributor that are necessarily infringed by their Contribution(s) alone or by combination of their Contribution(s) with the Work to which such Contribution(s) was submitted. If You institute patent litigation against any entity (including a cross-claim or counterclaim in a lawsuit) alleging that the Work or a Contribution incorporated within the Work constitutes direct or contributory patent infringement, then any patent licenses granted to You under this License for that Work shall terminate as of the date such litigation is filed.

    4. Redistribution. You may reproduce and distribute copies of the Work or Derivative Works thereof in any medium, with or without modifications, and in Source or Object form, provided that You meet the following conditions:

    You must give any other recipients of the Work or Derivative Works a copy of this License; and

    You must cause any modified files to carry prominent notices stating that You changed the files; and

    You must retain, in the Source form of any Derivative Works that You distribute, all copyright, patent, trademark, and attribution notices from the Source form of the Work, excluding those notices that do not pertain to any part of the Derivative Works; and

    If the Work includes a "NOTICE" text file as part of its distribution, then any Derivative Works that You distribute must include a readable copy of the attribution notices contained within such NOTICE file, excluding those notices that do not pertain to any part of the Derivative Works, in at least one of the following places: within a NOTICE text file distributed as part of the Derivative Works; within the Source form or documentation, if provided along with the Derivative Works; or, within a display generated by the Derivative Works, if and wherever such third-party notices normally appear. The contents of the NOTICE file are for informational purposes only and do not modify the License. You may add Your own attribution notices within Derivative Works that You distribute, alongside or as an addendum to the NOTICE text from the Work, provided that such additional attribution notices cannot be construed as modifying the License. You may add Your own copyright statement to Your modifications and may provide additional or different license terms and conditions for use, reproduction, or distribution of Your modifications, or for any such Derivative Works as a whole, provided Your use, reproduction, and distribution of the Work otherwise complies with the conditions stated in this License.

    5. Submission of Contributions. Unless You explicitly state otherwise, any Contribution intentionally submitted for inclusion in the Work by You to the Licensor shall be under the terms and conditions of this License, without any additional terms or conditions. Notwithstanding the above, nothing herein shall supersede or modify the terms of any separate license agreement you may have executed with Licensor regarding such Contributions.

    6. Trademarks. This License does not grant permission to use the trade names, trademarks, service marks, or product names of the Licensor, except as required for reasonable and customary use in describing the origin of the Work and reproducing the content of the NOTICE file.

    7. Disclaimer of Warranty. Unless required by applicable law or agreed to in writing, Licensor provides the Work (and each Contributor provides its Contributions) on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied, including, without limitation, any warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining the appropriateness of using or redistributing the Work and assume any risks associated with Your exercise of permissions under this License.

    8. Limitation of Liability. In no event and under no legal theory, whether in tort (including negligence), contract, or otherwise, unless required by applicable law (such as deliberate and grossly negligent acts) or agreed to in writing, shall any Contributor be liable to You for damages, including any direct, indirect, special, incidental, or consequential damages of any character arising as a result of this License or out of the use or inability to use the Work (including but not limited to damages for loss of goodwill, work stoppage, computer failure or malfunction, or any and all other commercial damages or losses), even if such Contributor has been advised of the possibility of such damages.

    9. Accepting Warranty or Additional Liability. While redistributing the Work or Derivative Works thereof, You may choose to offer, and charge a fee for, acceptance of support, warranty, indemnity, or other liability obligations and/or rights consistent with this License. However, in accepting such obligations, You may act only on Your own behalf and on Your sole responsibility, not on behalf of any other Contributor, and only if You agree to indemnify, defend, and hold each Contributor harmless for any liability incurred by, or claims asserted against, such Contributor by reason of your accepting any such warranty or additional liability.

    END OF TERMS AND CONDITIONS

-------------------------------------------------------------------------------

https://github.com/browserslist/update-db

-------------------------------------------------------------------------------

    The MIT License (MIT)

    Copyright 2022 Andrey Sitnik <andrey@sitnik.ru> and other contributors

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

-------------------------------------------------------------------------------

https://github.com/antfu/vue-demi

-------------------------------------------------------------------------------

    MIT License

    Copyright (c) 2020-present, Anthony Fu

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
