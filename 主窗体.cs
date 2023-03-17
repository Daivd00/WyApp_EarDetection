using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using WY_App.Utility;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using HalconDotNet;
using TcpClient = WY_App.Utility.TcpClient;
using Sunny.UI.Win32;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using static WY_App.Utility.Parameter;
using OpenCvSharp.Dnn;
using MvCamCtrl.NET;

namespace WY_App
{
    public partial class 主窗体 : Form
    {

        HslCommunication hslCommunication;
        public static string Alarm = "";
        public static List<string> AlarmList = new List<string>();
        Thread myThread;
        Thread MainThread0;
        Thread MainThread1;
        //Thread ImageThread;

        public static Parameter.Rect1[] specifications;
        HWindow[] hWindows;
        public static List<HObject> ho_Image = new List<HObject>();
        public static List<HObject> ho_DefectImage = new List<HObject>();
        public static List<HObject> ho_OrigalImage = new List<HObject>();
        public static HObject[] hImage = new HObject[2];
        public static HObject[] hImage2 = new HObject[2];
        public static HTuple[] hv_AcqHandle = new HTuple[4];
        public static bool[] camera_opend = new bool[4];
        double[] defectionValues = new double[4];
        //Halcon halcon = new Halcon();
        MV2DCam mV2DCam = new MV2DCam();
        //
        private static Queue<Func<int>> m_List=new Queue<Func<int>>();
        private static object m_obj = new object();
        private bool isExit = false;
        HObject hObjectIn;
        HObject hObjectOut;
       
        public 主窗体()
        {
            InitializeComponent();
            hWindows = new HWindow[4] { hWindowControl1.HalconWindow, hWindowControl2.HalconWindow, hWindowControl3.HalconWindow, hWindowControl4.HalconWindow };
            HOperatorSet.ReadImage(out hImage[0], Application.StartupPath + "/image/N1.bmp");
            HOperatorSet.ReadImage(out hImage[1], Application.StartupPath + "/image/N2.bmp");
            HOperatorSet.ReadImage(out hImage2[0], Application.StartupPath + "/image/C1.bmp");
            HOperatorSet.ReadImage(out hImage2[1], Application.StartupPath + "/image/C2.bmp");
            HOperatorSet.GetImageSize(hImage[0], out Halcon.hv_Width[0], out Halcon.hv_Height[0]);
            HOperatorSet.GetImageSize(hImage[1], out Halcon.hv_Width[1], out Halcon.hv_Height[1]);
            HOperatorSet.GetImageSize(hImage2[0], out Halcon.hv_Width[2], out Halcon.hv_Height[2]);
            HOperatorSet.GetImageSize(hImage2[1], out Halcon.hv_Width[3], out Halcon.hv_Height[3]);
            HOperatorSet.SetPart(hWindows[0], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[1], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[2], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[3], 0, 0, -1, -1);//设置窗体的规格 
            hWindows[0].DispObj(hImage[0]);
            hWindows[1].DispObj(hImage[1]);
            hWindows[2].DispObj(hImage2[0]);
            hWindows[3].DispObj(hImage2[1]);
            pictureBox1.Load(Application.StartupPath + "/image/logo.png");
            try
            {
                Parameter.commministion = XMLHelper.BackSerialize<Parameter.Commministion>("Parameter/Commministion.xml");
            }
            catch
            {
                Parameter.commministion = new Parameter.Commministion();
                XMLHelper.serialize<Parameter.Commministion>(Parameter.commministion, "Parameter/Commministion.xml");
            }

            try
            {
                Parameter.counts = XMLHelper.BackSerialize<Parameter.Counts>("Parameter/CountsParams.xml");
            }
            catch
            {
                Parameter.counts = new Parameter.Counts();
                XMLHelper.serialize<Parameter.Counts>(Parameter.counts, "Parameter/CountsParams.xml");
            }
            try
            {
                Parameter.specificationsCam2[0] = XMLHelper.BackSerialize<Parameter.SpecificationsCam2>("Parameter/Cam2Specifications0.xml");
            }
            catch
            {
                Parameter.specificationsCam2[0] = new Parameter.SpecificationsCam2();
                XMLHelper.serialize<Parameter.SpecificationsCam2>(Parameter.specificationsCam2[0], "Parameter/Cam2Specifications0.xml");
            }

            try
            {
                Parameter.specificationsCam2[1] = XMLHelper.BackSerialize<Parameter.SpecificationsCam2>("Parameter/Cam2Specifications1.xml");
            }
            catch
            {
                Parameter.specificationsCam2[1] = new Parameter.SpecificationsCam2();
                XMLHelper.serialize<Parameter.SpecificationsCam2>(Parameter.specificationsCam2[1], "Parameter/Cam2Specifications1.xml");
            }
            try
            {
                Parameter.specificationsCam1[0] = XMLHelper.BackSerialize<Parameter.SpecificationsCam1>("Parameter/Cam1Specifications0.xml");
            }
            catch
            {
                Parameter.specificationsCam1[0] = new Parameter.SpecificationsCam1();
                XMLHelper.serialize<Parameter.SpecificationsCam1>(Parameter.specificationsCam1[0], "Parameter/Cam1Specifications0.xml");
            }

            try
            {
                Parameter.specificationsCam1[1] = XMLHelper.BackSerialize<Parameter.SpecificationsCam1>("Parameter/Cam1Specifications1.xml");
            }
            catch
            {
                Parameter.specificationsCam1[1] = new Parameter.SpecificationsCam1();
                XMLHelper.serialize<Parameter.SpecificationsCam1>(Parameter.specificationsCam1[1], "Parameter/Cam1Specifications1.xml");
            }
            Halcon.Cam1Connect = Halcon.initalCamera("CAM0", ref Halcon.hv_AcqHandle0);
            if(Halcon.Cam1Connect)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机1链接成功");
                主窗体.AlarmList.Add(System.DateTime.Now.ToString() + "相机1链接成功");
            }
            
            Halcon.Cam2Connect = Halcon.initalCamera("CAM1", ref Halcon.hv_AcqHandle1);
            if(Halcon.Cam2Connect)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机2链接成功");
                主窗体.AlarmList.Add(System.DateTime.Now.ToString() + "相机2链接成功");
            }
           
            uiDataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            uiDataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            myThread = new Thread(initAll);
            myThread.IsBackground = true;
            myThread.Start();
        }

        /// <summary>
        /// skinPictureBox1滚动缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {


            }
            catch (Exception x)
            {
                LogHelper.Log.WriteError("缩放异常：" + x.Message);
            }
        }

        //鼠标移动
        int xPos = 0;
        int yPos = 0;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_MouseMove1(object sender, MouseEventArgs e)
        {

        }


        private void initAll()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Task task = new Task(() =>
                {
                    MethodInvoker start = new MethodInvoker(() =>
                    {

                        if (Parameter.commministion.PlcEnable)
                        {
                            if (HslCommunication.plc_connect_result)
                            {
                                lab_PLCStatus.Text = "在线";
                                lab_PLCStatus.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_PLCStatus.Text = "离线";
                                lab_PLCStatus.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_PLCStatus.Text = "禁用";
                            lab_PLCStatus.BackColor = Color.Gray;
                        }
                        if (Parameter.commministion.TcpClientEnable)
                        {
                            if (TcpClient.TcpClientConnectResult)
                            {
                                lab_Client.Text = "在线";
                                lab_Client.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Client.Text = "等待";
                                lab_Client.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Client.Text = "禁用";
                            lab_Client.BackColor = Color.Gray;
                        }
                        if (Parameter.commministion.TcpServerEnable)
                        {
                            if (TcpServer.TcpServerConnectResult)
                            {
                                lab_Server.Text = "在线";
                                lab_Server.BackColor = Color.Green;
                            }
                            else
                            {
                                lab_Server.Text = "等待";
                                lab_Server.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            lab_Server.Text = "禁用";
                            lab_Server.BackColor = Color.Gray;
                        }
                        if (AlarmList.Count != 0)
                        {
                            lst_LogInfos.Items.Add(AlarmList[0]);
                            AlarmList.RemoveAt(0);
                        }
                        if (lst_LogInfos.Items.Count > 20)
                        {
                            lst_LogInfos.Items.RemoveAt(0);
                        }
                        if (Halcon.Cam1Connect)
                        {
                            lab_Camera1.Text = "在线";
                            lab_Camera1.BackColor = Color.Green;
                        }
                        else
                        {
                            lab_Camera1.Text = "等待";
                            lab_Camera1.BackColor = Color.Red;
                        }
                        if (Halcon.Cam2Connect)
                        {
                            lab_Camera2.Text = "在线";
                            lab_Camera2.BackColor = Color.Green;
                        }
                        else
                        {
                            lab_Camera2.Text = "等待";
                            lab_Camera2.BackColor = Color.Red;
                        }
                    });
                    this.BeginInvoke(start);
                });
                task.Start();
            }

        }
        static string movefile;
        public static int[] result1;
        bool[] testReslut1 = new bool[5];
        double[] value1 = new double[3];
        bool m_Pause = true;
        AutoResetEvent ImageEvent1 = new AutoResetEvent(false);
        private void MainRun0()
        {
            while (true)
            {
                if(m_Pause)
                {
                    ushort ushort100 = HslCommunication._NetworkTcpDevice.ReadUInt16(Parameter.plcParams.Trigger_Detection1).Content; // 读取寄存器100的ushort值              
                    if (ushort100 == 49)
                    {
                        uiDataGridView1.Rows[3].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[4].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[5].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[6].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[7].Cells[1].Style.BackColor = Color.Black;
                        
                        AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Trigger_Detection1 + "触发信号:" + ushort100.ToString());
                        mV2DCam.TriggerExec(0);
                        mV2DCam.ImageEvent[0].WaitOne();
                        HOperatorSet.CopyImage(mV2DCam.hvImage[0], out hImage[0]);
                        mV2DCam.hvImage[0].Dispose();
                        //if (Halcon.Cam1Connect)
                        //{
                        //    Halcon.GrabImage(Halcon.hv_AcqHandle0, out hImage[0]);
                        //}
                        //else
                        //{
                        //    HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 6);
                        //    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "6");
                        //}
                        HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection1, 0);
                        
                        HOperatorSet.GetImageSize(hImage[0], out Halcon.hv_Height[0], out Halcon.hv_Width[0]);
                        HOperatorSet.CopyImage(hImage[0],out hObjectIn);
                        HOperatorSet.SetPart(hWindows[0], 0, 0, -1, -1);//设置窗体的规格
                        HOperatorSet.DispObj(hImage[0], hWindows[0]);
                        检测1.Detection(0, hWindows[0], hImage[0], ref testReslut1, ref value1);
                        HOperatorSet.DumpWindowImage(out hObjectOut, hWindows[0]);
                        DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                        strDateTime = dtNow.ToString("yyyyMMddHHmmss");
                        strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
                        Push();
                        Thread th = new Thread(new ThreadStart(ThreadWork));
                        th.Start();

                        if (value1[0] - Parameter.specificationsCam1[0].胶宽.value < Parameter.specificationsCam1[0].胶宽.min || value1[0] - Parameter.specificationsCam1[0].胶宽.value > Parameter.specificationsCam1[0].胶宽.max)
                        {
                            uiDataGridView1.Rows[0].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[0].Cells[1].Value = value1[0].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[0].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[0].Cells[1].Value = value1[0].ToString("0.00000");
                        }
                        if (value1[1] - Parameter.specificationsCam1[0].胶高.value < Parameter.specificationsCam1[0].胶高.min || value1[1] - Parameter.specificationsCam1[0].胶高.value > Parameter.specificationsCam1[0].胶高.max)
                        {
                            uiDataGridView1.Rows[1].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[1].Cells[1].Value = value1[1].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[1].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[1].Cells[1].Value = value1[1].ToString("0.00000");
                        }

                        if (value1[2] - Parameter.specificationsCam1[0].胶线.value < Parameter.specificationsCam1[0].胶线.min || value1[2] - Parameter.specificationsCam1[0].胶线.value > Parameter.specificationsCam1[0].胶线.max)
                        {
                            uiDataGridView1.Rows[2].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[2].Cells[1].Value = value1[2].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[2].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[2].Cells[1].Value = value1[2].ToString("0.00000");
                        }


                        if (testReslut1[0] && testReslut1[1] && testReslut1[2] && testReslut1[3] && testReslut1[4])//OK
                        {
                            Parameter.counts.Counts1[4]++;
                            uiDataGridView1.Rows[7].Cells[1].Value = Parameter.counts.Counts1[4];
                            uiDataGridView1.Rows[7].Cells[1].Style.BackColor = Color.Green;
                            HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "5");
                        }
                        else
                        {
                            Parameter.counts.Counts1[3]++;
                            uiDataGridView1.Rows[6].Cells[1].Value = Parameter.counts.Counts1[3];
                            uiDataGridView1.Rows[6].Cells[1].Style.BackColor = Color.Red;
                            if (testReslut1[4])//胶线不良
                            {
                                Parameter.counts.Counts1[0]++;
                                uiDataGridView1.Rows[3].Cells[1].Value = Parameter.counts.Counts1[0];
                                uiDataGridView1.Rows[3].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "8");
                            }
                            else if (testReslut1[3])//胶面不良
                            {
                                Parameter.counts.Counts1[1]++;
                                uiDataGridView1.Rows[4].Cells[1].Value = Parameter.counts.Counts1[1];
                                uiDataGridView1.Rows[4].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "8");
                            }
                            else //尺寸不良
                            {
                                Parameter.counts.Counts1[2]++;
                                uiDataGridView1.Rows[5].Cells[1].Value = Parameter.counts.Counts1[2];
                                uiDataGridView1.Rows[5].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "6");
                            }
                        }
                    }
                    else if (ushort100 == 50)
                    {
                        uiDataGridView1.Rows[3].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[4].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[5].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[6].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView1.Rows[7].Cells[2].Style.BackColor = Color.Black;
                        
                        AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Trigger_Detection1 + "触发信号:" + ushort100.ToString());
                        mV2DCam.TriggerExec(0);
                        mV2DCam.ImageEvent[0].WaitOne();
                        HOperatorSet.ClearWindow(hWindows[0]);
                        HOperatorSet.CopyImage(mV2DCam.hvImage[0], out hImage[1]);
                        mV2DCam.hvImage[0].Dispose();
                        //if (Halcon.Cam1Connect)
                        //{
                        //    Halcon.GrabImage(Halcon.hv_AcqHandle0, out hImage[1]);
                        //}
                        //else
                        //{
                        //    HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 6);
                        //    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "6");
                        //}
                        HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection1, 0);
                        
                        HOperatorSet.GetImageSize(hImage[1], out Halcon.hv_Height[1], out Halcon.hv_Width[1]);
                        HOperatorSet.CopyImage(hImage[1], out hObjectIn);
                        HOperatorSet.SetPart(hWindows[1], 0, 0, -1, -1);//设置窗体的规格
                        HOperatorSet.DispObj(hImage[1], hWindows[1]);
                        检测1.Detection(1, hWindows[1], hImage[1], ref testReslut1, ref value1);
                        HOperatorSet.DumpWindowImage(out hObjectOut, hWindows[1]);
                        DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                        strDateTime = dtNow.ToString("yyyyMMddHHmmss");
                        strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
                        Push();
                        Thread th = new Thread(new ThreadStart(ThreadWork));
                        th.Start();

                        if (value1[0] - Parameter.specificationsCam1[0].胶宽.value < Parameter.specificationsCam1[0].胶宽.min || value1[0] - Parameter.specificationsCam1[0].胶宽.value > Parameter.specificationsCam1[0].胶宽.max)
                        {
                            uiDataGridView1.Rows[0].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[0].Cells[2].Value = value1[0].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[0].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[0].Cells[2].Value = value1[0].ToString("0.00000");
                        }
                        if (value1[1] - Parameter.specificationsCam1[0].胶高.value < Parameter.specificationsCam1[0].胶高.min || value1[1] - Parameter.specificationsCam1[0].胶高.value > Parameter.specificationsCam1[0].胶高.max)
                        {
                            uiDataGridView1.Rows[1].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[1].Cells[2].Value = value1[1].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[1].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[1].Cells[2].Value = value1[1].ToString("0.00000");
                        }

                        if (value1[2] - Parameter.specificationsCam1[0].胶线.value < Parameter.specificationsCam1[0].胶线.min || value1[2] - Parameter.specificationsCam1[0].胶线.value > Parameter.specificationsCam1[0].胶线.max)
                        {
                            uiDataGridView1.Rows[2].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView1.Rows[2].Cells[2].Value = value1[2].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView1.Rows[2].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView1.Rows[2].Cells[2].Value = value1[2].ToString("0.00000");
                        }
                        if (testReslut1[0] && testReslut1[1] && testReslut1[2] && testReslut1[3] && testReslut1[4])//OK
                        {
                            Parameter.counts.Counts2[4]++;
                            uiDataGridView1.Rows[7].Cells[2].Value = Parameter.counts.Counts2[4];
                            uiDataGridView1.Rows[7].Cells[2].Style.BackColor = Color.Green;
                            HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "5");
                        }
                        else
                        {
                            Parameter.counts.Counts2[3]++;
                            uiDataGridView1.Rows[6].Cells[2].Value = Parameter.counts.Counts2[3];
                            uiDataGridView1.Rows[6].Cells[2].Style.BackColor = Color.Red;
                            if (testReslut1[4])//胶线不良
                            {
                                Parameter.counts.Counts2[0]++;
                                uiDataGridView1.Rows[3].Cells[2].Value = Parameter.counts.Counts2[0];
                                uiDataGridView1.Rows[3].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "8");
                            }
                            else if(testReslut1[3])//胶面不良
                            {
                                Parameter.counts.Counts2[1]++;
                                uiDataGridView1.Rows[4].Cells[2].Value = Parameter.counts.Counts2[1];
                                uiDataGridView1.Rows[4].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "8");
                            }
                            else //尺寸不良
                            {
                                Parameter.counts.Counts2[2]++;
                                uiDataGridView1.Rows[5].Cells[2].Value = Parameter.counts.Counts2[2];
                                uiDataGridView1.Rows[5].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion1, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion1 + "写入:" + "6");
                            }
                        }
                    }
                }
                
                Thread.Sleep(50);

            }
        }
        public static int[] result2;
        bool[] testReslut2 = new bool[8];
        double[] value2 = new double[5];

        private void MainRun1()
        {
            while (true)
            {
                if (m_Pause)
                {
                    ushort ushort100 = HslCommunication._NetworkTcpDevice.ReadUInt16(Parameter.plcParams.Trigger_Detection2).Content; // 读取寄存器100的ushort值              
                    if (ushort100 == 49)
                    {
                        uiDataGridView2.Rows[5].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[6].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[7].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[8].Cells[1].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[9].Cells[1].Style.BackColor = Color.Black;
                        AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Trigger_Detection2 + "触发信号:" + ushort100.ToString());
                        mV2DCam.TriggerExec(1);
                        mV2DCam.ImageEvent[1].WaitOne();
                        HOperatorSet.ClearWindow(hWindows[2]);
                        HOperatorSet.CopyImage(mV2DCam.hvImage[1], out hImage2[0]);
                        mV2DCam.hvImage[1].Dispose();
                        //if (Halcon.Cam2Connect)
                        //{
                        //    Halcon.GrabImage(Halcon.hv_AcqHandle1, out hImage2[0]);
                        //}
                        //else
                        //{
                        //    HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 6);
                        //    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "6");
                        //}
                        HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection2, 0);
                        
                        HOperatorSet.GetImageSize(hImage2[0], out Halcon.hv_Height[2], out Halcon.hv_Width[2]);
                        HOperatorSet.CopyImage(hImage2[0], out hObjectIn);
                        HOperatorSet.SetPart(hWindows[2], 0, 0, -1, -1);//设置窗体的规格
                        HOperatorSet.DispObj(hImage2[0], hWindows[2]);
                        检测2.Detection(0, hWindows[2], hImage2[0], ref testReslut2, ref value2);
                        HOperatorSet.DumpWindowImage(out hObjectOut, hWindows[2]);
                        DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                        strDateTime = dtNow.ToString("yyyyMMddHHmmss");
                        strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
                        Push();
                        Thread th = new Thread(new ThreadStart(ThreadWork));
                        th.Start();

                        if (value2[0] - Parameter.specificationsCam2[0].料长.value < Parameter.specificationsCam2[0].料长.min || value2[0] - Parameter.specificationsCam2[0].料长.value > Parameter.specificationsCam2[0].料长.max)
                        {
                            uiDataGridView2.Rows[0].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[0].Cells[1].Value = value2[0].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[0].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[0].Cells[1].Value = value2[0].ToString("0.00000");
                        }
                        if (value2[1] - Parameter.specificationsCam2[0].料宽.value < Parameter.specificationsCam2[0].料宽.min || value2[1] - Parameter.specificationsCam2[0].料宽.value > Parameter.specificationsCam2[0].料宽.max)
                        {
                            uiDataGridView2.Rows[1].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[1].Cells[1].Value = value2[1].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[1].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[1].Cells[1].Value = value2[1].ToString("0.00000");
                        }
                        if (value2[2] - Parameter.specificationsCam2[0].胶宽.value < Parameter.specificationsCam2[0].胶宽.min || value2[2] - Parameter.specificationsCam2[0].胶宽.value > Parameter.specificationsCam2[0].胶宽.max)
                        {
                            uiDataGridView2.Rows[2].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[2].Cells[1].Value = value2[2].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[2].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[2].Cells[1].Value = value2[2].ToString("0.00000");
                        }
                        if (value2[3] - Parameter.specificationsCam2[0].长端.value < Parameter.specificationsCam2[0].长端.min || value2[3] - Parameter.specificationsCam2[0].长端.value > Parameter.specificationsCam2[0].长端.max)
                        {
                            uiDataGridView2.Rows[3].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[3].Cells[1].Value = value2[3].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[3].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[3].Cells[1].Value = value2[3].ToString("0.00000");
                        }
                        if (value2[4] - Parameter.specificationsCam2[0].料长.value < Parameter.specificationsCam2[0].料长.min || value2[4] - Parameter.specificationsCam2[0].料长.value > Parameter.specificationsCam2[0].料长.max)
                        {
                            uiDataGridView2.Rows[4].Cells[1].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[4].Cells[1].Value = value2[4].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[4].Cells[1].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[4].Cells[1].Value = value2[4].ToString("0.00000");
                        }


                        if (testReslut2[0] && testReslut2[1] && testReslut2[2] && testReslut2[3] && testReslut2[4] && testReslut2[5] && testReslut2[6] && testReslut2[7])//OK
                        {
                            Parameter.counts.Counts3[4]++;
                            uiDataGridView2.Rows[9].Cells[1].Value = Parameter.counts.Counts3[4];
                            uiDataGridView2.Rows[9].Cells[2].Style.BackColor = Color.Green;
                            HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "5");
                        }
                        else
                        {
                            Parameter.counts.Counts3[3]++;
                            uiDataGridView2.Rows[8].Cells[1].Value = Parameter.counts.Counts3[3];
                            uiDataGridView2.Rows[8].Cells[1].Style.BackColor = Color.Red;
                            if (testReslut2[1])//胶面不良
                            {
                                Parameter.counts.Counts3[0]++;
                                uiDataGridView2.Rows[5].Cells[1].Value = Parameter.counts.Counts3[0];
                                uiDataGridView2.Rows[5].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "8");
                            }
                            else if (testReslut2[0]|| testReslut2[2])//料面不良
                            {
                                Parameter.counts.Counts3[1]++;
                                uiDataGridView2.Rows[6].Cells[1].Value = Parameter.counts.Counts3[1];
                                uiDataGridView2.Rows[6].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "8");
                            }
                            else //尺寸不良
                            {
                                Parameter.counts.Counts3[2]++;
                                uiDataGridView2.Rows[7].Cells[1].Value = Parameter.counts.Counts3[2];
                                uiDataGridView2.Rows[7].Cells[1].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 6);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "6");
                            }
                        }
                    }
                    else if (ushort100 == 50)
                    {
                        uiDataGridView2.Rows[5].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[6].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[7].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[8].Cells[2].Style.BackColor = Color.Black;
                        uiDataGridView2.Rows[9].Cells[2].Style.BackColor = Color.Black;
                        
                        AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Trigger_Detection2 + "触发信号:" + ushort100.ToString());
                        mV2DCam.TriggerExec(1);
                        mV2DCam.ImageEvent[1].WaitOne();
                        HOperatorSet.ClearWindow(hWindows[2]);
                        HOperatorSet.CopyImage(mV2DCam.hvImage[1], out hImage2[1]);
                        mV2DCam.hvImage[1].Dispose();
                        //if (Halcon.Cam2Connect)
                        //{
                        //    Halcon.GrabImage(Halcon.hv_AcqHandle1, out hImage2[1]);
                        //}
                        //else
                        //{
                        //    HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 6);
                        //    AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "6");
                        //}
                        HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection2, 0);
                       
                        HOperatorSet.GetImageSize(hImage2[1], out Halcon.hv_Height[3], out Halcon.hv_Width[3]);
                        HOperatorSet.CopyImage(hImage2[1], out hObjectIn);
                        HOperatorSet.SetPart(hWindows[3], 0, 0, -1, -1);//设置窗体的规格
                        HOperatorSet.DispObj(hImage2[1], hWindows[3]);
                        检测2.Detection(1, hWindows[3], hImage2[1], ref testReslut2, ref value2);
                        HOperatorSet.DumpWindowImage(out hObjectOut, hWindows[3]);
                        DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                        strDateTime = dtNow.ToString("yyyyMMddHHmmss");
                        strDateTimeDay = dtNow.ToString("yyyy-MM-dd");
                        Push();
                        Thread th = new Thread(new ThreadStart(ThreadWork));
                        th.Start();

                        if (value2[0] - Parameter.specificationsCam2[0].料长.value < Parameter.specificationsCam2[0].料长.min || value2[0] - Parameter.specificationsCam2[0].料长.value > Parameter.specificationsCam2[0].料长.max)
                        {
                            uiDataGridView2.Rows[0].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[0].Cells[2].Value = value2[0].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[0].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[0].Cells[2].Value = value2[0].ToString("0.00000");
                        }
                        if (value2[1] - Parameter.specificationsCam2[0].料宽.value < Parameter.specificationsCam2[0].料宽.min || value2[1] - Parameter.specificationsCam2[0].料宽.value > Parameter.specificationsCam2[0].料宽.max)
                        {
                            uiDataGridView2.Rows[1].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[1].Cells[2].Value = value2[1].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[1].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[1].Cells[2].Value = value2[1].ToString("0.00000");
                        }
                        if (value2[2] - Parameter.specificationsCam2[0].胶宽.value < Parameter.specificationsCam2[0].胶宽.min || value2[2] - Parameter.specificationsCam2[0].胶宽.value > Parameter.specificationsCam2[0].胶宽.max)
                        {
                            uiDataGridView2.Rows[2].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[2].Cells[2].Value = value2[2].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[2].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[2].Cells[2].Value = value2[2].ToString("0.00000");
                        }
                        if (value2[3] - Parameter.specificationsCam2[0].长端.value < Parameter.specificationsCam2[0].长端.min || value2[3] - Parameter.specificationsCam2[0].长端.value > Parameter.specificationsCam2[0].长端.max)
                        {
                            uiDataGridView2.Rows[3].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[3].Cells[2].Value = value2[3].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[3].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[3].Cells[2].Value = value2[3].ToString("0.00000");
                        }
                        if (value2[4] - Parameter.specificationsCam2[0].料长.value < Parameter.specificationsCam2[0].料长.min || value2[4] - Parameter.specificationsCam2[0].料长.value > Parameter.specificationsCam2[0].料长.max)
                        {
                            uiDataGridView2.Rows[4].Cells[2].Style.BackColor = Color.Red;
                            uiDataGridView2.Rows[4].Cells[2].Value = value2[4].ToString("0.00000");
                        }
                        else
                        {
                            uiDataGridView2.Rows[4].Cells[2].Style.BackColor = Color.Green;
                            uiDataGridView2.Rows[4].Cells[2].Value = value2[4].ToString("0.00000");
                        }


                        if (testReslut2[0] && testReslut2[1] && testReslut2[2] && testReslut2[3] && testReslut2[4] && testReslut2[5] && testReslut2[6] && testReslut2[7])//OK
                        {
                            Parameter.counts.Counts4[4]++;
                            uiDataGridView2.Rows[9].Cells[2].Value = Parameter.counts.Counts4[4];
                            uiDataGridView2.Rows[9].Cells[2].Style.BackColor = Color.Green;
                            HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 5);
                            AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "5");
                        }
                        else
                        {
                            Parameter.counts.Counts4[3]++;
                            uiDataGridView2.Rows[8].Cells[2].Value = Parameter.counts.Counts4[3];
                            uiDataGridView2.Rows[8].Cells[2].Style.BackColor = Color.Red;
                            if (testReslut2[1])//胶面不良
                            {
                                Parameter.counts.Counts4[0]++;
                                uiDataGridView2.Rows[5].Cells[2].Value = Parameter.counts.Counts4[0];
                                uiDataGridView2.Rows[5].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "8");
                            }
                            else if (testReslut2[0] || testReslut2[2])//料面不良
                            {
                                Parameter.counts.Counts4[1]++;
                                uiDataGridView2.Rows[6].Cells[2].Value = Parameter.counts.Counts4[1];
                                uiDataGridView2.Rows[6].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 8);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "8");
                            }
                            else //尺寸不良
                            {
                                Parameter.counts.Counts4[2]++;
                                uiDataGridView2.Rows[7].Cells[2].Value = Parameter.counts.Counts4[2];
                                uiDataGridView2.Rows[7].Cells[2].Style.BackColor = Color.Red;
                                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Completion2, 6);
                                AlarmList.Add("IP:" + Parameter.commministion.PlcIpAddress + "Port:" + Parameter.commministion.PlcIpPort + "Add:" + Parameter.plcParams.Completion2 + "写入:" + "6");
                            }
                        }
                    }
                }

                Thread.Sleep(50);

            }
        }

        public class FileTimeInfo
        {
            public string FileName;  //文件名
            public DateTime FileCreateTime; //创建时间
        }
        static void GetLatestFileTimeInfo(string dir, string ext)
        {
            DirectoryInfo folder = new DirectoryInfo(dir);
            List<FileTimeInfo> list = new List<FileTimeInfo>();
            string[] files = Directory.GetFiles(dir, "*.jpg", SearchOption.AllDirectories);
            //遍历文件夹中的
            foreach (FileInfo file in folder.GetFiles("*.jpg", SearchOption.AllDirectories))
            {
                if (file.Extension.ToUpper() == ext.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        FileCreateTime = file.CreationTime
                    });
                }
            }
            var f = from x in list
                    orderby x.FileCreateTime
                    select x;
            movefile = f.LastOrDefault().FileName;
        }


        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定关闭程序吗？", "软件关闭提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Parameter.counts.Counts1[0] = (int)uiDataGridView1.Rows[3].Cells[1].Value;
                Parameter.counts.Counts1[1] = (int)uiDataGridView1.Rows[4].Cells[1].Value;
                Parameter.counts.Counts1[2] = (int)uiDataGridView1.Rows[5].Cells[1].Value;
                Parameter.counts.Counts1[3] = (int)uiDataGridView1.Rows[6].Cells[1].Value;
                Parameter.counts.Counts1[4] = (int)uiDataGridView1.Rows[6].Cells[1].Value;
                Parameter.counts.Counts2[0] = (int)uiDataGridView1.Rows[3].Cells[2].Value;
                Parameter.counts.Counts2[1] = (int)uiDataGridView1.Rows[4].Cells[2].Value;
                Parameter.counts.Counts2[2] = (int)uiDataGridView1.Rows[5].Cells[2].Value;
                Parameter.counts.Counts2[3] = (int)uiDataGridView1.Rows[6].Cells[2].Value;
                Parameter.counts.Counts2[4] = (int)uiDataGridView1.Rows[6].Cells[2].Value;

                Parameter.counts.Counts3[0] = (int)uiDataGridView2.Rows[5].Cells[1].Value;
                Parameter.counts.Counts3[1] = (int)uiDataGridView2.Rows[6].Cells[1].Value;
                Parameter.counts.Counts3[2] = (int)uiDataGridView2.Rows[7].Cells[1].Value;
                Parameter.counts.Counts3[3] = (int)uiDataGridView2.Rows[8].Cells[1].Value;
                Parameter.counts.Counts3[4] = (int)uiDataGridView2.Rows[9].Cells[1].Value;
                Parameter.counts.Counts4[0] = (int)uiDataGridView2.Rows[5].Cells[2].Value;
                Parameter.counts.Counts4[1] = (int)uiDataGridView2.Rows[6].Cells[2].Value;
                Parameter.counts.Counts4[2] = (int)uiDataGridView2.Rows[7].Cells[2].Value;
                Parameter.counts.Counts4[3] = (int)uiDataGridView2.Rows[8].Cells[2].Value;
                Parameter.counts.Counts4[4] = (int)uiDataGridView2.Rows[9].Cells[2].Value;
                XMLHelper.serialize<Parameter.Counts>(Parameter.counts, "Parameter/CountsParams.xml");
                //Parameter.specifications.右短端.value = 10;
                //XMLHelper.serialize<Parameter.Specifications>(Parameter.specifications, "Specifications.xml");
                //mV2DCam.CloseDevice();
                myThread.Abort();               
                LogHelper.Log.WriteInfo("软件关闭。");
                this.Close();
            }
        }

        private void btn_Minimizid_System_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            LogHelper.Log.WriteInfo("窗体最小化。");
            主窗体.AlarmList.Add("窗体最小化。");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();//隐藏主窗体  
                LogHelper.Log.WriteInfo("主窗体隐藏。");
                主窗体.AlarmList.Add("主窗体隐藏。");
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//当鼠标点击为左键时  
            {
                this.Show();//显示主窗体  
                LogHelper.Log.WriteInfo("主窗体恢复。");
                主窗体.AlarmList.Add("主窗体恢复。");
                this.WindowState = FormWindowState.Normal;//主窗体的大小为默认  
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Parameter.commministion.PlcEnable)
            {
                hslCommunication = new HslCommunication();
                Thread.Sleep(1000);
                if (HslCommunication.plc_connect_result)
                {
                    lab_PLCStatus.Text = "在线";
                    lab_PLCStatus.BackColor = Color.Green;
                }
                else
                {
                    lab_PLCStatus.Text = "离线";
                    lab_PLCStatus.BackColor = Color.Red;
                }
            }
            else
            {
                lab_PLCStatus.Text = "禁用";
                lab_PLCStatus.BackColor = Color.Gray;
            }

            if (Parameter.commministion.TcpClientEnable)
            {
                TcpClient tcpClientr = new TcpClient();
                Thread.Sleep(1000);
                if (TcpClient.TcpClientConnectResult)
                {
                    lab_Client.Text = "在线";
                    lab_Client.BackColor = Color.Green;
                }
                else
                {
                    lab_Client.Text = "等待";
                    lab_Client.BackColor = Color.Red;
                }
            }
            else
            {
                lab_Client.Text = "禁用";
                lab_Client.BackColor = Color.Gray;
            }

            if (Parameter.commministion.TcpServerEnable)
            {
                TcpServer tcpServer = new TcpServer();
                Thread.Sleep(1000);
                if (TcpServer.TcpServerConnectResult)
                {
                    lab_Server.Text = "在线";
                    lab_Server.BackColor = Color.Green;
                }
                else
                {
                    lab_Server.Text = "等待";
                    lab_Server.BackColor = Color.Red;
                }
            }
            else
            {
                lab_Server.Text = "禁用";
                lab_Server.BackColor = Color.Gray;
            }           
            DataGridViewRow row0 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row0);
            DataGridViewRow row1 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row1);
            DataGridViewRow row2 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row2);

            DataGridViewRow row3 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row3);
            DataGridViewRow row4 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row4);
            DataGridViewRow row5 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row5);
            DataGridViewRow row6 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row6);
            DataGridViewRow row7 = new DataGridViewRow();
            uiDataGridView1.Rows.Add(row7);
            uiDataGridView1.Rows[0].Cells[0].Value = "肩宽/mm";
            uiDataGridView1.Rows[1].Cells[0].Value = "肩高/mm";
            uiDataGridView1.Rows[2].Cells[0].Value = "胶线/mm";

            uiDataGridView1.Rows[3].Cells[0].Value = "胶线NG";
            uiDataGridView1.Rows[4].Cells[0].Value = "胶面NG";
            uiDataGridView1.Rows[5].Cells[0].Value = "尺寸NG";
            uiDataGridView1.Rows[6].Cells[0].Value = "总NG";
            uiDataGridView1.Rows[7].Cells[0].Value = "总OK";

            uiDataGridView1.Rows[3].Cells[1].Value = Parameter.counts.Counts1[0];//胶线NG
            uiDataGridView1.Rows[4].Cells[1].Value = Parameter.counts.Counts1[1];//胶面NG
            uiDataGridView1.Rows[5].Cells[1].Value = Parameter.counts.Counts1[2];//尺寸NG
            uiDataGridView1.Rows[6].Cells[1].Value = Parameter.counts.Counts1[3];//总NG
            uiDataGridView1.Rows[7].Cells[1].Value = Parameter.counts.Counts1[4];//总OK

            uiDataGridView1.Rows[3].Cells[2].Value = Parameter.counts.Counts2[0];
            uiDataGridView1.Rows[4].Cells[2].Value = Parameter.counts.Counts2[1];
            uiDataGridView1.Rows[5].Cells[2].Value = Parameter.counts.Counts2[2];
            uiDataGridView1.Rows[6].Cells[2].Value = Parameter.counts.Counts2[3];
            uiDataGridView1.Rows[7].Cells[2].Value = Parameter.counts.Counts2[4];


            DataGridViewRow row20 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row20);
            DataGridViewRow row21 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row21);
            DataGridViewRow row22 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row22);
            DataGridViewRow row23 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row23);
            DataGridViewRow row24 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row24);

            DataGridViewRow row25 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row25);
            DataGridViewRow row26 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row26);
            DataGridViewRow row27 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row27);
            DataGridViewRow row28 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row28);
            DataGridViewRow row29 = new DataGridViewRow();
            uiDataGridView2.Rows.Add(row29);
            uiDataGridView2.Rows[0].Cells[0].Value = "料长/mm";
            uiDataGridView2.Rows[1].Cells[0].Value = "料宽/mm";
            uiDataGridView2.Rows[2].Cells[0].Value = "胶宽/mm";
            uiDataGridView2.Rows[3].Cells[0].Value = "长端/mm";
            uiDataGridView2.Rows[4].Cells[0].Value = "短端/mm";

            uiDataGridView2.Rows[5].Cells[0].Value = "胶面NG";
            uiDataGridView2.Rows[6].Cells[0].Value = "料面NG";
            uiDataGridView2.Rows[7].Cells[0].Value = "尺寸NG";
            uiDataGridView2.Rows[8].Cells[0].Value = "总NG";
            uiDataGridView2.Rows[9].Cells[0].Value = "总OK";

            uiDataGridView2.Rows[5].Cells[1].Value = Parameter.counts.Counts3[0];//胶线NG
            uiDataGridView2.Rows[6].Cells[1].Value = Parameter.counts.Counts3[1];//胶面NG
            uiDataGridView2.Rows[7].Cells[1].Value = Parameter.counts.Counts3[2];//尺寸NG
            uiDataGridView2.Rows[8].Cells[1].Value = Parameter.counts.Counts3[3];//总NG
            uiDataGridView2.Rows[9].Cells[1].Value = Parameter.counts.Counts3[4];//总OK

            uiDataGridView2.Rows[5].Cells[2].Value = Parameter.counts.Counts4[0];
            uiDataGridView2.Rows[6].Cells[2].Value = Parameter.counts.Counts4[1];
            uiDataGridView2.Rows[7].Cells[2].Value = Parameter.counts.Counts4[2];
            uiDataGridView2.Rows[8].Cells[2].Value = Parameter.counts.Counts4[3];
            uiDataGridView2.Rows[9].Cells[2].Value = Parameter.counts.Counts4[4];



            HOperatorSet.SetPart(hWindows[0], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[1], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[2], 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.SetPart(hWindows[3], 0, 0, -1, -1);//设置窗体的规格 
            hWindows[0].DispObj(hImage[0]);
            hWindows[1].DispObj(hImage[1]);
            hWindows[2].DispObj(hImage2[0]);
            hWindows[3].DispObj(hImage2[1]);
            Halcon.DetectionHalconReadDlModel(Application.StartupPath + "/halcon/N1.hdl", out 检测1.hv_DLModelHandle[0], out 检测1.hv_DLPreprocessParam[0], 
                out 检测1.hv_InferenceClassificationThreshold[0], out 检测1.hv_InferenceSegmentationThreshold[0]);
            Halcon.DetectionHalconReadDlModel(Application.StartupPath + "/halcon/N2.hdl", out 检测1.hv_DLModelHandle[1], out 检测1.hv_DLPreprocessParam[1], 
                out 检测1.hv_InferenceClassificationThreshold[1], out 检测1.hv_InferenceSegmentationThreshold[1]);

            Halcon.DetectionHalconReadDlModel(Application.StartupPath + "/halcon/C1.hdl", out 检测2.hv_DLModelHandle[0], out 检测2.hv_DLPreprocessParam[0],
                out 检测2.hv_InferenceClassificationThreshold[0], out 检测2.hv_InferenceSegmentationThreshold[0]);
            Halcon.DetectionHalconReadDlModel(Application.StartupPath + "/halcon/C2.hdl", out 检测2.hv_DLModelHandle[1], out 检测2.hv_DLPreprocessParam[1],
                out 检测2.hv_InferenceClassificationThreshold[1], out 检测2.hv_InferenceSegmentationThreshold[1]);
            LogHelper.Log.WriteInfo("初始化完成");
            主窗体.AlarmList.Add("初始化完成");
        }

        #region 点击panel控件移动窗口
        System.Drawing.Point downPoint;
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new System.Drawing.Point(e.X, e.Y);
        }
        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new System.Drawing.Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }
        #endregion
        public static int formloadIndex = 0;
        public static int LineIndex = 0;
        private void btn_SettingMean_Click(object sender, EventArgs e)
        {
            formloadIndex = 1;
            FormLogin flg = new FormLogin();
            flg.ShowDialog();
        }

        //    Directory.CreateDirectory(string path);//在指定路径中创建所有目录和子目录，除非已经存在
        //    Directory.Delete(string path);//从指定路径删除空目录
        //    Directory.Delete(string path, bool recursive);//布尔参数为true可删除非空目录
        //    Directory.Exists(string path);//确定路径是否存在
        //    Directory.GetCreationTime(string path);//获取目录创建日期和时间
        //    Directory.GetCurrentDirectory();//获取应用程序当前的工作目录
        //    Directory.GetDirectories(string path);//返回指定目录所有子目录名称，包括路径
        //    Directory.GetFiles(string path);//获取指定目录中所有文件的名称，包括路径
        //    Directory.GetFileSystemEntries(string path);//获取指定路径中所有的文件和子目录名称
        //    Directory.GetLastAccessTime(string path);//获取上次访问指定文件或目录的时间和日期
        //    Directory.GetLastWriteTime(string path);//返回上次写入指定文件或目录的时间和日期
        //    Directory.GetParent(string path);//检索指定路径的父目录，包括相对路径和绝对路径
        //    Directory.Move(string soureDirName, string destName);//将文件或目录及其内容移到新的位置
        //    Directory.SetCreationTime(string path);//为指定的目录或文件设置创建时间和日期
        //    Directory.SetCurrentDirectory(string path);//将应用程序工作的当前路径设为指定路径
        //    Directory.SetLastAccessTime(string path);//为指定的目录或文件设置上次访问时间和日期
        //    Directory.SetLastWriteTime(string path);//为指定的目录和文件设置上次访问时间和日期


        private void btn_Start_Click(object sender, EventArgs e)
        {
            //if (!Halcon.Cam1Connect || !Halcon.Cam2Connect)
            //{
            //    MessageBox.Show("相机链接异常，请检查!");
            //    return;
            //}
            if (HslCommunication.plc_connect_result)
            {
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.StartAdd, 1);
                m_Pause = true;
                btn_Start.Enabled = false;
                btn_检测设置.Enabled = false;
                btn_检测设置2.Enabled = false;
                btn_Connutius.Enabled = true;
                btn_SettingMean.Enabled = false;
                btn_SpecicationSetting.Enabled = false;
                btn_Stop.Enabled = true;
                btn_CameraLiving.Enabled = false;
                btn_Close_System.Enabled = false;
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection1, 0);
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.Trigger_Detection2, 0);
            }
            else
            {              
                MessageBox.Show("链接异常，请检查!");
                return;
            }
            MainThread0 = new Thread(MainRun0);
            MainThread0.IsBackground = true;
            MainThread0.Start();

            MainThread1 = new Thread(MainRun1);
            MainThread1.IsBackground = true;
            MainThread1.Start();          
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            //if (!Halcon.Cam1Connect || !Halcon.Cam2Connect)
            //{
            //    MessageBox.Show("相机链接异常，请检查!");
            //    return;
            //}
            //else
            //{
            //    Halcon.StopImage(Halcon.hv_AcqHandle0);
            //    Halcon.StopImage(Halcon.hv_AcqHandle1);
            //}

            if (HslCommunication.plc_connect_result )
            {
                HslCommunication._NetworkTcpDevice.Write(Parameter.plcParams.StartAdd, 0);                            
                btn_Start.Enabled = true;
                btn_SettingMean.Enabled = true;
                btn_Stop.Enabled = false;
                btn_检测设置.Enabled = true;
                btn_检测设置2.Enabled = true;
                btn_Close_System.Enabled = true;
                btn_SpecicationSetting.Enabled = true;
                btn_CameraLiving.Enabled = true;
            }
            else
            {
                MessageBox.Show("PLC链接异常，请检查!");
            }
            if (MainThread0 != null)
            {
                MainThread0.Abort();
            }
            if (MainThread1 != null)
            {
                MainThread1.Abort();
            }
            
        }

        private void uiDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

        }

        private void btn_SpecicationSetting_Click(object sender, EventArgs e)
        {
            formloadIndex = 2;
            FormLogin flg = new FormLogin();
            flg.ShowDialog();
        }
 
        private void btn_CameraLiving_Click(object sender, EventArgs e)
        {
            if (!Halcon.Cam1Connect || !Halcon.Cam2Connect)
            {
                MessageBox.Show("相机链接异常，请检查!");
                return;
            }
            else
            {
                Halcon.GrabImageLive(hWindows[0],Halcon.hv_AcqHandle0,out hImage[0]);
                Halcon.GrabImageLive(hWindows[2],Halcon.hv_AcqHandle1,out hImage2[0]);
            }
            //mV2DCam.ContinuesMode(true);
            //mV2DCam.SoftTrigger(true);
            //mV2DCam.StartGrab();
            //mV2DCam.TriggerExec(0);
            //hWindows[0].SetPart(0,0,-1,-1);
            //mV2DCam.ImageEvent[0].WaitOne();
            //hWindows[0].DispObj(mV2DCam.hvImage[0]);

            //mV2DCam.TriggerExec(1);
            //hWindows[2].SetPart(0, 0, -1, -1);
            //mV2DCam.ImageEvent[1].WaitOne();
            //hWindows[2].DispObj(mV2DCam.hvImage[1]);
            //if (camera_opend[0])
            //{

            //    btn_Start.Enabled = false;
            //    btn_Stop.Enabled = true;
            //    btn_CameraLiving.Enabled = false;
            //}
            //else
            //{
            //    MessageBox.Show("相机未连接!");
            //}
        }

        private void btn_检测设置_Click(object sender, EventArgs e)
        {
            formloadIndex = 3;
            FormLogin flg = new FormLogin();
            flg.ShowDialog();
        }

        private void btn_Connutius_Click(object sender, EventArgs e)
        {
            if (btn_Connutius.Text == "暂停")
            {
                m_Pause = false;
                btn_Connutius.Text = "继续";
            }
            else
            {
                m_Pause = true;
                btn_Connutius.Text = "暂停";
            }

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Parameter.counts.Counts1[0] = 0;
            Parameter.counts.Counts1[1] = 0;
            Parameter.counts.Counts1[2] = 0;
            Parameter.counts.Counts1[3] = 0;
            Parameter.counts.Counts1[4] = 0;
            Parameter.counts.Counts2[0] = 0;
            Parameter.counts.Counts2[1] = 0;
            Parameter.counts.Counts2[2] = 0;
            Parameter.counts.Counts2[3] = 0;
            Parameter.counts.Counts2[4] = 0;

            uiDataGridView1.Rows[4].Cells[1].Value = 0;
            uiDataGridView1.Rows[5].Cells[1].Value = 0;
            uiDataGridView1.Rows[6].Cells[1].Value = 0;
            uiDataGridView1.Rows[7].Cells[1].Value = 0;

            uiDataGridView1.Rows[4].Cells[2].Value = 0;
            uiDataGridView1.Rows[5].Cells[2].Value = 0;
            uiDataGridView1.Rows[6].Cells[2].Value = 0;
            uiDataGridView1.Rows[7].Cells[2].Value = 0;


            Parameter.counts.Counts3[0] = 0;
            Parameter.counts.Counts3[1] = 0;
            Parameter.counts.Counts3[2] = 0;
            Parameter.counts.Counts3[3] = 0;
            Parameter.counts.Counts3[4] = 0;
            Parameter.counts.Counts4[0] = 0;
            Parameter.counts.Counts4[1] = 0;
            Parameter.counts.Counts4[2] = 0;
            Parameter.counts.Counts4[3] = 0;
            Parameter.counts.Counts4[4] = 0;

           
            uiDataGridView2.Rows[5].Cells[1].Value = 0;
            uiDataGridView2.Rows[6].Cells[1].Value = 0;
            uiDataGridView2.Rows[7].Cells[1].Value = 0;
            uiDataGridView2.Rows[8].Cells[1].Value = 0;

          
            uiDataGridView2.Rows[5].Cells[2].Value = 0;
            uiDataGridView2.Rows[6].Cells[2].Value = 0;
            uiDataGridView2.Rows[7].Cells[2].Value = 0;
            uiDataGridView2.Rows[8].Cells[2].Value = 0;
        }

        #region 任务队列

        public Func<int> Pop()
        {
            Monitor.Enter(m_obj);
            //Action ac = null;
            Func<int> ac = null;
            try
            {
                if (m_List.Count > 0)
                {
                    ac = m_List.Dequeue();
                }
                else
                {
                    //Monitor.Wait(m_obj);
                    //ac = m_List.Dequeue();
                }
            }
            finally
            {
                Monitor.Exit(m_obj);
            }
            return ac;
        }

        public void Push()
        {
            Monitor.Enter(m_obj);

            //Func<int> fuc = new Func<int>(ImgSaveIn);
            Func<int> fuc1 = new Func<int>(ImgSaveOut);
            //m_List.Enqueue(fuc);
            m_List.Enqueue(fuc1);
            Monitor.Pulse(m_obj);
            Monitor.Exit(m_obj);
        }

        public void ThreadWork()
        {
            while (!isExit)
            {
                Func<int> work = Pop();

                try
                {
                    if (work != null)
                    {
                        work();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("error..");
                    return;
                }

            }
            Console.WriteLine("exit threadWork");
        }
        #endregion

        string strDateTime;
        string strDateTimeDay;
        private int ImgSaveIn()
        {
            #region 保存图片            
            // 文件命名规则
            
            return 0;
            #endregion
        }

        private int ImgSaveOut()
        {
            #region 保存图片
            // 文件命名规则
            if (Parameter.specificationsCam1[0].SaveDefeatImage)
            {
                string stfFileNameOut = "Out" + strDateTime;  // 默认的图像保存名称
                string pathOut = Parameter.commministion.ImageSavePath + strDateTimeDay + "/Out/";
                if (!System.IO.Directory.Exists(pathOut))
                {
                    System.IO.Directory.CreateDirectory(pathOut);//不存在就创建文件夹
                }
                HOperatorSet.WriteImage(hObjectOut, "bmp", 0, pathOut + stfFileNameOut + ".bmp");
                hObjectOut.Dispose();
            }
            if(Parameter.specificationsCam1[0].SaveOrigalImage)
            {
                string stfFileNameIn = "In" + strDateTime;  // 默认的图像保存名称
                string pathIn = Parameter.commministion.ImageSavePath + strDateTimeDay + "/In/";
                if (!System.IO.Directory.Exists(pathIn))
                {
                    System.IO.Directory.CreateDirectory(pathIn);//不存在就创建文件夹
                }
                HOperatorSet.WriteImage(hObjectIn, "bmp", 0, pathIn + stfFileNameIn + ".bmp");
                hObjectIn.Dispose();
            }
           
            return 0;
            #endregion
        }

        private void btn_检测设置2_Click(object sender, EventArgs e)
        {
            formloadIndex = 5;
            FormLogin flg = new FormLogin();
            flg.ShowDialog();
        }

       
    }
}
