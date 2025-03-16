using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
namespace Sport_Retake_scheuer;

public class HttpRequestParser
{
    public static async Task<HttpRequest> ParseFromStreamAsync(Stream stream)
    {
        //reader soll nach dem Lesen nicht geschlossen werden
        using var
            reader = new StreamReader(stream, Encoding.UTF8, false, 8192,
                leaveOpen: true); // anm. using var -> sorgt hier dafür, dass der Reader am Ende des Gültikeitszeitraums automatisch freigegeben wird, also reader.Dispose() wird automatisch aufgerufen 
        var requestLine = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(requestLine))
        {
            throw new Exception("Leere oder ungültige Request-Line.");
        }

        //Request ist potenziell Gültig -> nächster Schritt ist den request in Tokens aufzuteilen
        var tokens = requestLine.Split(' ');
        if (tokens.Length < 3)
        {
            throw new Exception("FEHLER: Request muss mindestens Methode, Pfad und Body enthalten!");
        }

        var method = tokens[0];
        var path = tokens[1];
        var httpVersion = tokens[2];

        // Die Header Zeilen werden nacheinander gelesen bis ""
        string?
            line; // anm. ? -> “nullable string”, also ein string, der auch den Wert null annehmen darf. Macht hier sinn weil nur mindestens 
        string? authorization = null;
        int contentLength = 0;
        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
        {
            if(line.StartsWith("Authorization:", StringComparison.OrdinalIgnoreCase))
            {
                authorization =line.Substring("Authorization:".Length).Trim(); //Wert von auth Header wird gespeichert
            }
            else if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            {
                 var content_legth = line.Substring("Content-Length:".Length).Trim();
                 int.TryParse(content_legth, out contentLength);  // anm. out contentLength -> gibt an wie lange der Body ist, nötig um zu wissen wie viele Bytes nach dem HTTP-Header noch eingelesen werden müssen
            }
        }

        string body = "";
        if (contentLength > 0) //wenn es einen Body gibt wird der boddy in einen buffer gelesen  und in einen String gespeichert
        {
            var buffer = new char[contentLength];
            int read = await reader.ReadBlockAsync(buffer, 0, contentLength);
            body = new string(buffer);
        }

        return new HttpRequest
        {
            Method = method,
            Path = path,
            Body = body,
            Authorization = authorization ?? string.Empty
        };
    }
}