using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace SMSService
{
    public partial class SMS : ServiceBase
    {
    //    System.Timers.Timer timer1;
        string Host;
        static string Logpath;
        SMS_Class.SMS_Class sms;
        System.Timers.Timer timer1;
        public SMS()
        {
            InitializeComponent();
            Properties.Settings T = new Properties.Settings();
            Logpath = T.Logpath;
        }
        /*
	  public static void sms.writelog(string msg)
	  {
		FileStream fout;
		string OutMsg = DateTime.Now.ToString("HH:mm:ss.fff") + " | " + msg + "\r\n";
        string Fname = Logpath + "\\SMS_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
		try
		{
		    fout = new FileStream(Fname, FileMode.Append);
		}
		catch (FileNotFoundException)
		{
		    fout = new FileStream(Fname, FileMode.Create);
		}
		byte[] byteArray = Encoding.ASCII.GetBytes(OutMsg);
		//        for (int i=0;i<charstr.Length;i++)
		fout.Write(byteArray, 0, byteArray.Length); // (byte) charstr[i]);
		fout.Close();
	  }
*/
/*
protected void Job_Run()
	  {
		SqlConnection conn = new SqlConnection("data source=192.168.1.1\\MTBase; initial catalog=MyBase; user id=Zurabgz; password=zebo; Pooling = true; Connect Timeout=100; Max Pool Size=1000");
		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
	  try {
	  conn.Open();
	  cmd.CommandText = "Select JobId,status from SMS_Manager where status in (1,3) and Jobdate between Getdate()-1 and GetDate()";
	  SqlDataReader reader = cmd.ExecuteReader();
	  string st=String.Empty;
	   while (reader.Read())
	   { st += reader[0].ToString() + "-" + reader[1].ToString() + "|"; }
	  reader.Close();
      if (st != String.Empty)
      {
          string[] sl = st.Substring(0, st.Length - 1).Split('|');
          foreach (string s in sl)
          {
              string jn = s.Substring(0, s.IndexOf('-'));
              string status = s.Substring(s.IndexOf('-') + 1);
              if (status == "1")
              #region fill and send
              {
                  cmd.CommandText = "SMS_AMD_Fill_and_Send";
                  cmd.CommandType = System.Data.CommandType.StoredProcedure;
                  SqlParameter job = new SqlParameter();
                  job.Direction = System.Data.ParameterDirection.Input;
                  job.DbType = System.Data.DbType.Int32;
                  job.ParameterName = "JobId";
                  cmd.Parameters.Add(job);
                  sms.writelog("Call SMS_AMD_Fill_and_Send(Jobid=>" + s + ") ...");
                  job.Value = Convert.ToInt32(jn);
                  cmd.CommandTimeout = 0;
                  cmd.ExecuteNonQuery();
                  sms.writelog("End Jobid=>" + s);

                  cmd.CommandText = "Select ID,Phone,FromSms,[Message] from sms_manual where JobId=" + jn + " and [status]=0";
                  cmd.CommandType = System.Data.CommandType.Text;

                  SqlDataReader dr = cmd.ExecuteReader();
                  while (dr.Read())
                  #region Send SMS
                  {
                      string _Phone = dr["Phone"].ToString();
                      string _msg = dr["Message"].ToString();
                      string _from = dr["FromSms"].ToString();
                      string _id = dr["ID"].ToString();

                      string res = sms.SendAmdSMS(_Phone, _msg, _from);

                      if (res != String.Empty && res != "END" && res.IndexOf("ERR") != 0)
                      {
                          string[] ml = res.Split(':', ';', '\r');
                          string msg_id = ml[1].Trim();
                          string CheckN = ml[3].Trim();
                          //SUCCESS MessageId: 28179883; Cost: 0.80; 0: Accepted for delivery;
                          SqlCommand cmdu = new SqlCommand("update sms_Manual set Send=Getdate(),AMD_ID='" + msg_id +
                              "',Response='" + ml[0] + ml[1] + ml[2] + ml[3] + "',status=100 where ID=" +
                              _id + " and status=0 and send is null", conn);
                          cmdu.Transaction = conn.BeginTransaction();

                          cmdu.ExecuteNonQuery();
                          cmdu.Transaction.Commit();
                      }
                      else
                          if (res == "END")
                          {
                              SqlCommand cmdu = new SqlCommand("update sms_Manual set status=-1 where job='" + jn
                                  + " and status=0 and send is null", conn);
                              cmdu.ExecuteNonQuery();
                              conn.Close();
                              dr.Close();
                              return;
                          }
                  }
                  #endregion
                  dr.Close();

                  cmd.CommandText = "Update SMS_Manager set status=3 where jobid=" + jn;
                  cmd.ExecuteNonQuery();
              }
              #endregion
              else
              #region getstatuses
              {
                  cmd.CommandText = "Select ID,AMD_ID from sms_Manual where status=100 and send>getdate()-1";

                  SqlCommand updStat = new SqlCommand("update sms_Manual set Status=@NewStatus where IDd=@id and Amd_ID=@amdId",
                              conn);
                  updStat.Parameters.Add(new SqlParameter("@NewStatus", SqlDbType.SmallInt));
                  updStat.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                  updStat.Parameters.Add(new SqlParameter("@amdId", SqlDbType.VarChar, 24));


                  SqlDataReader dr = cmd.ExecuteReader();
                  while (dr.Read())
                  {
                      string rr = sms.TrackingAmdSMS(dr["AMD_ID"].ToString());
                      if (rr != "END")
                          try
                          {
                              updStat.Parameters["@NewStatus"].Value = int.Parse(rr.Substring(rr.IndexOf("Code") + 7, 3));
                              updStat.Parameters["@Docid"].Value = int.Parse(dr["ID"].ToString());
                              updStat.Parameters["@amdId"].Value = dr["AMD_ID"].ToString();
                              updStat.ExecuteNonQuery();
                          }
                          catch { };
                  }
                  dr.Close();
                  cmd.CommandText = "Update SMS_Manager set status=4 where jobid=" + jn;
                  cmd.ExecuteNonQuery();
              }
              #endregion
          }
      }
	  conn.Close();
      }     
      catch (Exception ex) {
	  sms.writelog("ERROR:"+cmd.CommandText);
	  sms.writelog(ex.Message);
	  if (conn.State== System.Data.ConnectionState.Open)
	  conn.Close();
	  }
    }

*/
        protected override void OnStart(string[] args)
        {
            int step = 0;
            try
            {
                Properties.Settings t = new Properties.Settings();
                step++;
                this.Host = t.Host;
                step++;
                this.sms = new SMS_Class.SMS_Class();
                step++;
                this.timer1 = new System.Timers.Timer();
                step++;
                this.timer1.Interval = 10000;
                step++;
                this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
                step++;
                this.timer1.AutoReset = true;
                step++;
                this.timer1.Enabled = true;
                step++;
                writeLog(" Starting Service ...");
                step++;
                this.timer1.Start();
            }
            catch (Exception ex) 
            { writeLog($"step:{step} => {ex.Message}");
                Application.Exit();
                return;
            }
        }
        private void writeLog(string msg)
        {
            string logpath = @"C:\Logs";
            string OutMsg = DateTime.Now.ToString("HH:mm:ss.fff") + " | " + msg + "/r/n";
            string Fname = logpath + "\\SMS_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            FileStream fout;
            try
            {
                fout = new FileStream(Fname, FileMode.Append);
            }
            catch (FileNotFoundException)
            {
                fout = new FileStream(Fname, FileMode.Create);
            }
            catch
            {
                return;
            }
            byte[] byteArray = Encoding.ASCII.GetBytes(OutMsg);
            try
            {
                fout.Write(byteArray, 0, byteArray.Length);
                fout.Flush();
            }
            finally
            {
                fout.Close();
            }
        }
        protected override void OnStop()
        {
            this.timer1.Stop();
            writeLog(" Service Stop");

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            writeLog("timer ...");
            timer1.Enabled = false;
            try
            {
                writeLog("checkmessages()");
                sms.checkmessages();
            }
            catch (Exception x) { sms.writelog("Checkmessage:"+x.Message); }
            try
            {
                sms.Job_Run();
            }
            catch (Exception x) { sms.writelog("Job_Run:"+x.Message); }
            finally
            {
                timer1.Enabled = true;
                writeLog("timer Finally");
            }
        }
    }
}
