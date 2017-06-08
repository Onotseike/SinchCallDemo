using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SinchCallDemo.Interface;
using Com.Sinch.Android.Rtc;
using Com.Sinch.Android.Rtc.Calling;
using Com.Sinch.Android.Rtc.Messaging;
using Android.Util;
using SinchCallDemo.Droid.Helpers;
using Xamarin.Forms;

namespace SinchCallDemo.Droid.InterfaceImplementations
{
    [Service]
    public class SinchService : Service, ISinchService
    {
        static readonly string APP_KEY = "0a708290-73f0-4796-8314-a93764ce78d7";
        static readonly string APP_SECRET = "rf6DFCbKEkuu2VC6NPNNmg==";
        static readonly string ENVIRONMENT = "sandbox.sinch.com";

        public static readonly string CALL_ID = "CALL_ID";
        static readonly string TAG = nameof(SinchService);

        SinchServiceInterface mSinchServiceInterface;
        ISinchClient mSinchClient;
        string mUserId;

        IStartFailedListener mListener;      

        public SinchService()
        {
            mSinchServiceInterface = new SinchServiceInterface(this);
        }

        public void SendMessage(string receipient, string message)
        {
            mSinchServiceInterface.SendUserMessage(receipient, message);
        }

        public void CallVideo(string receipientId)
        {
            mSinchServiceInterface.CallUserVideo(receipientId);
        }

        public void CallVoice(string receipientId)
        {
            mSinchServiceInterface.CallUserVoice(receipientId);
        }       

        public override IBinder OnBind(Intent intent)
        {
            return mSinchServiceInterface;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            if (mSinchClient != null && mSinchClient.IsStarted)
            {
                mSinchClient.Terminate();
            }
            base.OnDestroy();
        }
       
        public void StartSinchClient(string username)
        {
            if (mSinchClient == null)
            {
                mUserId = username;
                mSinchClient = Sinch.SinchClientBuilder.Context(ApplicationContext).UserId(username)
                        .ApplicationKey(APP_KEY)
                        .ApplicationSecret(APP_SECRET)
                        .EnvironmentHost(ENVIRONMENT).Build();

                mSinchClient.SetSupportCalling(true);
                mSinchClient.SetSupportMessaging(true);
                mSinchClient.SetSupportPushNotifications(true);
                mSinchClient.SetSupportManagedPush(true);
                mSinchClient.SetSupportActiveConnectionInBackground(true);
                mSinchClient.StartListeningOnActiveConnection();

                mSinchClient.AddSinchClientListener(new SinchClientListener(this));
                // Permission READ_PHONE_STATE is needed to respect native calls.
                mSinchClient.CallClient.SetRespectNativeCalls(false);
                mSinchClient.CallClient.AddCallClientListener(new SinchCallClientListener(this));
                mSinchClient.MessageClient.AddMessageClientListener(new SinchMessageClientListener(this));

                mSinchClient.Start();
            }
        }

        public void StopSinchClient()
        {
            if (mSinchClient != null)
            {
                mSinchClient.Terminate();
                mSinchClient = null;
            }
        }

        public bool IsSinchClientStarted()
        {
            return (mSinchClient != null && mSinchClient.IsStarted);
        }

        public void MessageDetails()
        {
            throw new NotImplementedException();
        }

        public interface IStartFailedListener
        {
            void OnStartFailed(ISinchError error);

            void OnStarted();
        }

        public class SinchServiceInterface : Binder
        {
            private SinchService sinchService;

            public SinchServiceInterface(SinchService sinchService)
            {
                this.sinchService = sinchService;
            }

            public ICall CallPhoneNumber(String phoneNumber)
            {
                return sinchService.mSinchClient.CallClient.CallPhoneNumber(phoneNumber);
            }

            public ICall CallUserVoice(String userId)
            {
                if (sinchService.mSinchClient == null)
                {
                    return null;
                }
                return sinchService.mSinchClient.CallClient.CallUser(userId);
            }

            public ICall CallUserVideo(String userId)
            {
                if (sinchService.mSinchClient == null)
                {
                    return null;
                }
                return sinchService.mSinchClient.CallClient.CallUser(userId);
            }

            public void SendUserMessage(String userId, String message)
            {
                if(sinchService.mSinchClient.IsStarted)
                {
                    WritableMessage newMessage = new WritableMessage(userId, message);
                    sinchService.mSinchClient.MessageClient.Send(newMessage);
                }
                
            }

