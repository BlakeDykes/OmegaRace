using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using System.Linq;

namespace OmegaRace
{
    class CommandQueueManager
    {


        private static CommandQueueManager instance = null;
        private List<QueueBase> _Queues;
        private CommandCollection _Commands;

        private CommandQueueManager()
        {
            _Queues = new List<QueueBase>();
            _Commands = new CommandCollection();
        }

        public static void Initialize()
        {
            instance = new CommandQueueManager();
        }

        public static QueueBase Add(QUEUE_NAME name, QUEUE_TYPE type, NetworkEnv env, RECORDER_TYPE recorderType)
        {
            CommandQueueManager inst = GetInstance();

            QueueBase q;

            switch (type)
            {
                case QUEUE_TYPE.LOCAL:
                    q = new LocalCommandQueue(name, env, PlaybackManager.CreateRecorder(name, recorderType));
                    break;
                case QUEUE_TYPE.NETWORK:
                    q = new CommandQueue(name, env, PlaybackManager.CreateRecorder(name, recorderType));
                    break;

                case QUEUE_TYPE.NULL:
                case QUEUE_TYPE.PLAYBACK:
                default:
                    q = null;
                    Debug.Assert(false);
                    break;
            }

            inst._Queues.Add(q);

            return q;
        }

        public static QueueBase Add(QUEUE_NAME name, QUEUE_TYPE type, RECORDER_TYPE recorderType)
        {
            CommandQueueManager inst = GetInstance();

            QueueBase q;

            switch (type)
            {
                case QUEUE_TYPE.LOCAL:
                    q = new LocalCommandQueue(name, PlaybackManager.CreateRecorder(name, recorderType));
                    break;
                case QUEUE_TYPE.NETWORK:
                    q = new CommandQueue(name, PlaybackManager.CreateRecorder(name, recorderType));
                    break;
                case QUEUE_TYPE.PLAYBACK:
                    q = new PlaybackQueue(name);
                    break;
                case QUEUE_TYPE.NULL:
                    q = new NullQueue(name);
                    break;
                default:
                    q = null;
                    Debug.Assert(false);
                    break;
            }

            inst._Queues.Add(q);

            return q;
        }

        public static void ReturnCommand(Command com)
        {
            GetInstance()._Commands.ReturnCommand(com);
        }

        public static Command GetCommand(COMMAND_TYPE type)
        {
            return GetInstance()._Commands.GetCommand(type);
        }

