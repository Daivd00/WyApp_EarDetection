using HalconDotNet;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using OpenCvSharp.Flann;
using System.IO;
using OpenCvSharp.Dnn;

namespace WY_App.Utility
{
    internal class MV2DCam
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public UInt32[] m_nSaveImageBufSize = new UInt32[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public IntPtr[] m_pSaveImageBuf = new IntPtr[8] { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };
        private Object[] m_BufForSaveImageLock = new Object[8];
        MyCamera.MV_FRAME_OUT_INFO_EX[] m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX[8];

        MyCamera.cbOutputExdelegate cbImage;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        private MyCamera[] m_pMyCamera;
        MyCamera.MV_CC_DEVICE_INFO[] m_pDeviceInfo;
        bool m_bGrabbing;
        int m_nCanOpenDeviceNum;        // ch:设备使用数量 | en:Used Device Number
        public int m_nDevNum;        // ch:在线设备数量 | en:Online Device Number
        int[] m_nFrames;      // ch:帧数 | en:Frame Number
        bool m_bTimerFlag;     // ch:定时器开始计时标志位 | en:Timer Start Timing Flag Bit
        public bool[] bOpened = new bool[8] { false,false, false, false, false, false, false, false };
        public List<string> cbDeviceList = new List<string>();
        public AutoResetEvent[] ImageEvent = null;
        public HObject[] hvImage= new HObject[8];
        public MV2DCam()
        {
            ImageEvent = new AutoResetEvent[8]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            
            m_pMyCamera = new MyCamera[8];
            m_pDeviceInfo = new MyCamera.MV_CC_DEVICE_INFO[4];
            m_nFrames = new int[8];
            cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack);
            for (int i = 0; i < 8; ++i)
            {
                m_BufForSaveImageLock[i] = new Object();
            }
            m_bTimerFlag = false;
            Thread th = new Thread(ini_MVLineScanCam);
            th.IsBackground = true;
            th.Start();
        }

        private void ini_MVLineScanCam()
        {
            while (true)
            {
                try
                {
                    if (m_pDeviceList.nDeviceNum == 0)
                    {
                        DeviceListAcq();
                    }
                    else
                    {                                                
                        for (uint index = 0; index < m_pDeviceList.nDeviceNum; index++)
                        {
                            if (!bOpened[index])
                            {
                                OpenDevice(index);
                            }   
                        }

                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "相机链接失败:", ex.Message);
                }
                Thread.Sleep(3000);
            }
        }


