using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml.Linq;
using MASTER_CONTROLL;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace pdf
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sync : Page
    {
        public Sync()
        {
            this.InitializeComponent();
        }

        MESSAGE M;
        SPECIFIC_COMMUNICATION m_s_communication = new SPECIFIC_COMMUNICATION();
        int WordNumber = 0;
        string[] Wordlist;
        bool bgetword = false;
        private async Task ImportWordList()
        {
            WordNumber = 0;
            M.parti = PARTICIPANT.P_M;
            M.method_p_m = METHOD_P_M.method_request_database_word;
            M.in_message.wordbook_str = "default";
            m_s_communication.message = M;
            await m_s_communication.send_messageAsync();
            String WordList = m_s_communication.message.in_message.word_str;
            if (WordList.Substring(0, 4) == "true")
            {
                int l = WordList.Length;
                WordList = WordList.Substring(5);
            }
            else
            {
                WordList = "";
            }
            for (int i = 0; i < WordList.Length; i++)
            {
                if (WordList[i] == '\0')
                {
                    WordNumber++;
                }
            }
            Wordlist = new string[WordNumber];
            for (int i = 0, j = 0, m = 0; i < WordList.Length; i++)
            {
                if (WordList[i] == '\0')
                {
                    Wordlist[j] = WordList.Substring(m, i - m);
                    j++;
                    m = i + 1;
                }
            }

            M.parti = PARTICIPANT.M_S;
            M.method_m_s = METHOD_M_S.method_lookup_word;
            M.in_message.wordbook_str = "default";

            for (int i = 0; i < Wordlist.Length; i++)
            {
                M.in_message.word_str = Wordlist[i];
                m_s_communication.message = M;
                await m_s_communication.send_messageAsync();
                Wordlist[i] = m_s_communication.message.out_message.info;
            }
            bgetword = true;
        }
        private async void Sync_upAction(object sender, RoutedEventArgs e)
        {
            /*
             if (WordList.Substring(0, 4) == "false")
             {
                 var dialog = new ContentDialog()
                 {
                     Title = "Info",
                     Content = "WordList is Null!",
                     PrimaryButtonText = "OK",
                     SecondaryButtonText = "Cancel",
                     FullSizeDesired = false,
                 };

                 dialog.PrimaryButtonClick += (_s, _e) => { };
                 await dialog.ShowAsync();
             }*/
            // Clear Erro message 
            // Create proxy instance 
            String Userid = "ljp";
            pdf.ServiceReference1.Service1Client serviceClient_f = new pdf.ServiceReference1.Service1Client();

            // async call WCF method to get returned data 
            pdf.ServiceReference1.querySqlRequest request_f = new pdf.ServiceReference1.querySqlRequest(Userid, "123", true);
            pdf.ServiceReference1.querySqlResponse ds_f = await serviceClient_f.querySqlAsync(request_f);


            if (ds_f.queryParam)
            {
                // Convert Xml to List<Article> 
                //String a = ds_f.querySqlResult.Nodes[1].ToString();
                XDocument xdoc = XDocument.Parse(ds_f.querySqlResult.Nodes[1].ToString(), LoadOptions.None);
                var data = from query in xdoc.Descendants("Table")
                           select new Article_
                           {
                               userid = query.Element("userid").Value,
                               wordlist = query.Element("wordlist").Value,
                           };
                Task.Run(() => { ImportWordList(); });
                while (bgetword == false) ;
                bgetword = false;
                string WordList = "";
                for (int i = 0; i < Wordlist.Length; i++)
                {
                    WordList += Wordlist[i];
                }
                //string WordList = "name01名称";
                pdf.ServiceReference2.Service2Client serviceClient = new pdf.ServiceReference2.Service2Client();
               // WordList = "afcafnohsauauahiubcinskjiwejksbndfuihsdiusdfhajksdbcvnmxbcjhdhsfjhisahfiuafhasjkhfisahfhsajkfhajkajk";
                bool IsInsert = false;
                if (data.Count() == 0)
                {
                    IsInsert = true;
                }
                pdf.ServiceReference2.querySqlRequest request = new pdf.ServiceReference2.querySqlRequest(Userid, WordList, IsInsert);
                pdf.ServiceReference2.querySqlResponse ds = await serviceClient.querySqlAsync(request);

                if (ds.queryParam)
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "wordlist",
                        Content = "Sync Success!",
                        PrimaryButtonText = "OK",
                        SecondaryButtonText = "Cancel",
                        FullSizeDesired = false,
                    };

                    dialog.PrimaryButtonClick += (_s, _e) => { };
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "wordlist",
                        Content = "Insert Error occurs. Please make sure the database has been attached to SQL Server!",
                        PrimaryButtonText = "OK",
                        SecondaryButtonText = "Cancel",
                        FullSizeDesired = false,
                    };

                    dialog.PrimaryButtonClick += (_s, _e) => { };
                    await dialog.ShowAsync();
                }
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "wordlist",
                    Content = "Insert Error occurs. Please make sure the database has been attached to SQL Server!",
                    PrimaryButtonText = "OK",
                    SecondaryButtonText = "Cancel",
                    FullSizeDesired = false,
                };

                dialog.PrimaryButtonClick += (_s, _e) => { };
                await dialog.ShowAsync();
            }
        }
        private async void Sync_downAction(object sender, RoutedEventArgs e)
        {


            // Clear Erro message 
            // Create proxy instance 
            String Userid = "ljp";
            pdf.ServiceReference1.Service1Client serviceClient_f = new pdf.ServiceReference1.Service1Client();

            // async call WCF method to get returned data 
            pdf.ServiceReference1.querySqlRequest request_f = new pdf.ServiceReference1.querySqlRequest(Userid, "123", true);
            pdf.ServiceReference1.querySqlResponse ds_f = await serviceClient_f.querySqlAsync(request_f);


            if (ds_f.queryParam)
            {
                // Convert Xml to List<Article> 
                String a = ds_f.querySqlResult.Nodes[1].ToString();
                XDocument xdoc = XDocument.Parse(ds_f.querySqlResult.Nodes[1].ToString(), LoadOptions.None);
                var data = from query in xdoc.Descendants("Table")
                           select new Article_
                           {
                               userid = query.Element("userid").Value,
                               wordlist = query.Element("wordlist").Value,
                           };
                if (data.Count() == 0)
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "wordlist",
                        Content = "user doesn't exist!",
                        PrimaryButtonText = "OK",
                        SecondaryButtonText = "Cancel",
                        FullSizeDesired = false,
                    };

                    dialog.PrimaryButtonClick += (_s, _e) => { };
                    await dialog.ShowAsync();
                }
                else
                {
                    String WordList = xdoc.Root.Value.ToString();
                    WordList = WordList.Replace(Userid, "");
                    var dialog = new ContentDialog()
                    {
                        Title = "wordlist",
                        Content = "download success!",
                        PrimaryButtonText = "OK",
                        SecondaryButtonText = "Cancel",
                        FullSizeDesired = false,
                    };

                    dialog.PrimaryButtonClick += (_s, _e) => { };
                    await dialog.ShowAsync();
                }

            }
        }
    }
}
