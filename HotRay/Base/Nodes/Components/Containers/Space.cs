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
        public Space():base() { }
        public Space(Space other):base(other) 
        {
            TicksPerSecond = other.TicksPerSecond;
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
        public bool PrintTickInfo
        {
            get;set;
        }

        bool _running = false;
        public bool Running => _running;


        public Task RunAsync()
        {
            var cancelSrc = new CancellationTokenSource();
            var ctoken = cancelSrc.Token;

            var task = Task.Run(TaskCore, ctoken);
            
            Console.CancelKeyPress += (obj, arg) =>
            {
                try
                {
                    var timeout = 3.0 / TicksPerSecond;
                    Log($"Detected cancel event {arg.SpecialKey}. Trying cancel task in {timeout:F2} [sec]...");
                    SendCancelSignal();
                    if (!task.Wait(TimeSpan.FromSeconds(timeout)))
                    {
                        cancelSrc.Cancel();
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
            return task;
        }

        public void Run() => TaskCore();

        void TaskCore()
        {
            if (_running)
            {
                Log("Space is running. ");
                return;
            }
            _running = true;
            try
            {
                var routine = GetSpaceRoutine();
                double tps = TicksPerSecond;

                
                int tick = 0;
                long startMs = Environment.TickCount64;

                while (true)
                {
                    
                    try
                    {
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
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log($"Error: {e.Message}");
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
                _running = false;
            }
        }

        IEnumerator GetSpaceRoutine()
        {
            Log("Entry...");
            OnEntry();
            Log("Entry done. ");
            yield return null;

            var routine = GetRoutine();
            while(routine.MoveNext())
            {
                yield return null;
            }
        }


    }
}
