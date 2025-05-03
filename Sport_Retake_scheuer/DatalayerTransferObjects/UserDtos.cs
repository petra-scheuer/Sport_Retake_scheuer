namespace Sport_Retake_scheuer.DatalayerTransferObjects;

public class UserDtos
{
    public string Username { get; set; }

    public int Elo { get; set; }

}

public class UserStatsDto
{
    public string Username { get; set; }
    public int Elo { get; set; }
    public int TotalPushupCount { get; set; }
}

public class UserDataDto
{
    public string Username { get; set; }
    
    public string Token { get; set; }
}