using HotRay.Base.Nodes.Sources;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HotRay.Base.Nodes.Components.Containers
{
    public class Space:Box
    {



        public Space():base() 
        {
            TicksPerSecond = 10;
            MaxTick = -1;
            PrintTickInfo = false;
        }
        public Space(Space other):base(other) 
        {
            TicksPerSecond = other.TicksPerSecond;
            MaxTick = other.MaxTick;
            PrintTickInfo = other.PrintTickInfo;
        }

        public override NodeBase CloneNode()
        {
            return new Space(this);
        }

        public int TicksPerSecond
        {
            get;set;
        }
        /// <summary>
        /// -1: No limitation.
        /// </summary>
        public int MaxTick
        {
            get;set;
        }

        public bool PrintTickInfo
        {
            get;set;
        }


        



        Task? runTask = null;
        public bool Running => runTask != null;

        CancellationTokenSource? cancellationSource;
        public CancellationToken cancellationToken;

        public Task RunAsync()
        {
            if (Running)
            {
                Log("Space is running. ");
                return runTask!;
            }

            cancellationSource = new CancellationTokenSource();
            cancellationToken = cancellationSource.Token;

            runTask = Task.Run(TaskCore, cancellationToken);
            
            Console.CancelKeyPress += (obj, arg) =>
            {
                try
                {
                    var timeoutTicks = 3.0f;
                    Log($"Detected cancel event {arg.SpecialKey}. Trying cancel task in {timeoutTicks:F2} [ticks]...");
                    var task = Task.Run(() => CancelAsync(timeoutTicks));
                    if(task.IsFaulted)
                    {
                        throw task.Exception ?? new Exception("Unknown");
                    }
                }
                catch(Exception e)
                {
                    Log(e.Message);
                }
                finally
                {
                    arg.Cancel = true;
                }
            };
            
            return runTask;
        }


        public async Task CancelAsync(float timeoutTicks = 3.0f)
        {
            if (cancellationSource == null) return;
            if (runTask == null) return;
            try
            {
                cancellationSource.Cancel();
                var timeout = timeoutTicks / TicksPerSecond;

                cancellationSource.Cancel();
                await runTask.WaitAsync(TimeSpan.FromSeconds(timeout));
                if (runTask.IsCompleted)
                {
                    
                }
                else
                {
                    Log($"Failed to cancel task in {timeoutTicks} [ticks]. ");
                }
            }
            finally
            {
                cancellationSource.Dispose();
                runTask = null;
            }
        }


        bool ReachedMaxTick(int curTick)
        {
            if (MaxTick < 0) return false;
            return curTick >= MaxTick;
        }

        void TaskCore()
        {
            try
            {
                Init();
                var routine = GetSpaceRoutine();
                double tps = TicksPerSecond;

                
                int tick = 0;
                long startMs = Environment.TickCount64;

                while (true)
                {
                    
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            if(PrintTickInfo)
                                Log($"Cancelled at tick: {tick, 7}");
                            break;
                        }
                        if (PrintTickInfo)  Log($"Tick {tick,7}");
                        if (routine.MoveNext())
                        {
                            if (PrintTickInfo)
                            {
                                Log($"Tick {tick,7} done. \n");
                            }
                        }
                        else
                        {
                            if (PrintTickInfo)
                            {
                                Log($"Tick {tick,7} done. Terminate signal detected. \n");
                            }
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log($"Error: {e.Message}");
                    }

                    if (ReachedMaxTick(tick))
                    {
                        if(PrintTickInfo) 
                            Log($"Reached max tick {MaxTick}. Terminating... \n");
                        break;
                    }

                    ++tick;

                    double waitTime = (tick * 1000 / tps) 
                        - ((int)(Environment.TickCount64- startMs));

                    if (waitTime > 0) 
                        Thread.Sleep((int)waitTime);
                }
            }
            catch (Exception e)
            {
                Log($"Space crashed error: {e.Message}");
            }
            finally
            {
            }
        }

        IEnumerator GetSpaceRoutine()
        {
            Log("Entry...");
            OnEntry();
            Log("Entry done. ");
            yield return null;

            var routine = GetBoxRoutine();
            while(routine.MoveNext())
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                var status = routine.Current;
                if(status.Finished)
                {
                    yield break;
                }
                yield return null;
            }
        }

        public override Space? GetCurrentSpace()
        {
            return this;
        }
    }
}
