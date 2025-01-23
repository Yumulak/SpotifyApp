public static class WaitForSpotifyCallback
{
    public static string WaitForCallback(){

        //start http listener for callback
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/callback/");
        listener.Start();
        Console.WriteLine("Listening for callback...");

        //wait for callback from spotify and get the authorization code from the request
        var listenerContext = listener.GetContext();
        var request = listenerContext.Request.QueryString;
        string callbackReturnVal = null;
        string browserMessage = "Error: No code or error returned from Spotify. Check API communication logic.";
        try{
            callbackReturnVal = request.Get("code");
            browserMessage = "Authorization code received. You can close this window.";
        }
        catch(Exception e){
            callbackReturnVal = request.Get("error");
            browserMessage = "Error: " + callbackReturnVal;
        }
        finally{
        //send response to the browser and stop the listener
            var response = listenerContext.Response;
            string responseString = $"<html><body>{browserMessage}</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            listener.Stop();
        }
        return callbackReturnVal;
    }
}