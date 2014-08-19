When deploying to a new server, be sure to follow the below steps! Othewise, the application will not work with a very cryping "Access Denied" error.

After deploying to a new server, make sure you enable an access control rule for the RavenDB management studio.
The system expects it to be available on port 8888.
To do so, run this command as admin on the server: netsh http add urlacl url=http://+:8888/ user="IIS AppPool\BeerNotifier"
If the app pool is different, then change the app pool name to match

Also, make sure you have the below in your web.config, otherwise the scheduler may get disabled when the app pool restarts
<applicationInitialization doAppInitAfterRestart="true" remapManagedRequestsTo="Startup.html">
      <add initializationPage="/" />
    </applicationInitialization>