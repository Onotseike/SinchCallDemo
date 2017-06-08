using System;

using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Text;
using System.Collections;
using Com.Sinch.Android.Rtc.Messaging;
using Android.Util;
using System.Collections.Generic;
using Android.Support.V4.Util;

namespace SinchCallDemo.Droid.Helpers
{
    public class MessageAdapter : BaseAdapter
    {
        public static int DIRECTION_INCOMING = 0;

        public static int DIRECTION_OUTGOING = 1;

        private IList<Tuple<IMessage, int>> mMessages;
        
        private SimpleDateFormat mFormatter;

        private LayoutInflater mInflater;


        public MessageAdapter(Activity activity)
        {
            mInflater = activity.LayoutInflater;
            mMessages = new List<Tuple<IMessage, int>>();
            mFormatter = new SimpleDateFormat("HH:mm");
        }

        public void AddMessage(IMessage message, int direction)
        {
            Tuple<IMessage, int> tuplePair = new Tuple<IMessage, int>(message, direction);
            mMessages.Add(tuplePair);
            NotifyDataSetChanged();
        }


        public override int Count => mMessages.Count;

        public Tuple<IMessage,int> GetMessage(int position)
        {
            return mMessages[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public int GetMessageDirection(int position)
        {
            return mMessages[position].Item2;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int direction = GetMessageDirection(position);

            if (convertView == null)
            {
                int res = 0;
                if (direction == DIRECTION_INCOMING)
                {
                    //res = R.layout.message_right;
                }
                else if (direction == DIRECTION_OUTGOING)
                {
                    //res = R.layout.message_left;
                }
                //convertView = mInflater.inflate(res, viewGroup, false);
            }

            IMessage message = mMessages[position].Item1;
            string name = message.SenderId;

            //TextView txtSender = (TextView)convertView.findViewById(R.id.txtSender);
            //TextView txtMessage = (TextView)convertView.findViewById(R.id.txtMessage);
            //TextView txtDate = (TextView)convertView.findViewById(R.id.txtDate);

            //txtSender.setText(name);
            //txtMessage.setText(message.getTextBody());
            //txtDate.setText(mFormatter.format(message.getTimestamp()));

            return convertView;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
    }
}