        public static Command GetCommand(ref BinaryReader reader)
        {
            if (Command.GetType(ref reader) != COMMAND_TYPE.UNDEFINED)
            {
                CommandQueueManager inst = GetInstance();

                Command com = inst._Commands.GetCommand(ref reader);

                return com;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Route a command by name with an option to set a callback
        /// </summary>
        public static void RouteOut(COMMAND_TYPE com, byte subject = NetworkEnv.UNINITIALIZED_MESSAGE_ID, byte dest = NetworkEnv.SERVER_MESSAGE_ID, COMMAND_TYPE netCallback = COMMAND_TYPE.UNDEFINED)
        {
            if (com != COMMAND_TYPE.UNDEFINED)
            {
                CommandQueueManager inst = GetInstance();

                Command c = (Command)inst._Commands.GetCommand(com);
                c.SetDestAndSubject(dest, subject);
                c.NetCallback = netCallback;

                inst.privRouteOut(c);
            }
        }

        /// <summary>
        /// Routes a command to it's output queue
        /// </summary>
        public static void RouteOut(Command com)
        {
            if (com.Type != COMMAND_TYPE.UNDEFINED)
            {
                GetInstance().privRouteOut(com);
            }
        }

        /// <summary>
        /// Route a list of Commands to dest in the same payload using custom delivery settings
        /// </summary>

        public static void RouteOut(byte subject, byte dest, MessageDeliverySettings settings, params COMMAND_TYPE[] coms)
        {
            // Single Command case
            if (coms.Length == 1)
            {
                CommandQueueManager inst = GetInstance();

                Command c = (Command)inst._Commands.GetCommand(coms[0]);
                c.SetDestAndSubject(dest, subject);
                c.SetDeliverySettings(settings);

                inst.privRouteOut(c);
            }
            // Commands > 1 - use a delivery wrapper
            else if (coms.Length > 0)
            {
                CommandQueueManager inst = GetInstance();

                C_DeliveryWrapper deliveryCom = (C_DeliveryWrapper)CommandQueueManager.GetCommand(COMMAND_TYPE.C_DeliveryWrapper);
                deliveryCom.SetDestAndSubject(dest, subject);
                deliveryCom.SetDeliverySettings(settings);

                foreach (COMMAND_TYPE com in coms)
                {
                    deliveryCom.Add(com);
                }

                inst.privRouteOut(deliveryCom);
            }
        }

        /// <summary>
        /// Route a list of Commands to dest in the same payload using custom delivery settings
        /// </summary>

        public static void RouteOut(MessageDeliverySettings settings, params Command[] coms)
        {
            // Single Command case
            if (coms.Length == 1)
            {
                CommandQueueManager inst = GetInstance();
                coms[0].SetDeliverySettings(settings);

                inst.privRouteOut(coms[0]);
            }
            // Commands > 1 - use a delivery wrapper
            else if (coms.Length > 0)
            {
                CommandQueueManager inst = GetInstance();

                C_DeliveryWrapper deliveryCom = (C_DeliveryWrapper)CommandQueueManager.GetCommand(COMMAND_TYPE.C_DeliveryWrapper);

                deliveryCom.Destination = coms[0].Destination;
                deliveryCom.SetDeliverySettings(settings);

                foreach (Command com in coms)
                {
                    deliveryCom.Add(com);
                }

                inst.privRouteOut(deliveryCom);
            }
        }

        private void RouteOutNetCallback(COMMAND_TYPE com, byte destination)
        {
            if (com != COMMAND_TYPE.UNDEFINED)
            {
                Command c = (Command)_Commands.GetCommand(com);

                QueueBase queue = Find(c.OutQueue);

                if (queue.QueueType == QUEUE_TYPE.NETWORK)
                {
                    c.Destination = destination;

                    queue.Receive(c);
                }
                else
                {
                    ReturnCommand(c);
                }
            }
        }

        public static void RouteIn(Command com)
        {
            if (com.Type != COMMAND_TYPE.UNDEFINED)
            {
                GetInstance().privRouteIn(com);
            }
        }

        public static void RouteIn(ref BinaryReader reader)
        {
            if (Command.GetType(ref reader) != COMMAND_TYPE.UNDEFINED)
            {
                CommandQueueManager inst = GetInstance();

                Command com = inst._Commands.GetCommand(ref reader);

                inst.privRouteIn(com);
            }
        }

        public static void RouteIn(QUEUE_NAME queue, Command[] coms)
        {
            CommandQueueManager inst = GetInstance();

            inst.privRouteIn(queue, coms);
        }

        /// <summary>
        /// Routes a command to all connections
        /// </summary>
        /// <param name="sendToSelf">if true, routes command to it's input queue</param>
        public static void Broadcast(bool sendToSelf, Command com)
        {
            if (com.Type != COMMAND_TYPE.UNDEFINED)
            {
                CommandQueueManager inst = GetInstance();

                QueueBase outQueue = inst.Find(com.OutQueue);
                Debug.Assert(outQueue.Name != QUEUE_NAME.UNDEFINED);

                if (outQueue.QueueType == QUEUE_TYPE.LOCAL)
                {
                    outQueue.Receive(com);
                }
                else if (outQueue.QueueType == QUEUE_TYPE.NETWORK)
                {
                    if (sendToSelf)
                    {
                        Command selfCom = inst._Commands.GetCommand(com.Type);
                        selfCom.Copy(com);

                        QueueBase inQueue = inst.Find(selfCom.InQueue);

                        inQueue.Receive(selfCom);
                    }

                    outQueue.Broadcast(com);
                }

                inst.RouteOutNetCallback(com.NetCallback, com.Origin);
            }
        }

        /// <summary>
        /// Route an array of commands to all connections
        /// </summary>
        /// <param name="sendToSelf">if true, routes command to it's input queue</param>

        public static void Broadcast(bool sendToSelf, MessageDeliverySettings settings, params Command[] coms)
        {
            if (coms.Length == 1)
            {
                coms[0].SetDeliverySettings(settings);
                Broadcast(sendToSelf, coms[0]);
            }
            else if (coms.Length > 1)
            {
                C_DeliveryWrapper deliveryCom = (C_DeliveryWrapper)CommandQueueManager.GetCommand(COMMAND_TYPE.C_DeliveryWrapper);

                deliveryCom.Destination = coms[0].Destination;
                deliveryCom.SetDeliverySettings(settings);

                deliveryCom.Add(coms);

                Broadcast(sendToSelf, deliveryCom);
            }
        }

        private void privRouteOut(Command com)
        {
            QueueBase queue = Find(com.OutQueue);

            Debug.Assert(queue.Name != QUEUE_NAME.UNDEFINED);

            queue.Receive(com);
        }

        private void privRouteIn(Command com)
        {
            QueueBase queue = Find(com.InQueue);

            Debug.Assert(queue.Name != QUEUE_NAME.UNDEFINED);

            queue.Receive(com);

            RouteOut(com.NetCallback, com.Origin, com.Origin);
        }

        private void privRouteIn(QUEUE_NAME queue, Command[] coms)
        {
            QueueBase q = Find(queue);

            if (q != null)
            {
                q.Receive(coms);
            }
        }

        private QueueBase Find(QUEUE_NAME name)
        {
            return _Queues.Find(x => x.Name == name);
        }
        public static void PrintStats()
        {
            GetInstance()._Commands.PrintStats();
        }

        static private CommandQueueManager GetInstance()
        {
            return instance;
        }
    }
}
