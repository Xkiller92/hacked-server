public class HackaInst{
    public bool hasStarted { get; set; }
    public bool finished { get; set; }
    private string name { get; set; }
    private int challangeCount { get; set; }
    private int duratioonInMinutes { get; set; }
    private int authCount = 0;
    public Dictionary<AuthToken, UserInfo> users = new();
    private System.Timers.Timer countdown;
    public ScoreHandler scoreHandler;

    public HackaInst(string name, int challangeCount, int durationInMinutes)
    {
        this.name = name;
        this.challangeCount = challangeCount;
        this.duratioonInMinutes = durationInMinutes;
        hasStarted = false;  
        finished = false;
        scoreHandler = new ScoreHandler(0, challangeCount);
    }

    public AuthToken RegesterUser(UserInfo user){
        AuthToken token = new AuthToken{Value = authCount++};
        users.Add(token, user);

        scoreHandler.AdjustScoreForAll(users);

        return token;   
    }

    public Empty Withdraw(AuthToken token){
        users.Remove(token);
        //adjust score in reverse
        return new Empty();
    }

    public void Start(){
        countdown = new System.Timers.Timer();
        countdown.AutoReset = false;
        countdown.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
        countdown.Interval = duratioonInMinutes * 60 * 1000;
        countdown.Start();
        Console.WriteLine("started");
        hasStarted = true;
    }

    private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
    {   
        finished = true;
        Console.WriteLine("finished");
        countdown.Stop();
    }

    public async Task<HackathonResults> CalculateScoreAsync(){
        return await Task.FromResult(
            new HackathonResults()
        );
    }
}