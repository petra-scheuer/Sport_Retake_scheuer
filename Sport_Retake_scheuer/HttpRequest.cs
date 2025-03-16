namespace Sport_Retake_scheuer;

// Stellt klar, woraus ein Request besteht -> 
// 1. Methode (GET, POST, PUT, PATCH, DELETE)
// 2. Path ( z.b. /users)
// 3. Body ( Body Daten)
// 4. Authorization (Bearer Token)
public class HttpRequest
{
    public string Method { get; set; } = string.Empty; // anm. zu string.Empty ->  macht man um sie gleich mit einem Leeren String zu initialoisieren, das schützt vor NULL values (weil es ein "" ist) und man vermeidet damit NullReferenceExceptions.

    public string Path { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
    
    public string Authorization { get; set; } = string.Empty;
}