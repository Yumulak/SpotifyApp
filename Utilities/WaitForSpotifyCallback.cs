using System;
using System.Net;
using Microsoft.VisualBasic;

public static class WaitForSpotifyCallback
{
    public static string WaitForCallback(){
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/callback/");
        listener.Start();
        Console.WriteLine("Listening for callback...");

        var listenerContext = listener.GetContext();
        var request = listenerContext.Request.QueryString;
        var code = request.Get("code");

        var response = listenerContext.Response;
        string responseString = "<html><body>Authorization complete. You can close this window.</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();

        listener.Stop();

        return code;
    }
}