        private void DeviceListAcq()
        {
            System.GC.Collect();
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "枚举相机失败");
                return;
            }
            m_nDevNum = (int)m_pDeviceList.nDeviceNum;
        }

        public void ResetMember()
        {
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            DeviceListAcq();
            m_nFrames = new int[8];
            cbImage = new MyCamera.cbOutputExdelegate(ImageCallBack);
            m_bTimerFlag = false;
            m_pDeviceInfo = new MyCamera.MV_CC_DEVICE_INFO[8];
        }

        private void OpenDevice(uint i)
        {
            // ch:参数检测 | en:Parameters inspection

                //ch:获取选择的设备信息 | en:Get Selected Device Information
                MyCamera.MV_CC_DEVICE_INFO device =(MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                //ch:打开设备 | en:Open Device
                if (null == m_pMyCamera[i])
                {
                    m_pMyCamera[i] = new MyCamera();
                    if (null == m_pMyCamera[i])
                    {
                        return;
                    }
                }

                int nRet = m_pMyCamera[i].MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_pMyCamera[i].MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + String.Format("相机" + i +"打开失败，故障码:"+ nRet.ToString("X")));
                return;
            }
            else
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + String.Format("Open Device success!" + i));

                m_nCanOpenDeviceNum++;
                m_pDeviceInfo[i] = device;
                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_pMyCamera[i].MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_pMyCamera[i].MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {
                            LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "Set Packet Size failed! nRet=0x{0}" + nRet.ToString("X"));
                        }
                    }
                    else
                    {
                        LogHelper.Log.WriteError(System.DateTime.Now.ToString() + "Get Packet Size failed! nRet=0x{0}" + nPacketSize.ToString("X"));
                    }
                }

                m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                m_pMyCamera[i].MV_CC_RegisterImageCallBackEx_NET(cbImage, (IntPtr)i);
                bOpened[i] = true;
            }
            
        }
        // ch:取流回调函数 | en:Aquisition Callback Function
        private void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            int nIndex = (int)pUser;

            // ch:抓取的帧数 | en:Aquired Frame Number
            ++m_nFrames[nIndex];

            lock (m_BufForSaveImageLock[nIndex])
            {
                if (m_pSaveImageBuf[nIndex] == IntPtr.Zero || pFrameInfo.nFrameLen > m_nSaveImageBufSize[nIndex])
                {
                    if (m_pSaveImageBuf[nIndex] != IntPtr.Zero)
                    {
                        Marshal.Release(m_pSaveImageBuf[nIndex]);
                        m_pSaveImageBuf[nIndex] = IntPtr.Zero;
                    }

                    m_pSaveImageBuf[nIndex] = Marshal.AllocHGlobal((Int32)pFrameInfo.nFrameLen);
                    if (m_pSaveImageBuf[nIndex] == IntPtr.Zero)
                    {
                        return;
                    }
                    m_nSaveImageBufSize[nIndex] = pFrameInfo.nFrameLen;
                }

                m_stFrameInfo[nIndex] = pFrameInfo;
                CopyMemory(m_pSaveImageBuf[nIndex], pData, pFrameInfo.nFrameLen);
            }

            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();
            stDisplayInfo.pData = pData;
            stDisplayInfo.nDataLen = pFrameInfo.nFrameLen;
            stDisplayInfo.nWidth = pFrameInfo.nWidth;
            stDisplayInfo.nHeight = pFrameInfo.nHeight;
            stDisplayInfo.enPixelType = pFrameInfo.enPixelType;
            try
            {
                HOperatorSet.GenImage1Extern(out hvImage[nIndex], "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, m_pSaveImageBuf[nIndex], IntPtr.Zero);
                ImageEvent[nIndex].Set();
            }
            catch (System.Exception ex)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + ex.ToString());

            }
            m_pMyCamera[nIndex].MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
        }


        public void CloseDevice()
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                int nRet;

                nRet = m_pMyCamera[i].MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return;
                }

                nRet = m_pMyCamera[i].MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return;
                }
            }
            // ch:取流标志位清零 | en:Zero setting grabbing flag bit
            m_bGrabbing = false;
            // ch:重置成员变量 | en:Reset member variable
            ResetMember();
        }

        public void ContinuesMode(bool ContinuesMode)
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            }
        }

        public void TriggerMode(bool TriggerMode)
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
                //           1 - Line1;
                //           2 - Line2;
                //           3 - Line3;
                //           4 - Counter;
                //           7 - Software;
                if (TriggerMode)
                {
                    m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                }
                else
                {
                    m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
                }
            }
        }

        public void SoftTrigger(bool SoftMode)
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                if (SoftMode)
                {
                    // ch: 触发源设为软触发 || en: set trigger mode as Software
                    m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerSource", 7);
                }
                else
                {
                    m_pMyCamera[i].MV_CC_SetEnumValue_NET("TriggerSource", 0);
                }
            }
        }

        public void StartGrab()
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                int nRet;
                m_nFrames[i] = 0;
                m_stFrameInfo[i].nFrameLen = 0;//取流之前先清除帧长度
                m_stFrameInfo[i].enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                nRet = m_pMyCamera[i].MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    LogHelper.Log.WriteError(System.DateTime.Now.ToString() + ":Start Grabbing failed!" + i + nRet.ToString("X"));
                }
            }
        }
        public void TriggerExec(uint i)
        {
            int nRet;
            nRet = m_pMyCamera[i].MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                LogHelper.Log.WriteError(System.DateTime.Now.ToString() + ":Set software trigger failed!" + i + nRet.ToString("X"));
               
            }

        }

    }
}
