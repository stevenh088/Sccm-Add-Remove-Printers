<h1> Sccm Add/Remove Printers</h1>

<h2>Description</h2>
A tool to add or remove printers from to sccm using a csv, direct name, or from a printer server.

This tool could be modified to add another type of application in bulk, it would need modified in the code, however.

<h3>Usage</h3>
If you are not interested in the source code 

Change app_config.json in the release/debug folder to match your environment.

Requires rights to both sccm and a printerserver(s), the printer server in the config servers mainly as a default,
it can be changed after selecting the use printer server checkbox.

Use on a computer with the configuration manager console installed. (May not be necessary, haven't tested)

When adding printers the program will create an application, collection, and deploy the application to the collection and to an all printers collection as specified in the app_config.json The application and collection will be named the same as the printer. Simply add   users/devices to these collections to deploy the printer to them.

Adding a printer multiple times will modify it, however, doing so will create a new version in sccm, I would suggest removing and re adding the printer after 2 or 3 modifications. It is possible to remove the versions through powershell, or by modifying the code.

When removing printers the program will remove the application, its versions, and all deployments. The collection should be manually      removed. This was done intentionally.

By default the program will add the printers to a user collection, to add to a device collection the program can be started with an argument of "device". Example: cmd.exe /c "add_application.exe device"

<h3>Known Bugs</h3>
There are some rare situations where this application will crash when cancelling.
When attempting to deploy the same printer to both a user and device collection the program will not create the second one.

<h3>Other Notes</h3>
I do not claim to be the greatest programmer, this program may not be the "best" way to code, however I do attempt
to follow best practices. Any advice or upgrades is welcome.
<br>
Open source projects such as this and general help online have been extremely useful in my career. 
My hopes are that this along with anything else I produce will be a way to contribute back to the community and help others as I was.

There are some features and/or bugs that may be added/fixed in later releases.
