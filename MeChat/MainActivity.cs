using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Timers;
using Android.Views.InputMethods;

namespace MeChat
{
    [Activity(Label = "MeChat", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<Message> listMessage;
        private Button btnClear, btnSend;
        private EditText txtMessage;
        private ListView listView;
        private MessageAdapter mAdapter;
        private WebClient client;
        private Uri uri;

        System.Timers.Timer t;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            client = new WebClient();
            listMessage = new List<Message>();
            btnClear = FindViewById<Button>(Resource.Id.btnClear);
            btnSend = FindViewById<Button>(Resource.Id.btnSend);
            txtMessage = FindViewById<EditText>(Resource.Id.txtInput);
            listView = FindViewById<ListView>(Resource.Id.listViewMessages);
            downloadJson();
            

            //Lager en timer som kjører en methode hver 2 sekund
            t = new System.Timers.Timer();
            t.Interval = 2000;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();

            btnClear.Click += btnClear_Click;
            btnSend.Click += btnSend_Click;
        }
        //Oppdaterer listview i bakgrunnen 
        protected void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                downloadJson();
            });
        }

        void btnSend_Click(object sender, EventArgs e)
        {
            //Henter json data fra serveren og legger alle dem listMessage. Deretter legger inn det nye json objektet og oppdaterer json filet på serveren
            if (txtMessage.Text != "")
            {
                uri = new Uri("http://long.stevenkas.no/AddMessage.php");
                NameValueCollection parameters = new NameValueCollection();
                downloadJson();
                listMessage.Add(new Message()
                {
                    date = DateTime.Now.ToString("d/m/yyyy hh:mm:ss"),
                    text = txtMessage.Text,
                });
                string json = JsonConvert.SerializeObject(listMessage.ToArray());
                parameters.Add("text", json);

                client.UploadValuesCompleted += client_UploadValuesCompleted;
                client.UploadValuesAsync(uri, parameters);
            }
            
        }
        //Kjører clean-up code når opplastingen er fullført
        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            clear();
            downloadJson();
        }

   

        void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        public void clear()
        {//Sletter alt i EditText og skjuler keyboard
            txtMessage.Text = "";
            var parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(parentContainer.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
            
        }

        public void downloadJson()
        {//Laster ned json fila og legger inneholdet i en String. Deretter konverterer json string til json objekter som legges i en liste.
            if (listMessage.Count != 0)
            {
                listMessage.Clear();
                client = new WebClient();
                string json = client.DownloadString("http://long.stevenkas.no/meldinger.txt");
                listMessage = JsonConvert.DeserializeObject<List<Message>>(json);
                mAdapter = new MessageAdapter(this, listMessage);
                listView.Adapter = mAdapter;
            }
            else
            {
                client = new WebClient();
                string json = client.DownloadString("http://long.stevenkas.no/meldinger.txt");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    listMessage = JsonConvert.DeserializeObject<List<Message>>(json);
                    mAdapter = new MessageAdapter(this, listMessage);
                    listView.Adapter = mAdapter;
                }
                
            }
          
           
        }

    
    }
}

