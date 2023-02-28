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

        /// <summary>
        /// >0: Finite ticks per second. <br/>
        /// otherwise: Full speed running. 
        /// </summary>
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
        public bool Running => runTask != null && !paused;

        CancellationTokenSource? cancellationSource;
        public CancellationToken cancellationToken;

        /// <summary>
        /// Start running space. <br/>
        /// See <see cref="PauseAsync"/> if you want to pause the space. <br/>
        /// See <see cref="UnpauseAsync"/> if you want to continue a paused space. <br/>
        /// See <see cref="StopAsync(float)"/> if you want to stop the space. 
        /// </summary>
        /// <returns></returns>
        public Task StartAndRunAsync()
        {
            if (Running)
            {
                Log("Space is running. ");
                return runTask!;
            }
            else if(runTask != null)
            {
                Log("Space has already big-banged, will return existing task instance. ");
                return runTask;
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
                    var task = Task.Run(() => StopAsync(timeoutTicks));
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


        public async Task PauseAsync()
        {
            if (!Running)
            {
                Log("Failed to pause: space is not running. ");
                return;
            };
            pauseRequest = true;
            while(!paused)
            {
                await Task.Delay(100);
            }
            await OnPause();
            Log("Paused sapce. ");
        }

        public async Task UnpauseAsync()
        {
            if (runTask == null)
            {
                Log("Call RunAsync first. ");
                return;
            }
            if (Running) return;
            await OnUnpause();
            pauseRequest = false;
            while (paused)
            {
                await Task.Delay(100);
            }
            Log("Continued space.");
        }

        public async Task StopAsync(float timeoutTicks = 3)
        {
            if (cancellationSource == null) return;
            if (runTask == null) return;
            try
            {
                if (!paused)
                {
                    await PauseAsync();
                }
                await OnStop();

                cancellationSource.Cancel();
                var timeout = TicksPerSecond <= 0 ? 3 : timeoutTicks / TicksPerSecond;
                await runTask.WaitAsync(TimeSpan.FromSeconds(timeout));
                if (runTask.IsCompleted)
                {
                    Log($"Space is successfully canceled. ");
                }
                else
                {
                    Log($"Failed to cancel task in {timeout} [sec]. ");
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

        public bool pauseRequest = false;
        public bool paused = false;

        

        async Task TaskCore()
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
                            goto END;
                        }
                        if (PrintTickInfo)  Log($"Tick {tick,7}");
                        if (await routine.MoveNextAsync())
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
                            goto END;
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
                        goto END;
                    }

                    ++tick;

                    double waitTime = tps <= 0 ? 0 :((tick * 1000 / tps) 
                        - ((int)(Environment.TickCount64- startMs)));

                    if (waitTime > 0) 
                        await Task.Delay((int)waitTime);

                    while (pauseRequest)
                    {
                        paused = true;
                        if(cancellationToken.IsCancellationRequested)
                        {
                            goto END;
                        }
                        await Task.Delay(1000);
                    }
                    paused = false;
                }
            END:;
            }
            catch (Exception e)
            {
                Log($"Space crashed error: {e.Message}");
            }
            finally
            {
            }
        }

        async IAsyncEnumerator<object?> GetSpaceRoutine()
        {
            Log("Big banging...");
            await OnStart();
            Log("Big bang done. ");
            yield return null;

            var routine = GetBoxRoutine();
            while(await routine.MoveNextAsync())
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
