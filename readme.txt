******************************************************
Tweeta
******************************************************
This application was built for the future of mobile conference and is not a fully featured, fully QA'd application.
There are some known issues along with many unimplemented features.
The code is intended to show off some of the things that were discussed in our FOM talk.


******************************************************
*TWITTER KEYS*
******************************************************
To run this app you will need to add 2 twitter keys.
You'll first need to register an app with twitter.
This app also uses x-Auth so you would require that, or rewrite the login process to support web-auth.

Change these 2 values in the code
public static string CONSUMER_SECRET = "{CONSUMERSECRET}";
public static string CONSUMER_KEY = "{CONSUMERKEY}";


******************************************************
** Known Issues **
******************************************************

 * Application will not work without valid keys as it uses xAuth to authenticate.
 * Data caching isnt fully implemented
 * Some screens are not showing progress bars
 * Some screens dont have transitions configured.
 * Twit pic integration is not implemented.