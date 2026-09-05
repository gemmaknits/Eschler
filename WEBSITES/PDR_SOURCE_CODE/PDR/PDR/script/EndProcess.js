try
{
var splash = null;
//if(upLevel) 
//{
   splash = document.getElementById("splashScreen");
//}
}
catch(e)
{
if(ns4)
{
    splash = document.splashScreen;
}
else if(ie4) 
{
   splash = document.all.splashScreen;
}
}
if(splash != null)
 hideObject(splash);

