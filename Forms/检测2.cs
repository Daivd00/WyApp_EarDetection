using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.ObjectModel;
using WY_App.Utility;
using OpenCvSharp.XImgProc;
using HalconDotNet;
using Sunny.UI;
using static WY_App.Utility.Parameter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WY_App
{
    public partial class 检测2 : Form
    {

        public static HRect1[] pointReault = new HRect1[8];
        static HRect1[] BaseReault = new HRect1[3];
        public 检测2()
        {
            InitializeComponent();
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.DispObj(主窗体.hImage2[0], hWindowControl1.HalconWindow);
        }
        Point downPoint;

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }


        private void btn_Close_System_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void btn_加载检测图片_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();

            if (openfile.ShowDialog() == DialogResult.OK && (openfile.FileName != ""))
            {
                Halcon.ImgDisplay2(uiComboBox1.SelectedIndex, openfile.FileName, hWindowControl1.HalconWindow);
            }
            openfile.Dispose();
        }

        private void btn_SaveParams_Click(object sender, EventArgs e)
        {
            XMLHelper.serialize<Parameter.SpecificationsCam2>(Parameter.specificationsCam2[uiComboBox1.SelectedIndex], "Parameter/Cam2Specifications" + uiComboBox1.SelectedIndex + ".xml");
        }

        private void btn_Close_System_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 检测2_Load(object sender, EventArgs e)
        {
            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.SelectedIndex = 0;
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.DispObj(主窗体.hImage2[0], hWindowControl1.HalconWindow);
            num_AreaHigh.Value = Parameter.specificationsCam2[0].AreaHigh[0];
            num_AreaLow.Value = Parameter.specificationsCam2[0].AreaLow[0];
            num_ThresholdHigh.Value = Parameter.specificationsCam2[0].ThresholdHigh[0];
            num_ThresholdLow.Value = Parameter.specificationsCam2[0].ThresholdLow[0];
            num_PixelResolution.Value = Parameter.specificationsCam2[0].PixelResolution;
            chk_SaveOrigalImage.Checked = Parameter.specificationsCam2[0].SaveOrigalImage;
            chk_SaveDefeatImage.Checked = Parameter.specificationsCam2[0].SaveDefeatImage;
        }

        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            num_AreaHigh.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaHigh[uiComboBox2.SelectedIndex];
            num_AreaLow.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaLow[uiComboBox2.SelectedIndex];
            num_ThresholdHigh.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdHigh[uiComboBox2.SelectedIndex];
            num_ThresholdLow.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdLow[uiComboBox2.SelectedIndex];
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            uiComboBox2.SelectedIndex = 0;
            主窗体.formloadIndex = uiComboBox1.SelectedIndex + 5;

            uiComboBox2_SelectedIndexChanged( sender,  e);
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, -1, -1);//设置窗体的规格 
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);

            num_AreaHigh.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaHigh[uiComboBox2.SelectedIndex];
            num_AreaLow.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaLow[uiComboBox2.SelectedIndex];
            num_ThresholdHigh.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdHigh[uiComboBox2.SelectedIndex];
            num_ThresholdLow.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdLow[uiComboBox2.SelectedIndex];
            num_PixelResolution.Value = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].PixelResolution;
            chk_SaveOrigalImage.Checked = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].SaveOrigalImage;
            chk_SaveDefeatImage.Checked = Parameter.specificationsCam2[uiComboBox1.SelectedIndex].SaveDefeatImage;

        }

        private void uiButton14_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[0]);
        }

        private void uiButton16_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[1]);
        }

        private void uiButton15_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[2]);
        }

        private void uiButton18_Click(object sender, EventArgs e)
        {
            HObject hImage = new HObject();
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);
            HOperatorSet.Threshold(主窗体.hImage2[uiComboBox1.SelectedIndex], out hImage, 16, 255);
           // HOperatorSet.DispObj(hImage, hWindowControl1.HalconWindow);
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex+2, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[0], 200, ref BaseReault[0]);
            hImage.Dispose();

        }

        private void uiButton17_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex+2, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[1], 200, ref BaseReault[1]);
        }

        private void uiButton19_Click(object sender, EventArgs e)
        {
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex + 2, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[2], 200, ref BaseReault[2]);
        }

        private void uiButton46_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[0], 200, ref BaseReault[0]);

            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[1], 200, ref BaseReault[1]);

            HTuple Row, Column, IsOverlapping;
            HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out Row, out Column, out IsOverlapping);
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseRow = Row;
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseColumn = Column;
            HOperatorSet.DispCross(hWindowControl1.HalconWindow, Row, Column, 60, 0);
            Row.Dispose();
            Column.Dispose();
            IsOverlapping.Dispose();

            HTuple angle;
            HOperatorSet.AngleLx(BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out angle);
            HTuple HomMat2DIdentity;
            HTuple HomMat2DRotate;
            HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
            HOperatorSet.HomMat2dRotate(HomMat2DIdentity, -angle, Row, Column, out HomMat2DRotate);
            HOperatorSet.AffineTransImage(主窗体.hImage2[uiComboBox1.SelectedIndex], out 主窗体.hImage2[uiComboBox1.SelectedIndex], HomMat2DRotate, "constant", "false");

            HomMat2DIdentity.Dispose();
            HomMat2DRotate.Dispose();

            HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
            HOperatorSet.HomMat2dTranslate(HomMat2DIdentity, -Row + Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseRow, -Column + Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseColumn, out HomMat2DRotate);
            HOperatorSet.AffineTransImage(主窗体.hImage2[uiComboBox1.SelectedIndex], out 主窗体.hImage2[uiComboBox1.SelectedIndex], HomMat2DRotate, "constant", "false");

            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);

            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[0], 200, ref BaseReault[0]);

            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].基准[1], 200, ref BaseReault[1]);

            HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out Row, out Column, out IsOverlapping);
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseRow = Row;
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].BaseColumn = Column;
            HOperatorSet.DispCross(hWindowControl1.HalconWindow, Row, Column, 60, 0);
            Row.Dispose();
            Column.Dispose();
            IsOverlapping.Dispose();



            XMLHelper.serialize<Parameter.SpecificationsCam2>(Parameter.specificationsCam2[uiComboBox1.SelectedIndex], "Parameter/Cam2Specifications" + uiComboBox1.SelectedIndex + ".xml");
        }

        private void btn_显示检测区域_Click(object sender, EventArgs e)
        {
            bool result = true;
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex],hWindowControl1.HalconWindow);
            Halcon.DetectionHalconRect1(uiComboBox1.SelectedIndex, uiComboBox2.SelectedIndex,hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].矩形检测区域[uiComboBox2.SelectedIndex], ref result);
            bool dtResult = false;
            Halcon.DetectionHalconRect2(uiComboBox1.SelectedIndex, uiComboBox2.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].矩形检测区域[uiComboBox2.SelectedIndex], ref dtResult);
        }

        private void uiButton29_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawRectAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].矩形检测区域[uiComboBox2.SelectedIndex]);
        }

        private void num_ThresholdLow_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdLow[uiComboBox2.SelectedIndex] =value;
        }

        private void num_ThresholdHigh_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].ThresholdHigh[uiComboBox2.SelectedIndex] = value;
        }

        private void num_AreaLow_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaLow[uiComboBox2.SelectedIndex] = value;
        }

        private void num_AreaHigh_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].AreaHigh[uiComboBox2.SelectedIndex] = value;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[0]);
        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[1]);
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[2]);
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[3]);
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[4]);
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            Halcon.DetectionDrawLineAOI(hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[5]);
        }

        private void uiButton10_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[0], 100, ref pointReault[0]);
        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[1], 100, ref pointReault[1]);
        }

        private void uiButton9_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[2], 100, ref pointReault[2]);
        }

        private void uiButton11_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[3], 100, ref pointReault[3]);
        }

        private void uiButton12_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[4], 100, ref pointReault[4]);
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            Halcon.DetectionHalconLine(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], Parameter.specificationsCam2[uiComboBox1.SelectedIndex].模板区域[5], 100, ref pointReault[5]);
        }

        private void uiButton13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
                               //Halcon.DetectionHalcon(hWindowControl1.HalconWindow, 主窗体.hImage,ref 主窗体.result);
                               //Halcon.DetectionHalconDeepLearning(hWindowControl1.HalconWindow, 主窗体.hImage, 0, hv_DLModelHandle, hv_DLPreprocessParam, hv_InferenceClassificationThreshold, hv_InferenceSegmentationThreshold, point[1], point[2], point[4]);
            bool[] result = new bool[8];
            double[] value = new double[5];
            HOperatorSet.DispObj(主窗体.hImage2[uiComboBox1.SelectedIndex], hWindowControl1.HalconWindow);
            Detection(uiComboBox1.SelectedIndex, hWindowControl1.HalconWindow, 主窗体.hImage2[uiComboBox1.SelectedIndex], ref result, ref value);
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数           
            time.Text = milliseconds.ToString();
        }
        public static HTuple[] hv_DLModelHandle = new HTuple[2];
        public static HTuple[] hv_DLPreprocessParam = new HTuple[2];
        public static HTuple[] hv_InferenceClassificationThreshold = new HTuple[2];
        public static HTuple[] hv_InferenceSegmentationThreshold = new HTuple[2];
        public static void Detection(int i, HWindow hWindow, HObject hImage, ref bool[] result, ref double[] value)
        {
            try
            {
                bool[] resultvalue = new bool[8];
                System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); //  开始监视代码运行时间
                                   //HOperatorSet.DispObj(hImage, hWindow);
                
                Halcon.DetectionHalconLine(i, hWindow, hImage, Parameter.specificationsCam2[i].基准[0], 200, ref BaseReault[0]);
                Halcon.DetectionHalconLine(i, hWindow, hImage, Parameter.specificationsCam2[i].基准[1], 200, ref BaseReault[1]);

                HTuple angle, Row, Column, IsOverlapping;
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out Row, out Column, out IsOverlapping);

                HOperatorSet.AngleLx(BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out angle);
                HTuple HomMat2DIdentity;
                HTuple HomMat2DRotate;
                HObject ImageAffineTran;
                HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
                HOperatorSet.HomMat2dRotate(HomMat2DIdentity, -angle, Row, Column, out HomMat2DRotate);
                HOperatorSet.AffineTransImage(hImage, out ImageAffineTran, HomMat2DRotate, "constant", "false");

                HomMat2DIdentity.Dispose();
                HomMat2DRotate.Dispose();

                HObject ImageAffineTrans;
                HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);
                HOperatorSet.HomMat2dTranslate(HomMat2DIdentity, -Row + Parameter.specificationsCam2[i].BaseRow, -Column + Parameter.specificationsCam2[i].BaseColumn, out HomMat2DRotate);
                HOperatorSet.AffineTransImage(ImageAffineTran, out 主窗体.hImage2[i], HomMat2DRotate, "constant", "false");
                HOperatorSet.AffineTransImage(ImageAffineTran, out ImageAffineTrans, HomMat2DRotate, "constant", "false");
                HOperatorSet.DispObj(ImageAffineTrans, hWindow);
                Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].基准[0], 200, ref BaseReault[0]);
                Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].基准[1], 200, ref BaseReault[1]);
                resultvalue[0] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[0], 100, ref pointReault[0]);
                resultvalue[1] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[1], 100, ref pointReault[1]);
                resultvalue[2] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[2], 100, ref pointReault[2]);
                resultvalue[3] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[3], 100, ref pointReault[3]);
                resultvalue[4] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[4], 100, ref pointReault[4]);
                resultvalue[5] = Halcon.DetectionHalconLine(i, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].模板区域[5], 100, ref pointReault[5]);
                bool grayResult = false;
                Halcon.DetectionHalconRect1(i, 0, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].矩形检测区域[0], ref result[0]);
                Halcon.DetectionHalconRect1(i, 1, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].矩形检测区域[1], ref grayResult);
                Halcon.DetectionHalconRect1(i, 2, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].矩形检测区域[2], ref result[2]);

                bool dtResult = false;
                Halcon.DetectionHalconRect2(i, 1, hWindow, ImageAffineTrans, Parameter.specificationsCam2[i].矩形检测区域[1], ref dtResult);

                bool dplResult = false;
                Halcon.DetectionHalconDeepLearning1(i, hWindow, ImageAffineTrans, hv_DLModelHandle[i], hv_DLPreprocessParam[i], hv_InferenceClassificationThreshold[i], hv_InferenceSegmentationThreshold[i], Row, Column, ref dplResult);

                if(grayResult && dtResult/*&& dplResult*/)
                {
                    result[1] = true;
                }
                else
                {
                    result[1] = false;
                }
                HOperatorSet.IntersectionLines(BaseReault[0].Row1, BaseReault[0].Colum1, BaseReault[0].Row2, BaseReault[0].Colum2,
                    BaseReault[1].Row1, BaseReault[1].Colum1, BaseReault[1].Row2, BaseReault[1].Colum2, out Row, out Column, out IsOverlapping);
                
                HOperatorSet.DispCross(hWindow, Row, Column, 60, 0);
                HTuple minDistance, maxDistance;
                HOperatorSet.DistanceSs(pointReault[0].Row1, pointReault[0].Colum1, pointReault[0].Row2, pointReault[0].Colum2,
                    pointReault[1].Row1, pointReault[1].Colum1, pointReault[1].Row2, pointReault[1].Colum2, out minDistance, out maxDistance);
                HOperatorSet.SetTposition(hWindow, 100, 100);
                value[0] = minDistance * Parameter.specificationsCam2[i].PixelResolution + Parameter.specificationsCam2[i].胶宽.adjust;
                if (value[0] - Parameter.specificationsCam2[0].料长.value < Parameter.specificationsCam2[0].料长.min ||
                    value[0] - Parameter.specificationsCam2[0].料长.value > Parameter.specificationsCam2[0].料长.max)
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    result[3] = false;
                }
                else
                {
                    result[3] = true;
                    HOperatorSet.SetColor(hWindow, "green");
                }
                HOperatorSet.WriteString(hWindow, "料长" + value[0]);
                HOperatorSet.DistanceSs(pointReault[2].Row1, pointReault[2].Colum1, pointReault[2].Row2, pointReault[2].Colum2,
                    pointReault[3].Row1, pointReault[3].Colum1, pointReault[3].Row2, pointReault[3].Colum2, out minDistance, out maxDistance);
                HOperatorSet.SetTposition(hWindow, 200, 100);
                value[1] = minDistance * Parameter.specificationsCam2[i].PixelResolution + Parameter.specificationsCam2[i].料宽.adjust;

                if (value[1] - Parameter.specificationsCam2[0].料宽.value < Parameter.specificationsCam2[0].料宽.min ||
                    value[1] - Parameter.specificationsCam2[0].料宽.value > Parameter.specificationsCam2[0].料宽.max)
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    result[4] = false;
                }
                else
                {
                    result[4] = true;
                    HOperatorSet.SetColor(hWindow, "green");
                }
                HOperatorSet.WriteString(hWindow, "料宽" + value[1]);
                HOperatorSet.DistanceSs(pointReault[5].Row1, pointReault[5].Colum1, pointReault[5].Row2, pointReault[5].Colum2,
                    pointReault[4].Row1, pointReault[4].Colum1, pointReault[4].Row2, pointReault[4].Colum2, out minDistance, out maxDistance);
                HOperatorSet.SetTposition(hWindow, 300, 100);
                value[2] = minDistance * Parameter.specificationsCam2[i].PixelResolution + Parameter.specificationsCam2[i].胶宽.adjust;

                if (value[2] - Parameter.specificationsCam2[0].胶宽.value < Parameter.specificationsCam2[0].胶宽.min ||
                    value[2] - Parameter.specificationsCam2[0].胶宽.value > Parameter.specificationsCam2[0].胶宽.max)
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    result[5] = false;
                }
                else
                {
                    result[5] = true;
                    HOperatorSet.SetColor(hWindow, "green");
                }
                minDistance.Dispose();
                maxDistance.Dispose();
                HOperatorSet.WriteString(hWindow, "胶宽" + value[2]);
                HOperatorSet.DistanceSs(pointReault[3].Row1, pointReault[3].Colum1, pointReault[3].Row2, pointReault[3].Colum2,
                    pointReault[5].Row1, pointReault[5].Colum1, pointReault[5].Row2, pointReault[5].Colum2, out minDistance, out maxDistance);
                HOperatorSet.SetTposition(hWindow, 400, 100);
                value[3] = minDistance * Parameter.specificationsCam2[i].PixelResolution + Parameter.specificationsCam2[i].长端.adjust;

                if (value[3] - Parameter.specificationsCam2[0].长端.value < Parameter.specificationsCam2[0].长端.min ||
                    value[3] - Parameter.specificationsCam2[0].长端.value > Parameter.specificationsCam2[0].长端.max)
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    result[6] = false;
                }
                else
                {
                    result[6] = true;
                    HOperatorSet.SetColor(hWindow, "green");
                }
                minDistance.Dispose();
                maxDistance.Dispose();
                HOperatorSet.WriteString(hWindow, "长端" + value[3]);
                HOperatorSet.DistanceSs(pointReault[2].Row1, pointReault[2].Colum1, pointReault[2].Row2, pointReault[2].Colum2,
                    pointReault[4].Row1, pointReault[4].Colum1, pointReault[4].Row2, pointReault[4].Colum2, out minDistance, out maxDistance);
                HOperatorSet.SetTposition(hWindow, 500, 100);
                value[4] = minDistance * Parameter.specificationsCam2[i].PixelResolution + Parameter.specificationsCam2[i].短端.adjust;

                if (value[4] - Parameter.specificationsCam2[0].短端.value < Parameter.specificationsCam2[0].短端.min ||
                    value[4] - Parameter.specificationsCam2[0].短端.value > Parameter.specificationsCam2[0].短端.max)
                {
                    HOperatorSet.SetColor(hWindow, "red");
                    result[7] = false;
                }
                else
                {
                    result[7] = true;
                    HOperatorSet.SetColor(hWindow, "green");
                }
                minDistance.Dispose();
                maxDistance.Dispose();
                HOperatorSet.WriteString(hWindow, "短端" + value[4]);

                angle.Dispose();
                Row.Dispose();
                Column.Dispose();
                IsOverlapping.Dispose();
                HomMat2DIdentity.Dispose();
                HomMat2DRotate.Dispose();
                ImageAffineTran.Dispose();
                ImageAffineTrans.Dispose();
                stopwatch.Stop(); //  停止监视
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数           
                LogHelper.Log.WriteInfo("程序检测时间:" + System.DateTime.Now.ToString("ss-fff") + "程序检测时长:" + milliseconds.ToString() + "ms");
                主窗体.AlarmList.Add("程序检测时间:" + System.DateTime.Now.ToString("ss-fff") + "程序检测时长:" + milliseconds.ToString() + "ms");
            }
            catch
            {

            }

        }

        private void num_PixelResolution_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].PixelResolution= value;
        }

        private void uiButton20_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 0;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton21_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 1;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton22_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 2;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton23_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 3;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton24_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 4;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton25_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 5;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton26_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 6;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton27_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 7;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiButton28_Click(object sender, EventArgs e)
        {
            主窗体.LineIndex = 8;
            直线工具属性 flg = new 直线工具属性();
            flg.ShowDialog();
        }

        private void uiDoubleUpDown1_ValueChanged(object sender, double value)
        {
            Parameter.specificationsCam2[uiComboBox1.SelectedIndex].DeepLearningRate = value;
        }
    }
}
