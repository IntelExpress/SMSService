using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace SmsTestWin
{
    public partial class Form1 : Form
    {
        static string logpath = "C:\\Logs";
        public Form1()
        {
            InitializeComponent();
        }

        private static void WriteLog(string Filename, string msg)
        {
            FileStream fout;
            try
            {
                fout = new FileStream(Filename, FileMode.Append);
            }
            catch (FileNotFoundException)
            {
                fout = new FileStream(Filename, FileMode.Create);
            }
            byte[] byteArray = Encoding.ASCII.GetBytes(msg);
            fout.Write(byteArray, 0, byteArray.Length);
            fout.Close();

        }
        private static void WriteLog(string msg)
        {
            string OutMsg = DateTime.Now.ToString("HH:mm:ss.fff") + " | " + msg + "\r\n";
            string Fname = logpath + "\\SMS_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteLog(Fname, OutMsg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SMS_Class.SMS_Class cl = new SMS_Class.SMS_Class();
            cl.Job_Run();
            //cl.checkmessages();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                           
            string res= richTextBox1.Text.Trim();
            string CommandText="";
            int row = 0;
            if (res != String.Empty && res != "END" && res.IndexOf("ERR-") != 0)
            {
                int spos = 0;
                int curpos = res.IndexOf("SUCCESS MessageId:", spos);
                while (curpos >= 0)
                {
                    string s2 = res.Substring(curpos, 66);
                    string msg_id = s2.Substring(19, 24);
                    string CheckN = s2.Substring(54);
                    spos = curpos + 1;
                    curpos = res.IndexOf("SUCCESS MessageId:", spos);
                    
                    //CommandText = "update sms_Manual set AMD_ID='" + msg_id +
                      //                          "',Response='" + s2 + "',status=200 where JobID=246 and Phone='" + CheckN + "' " + //in (" +
                        //                        "and status=0 and send is null\n"; //, connU);
                                    WriteLog(logpath + "\\SMS_RESP_246.sql", msg_id+"|"+CheckN+"\n");
                                    row++;

                }
                MessageBox.Show("Success rows:" + row.ToString());
            }
                
                /*string[] ml = res.Replace("SUCCESS MessageId: ", "").Replace("Recipient:", "").Replace("<br />", "").Replace("\r", "").Split('\n');

                                    foreach (string s2 in ml)
                                    {
                                        string s3 = "";
                                        if (s2.IndexOf("ERROR") >= 0)
                                            s3 = s2.Substring(s2.LastIndexOf(']') + 1);
                                        else s3 = s2;
                                        if (s3.Length > 30)
                                        {
                                            string msg_id = s3.Substring(0, 24);
                                            string CheckN = s3.Substring(25);

                                            CommandText = "update sms_Manual set Send=Getdate(),AMD_ID='" + msg_id +
                                                "',Response='" + s3 + "',status=100 where JobID=:jn and Phone='" + CheckN + "' " + //in (" +
                                                "and status=0 and send is null"; 
                                           
                                        }
                                    }
                                }
                                else*/
                                //if (res == "END")
                                {
                                    CommandText = "update sms_Manual set status=-1 where job=:jon and ID in (" +
                                     ") and status=0 and send is null";
                                    return;
                                }

                            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SMS_Class.SMS_Class cl = new SMS_Class.SMS_Class();
            cl.checkmessages();
            return;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=192.168.1.1\\MTBASE;initial catalog=MyBase; user id=Zurabgz; password=zebo; Pooling = true; Connect Timeout=1600; Max Pool Size=1000";
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SMS_Job_Create", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Msg", "DataGridViewHitTestType Messagge"));
                    cmd.Parameters.Add(new SqlParameter("@Country", "DEFAULT"));
                    cmd.Parameters.Add(new SqlParameter("@ServiceID", 1));
                    cmd.Parameters.Add(new SqlParameter("@FromSms", "Intelexpres"));
                    cmd.Parameters.Add(new SqlParameter("@JobDate", DateTime.Now.AddDays(2.2)));
                    cmd.Parameters.Add(new SqlParameter("@operator", "Zura"));
                    cmd.Parameters.Add(new SqlParameter("@Result", System.Data.SqlDbType.Int));
                    cmd.Parameters["@Result"].Direction = System.Data.ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    int jobid = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                    MessageBox.Show("JobID=" + jobid.ToString());
                    conn.Close();
                }
            }
        }
    }
}
