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

namespace MeChat
{
    class MessageAdapter : BaseAdapter<Message>
    {
        private Context context;
        private List<Message> mlist;

        public MessageAdapter(Context context, List<Message> mlist)
        {
            this.context = context;
            this.mlist = mlist;
        }

        public override int Count
        {
            get { return mlist.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override Message this[int position]
        {
            get { return mlist[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.message_row, null, false);
            }
            TextView txtviewMessage = row.FindViewById<TextView>(Resource.Id.txtviewMessage);
            TextView txtviewDate = row.FindViewById<TextView>(Resource.Id.txtviewDate);

            txtviewDate.Text = mlist[position].date + "\n" + " Anonymous:";
            txtviewMessage.Text = mlist[position].text;
            return row;
        }
    }
}