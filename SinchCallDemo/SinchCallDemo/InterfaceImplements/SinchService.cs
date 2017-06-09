using SinchCallDemo.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SinchCallDemo.InterfaceImplements
{
    public class SinchService : ISinchService
    {

        ISinchService sinchService;// = DependencyService.Get<ISinchService>();

        public SinchService()
        {
            sinchService = DependencyService.Get<ISinchService>();
        }

        public void CallVideo(string receipientId)
        {
            sinchService.CallVideo(receipientId);
        }

        public void CallVoice(string receipientId)
        {
            sinchService.CallVoice(receipientId);
        }

        public bool IsSinchClientStarted()
        {
            return sinchService.IsSinchClientStarted();
        }

        public void MessageDetails()
        {
            sinchService.MessageDetails();
        }

        public void SendMessage(string receipient, string message)
        {
            sinchService.SendMessage(receipient, message);
        }

        public void StartSinchClient(string username)
        {
            sinchService.StartSinchClient(username);
        }

        public void StopSinchClient()
        {
            sinchService.StopSinchClient();
        }
    }
}