            public string GetUserName()
            {
                return sinchService.mUserId;
            }

            public bool IsStarted()
            {
                return sinchService.IsSinchClientStarted();
            }

            public void StartClient(String userName)
            {
                sinchService.StartSinchClient(userName);
            }

            public void StopClient()
            {
                sinchService.StopSinchClient();
            }

            public void SetStartListener(IStartFailedListener listener)
            {
                sinchService.mListener = listener;
            }

            public ICall GetCall(String callId)
            {
                return sinchService.mSinchClient.CallClient.GetCall(callId);
            }
        }

        class SinchCallClientListener : Java.Lang.Object, ICallClientListener
        {
            private SinchService sinchService;

            public SinchCallClientListener(SinchService sinchService)
            {
                this.sinchService = sinchService;
            }

            public void OnIncomingCall(ICallClient callClient, ICall call)
            {
                Log.Debug(TAG, "Incoming call");
                Intent intent = new Intent(sinchService, typeof(IncomingCallScreenActivity));
                intent.PutExtra(CALL_ID, call.CallId);
                intent.AddFlags(ActivityFlags.NewTask);
                sinchService.StartActivity(intent);
            }
        }

        class SinchClientListener : Java.Lang.Object, ISinchClientListener
        {
            private readonly SinchService sinchService;

            public SinchClientListener(SinchService sinchService)
            {
                this.sinchService = sinchService;
            }
                       
            public void OnClientFailed(ISinchClient sinchClient, ISinchError sinchError)
            {
                if (sinchService.mListener != null)
                {
                    sinchService.mListener.OnStartFailed(sinchError);
                }
                sinchService.mSinchClient.Terminate();
                sinchService.mSinchClient = null;
            }

            public void OnClientStarted(ISinchClient sinchClient)
            {
                Log.Debug(TAG, "SinchClient started");
                if (sinchService.mListener != null)
                {
                    sinchService.mListener.OnStarted();
                }
            }

            public void OnClientStopped(ISinchClient sinchClient)
            {
                Log.Debug(TAG, "SinchClient stopped");
            }

            public void OnLogMessage(int level, string area, string message)
            {
                switch (level)
                {
                    case 3://Log.DEBUG:
                        Log.Debug(area, message);
                        break;
                    case 6://Log.ERROR:
                        Log.Error(area, message);
                        break;
                    case 4://Log.INFO:
                        Log.Info(area, message);
                        break;
                    case 2://Log.VERBOSE:
                        Log.Verbose(area, message);
                        break;
                    case 5://Log.WARN:
                        Log.Warn(area, message);
                        break;
                }
            }

            public void OnRegistrationCredentialsRequired(ISinchClient sinchClient, IClientRegistration clientReg)
            {
                //throw new NotImplementedException();
            }
        }

        class SinchMessageClientListener : Java.Lang.Object, IMessageClientListener
        {
            private SinchService sinchService;
            MessageAdapter mMessageAdapter;

            public SinchMessageClientListener(SinchService sinchService)
            {
                this.sinchService = sinchService;
                mMessageAdapter = new MessageAdapter((Activity)Xamarin.Forms.Forms.Context);
            }

            public void OnIncomingMessage(IMessageClient messageClient, IMessage message)
            {
                mMessageAdapter.AddMessage(message, MessageAdapter.DIRECTION_INCOMING);
            }

            public void OnMessageDelivered(IMessageClient messageClient, IMessageDeliveryInfo msgDeliveryInfo)
            {
                Log.Debug(TAG, "DELIVERED");
            }

            public void OnMessageFailed(IMessageClient messageClient, IMessage message, IMessageFailureInfo msgFailureInfo)
            {
                StringBuilder strngBuilder = new StringBuilder();
                strngBuilder.Append("Sending Failed").Append(msgFailureInfo.SinchError.Message);

                Toast.MakeText(Forms.Context, strngBuilder.ToString(), ToastLength.Long).Show();

                Log.Debug(TAG, strngBuilder.ToString());
            }

            public void OnMessageSent(IMessageClient messageClient, IMessage message, string reciepientId)
            {
                mMessageAdapter.AddMessage(message, MessageAdapter.DIRECTION_OUTGOING);
            }

            public void OnShouldSendPushData(IMessageClient messageClient, IMessage message, IList<IPushPair> pushPairs)
            {
                //throw new NotImplementedException();
            }
        }
    }

   
}