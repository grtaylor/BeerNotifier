When deploying to a new server, be sure to follow the below steps! Othewise, the application will not work with a very cryping "Access Denied" error.

After deploying to a new server, make sure you enable an access control rule for the RavenDB management studio.
The system expects it to be available on port 8888.
To do so, run this command as admin on the server: netsh http add urlacl url=http://+:8888/ user="IIS AppPool\DefaultAppPool"
If the app pool is different, then change the app pool name to match