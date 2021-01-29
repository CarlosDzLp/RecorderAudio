using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.AudioRecorder;
using Xamarin.Forms;

namespace RecorderAudioXam
{
    public partial class MainPage : ContentPage
    {
        AudioRecorderService recorder;
        AudioPlayer player;

        public MainPage()
        {
            InitializeComponent();

            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(10),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };
            player = new AudioPlayer();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                RecordAudio();
            }
            catch(Exception ex)
            {

            }
        }

        void RecordAudio()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (!recorder.IsRecording)
                    {
                        recorder.StopRecordingOnSilence = false;
                        //start recording audio
                        var audioRecordTask = await recorder.StartRecording();
                        await audioRecordTask;

                        getAudio();
                    }
                    else //Stop button clicked
                    {
                        //await recorder.StopRecording();
                    }
                });
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        private void getAudio()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //var audio = recorder.GetAudioFilePath();

                    //OBTIENE EL STREAM PARA PODER MANIPULARLO Y CONVERTILO A BYTES
                    var filePath = recorder.GetAudioFileStream();

                    if (filePath != null)//VALIDAS QUE NO ESTE NULL EL STREAM
                    {
                        
                        var bytes = ReadStream(filePath);//CONVIERTES EL STREAM A BYTE PARA ENVIARLO A UN WEB SERVICE


                        var convertBase64 = Convert.ToBase64String(bytes);//POR SI SE QUIERE CONVERTIR A BASE64

                        player.Play(recorder.FilePath);//SE OBTIENE EL PATH DEL ARCHIVO DE AUDIO
                    }
                });
            }
            catch (Exception ex)
            {
                //blow up the app!

            }
        }

        public static byte[] ReadStream(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
