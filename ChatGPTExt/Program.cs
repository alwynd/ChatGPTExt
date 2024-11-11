namespace ChatGPTExt
{
    public enum GPTMode
    {
        ListModels = 0,
        Request = 1
    }

    /// <summary>
    /// Main entry class.
    /// </summary>
    public class Program
    {
        private static readonly ChatGPT InstanceGPT = new ChatGPT();

        private static GPTMode mode = GPTMode.ListModels;

        /// <summary>
        /// Debug to console.
        /// </summary>
        public static void Debug(string msg)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:fff} - {msg}");
        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        public static async Task Main(string[] args)
        {

            Debug($"Usage: [-api-key APIKEY] [-org-id ORGANIZATIONID] [-project-id PROJECTID] OPTIONS{Environment.NewLine}" +
                  $"Where Options are:{Environment.NewLine}" +
                  $"\t [-list-models] Lists currently available gpt models." +
                  $"\t [-request TEXT] Code review/edit request.{Environment.NewLine}" +
                  $"\t\t [-Model gptmodel] optional{Environment.NewLine}" +
                  $"\t\t [-system SYSTEMMESSAGE] optional{Environment.NewLine}" +
                  $"");


            string prev = "";
            string request = "";
            args.ToList().ForEach(arg =>
            {
                Debug($"{nameof(Program)}.Main arg: {arg}");

                if (prev == "-api-key") InstanceGPT.ApiKey = arg;
                if (prev == "-org-id") InstanceGPT.OrganizationId = arg;
                if (prev == "-project-id") InstanceGPT.ProjectId = arg;
                if (prev == "-model") InstanceGPT.Model = arg;
                if (prev == "-system") InstanceGPT.SystemMessage = arg;
                if (prev == "-request")
                {
                    mode = GPTMode.Request;
                    request = arg;
                }

                prev = arg;

                if (arg == "-list-models")
                {
                    mode = GPTMode.ListModels;
                }
            });

            Debug($"{nameof(Program)}.Main projectId: {InstanceGPT.ProjectId}, organizationId: {InstanceGPT.OrganizationId}, apiKey: {InstanceGPT.ApiKey}");
            if (mode == GPTMode.ListModels)
            {
                await InstanceGPT.ListModels();
            }

            if (mode == GPTMode.Request)
            {
                await InstanceGPT.InvokeChatGPT(request);
            }

        }
    }
}
