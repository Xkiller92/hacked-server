using System.Threading.Tasks;
using Grpc.Core;
using hacked_instance_handler;
using System.Timers;
using System.IO;
namespace hacked_instance_handler.Services;

public class HackathonService : Hackathon.HackathonBase
{
    private readonly ILogger<HackathonService> _logger;
    public HackathonService(ILogger<HackathonService> logger)
    {
        _logger = logger;
    }

    //admin client rpcs
    private static HackaInst hackathonInstance = null;
    public override Task<InstanceState> CheckInstancePresence(Empty request, Grpc.Core.ServerCallContext context)
    {
        State currState = State.Active;

        if(hackathonInstance == null){
            currState = State.Empty;
        }

        return Task.FromResult(new InstanceState{
            State = currState
        });
    }

    public override Task<Empty> CreateInstance(InstanceInfo request, ServerCallContext context)
    {
        hackathonInstance = new HackaInst(request.Name, request.ChallangeCount, request.DurationInMinutes);
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> StartInstance(Empty request, ServerCallContext context)
    {
        hackathonInstance.Start();
        return Task.FromResult(new Empty());
    }
    public override async Task<HackathonResults> StopInstance(Empty request, ServerCallContext context)
    {
        HackathonResults results = await hackathonInstance.CalculateScoreAsync();
        hackathonInstance = null;
        return await Task.FromResult(results);
    }

    //user client rpcs
    public override async Task CheckInstanceCurrentPresence(Empty request, IServerStreamWriter<InstanceState> responseStream, ServerCallContext context)
    {
        while (hackathonInstance is null || !hackathonInstance.hasStarted)
        {
            await responseStream.WriteAsync(new InstanceState{ State = State.Empty});      
        }

        await responseStream.WriteAsync(new InstanceState{ State = State.Running});
    }

    public override Task<AuthToken> Register(UserInfo request, ServerCallContext context)
    {   
        return Task.FromResult(hackathonInstance.RegesterUser(request));
    }

    public override Task<Empty> Withdraw(AuthToken request, ServerCallContext context)
    {
        return Task.FromResult(hackathonInstance.Withdraw(request));
    }


    public override async Task CheckHackathonFinished(Empty request, IServerStreamWriter<InstanceState> responseStream, ServerCallContext context)
    {
       while (hackathonInstance != null || !hackathonInstance.finished)
        {
            await responseStream.WriteAsync(new InstanceState{ State = State.Running});      
        }

        await responseStream.WriteAsync(new InstanceState{ State = State.Finished}); 
    }

    public override Task<InputResponseParams> RequestInputString(InputRequestParams request, ServerCallContext context)
    {
        string[] lines = File.ReadAllLines(@"./src/Resources/input.txt");

        return Task.FromResult(new InputResponseParams{
            Value = lines[request.ChallangeNumber]
        });
    }

    public override Task<AnswerState> ValidateAnswer(InputAnswerParams request, ServerCallContext context)
    {
        string[] lines = File.ReadAllLines(@"./src/Resources/input.txt");

        AnswerState state = new();

        if(request.Value == lines[request.ChallangeNumber]){
            state.Value = "good job you got it right!!!!";

            hackathonInstance.scoreHandler.ScoreUser(hackathonInstance.users[request.Token], request.ChallangeNumber);
        }
        else{
            state.Value = "oh no looks like you're answer is wrong :c";
        }

        return Task.FromResult(state);
    }
}