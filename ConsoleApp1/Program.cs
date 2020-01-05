using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using BililiveRecorder.Core;
using BililiveRecorder.FlvProcessor;
using CommandLine;

namespace ConsoleApp1
{
    class Program
    {
        private static int roomid = 528819;
        public static IContainer Container { get; private set; }
        public static ILifetimeScope RootScope { get; private set; }
        public static IRecorder Recorder { get; private set; }

        static void Main(string[] _)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<FlvProcessorModule>();
            builder.RegisterModule<CoreModule>();
            Container = builder.Build();
            RootScope = Container.BeginLifetimeScope("recorder_root");
            Recorder = RootScope.Resolve<IRecorder>();
            if (!Recorder.Initialize(System.IO.Directory.GetCurrentDirectory()))
            {
                Console.WriteLine("Initialize Error");
                return;
            }
            Parser.Default
                .ParseArguments<CommandLineOption>(Environment.GetCommandLineArgs())
                .WithParsed(Run);
        }

        private static void Run(CommandLineOption option)
        {
            roomid = option.RoomID;
            Recorder.Config.AvoidTxy = true;

            if (Recorder.Where(r => r.RoomId == roomid).Count() == 0)
            {
                Recorder.AddRoom(roomid);
            }
            Task.WhenAll(Recorder.Where(r => r.RoomId == roomid).Select(x => Task.Run(() => x.Start()))).Wait();

            using CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task task = Task.Run(() => NumValue(tokenSource.Token), tokenSource.Token);

            Console.CancelKeyPress += (sender, e) =>
            {
                Task.WhenAll(Recorder.Where(r => r.RoomId == roomid).Select(x => Task.Run(() => x.StopRecord()))).Wait();
                tokenSource.Cancel();
            };
            task.Wait();
        }

        private static void NumValue(CancellationToken token)
        {
            bool cancellationFlag = false;
            token.Register(() =>
            cancellationFlag = true);
            Console.WriteLine("starting the task");
            while (true)
            {
                if (cancellationFlag)
                {
                    Console.WriteLine("task stop");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }
    }
}
